using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public abstract class Snake : MonoBehaviour
{
    protected Vector2 currentDirection = Vector2.right;
    protected Vector2 nextDirection;
    private const float BaseMoveDelay = 0.1f;
    private const float Scale = 0.2f;
    private float timer;
    private bool ate = false;
    public GameObject tailPrefab;
    private List<Transform> tail = new List<Transform>();

    // Power-up states
    private bool isShieldActive = false;
    private bool isScoreBoostActive = false;
    private bool isSpeedUpActive = false;

    // Power-up durations
    private float shieldDuration = 10f; // Duration in seconds
    private float speedUpDuration = 10f; // Duration in seconds
    private float scoreBoostDuration = 10f;
    private float powerUpCooldown = 3f;

    // Trackers for power-up cooldowns
    private float shieldCooldownTimer = 0f;
    private float scoreBoostCooldownTimer = 0f;
    private float speedUpCooldownTimer = 0f;

    // Power-up effects
    private float scoreMultiplier = 1f;
    private float moveDelay = BaseMoveDelay;

    // Power-up prefabs
    public GameObject shieldPrefab;
    public GameObject scoreBoostPrefab;
    public GameObject speedUpPrefab; 

    // Border objects to define boundaries
    public GameObject topBorder;
    public GameObject bottomBorder;
    public GameObject leftBorder;
    public GameObject rightBorder;

    // Border positions
    private float topBorderY;
    private float bottomBorderY;
    private float leftBorderX;
    private float rightBorderX;

    // Score
    private int score = 0;
    public TextMeshProUGUI scoreText;
    public GameObject scoreManagerObject;
    private ScoreManager scoreManager;

    // Player number
    public int playerNumber;

    void Start()
    {
        nextDirection = currentDirection;
        // Get border positions
        topBorderY = topBorder.transform.position.y;
        bottomBorderY = bottomBorder.transform.position.y;
        leftBorderX = leftBorder.transform.position.x;
        rightBorderX = rightBorder.transform.position.x;
        scoreManager = scoreManagerObject.GetComponent<ScoreManager>();
    }

    void Update()
    {
        HandleInput();
        UpdatePowerUpCooldowns();
    }

    void FixedUpdate()
    {
        timer += Time.fixedDeltaTime;

        if (timer >= moveDelay)
        {
            UpdateDirection();
            Move();
            timer = 0f;
        }
        if (isSpeedUpActive)
        {
            moveDelay = BaseMoveDelay / 2;
        }
        else
        {
            moveDelay = BaseMoveDelay;
        }
    }

    protected abstract void HandleInput();

    protected void UpdateDirection()
    {
        currentDirection = nextDirection;
    }

    protected void Move()
    {
        Vector2 previousPosition = transform.position;
        Vector2 newPosition = CalculateNewPosition();

        if (CheckSelfCollision(newPosition) || CheckOtherSnakeCollision(newPosition))
        {
            if (isShieldActive)
            {
                isShieldActive = false;
            }
            else
            {
                HandleDeath();
                return;
            }
        }

        // Apply screen wrapping
        newPosition = ApplyScreenWrap(newPosition);

        transform.position = newPosition;

        if (ate)
        {
            GrowTail(previousPosition);
            ate = false;
        }
        else if (tail.Count > 0)
        {
            MoveTail(previousPosition);
        }
    }

    protected Vector2 CalculateNewPosition()
    {
        return new Vector2(
            transform.position.x + currentDirection.x * Scale,
            transform.position.y + currentDirection.y * Scale
        );
    }

    protected bool CheckSelfCollision(Vector2 headPosition)
    {
        // Check if the head position overlaps with any part of the tail
        for (int i = 0; i < tail.Count; i++)
        {
            if (headPosition == (Vector2)tail[i].position)
            {
                return true;
            }
        }
        return false;
    }

    protected bool CheckOtherSnakeCollision(Vector2 headPosition)
    {
        string otherSnakeTag = gameObject.tag == "SnakePlayerOne" ? "SnakePlayerTwo" : "SnakePlayerOne";
        GameObject otherSnake = GameObject.FindGameObjectWithTag(otherSnakeTag);
        if (otherSnake != null)
        {
            Snake otherSnakeScript = otherSnake.GetComponent<Snake>();
            if (headPosition == (Vector2)otherSnake.transform.position)
            {
                otherSnakeScript.HandleDeath();
                return true;
            }

            foreach (Transform tailPart in otherSnakeScript.tail)
            {
                if (headPosition == (Vector2)tailPart.position)
                {
                    otherSnakeScript.HandleDeath();
                    return true;
                }
            }
        }
        return false;
    }


    protected Vector2 ApplyScreenWrap(Vector2 position)
    {
        // Check and apply screen wrap based on border positions
        if (position.y > topBorderY)
            position.y = bottomBorderY;
        else if (position.y < bottomBorderY)
            position.y = topBorderY;

        if (position.x < leftBorderX)
            position.x = rightBorderX;
        else if (position.x > rightBorderX)
            position.x = leftBorderX;

        return position;
    }

    protected void GrowTail(Vector2 position)
    {
        GameObject newTailPart = Instantiate(tailPrefab, position, Quaternion.identity);
        tail.Insert(0, newTailPart.transform);
    }

    protected void MoveTail(Vector2 previousHeadPosition)
    {
        tail.Last().position = previousHeadPosition;
        tail.Insert(0, tail.Last());
        tail.RemoveAt(tail.Count - 1);
    }

    protected abstract void HandleDeath();

    protected void HandleGameOver()
    {
        // In single-player, just display the game-over screen without any message
        if (SceneManager.GetActiveScene().name == "Singleplayer")
        {
            GameObject.FindObjectOfType<GameOverManager>().ShowGameOverScreen("");
        }
        else
        {
            // In multiplayer, show appropriate win/loss messages
            string message = gameObject.tag == "SnakePlayerOne" ? "Player 1 Died. Player 2 Wins!" : "Player 2 Died. Player 1 Wins!";
            GameObject.FindObjectOfType<GameOverManager>().ShowGameOverScreen(message);
        }

        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.CompareTag("Food"))
        {
            Food food = coll.GetComponent<Food>();
            if (food != null)
            {
                if (food.type == FoodType.MassGainer)
                {
                    ate = true;
                    UpdateScore(1);
                }
                else if (food.type == FoodType.MassBurner)
                {
                    ShrinkTail();
                    UpdateScore(-2);
                }
                Destroy(coll.gameObject);
            }
        }
        else if (coll.CompareTag("PowerUp"))
        {
            ActivatePowerUp(coll.gameObject);
            Destroy(coll.gameObject);
        }
    }

    private void ActivatePowerUp(GameObject powerUp)
    {
        PowerUpType type = powerUp.GetComponent<PowerUp>().type;

        switch (type)
        {
            case PowerUpType.Shield:
                ActivateShield();
                break;
            case PowerUpType.ScoreBoost:
                ActivateScoreBoost();
                break;
            case PowerUpType.SpeedUp:
                ActivateSpeedUp();
                break;
        }
    }

    private void ActivateShield()
    {
        if (shieldCooldownTimer <= 0)
        {
            isShieldActive = true;
            shieldCooldownTimer = shieldDuration + powerUpCooldown;
            StartCoroutine(DeactivateShieldAfterDelay());
            Debug.Log("Shield activated!");
        }
    }

    private IEnumerator DeactivateShieldAfterDelay()
    {
        yield return new WaitForSeconds(shieldDuration);
        isShieldActive = false;
        Debug.Log("Shield deactivated.");
    }

    private void ActivateScoreBoost()
    {
        if (scoreBoostCooldownTimer <= 0)
        {
            isScoreBoostActive = true;
            scoreMultiplier = 2f;
            scoreBoostCooldownTimer = scoreBoostDuration + powerUpCooldown;
            StartCoroutine(DeactivateScoreBoostAfterDelay());
            Debug.Log("Score boost activated!");
        }
    }

    private IEnumerator DeactivateScoreBoostAfterDelay()
    {
        yield return new WaitForSeconds(scoreBoostDuration);
        isScoreBoostActive = false;
        scoreMultiplier = 1f;
        Debug.Log("Score boost deactivated.");
    }

    private void ActivateSpeedUp()
    {
        if (speedUpCooldownTimer <= 0)
        {
            isSpeedUpActive = true;
            moveDelay = BaseMoveDelay / 2;
            speedUpCooldownTimer = speedUpDuration + powerUpCooldown;
            StartCoroutine(DeactivateSpeedUpAfterDelay());
            Debug.Log("Speed up activated!");
        }
    }

    private IEnumerator DeactivateSpeedUpAfterDelay()
    {
        yield return new WaitForSeconds(speedUpDuration);
        isSpeedUpActive = false;
        moveDelay = BaseMoveDelay;
        Debug.Log("Speed up deactivated.");
    }

    protected void ShrinkTail()
    {
        if (tail.Count > 0)
        {
            Destroy(tail.Last().gameObject);
            tail.RemoveAt(tail.Count - 1);
        }
    }

    public int GetTailLength()
    {
        return tail.Count;
    }

    protected void UpdatePowerUpCooldowns()
    {
        if (shieldCooldownTimer > 0) shieldCooldownTimer -= Time.deltaTime;
        if (scoreBoostCooldownTimer > 0) scoreBoostCooldownTimer -= Time.deltaTime;
        if (speedUpCooldownTimer > 0) speedUpCooldownTimer -= Time.deltaTime;
    }

    public int GetScore()
    {
        return score;
    }

    private void UpdateScore(int points)
    {
        if (isScoreBoostActive)
        {
            points *= 2;
        }

        score += (int)(points * scoreMultiplier);

        scoreManager.UpdateScore(playerNumber, score);

        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }

}
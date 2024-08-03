using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public abstract class Snake : MonoBehaviour
{
    // Serialized fields for messages
    [SerializeField] private string singlePlayerGameOverMessage = "";
    private string player1DeathMessage = "Player 1 Died. Player 2 Wins!";
    private string player2DeathMessage = "Player 2 Died. Player 1 Wins!";

    private AudioManager audioManager;

    // Reference to the GameOverManager
    [SerializeField] private GameOverManager gameOverManager;

    // Reference to the other snake 
    [SerializeField] private Snake otherSnake;

    // Scene name for single-player mode
    [SerializeField] private string singlePlayerSceneName = "Singleplayer";

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

    [SerializeField] private int massGainerScore = 1;
    [SerializeField] private int massBurnerScore = -2;

    // Trackers for power-up cooldowns
    private float shieldCooldownTimer = 0f;
    private float scoreBoostCooldownTimer = 0f;
    private float speedUpCooldownTimer = 0f;

    // Power-up effects
    private float scoreMultiplier = 1f;
    private float moveDelay = BaseMoveDelay;

    // Power-up prefabs
    [SerializeField] private GameObject shieldPrefab;
    [SerializeField] private GameObject scoreBoostPrefab;
    [SerializeField] private GameObject speedUpPrefab;

    // Border objects to define boundaries
    [SerializeField] private GameObject topBorder;
    [SerializeField] private GameObject bottomBorder;
    [SerializeField] private GameObject leftBorder;
    [SerializeField] private GameObject rightBorder;

    // Border positions
    private float topBorderY;
    private float bottomBorderY;
    private float leftBorderX;
    private float rightBorderX;

    // Score
    private int score = 0;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private GameObject scoreManagerObject;
    private ScoreManager scoreManager;

    // Serialized fields for power-up multipliers
    [SerializeField] private float scoreBoostMultiplier = 2f; // Multiplier for score boost
    [SerializeField] private float defaultScoreMultiplier = 1f; // Default multiplier

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

        Debug.Log("Snake initialized: " + gameObject.tag);
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
        if (otherSnake != null)
        {
            // Check collision with the other snake’s head
            if (headPosition == (Vector2)otherSnake.transform.position)
            {
                // Handle the death of the current snake (this one)
                HandleDeath();
                return true;
            }

            // Check collision with the other snake’s tail
            foreach (Transform tailPart in otherSnake.tail)
            {
                if (headPosition == (Vector2)tailPart.position)
                {
                    // Handle the death of the current snake (this one)
                    HandleDeath();
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
        if (SceneManager.GetActiveScene().name == singlePlayerSceneName)
        {
            gameOverManager.ShowGameOverScreen(singlePlayerGameOverMessage);
        }
        else
        {
            string message = "";

            if (otherSnake == null)
            {
                message = singlePlayerGameOverMessage;
            }
            else
            {
                if (playerNumber == 1)
                {
                    message = player1DeathMessage; // Player 1 died
                }
                else if (playerNumber == 2)
                {
                    message = player2DeathMessage; // Player 2 died
                }
                else
                {
                    Debug.LogError("Unknown player number: " + playerNumber);
                }
            }

            Debug.Log("Game Over! Message: " + message + ", Current Snake: " + gameObject.name + ", Other Snake: " + (otherSnake ? otherSnake.name : "None"));
            gameOverManager.ShowGameOverScreen(message);
        }

        Destroy(gameObject);
        AudioManager.instance.PlaySFX(AudioManager.instance.gameOverSound);
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        Food food = coll.GetComponent<Food>();
        if (food != null)
        {
            if (food.type == FoodType.MassGainer)
            {
                ate = true;
                UpdateScore(massGainerScore);
                AudioManager.instance.PlaySFX(AudioManager.instance.eatSound);
            }
            else if (food.type == FoodType.MassBurner)
            {
                ShrinkTail();
                UpdateScore(massBurnerScore);
                AudioManager.instance.PlaySFX(AudioManager.instance.eatSound);
            }
            Destroy(coll.gameObject);
        }
        else
        {
            PowerUp powerUp = coll.GetComponent<PowerUp>();
            if (powerUp != null)
            {
                ActivatePowerUp(coll.gameObject);
                AudioManager.instance.PlaySFX(AudioManager.instance.powerUpSound);
                Destroy(coll.gameObject);
            }
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
            scoreMultiplier = scoreBoostMultiplier;
            scoreBoostCooldownTimer = scoreBoostDuration + powerUpCooldown;
            StartCoroutine(DeactivateScoreBoostAfterDelay());
            Debug.Log("Score boost activated!");
        }
    }

    private IEnumerator DeactivateScoreBoostAfterDelay()
    {
        yield return new WaitForSeconds(scoreBoostDuration);
        isScoreBoostActive = false;
        scoreMultiplier = defaultScoreMultiplier;
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

    public List<Transform> GetTail()
    {
        return tail;
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
        // Apply the score multiplier based on whether score boost is active
        float effectiveMultiplier = isScoreBoostActive ? scoreMultiplier : defaultScoreMultiplier;
        points = (int)(points * effectiveMultiplier);

        // Update the score while ensuring it doesn't go below zero
        score = Mathf.Max(score + points, 0);
        scoreManager.UpdateScore(playerNumber, score);

        // Update the score text display
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
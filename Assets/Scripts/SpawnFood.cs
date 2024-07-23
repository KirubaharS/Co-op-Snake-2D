using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnFood : MonoBehaviour
{
    public GameObject massGainerPrefab;
    public GameObject massBurnerPrefab;

    public Transform TopBorder;
    public Transform BottomBorder;
    public Transform LeftBorder;
    public Transform RightBorder;

    private float spawnIntervalMin = 5f;
    private float spawnIntervalMax = 10f;
    private float foodLifetime = 10f;

    void Start()
    {
        StartCoroutine(SpawnFoodAtRandomIntervals());
    }

    private IEnumerator SpawnFoodAtRandomIntervals()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(spawnIntervalMin, spawnIntervalMax));

            bool spawnMassBurner = ShouldSpawnMassBurner();
            GameObject selectedFood = spawnMassBurner ? massBurnerPrefab : massGainerPrefab;

            float x = Random.Range(LeftBorder.position.x, RightBorder.position.x);
            float y = Random.Range(BottomBorder.position.y, TopBorder.position.y);

            GameObject foodInstance = Instantiate(selectedFood, new Vector2(x, y), Quaternion.identity);
            Destroy(foodInstance, foodLifetime); // Destroy the food after its lifetime
        }
    }

    private bool ShouldSpawnMassBurner()
    {
        // Add logic to determine if Mass Burner should be spawned based on snake length
        // For example, if the snake's length is too short, return false
        // Here, we'll assume a basic condition where mass burner is not spawned if snake length is less than 5
        Snake snake = FindObjectOfType<Snake>();
        return snake != null && snake.GetTailLength() > 5;
    }
}


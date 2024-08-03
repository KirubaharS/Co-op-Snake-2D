using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnFood : MonoBehaviour
{
    [SerializeField] private GameObject massGainerPrefab;
    [SerializeField] private GameObject massBurnerPrefab;
    [SerializeField] private Transform TopBorder;
    [SerializeField] private Transform BottomBorder;
    [SerializeField] private Transform LeftBorder;
    [SerializeField] private Transform RightBorder;

    private float spawnIntervalMin = 5f;
    private float spawnIntervalMax = 10f;
    private float foodLifetime = 10f;

    [SerializeField] private int tailLengthThreshold = 5;
    [SerializeField] private float massBurnerProbabilityAboveThreshold = 0.3f;
    [SerializeField] private float massBurnerProbabilityBelowThreshold = 0.1f;

    [SerializeField] private List<Snake> snakes;

    void Start()
    {
        StartCoroutine(SpawnFoodAtRandomIntervals());
    }

    private IEnumerator SpawnFoodAtRandomIntervals()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(spawnIntervalMin, spawnIntervalMax));

            GameObject selectedFood = ChooseFoodPrefabBasedOnProbability();
            Vector2 spawnPosition;

            do
            {
                float x = Random.Range(LeftBorder.position.x, RightBorder.position.x);
                float y = Random.Range(BottomBorder.position.y, TopBorder.position.y);
                spawnPosition = new Vector2(x, y);
            } while (IsPositionOnSnakes(spawnPosition));

            GameObject foodInstance = Instantiate(selectedFood, spawnPosition, Quaternion.identity);
            Destroy(foodInstance, foodLifetime);
        }
    }

    private GameObject ChooseFoodPrefabBasedOnProbability()
    {
        if (snakes == null || snakes.Count == 0)
        {
            Debug.LogWarning("Snake references are not assigned, defaulting to MassGainer.");
            return massGainerPrefab;
        }

        // Calculate total tail length of all snakes
        int totalTailLength = 0;
        foreach (var snake in snakes)
        {
            totalTailLength += snake.GetTailLength();
        }

        bool shouldSpawnMassBurner = totalTailLength > tailLengthThreshold
            ? Random.value < massBurnerProbabilityAboveThreshold
            : Random.value < massBurnerProbabilityBelowThreshold;

        Debug.Log($"Total Tail Length: {totalTailLength}, Should Spawn MassBurner: {shouldSpawnMassBurner}");

        return shouldSpawnMassBurner ? massBurnerPrefab : massGainerPrefab;
    }

    private bool IsPositionOnSnakes(Vector2 position)
    {
        foreach (var snake in snakes)
        {
            if ((Vector2)snake.transform.position == position)
            {
                return true;
            }

            foreach (Transform tailPart in snake.GetTail())
            {
                if ((Vector2)tailPart.position == position)
                {
                    return true;
                }
            }
        }

        return false;
    }
}

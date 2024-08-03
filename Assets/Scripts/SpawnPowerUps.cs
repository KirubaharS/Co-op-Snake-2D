using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPowerUps : MonoBehaviour
{
    [SerializeField] private PowerUp[] powerUps;
    [SerializeField] private Transform TopBorder;
    [SerializeField] private Transform BottomBorder;
    [SerializeField] private Transform LeftBorder;
    [SerializeField] private Transform RightBorder;

    [SerializeField] private float minSpawnInterval = 5f;
    [SerializeField] private float maxSpawnInterval = 15f;

    void Start()
    {
        // Log border positions for debugging
        Debug.Log("TopBorder: " + TopBorder.position);
        Debug.Log("BottomBorder: " + BottomBorder.position);
        Debug.Log("LeftBorder: " + LeftBorder.position);
        Debug.Log("RightBorder: " + RightBorder.position);

        // Start the coroutine to spawn power-ups
        StartCoroutine(SpawnPowerUpAtRandomIntervals());
    }

    private IEnumerator SpawnPowerUpAtRandomIntervals()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minSpawnInterval, maxSpawnInterval));

            if (powerUps == null || powerUps.Length == 0)
            {
                Debug.LogError("PowerUps array is null or empty!");
                continue;
            }

            // Log powerUps array contents
            for (int i = 0; i < powerUps.Length; i++)
            {
                if (powerUps[i] == null)
                {
                    Debug.LogWarning("PowerUp at index " + i + " is null!");
                }
                else
                {
                    Debug.Log("PowerUp at index " + i + ": " + powerUps[i].name);
                }
            }

            List<PowerUp> validPowerUps = new List<PowerUp>();
            foreach (PowerUp powerUp in powerUps)
            {
                if (powerUp != null)
                {
                    validPowerUps.Add(powerUp);
                }
            }

            if (validPowerUps.Count == 0)
            {
                Debug.LogError("No valid PowerUps available to spawn!");
                continue;
            }

            int powerUpIndex = Random.Range(0, validPowerUps.Count);
            GameObject selectedPowerUp = validPowerUps[powerUpIndex].gameObject;

            if (TopBorder == null || BottomBorder == null || LeftBorder == null || RightBorder == null)
            {
                Debug.LogError("One or more border transforms are not assigned!");
                continue;
            }

            float x = Random.Range(LeftBorder.position.x, RightBorder.position.x);
            float y = Random.Range(BottomBorder.position.y, TopBorder.position.y);

            Debug.Log($"Spawning power-up {selectedPowerUp.name} at position: ({x}, {y})");
            Instantiate(selectedPowerUp, new Vector2(x, y), Quaternion.identity);
        }
    }
}

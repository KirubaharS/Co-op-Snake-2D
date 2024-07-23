using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPowerUps : MonoBehaviour
{
    public GameObject[] powerUps; 
    public Transform TopBorder;
    public Transform BottomBorder;
    public Transform LeftBorder;
    public Transform RightBorder;

    void Start()
    {
        StartCoroutine(SpawnPowerUpAtRandomIntervals());
    }

    private IEnumerator SpawnPowerUpAtRandomIntervals()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(5, 15)); 

            int powerUpIndex = Random.Range(0, powerUps.Length);
            GameObject selectedPowerUp = powerUps[powerUpIndex];

            float x = Random.Range(LeftBorder.position.x, RightBorder.position.x);
            float y = Random.Range(BottomBorder.position.y, TopBorder.position.y);

            Instantiate(selectedPowerUp, new Vector2(x, y), Quaternion.identity);
        }
    }
}

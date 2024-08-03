using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreTextA;
    [SerializeField] private TextMeshProUGUI scoreTextB;

    private int scoreA = 0;
    private int scoreB = 0;

    public void UpdateScore(int player, int points)
    {
        if (player == 1)
        {
            scoreA = points;
            scoreTextA.text = scoreA.ToString(); ;
        }
        else if (player == 2)
        {
            scoreB = points;
            scoreTextB.text = scoreB.ToString(); ;
        }
    }

    public int GetScore(int player)
    {
        if (player == 1)
            return scoreA;
        else if (player == 2)
            return scoreB;

        return 0;
    }
}

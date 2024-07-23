using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum PowerUpType
{
    Shield,
    ScoreBoost,
    SpeedUp
}
public class PowerUp : MonoBehaviour
{
    public PowerUpType type;
}

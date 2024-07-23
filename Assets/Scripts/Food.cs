using UnityEngine;

public enum FoodType
{
    MassGainer,
    MassBurner
}

public class Food : MonoBehaviour
{
    public FoodType type;
    public float lifetime = 10f; // Food will be destroyed after 10 seconds if not eaten

    void Start()
    {
        Destroy(gameObject, lifetime);
    }
}

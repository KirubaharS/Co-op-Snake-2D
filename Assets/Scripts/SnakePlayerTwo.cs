using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SnakePlayerTwo : Snake
{
    protected override void HandleInput()
    {
        if (Input.GetKey(KeyCode.RightArrow) && currentDirection != Vector2.left)
            nextDirection = Vector2.right;
        else if (Input.GetKey(KeyCode.DownArrow) && currentDirection != Vector2.up)
            nextDirection = Vector2.down;
        else if (Input.GetKey(KeyCode.LeftArrow) && currentDirection != Vector2.right)
            nextDirection = Vector2.left;
        else if (Input.GetKey(KeyCode.UpArrow) && currentDirection != Vector2.down)
            nextDirection = Vector2.up;

    }

    protected override void HandleDeath()
    {
        HandleGameOver();
    }
}
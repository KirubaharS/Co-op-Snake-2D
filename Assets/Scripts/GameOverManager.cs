using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    [SerializeField] private GameObject gameOverObject;
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private string menuSceneName = "MainMenu";

    void Start()
    {
        gameOverObject.SetActive(false);
    }

    public void ShowGameOverScreen(string message)
    {
        if (!string.IsNullOrEmpty(message))
        {
            gameOverText.text = message;
        }
        else
        {
            gameOverText.text = ""; // Clear the text if the message is empty
        }
        gameOverObject.SetActive(true);
        Time.timeScale = 0f; // Pause the game
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; // Resume the game
        gameOverObject.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnToMenu()
    {
        Time.timeScale = 1f; // Resume the game
        SceneManager.LoadScene(menuSceneName); // Make sure you have a scene named "MenuScene"
    }

    public void Quit()
    {
        Application.Quit();
    }
}
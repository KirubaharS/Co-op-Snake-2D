using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    public void Singleplayer()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
    }

    public void Multiplayer()
    {
        SceneManager.LoadScene("Multiplayer");
    }
    // Update is called once per frame
    public void Quit()
    {
        Application.Quit();
    }
}

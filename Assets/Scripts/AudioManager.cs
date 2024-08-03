using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource SFXSource;

    public AudioClip background;
    public AudioClip eatSound;
    public AudioClip gameOverSound;
    public AudioClip powerUpSound;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            // Optional: Uncomment to keep AudioManager across scene loads
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (musicSource == null || SFXSource == null)
        {
            Debug.LogError("AudioSource components missing on AudioManager GameObject.");
        }
    }

    private void Start()
    {
        musicSource.clip = background;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
        {
            SFXSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning("Attempted to play a null AudioClip.");
        }
    }
}
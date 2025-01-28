using UnityEngine;

/// <summary>
/// Manages background music for a specific scene.
/// </summary>
public class BackgroundMusic : MonoBehaviour
{
    public AudioClip musicClip; // Assign this in the Inspector

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = musicClip; // Assign the AudioClip to the AudioSource
        audioSource.loop = true; // Set the music to loop
        audioSource.Play(); // Start playing the music
    }
}

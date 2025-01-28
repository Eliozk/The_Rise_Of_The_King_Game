using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages video playback including play, skip functionality, and event handling for video end.
/// </summary>
public class VideoManager : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public Button playButton;
    public Button skipButton;
    public GameObject videoContainer;

    // <summary>
    /// Initializes video player settings and UI component visibility.
    /// </summary>
    void Start()
    {
        videoPlayer.Prepare();
        // Set up button listeners.
        if (playButton != null)
            playButton.onClick.AddListener(PlayVideo);

        // Add listener for skip button
        skipButton.onClick.AddListener(SkipVideo);

        // Add video events
        videoPlayer.loopPointReached += OnVideoEnd;
    }

    /// <summary>
    /// Plays the video and makes the video UI visible.
    /// </summary>
    public void PlayVideo()
    {
        Debug.Log("Play video button pressed");
        videoContainer.SetActive(true);
        Debug.Log($"Video container active: {videoContainer.activeSelf},{videoContainer.name}");

        if (!videoPlayer.enabled)
        {
            videoPlayer.enabled = true;
        }
        Debug.Log($"videoPlayer.Prepare ?: {videoPlayer.isPrepared},{videoPlayer.name}");

        Debug.Log("Play video occur");
        //skipButton.gameObject.SetActive(true);
        Debug.Log($"skipButton.isActive?: {skipButton.isActiveAndEnabled},{skipButton.name}");

        videoPlayer.Play();

    }

    /// <summary>
    /// Stops the video, hides the video UI, and optionally loads a new scene.
    /// </summary>
    public void SkipVideo()
    {
        Debug.Log("Skip video button pressed");
        videoPlayer.Stop();
        videoContainer.SetActive(false);
        //skipButton.gameObject.SetActive(false);

        // Optional: Load another scene immediately when skipping
        SceneManager.LoadScene("OpeningScene"); // Change "OpeningScene" to your desired scene
    }
    /// <summary>
    /// Handles actions to take when the video ends naturally.
    /// </summary>
    /// <param name="vp">The video player that reached the end of the video.</param>

    void OnVideoEnd(VideoPlayer vp)
    {
        Debug.Log("Video ended");
        videoContainer.SetActive(false);
        skipButton.gameObject.SetActive(false);

        // Load the next scene
        SceneManager.LoadScene("OpeningScene"); // Change "OpeningScene" to your desired scene
    }
}

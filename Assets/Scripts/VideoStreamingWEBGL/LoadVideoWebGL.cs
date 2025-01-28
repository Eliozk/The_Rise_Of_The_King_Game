using UnityEngine;

public class LoadVideoWebGL : MonoBehaviour
{
    public UnityEngine.Video.VideoPlayer videoPlayer; // VideoPlayer component reference
    [SerializeField] private string VideoName; // Name of the video file to be played

    void Start()
    {
        // Construct the path to the video based on the platform
        string videoPath = System.IO.Path.Combine(Application.streamingAssetsPath, VideoName);
        
        // Platform check
        #if UNITY_WEBGL
        // WebGL uses a direct URL (e.g., HTTP)
        videoPlayer.url = videoPath;
        Debug.Log("WebGL Video Path: " + videoPath);
        #else
        // For other platforms, prepend "file://" to the path
        videoPlayer.url = "file://" + videoPath;
        Debug.Log("Local Video Path: " + "file://" + videoPath);
        #endif

        // Prepare the video
        videoPlayer.Prepare();

        // Play the video once it is ready
        videoPlayer.prepareCompleted += (vp) => {
            vp.Play();
            Debug.Log("Video started playing.");
        };
    }
}

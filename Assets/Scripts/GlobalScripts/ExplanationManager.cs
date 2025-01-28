using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages the display and control of explanation slides.
/// </summary>
public class ExplanationManager : MonoBehaviour
{
    [Header("Explanation Settings")]
    [Tooltip("List of explanation slides to display.")]
    [SerializeField]
    private GameObject[] explanationSlides;

    [Tooltip("The current slide index to start the explanation.")]
    [SerializeField]
    private int currentSlideIndex; // Tracks the current slide index
    /// <summary>
    /// Updates the array of explanation slides based on the current slides found in the scene.
    /// </summary>
    public void UpdateExplanationSlides()
    {
        Transform slidesParent = GameObject.Find("ExplanationSlides")?.transform;
        if (slidesParent != null)
        {
            explanationSlides = new GameObject[slidesParent.childCount];
            for (int i = 0; i < slidesParent.childCount; i++)
            {
                explanationSlides[i] = slidesParent.GetChild(i).gameObject;
            }
            Debug.Log("Explanation slides updated after reloading IntroScene and amount of slides?--> "+explanationSlides.Length );
        }
        else
        {
            Debug.LogError("ExplanationSlides object not found in the scene.");
        }
    }

    /// <summary>
    /// Starts the explanation by showing the first slide and setting up initial conditions.
    /// </summary>
    public void ShowExplanation()
    {
        if (explanationSlides != null && explanationSlides.Length > 0)
        {
            currentSlideIndex = 0;  // Ensure we start at the first slide
            UpdateSlideVisibility();
            Debug.Log("Explanation started.");
        }
        else
        {
            Debug.LogWarning("Explanation slides are not assigned in the Inspector.");
        }
    }

    /// <summary>
    /// Advances to the next slide or ends the explanation if it was the last slide.
    /// </summary>
    public void NextSlide()
    {
        //Current slides starts from 0 so we run until length-1.
        if (currentSlideIndex < explanationSlides.Length - 1)
        {
            currentSlideIndex++;
            UpdateSlideVisibility();
        }
        else
        {
            EndExplanation();
        }
    }

    /// <summary>
    /// Skips the remaining slides and immediately ends the explanation.
    /// </summary>
    public void SkipExplanation()
    {
        Debug.Log("Explanation skipped.");
        EndExplanation();
    }

    /// <summary>
    /// Ends the explanation, hides all slides, and optionally loads the intro scene.
    /// </summary>
    private void EndExplanation()
    {
        foreach (var slide in explanationSlides)
        {
            slide.SetActive(false);
        }
        currentSlideIndex = 0; // Reset to start for next time explanation is triggered
    }

    /// <summary>
    /// Updates the visibility of slides, showing only the current one.
    /// </summary>
    private void UpdateSlideVisibility()
    {
        for (int i = 0; i < explanationSlides.Length; i++)
        {
            explanationSlides[i].SetActive(i == currentSlideIndex);
        }
        //For nicer format we start debug from 1 and not 0 therefore currentindex+1.
        Debug.Log($"Displaying slide {currentSlideIndex + 1}/{explanationSlides.Length}");
        Debug.Log($"Slides length--> "+ explanationSlides.Length);
    }
}

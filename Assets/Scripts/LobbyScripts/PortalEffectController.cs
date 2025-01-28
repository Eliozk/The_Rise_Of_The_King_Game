using System.Collections;
using UnityEngine;

/// <summary>
/// Handles the magical portal effect, making it expand and move to a target position.
/// </summary>
public class PortalEffectController : MonoBehaviour
{
    [SerializeField] private GameObject portalEffect; // The portal effect object
    [SerializeField] private Transform targetPosition; // Target position for the portal to move and expand to
    [SerializeField] private float expandSpeed; // Speed of portal expansion
    [SerializeField] private float maxScale; // Maximum scale for the portal effect

    private bool isPortalActive = false; // Prevents multiple activations of the portal effect

    /// <summary>
    /// Activates the portal effect, causing it to move and expand towards the target position.
    /// </summary>
    public void ActivatePortal()
    {
        if (!isPortalActive)
        {
            isPortalActive = true;
            Debug.Log("Activating portal effect...");
            // Ensure the portal effect is active
            portalEffect.SetActive(true);

            StartCoroutine(ExpandPortal());
        }
    }

    /// <summary>
    /// Coroutine to gradually move and expand the portal effect towards the target position.
    /// </summary>
    private IEnumerator ExpandPortal()
    {
        // Gradually move and expand the portal
        Vector3 targetScale = new Vector3(maxScale, maxScale, maxScale);

        while (portalEffect.transform.localScale.x < maxScale)
        {
            // Move portal towards the target position
            portalEffect.transform.position = Vector3.Lerp(portalEffect.transform.position, targetPosition.position, expandSpeed * Time.deltaTime);

            // Expand portal size
            portalEffect.transform.localScale = Vector3.Lerp(portalEffect.transform.localScale, targetScale, expandSpeed * Time.deltaTime);

            yield return null;
        }
    }
}

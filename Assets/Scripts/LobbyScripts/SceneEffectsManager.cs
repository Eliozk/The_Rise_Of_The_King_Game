using System.Collections.Generic;
using UnityEngine;

public class SceneEffectsManager : MonoBehaviour
{
    [Header("Effects List")]
    [SerializeField] private List<GameObject> effects = new List<GameObject>();

    private bool isActive = true;

    private void Start()
    {
        ActivateEffects();
    }

    private void Update()
    {
        if (!isActive) return;
    }

    public void ActivateEffects()
    {
        foreach (var effect in effects)
        {
            if (effect != null) effect.SetActive(true);
        }
    }

    public void DeactivateEffects()
    {
        foreach (var effect in effects)
        {
            if (effect != null) effect.SetActive(false);
        }
        isActive = false;
    }

    public void ToggleEffects()
    {
        isActive = !isActive;

        if (isActive)
        {
            ActivateEffects();
        }
        else
        {
            DeactivateEffects();
        }
    }
}

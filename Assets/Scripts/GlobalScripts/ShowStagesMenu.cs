using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

[System.Serializable]
public class StageButton
{
    public Button button; // The button component
    public string sceneName; // Scene name to load when the button is clicked
}

public class ShowStagesMenu : MonoBehaviour
{
    public GameObject stagesMenu; // The panel or image that contains the buttons
    public Button closeButton; // The close button to hide the menu
    public List<StageButton> stageButtons; // List of buttons and their corresponding scenes

    void Start()
    {
        closeButton.onClick.AddListener(CloseMenu);

        foreach (StageButton stageButton in stageButtons)
        {
            stageButton.button.onClick.AddListener(delegate { LoadStage(stageButton.sceneName); });
        }
    }

    public void ShowMenu()
    {
        stagesMenu.SetActive(true);
    }

    public void CloseMenu()
    {
        stagesMenu.SetActive(false);
    }

    public void LoadStage(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}

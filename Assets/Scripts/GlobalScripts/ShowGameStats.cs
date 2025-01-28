using UnityEngine;
using TMPro;

public class ShowGameStats : MonoBehaviour
{
    [Header("UI Components")]
    public GameObject statsPanel; // Panel containing the text objects
    public TMP_Text[] stageTexts; // Array of TextMeshPro objects for each stage

    [Header("Scriptable Object")]
    public GameStats gameStats; // Assign your GameStats ScriptableObject here

    private bool isPanelVisible; // Boolean to track visibility state of the panel

    private void Start()
    {
        if (gameStats == null)
        {
            Debug.LogError("[ShowGameStats] gameStats not assigned!");
            return;
        }
        HideStats();
    }

    public void ToggleStatsDisplay()
    {
        if (isPanelVisible)
        {
            HideStats();
        }
        else
        {
            ShowStats();
        }
    }

    private void ShowStats()
    {
        int textIndex = 0; // Initialize text index for accessing the stageTexts array

        foreach (var game in gameStats.games)
        {
            int stageIndex = 1; // Start stage numbering from 1
            foreach (var stage in game.stages)
            {
                if (textIndex < stageTexts.Length)
                {
                    string stageName = $"שלב {stageIndex++}";
                    string currentScore = $"ניקוד נוכחי: " + ReverseHebrew(stage.score.ToString());
                    string bestScore = $"ניקוד מיטבי: " + ReverseHebrew(stage.bestScore.ToString());
                    string currentTime = $"זמן נוכחי: " + FormatTimeForDisplay(stage.time);
                    string bestTimeText = FormatTimeForDisplay(stage.bestTime);

                    string displayText = "";
                    if (game.gameName != "DefendGame")
                    {
                        displayText = $"{stageName}: {currentScore}, {bestScore}, {currentTime}, זמן מיטבי: {bestTimeText}";
                    }
                    else
                    {
                        displayText = $"{stageName}: {currentScore}, {bestScore}";
                    }
                    displayText = ReverseHebrew(displayText);
                    stageTexts[textIndex].text = displayText;
                    textIndex++;
                }
            }
        }

        statsPanel.SetActive(true);
        isPanelVisible = true;
    }

    private string GetGameDisplayName(string gameName)
    {
        switch (gameName)
        {
            case "MazeGame":
                return "משחק המבוך";
            case "ArrangeGame":
                return "משחק סידור הממלכה";
            case "DefendGame":
                return "משחק ההגנה";
            default:
                return "משחק לא מוגדר";
        }
    }

    private string ReverseHebrew(string input)
    {
        char[] chars = input.ToCharArray();
        System.Array.Reverse(chars);
        return new string(chars);
    }

    private string FormatTimeForDisplay(float time)
    {
        return time != float.MaxValue ? ReverseHebrew(time.ToString("F2")) + " שניות" : "0 שניות";
    }

    private void HideStats()
    {
        statsPanel.SetActive(false);
        isPanelVisible = false;
    }
}

using UnityEngine;
using TMPro; // Namespace for TextMeshPro

public class ShowScore : MonoBehaviour
{
    public TextMeshProUGUI scoreText; // Reference to the TextMeshPro UI element for score
    public TextMeshProUGUI timeText; // Reference to the TextMeshPro UI element for time
    public bool addText = false;

    void Update()
    {
        // Update the score text
        if(scoreText!=null)
            scoreText.text = (addText?"Score: ":"") + GameManager.Instance.gameScoreKeeper.totalScore.ToString();

        // Update the elapsed time text
        // Assuming you have a way to calculate elapsed time in your GameManager
        if(timeText!=null)
            timeText.text = (addText?"Time: ":"") + FormatTime(GameManager.Instance.gameScoreKeeper.elapsedTime);
    }

    // Format the elapsed time into a minutes:seconds format
    private string FormatTime(float timeInSeconds)
    {
        int minutes = (int)timeInSeconds / 60;
        int seconds = (int)timeInSeconds % 60;
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}

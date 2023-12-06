using UnityEngine;
using TMPro; // Namespace for TextMeshPro

public class ShowScore : MonoBehaviour
{
    public TextMeshProUGUI scoreText; // Reference to the TextMeshPro UI element for score
    public TextMeshProUGUI highScoreText; // Reference to the TextMeshPro UI element for high score
    public TextMeshProUGUI timeText; // Reference to the TextMeshPro UI element for time
    public bool addText = false;
    public CanvasGroup canvasGroup;
    float counter = 0;
    public float canvasFadeTime;

    private const string HighScoreKey = "HighScore"; // Key to store the high score in PlayerPrefs

    void OnEnable()
    {
        counter = 0;
        UpdateHighScoreDisplay(); // Update high score display when enabled
    }

    void Update()
    {
        if (counter < 1)
        {
            counter += Time.deltaTime / canvasFadeTime;
        }

        if (canvasGroup != null)
            canvasGroup.alpha = counter;

        // Update the score text
        if (scoreText != null){
            int finalScore = GameManager.Instance.gameScoreKeeper.totalScore;
            scoreText.text = (addText ? "Score: " : "") + finalScore.ToString();
        }

        // Update the high score text
        if (highScoreText != null)
            highScoreText.text = (addText ? "High Score: " : "") + PlayerPrefs.GetInt(HighScoreKey+GlobalSettings.randomSeed, 0).ToString();

        // Update the elapsed time text
        if (timeText != null)
            timeText.text = (addText ? "Time: " : "") + FormatTime(GameManager.Instance.gameScoreKeeper.elapsedTime);
    }

    // Format the elapsed time into a minutes:seconds format
    private string FormatTime(float timeInSeconds)
    {
        int minutes = (int)timeInSeconds / 60;
        int seconds = (int)timeInSeconds % 60;
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void UpdateHighScoreDisplay()
    {
        if (highScoreText != null)
            highScoreText.text = (addText ? "High Score: " : "") + PlayerPrefs.GetInt(HighScoreKey+GlobalSettings.randomSeed, 0).ToString();
    }
}

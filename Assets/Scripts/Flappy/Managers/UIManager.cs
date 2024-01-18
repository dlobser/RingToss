using UnityEngine;
using UnityEngine.UI; // Include this if you are using standard UI Text
using TMPro; // Uncomment this if you are using TextMeshPro

namespace Quilt
{
    public class UIManager : Manager
    {
        public TextMeshProUGUI scoreText; // Change this to 'public TextMeshProUGUI scoreText;' if using TextMeshPro
        private ScoreManager scoreManager;

        void Start()
        {
            // Find the ScoreManager in the scene (assuming there's only one)
            scoreManager = FindObjectOfType<ScoreManager>();
        }

        void Update()
        {
            if (scoreManager != null)
            {
                // Update the UI text with the current score
                scoreText.text = "Score: " + scoreManager.totalScore;
            }
        }
    }
}

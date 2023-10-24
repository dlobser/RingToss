using UnityEngine;

public enum CustomTag
{
    Boundary,
    Target,
    // Add more custom tags as needed
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int score;
    public bool levelFinished = false;
    public int totalTargetsInLevel;

    // public Transform levelsParent; // Reference to the "Levels" GameObject
    public RandomLevelGenerator levelGenerator;
    GameObject root;
    public int currentLevelIndex = 0; // Index of the current level
    public int maxLevels = 100;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        score = 0;
        // levelsParent = GameObject.Find("Levels").transform; // Assuming the "Levels" object exists
        // ShowLevel(currentLevelIndex); // Show the initial level

        root = levelGenerator.GenerateLevel();
        LevelStart();
    }

    public void IncrementScore()
    {
        score++;
        CheckLevelFinished();
    }

    public int GetScore()
    {
        return score;
    }

    public void LevelStart()
    {
        totalTargetsInLevel = 0;
        // Find all GameObjects with the 'Target' tag
        Target[] targets = root.GetComponentsInChildren<Target>();// FindObjectsOfType<Target>();
        foreach (Target t in targets)
        {
            if (t.customTag == CustomTag.Target)
                totalTargetsInLevel++;
        }
        print(targets.Length);
    }

    public void EndGame()
    {
        // Implement game over logic
    }

    public void CheckLevelFinished()
    {
        // If the score matches the number of 'Target' objects, set 'levelFinished' to true
        if (score >= totalTargetsInLevel)
        {
            levelFinished = true;
            ShowNextLevel();
        }
    }

    private void ShowLevel(int levelIndex)
    {
        // for (int i = 0; i < levelsParent.childCount; i++)
        // {
        //     levelsParent.GetChild(i).gameObject.SetActive(i == levelIndex);
        // }
    }

    private void ShowNextLevel()
    {
        currentLevelIndex++;
        if (currentLevelIndex < maxLevels)
        {
            // ShowLevel(currentLevelIndex);
            root = levelGenerator.GenerateLevel();
            // ResetTargets();
            LevelStart();
            levelFinished = false;
            score = 0;
        }
        else
        {
            // All levels are finished, implement game completion logic
        }
    }

    private void ResetTargets()
    {
        // Reset the number of targets for the next level
        totalTargetsInLevel = 0;
        // Implement any other logic for resetting the level
    }
}


//using UnityEngine;

//public enum CustomTag
//{
//    Boundary,
//    Target,
//    // Add more custom tags as needed
//}

//public class GameManager : MonoBehaviour
//{
//    public static GameManager instance;

//    public int score;
//    public bool levelFinished = false;
//    int totalTargetsInLevel;

//    private void Awake()
//    {
//        instance = this;
//    }

//    private void Start()
//    {
//        score = 0;
//        LevelStart();
//    }

//    public void IncrementScore()
//    {
//        score++;
//        CheckLevelFinished();
//    }

//    public int GetScore()
//    {
//        return score;
//    }

//    public void LevelStart()
//    {
//        totalTargetsInLevel = 0;
//        // Find all GameObjects with the 'Target' tag
//        Target[] targets = FindObjectsOfType<Target>();
//        foreach(Target t in targets)
//        {
//            if (t.customTag == CustomTag.Target)
//                totalTargetsInLevel++;
//        }

//    }

//    public void EndGame()
//    {
//        // Implement game over logic
//    }

//    public void CheckLevelFinished()
//    {

//        // If the score matches the number of 'Target' objects, set 'levelFinished' to true
//        if (score >= totalTargetsInLevel)
//        {
//            levelFinished = true;
//        }
//    }
//}

////using UnityEngine;

////public enum CustomTag
////{
////    Boundary,
////    Target,
////    // Add more custom tags as needed
////}


////public class GameManager : MonoBehaviour
////{
////    public static GameManager instance;

////    public int score;



////    private void Awake()
////    {
////        instance = this;
////    }

////    private void Start()
////    {
////        score = 0;
////    }

////    public void IncrementScore()
////    {
////        score++;
////    }

////    public int GetScore()
////    {
////        return score;
////    }

////    public void EndGame()
////    {
////        // Implement game over logic
////    }
////}

// using UnityEngine;

// public enum CustomTag
// {
//     Boundary,
//     Target,
//     // Add more custom tags as needed
// }

// public class GameManager : MonoBehaviour
// {
//     public static GameManager instance;

//     public int score;
//     public bool levelFinished = false;
//     int totalTargetsInLevel;

//     public Transform levelsParent; // Reference to the "Levels" GameObject
//     public int currentLevelIndex = 0; // Index of the current level

//     private void Awake()
//     {
//         instance = this;
//     }

//     private void Start()
//     {
//         score = 0;
//         levelsParent = GameObject.Find("Levels").transform; // Assuming the "Levels" object exists
//         ShowLevel(currentLevelIndex); // Show the initial level
//         LevelStart();
//     }

//     public void IncrementScore()
//     {
//         score++;
//         CheckLevelFinished();
//     }

//     public int GetScore()
//     {
//         return score;
//     }

//     public void LevelStart()
//     {
//         totalTargetsInLevel = 0;
//         // Find all GameObjects with the 'Target' tag
//         Target[] targets = FindObjectsOfType<Target>();
//         foreach (Target t in targets)
//         {
//             if (t.customTag == CustomTag.Target)
//                 totalTargetsInLevel++;
//         }
//     }

//     public void EndGame()
//     {
//         // Implement game over logic
//     }

//     public void CheckLevelFinished()
//     {
//         // If the score matches the number of 'Target' objects, set 'levelFinished' to true
//         if (score >= totalTargetsInLevel)
//         {
//             levelFinished = true;
//             ShowNextLevel();
//         }
//     }

//     private void ShowLevel(int levelIndex)
//     {
//         for (int i = 0; i < levelsParent.childCount; i++)
//         {
//             levelsParent.GetChild(i).gameObject.SetActive(i == levelIndex);
//         }
//     }

//     private void ShowNextLevel()
//     {
//         currentLevelIndex++;
//         if (currentLevelIndex < levelsParent.childCount)
//         {
//             ShowLevel(currentLevelIndex);
//             ResetTargets();
//             LevelStart();
//             levelFinished = false;
//             score = 0;
//         }
//         else
//         {
//             // All levels are finished, implement game completion logic
//         }
//     }

//     private void ResetTargets()
//     {
//         // Reset the number of targets for the next level
//         totalTargetsInLevel = 0;
//         // Implement any other logic for resetting the level
//     }
// }


// //using UnityEngine;

// //public enum CustomTag
// //{
// //    Boundary,
// //    Target,
// //    // Add more custom tags as needed
// //}

// //public class GameManager : MonoBehaviour
// //{
// //    public static GameManager instance;

// //    public int score;
// //    public bool levelFinished = false;
// //    int totalTargetsInLevel;

// //    private void Awake()
// //    {
// //        instance = this;
// //    }

// //    private void Start()
// //    {
// //        score = 0;
// //        LevelStart();
// //    }

// //    public void IncrementScore()
// //    {
// //        score++;
// //        CheckLevelFinished();
// //    }

// //    public int GetScore()
// //    {
// //        return score;
// //    }

// //    public void LevelStart()
// //    {
// //        totalTargetsInLevel = 0;
// //        // Find all GameObjects with the 'Target' tag
// //        Target[] targets = FindObjectsOfType<Target>();
// //        foreach(Target t in targets)
// //        {
// //            if (t.customTag == CustomTag.Target)
// //                totalTargetsInLevel++;
// //        }

// //    }

// //    public void EndGame()
// //    {
// //        // Implement game over logic
// //    }

// //    public void CheckLevelFinished()
// //    {

// //        // If the score matches the number of 'Target' objects, set 'levelFinished' to true
// //        if (score >= totalTargetsInLevel)
// //        {
// //            levelFinished = true;
// //        }
// //    }
// //}

// ////using UnityEngine;

// ////public enum CustomTag
// ////{
// ////    Boundary,
// ////    Target,
// ////    // Add more custom tags as needed
// ////}


// ////public class GameManager : MonoBehaviour
// ////{
// ////    public static GameManager instance;

// ////    public int score;



// ////    private void Awake()
// ////    {
// ////        instance = this;
// ////    }

// ////    private void Start()
// ////    {
// ////        score = 0;
// ////    }

// ////    public void IncrementScore()
// ////    {
// ////        score++;
// ////    }

// ////    public int GetScore()
// ////    {
// ////        return score;
// ////    }

// ////    public void EndGame()
// ////    {
// ////        // Implement game over logic
// ////    }
// ////}

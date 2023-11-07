using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameScreenManager : MonoBehaviour
{
    // The static singleton instance
    public static GameScreenManager Instance { get; private set; }

    // Other variables and properties remain unchanged...

    void Awake() {
        // Check if an instance already exists
        if (Instance == null) {
            // If not, set the instance to this
            Instance = this;

            // Optional: if you want the singleton to persist across scenes
            DontDestroyOnLoad(gameObject);
        } else {
            // If it already exists and it's not this instance, destroy this object
            if (Instance != this) {
                Destroy(gameObject);
            }
        }
    }

    // The rest of your class implementation...
    
    public enum Screen { Title, Tutorial, Game, End };

    public Screen screen = Screen.Title;

    public GameScreen Title;
    public GameScreen Tutorial;
    public GameScreen Game;
    public GameScreen End;

    public void ShowScreen(Screen screenType)
    {
        // First, deactivate all screens if they are not null
        if(Title != null) Title.Deactivate(); // Assuming Deactivate() is a method in GameScreen
        if(Tutorial != null) Tutorial.Deactivate(); // Assuming Deactivate() is a method in GameScreen
        if(Game != null) Game.Deactivate(); // Assuming Deactivate() is a method in GameScreen
        if(End != null) End.Deactivate(); // Assuming Deactivate() is a method in GameScreen

        // Now, activate the requested screen
        switch (screenType)
        {
            case Screen.Title:
                if(Title != null) Title.Activate(); // Assuming Activate() is a method in GameScreen
                break;
            case Screen.Tutorial:
                if(Tutorial != null) Tutorial.Activate(); // Assuming Activate() is a method in GameScreen
                break;
            case Screen.Game:
                if(Game != null) Game.Activate(); // Assuming Activate() is a method in GameScreen
                break;
            case Screen.End:
                if(End != null) End.Activate(); // Assuming Activate() is a method in GameScreen
                break;
            default:
                Debug.LogWarning("Unknown screen type: " + screenType);
                break;
        }
    }

    public void ShowScreen(string screenName)
    {
        // Try to parse the string to the Screen enum
        if (Enum.TryParse(screenName, out Screen screenType))
        {
            // First, deactivate all screens if they are not null
            if (Title != null) Title.Deactivate();
            if (Tutorial != null) Tutorial.Deactivate();
            if (Game != null) Game.Deactivate();
            if (End != null) End.Deactivate();

            // Now, activate the requested screen
            switch (screenType)
            {
                case Screen.Title:
                    if (Title != null) Title.Activate();
                    break;
                case Screen.Tutorial:
                    if (Tutorial != null) Tutorial.Activate();
                    break;
                case Screen.Game:
                    if (Game != null) Game.Activate();
                    break;
                case Screen.End:
                    if (End != null) End.Activate();
                    break;
                default:
                    Debug.LogWarning("Unknown screen type provided: " + screenName);
                    break;
            }
        }
        else
        {
            Debug.LogError("Invalid screen name provided: " + screenName);
        }
    }


}

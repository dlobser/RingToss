using UnityEngine;
using UnityEngine.Events;

public class GameManagerEventListener : MonoBehaviour
{
    public UnityEvent onGameStart;
    public UnityEvent onGameEnd;

    private void Start()
    {
        SubscribeToEvents();
    }

    private void OnDisable()
    {
        UnsubscribeFromEvents();
    }

    private void SubscribeToEvents()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameStart += HandleGameStart;
            GameManager.Instance.GameEnd += HandleGameEnd;
        }
    }

    private void UnsubscribeFromEvents()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameStart -= HandleGameStart;
            GameManager.Instance.GameEnd -= HandleGameEnd;
        }
    }

    private void HandleGameStart()
    {
        Debug.Log("Game Start Event");
        onGameStart.Invoke();
    }

    private void HandleGameEnd()
    {
        Debug.Log("Game End Event");
        onGameEnd.Invoke();
    }
}


// using UnityEngine;
// using UnityEngine.Events;

// public class GameManagerEventListener : MonoBehaviour
// {
//     public UnityEvent onGameEnd;

//     private void Start()
//     {
//         print("on enable"+ GameManager.Instance );
//         if (GameManager.Instance != null)
//         {
//             print("added handle game end");
//             GameManager.Instance.GameEnd += HandleGameEnd;
//         }
//     }

//     private void OnDisable()
//     {
//         if (GameManager.Instance != null)
//         {
//             GameManager.Instance.GameEnd -= HandleGameEnd;
//         }
//     }

//     private void HandleGameEnd()
//     {
//         Debug.Log("Game End Event");
//         onGameEnd.Invoke();
//     }
// }

using UnityEngine;
using UnityEngine.Events;

public class GameManagerEventListener : MonoBehaviour
{
    public UnityEvent onGameEnd;

    private void Start()
    {
        print("on enable"+ GameManager.Instance );
        if (GameManager.Instance != null)
        {
            print("added handle game end");
            GameManager.Instance.GameEnd += HandleGameEnd;
        }
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameEnd -= HandleGameEnd;
        }
    }

    private void HandleGameEnd()
    {
        Debug.Log("Game End Event");
        onGameEnd.Invoke();
    }
}

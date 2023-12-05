using System;
using UnityEngine;

public class MyDebug : MonoBehaviour
{
    private static MyDebug _instance;
    private static readonly object Padlock = new object();

    public bool DebugMode = true;
    public bool A;
    public bool B;
    public bool C;
    public bool D;

    // Ensure there is only one instance of this object in the scene.
    void Awake()
    {
        lock (Padlock)
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(this.gameObject); // Optional: if you want it to persist between scenes
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    public static MyDebug Instance
    {
        get
        {
            lock (Padlock)
            {
                return _instance;
            }
        }
    }

    public void Log(string message, bool flag = true)
    {
        if (flag && DebugMode)
        {
            Debug.Log(message);
        }
    }

    // Static method for easier access
    public static void Print(string message, bool flag = true)
    {
        if (Instance != null)
        {
            Instance.Log(message, flag);
        }
    }
}

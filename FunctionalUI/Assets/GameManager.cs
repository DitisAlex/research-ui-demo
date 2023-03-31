using System;
using System.Collections;
using System.Collections.Generic;
using PlayerExample;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    //Singleton
    private static GameManager _instance;
    [SerializeField] private UIDocument uiDocument;
    private UIManager _uiManagerPlayer;
    

    // Start is called before the first frame update
    void Awake()
    {
        if (_instance == null)
        {
            DontDestroyOnLoad(gameObject);
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }

        _uiManagerPlayer = uiDocument.GetComponent<UIManager>();
    }

    public static void DisplayMessage(string message, float seconds)
    {
        _instance._uiManagerPlayer.DisplayMessageOnScreen(message, seconds);
    }

    public static void ShowHitScreen(float hpChange, float seconds)
    {
        _instance._uiManagerPlayer.DisplayHitScreen(hpChange, seconds);
    }
    
    public static void GetItemAction(string itemGuid)
    {
        Debug.Log(itemGuid);
        throw new NotImplementedException();
    }

    public static void AddCoin()
    {
        _instance._uiManagerPlayer.AddCoin();
    }
}
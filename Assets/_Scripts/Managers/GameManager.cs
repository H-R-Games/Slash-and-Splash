using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject GameOverUI;


    [Header("Game Settings")]
    public bool FirstRun = true;
    private PlayerController _p;

    [Header("Delegates")]
    public Action RestartGame;
    public Action Clear;

    

    void Start()
    {
        FirstRun = true;
        _p = FindObjectOfType<PlayerController>();
        _p.OnDeath += GameOver;

        if (FirstRun)
        {
            Clear?.Invoke();
            FirstRun = false;
        }
    }

    void Update()
    {
        
    }

    private void GameOver()
    {
        GameOverUI.SetActive(true);
    }

    public void Restart()
    {
        RestartGame?.Invoke();
    }

    public void ClearGame()
    {
        Clear?.Invoke();
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
    }
}

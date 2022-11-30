using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject GameOverUI;


    private PlayerController _p;

    [Header("Delegates")]
    public Action RestartGame;

    void Start()
    {
        _p = FindObjectOfType<PlayerController>();
        _p.OnDeath += GameOver;
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
        GameOverUI.SetActive(false);
        RestartGame?.Invoke();
    }
}

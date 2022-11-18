using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreSystem : MonoBehaviour
{
    [Header("Score Settings")]
    public float Score = 0;
    public int KillEnemyScore = 10;

    [Header("Combo Settings")]
    public int ComboCount = 0;
    public float ComboMultiplier = 1;
    private float _comboMultiplierScaler = 0.2f;
    private int _minComboStart = 4;

    public float ComboDuration = 5;
    private float _clock = 0;

    [SerializeField] private TMP_Text _scoreText;


    void Start()
    {
        Score = 0;
        ComboCount = 0;
        ComboMultiplier = 1;
    }

    void Update()
    {
        _scoreText.text = "Score: " + Score.ToString() +
            "\nCombo: " + ComboCount.ToString() +
            "\nMultiplier: " + ComboMultiplier.ToString();
    }

    private void FixedUpdate()
    {
        ComboController();
    }

    public void OnEnemyKilled()
    {
        AddCombo();
        Score += (KillEnemyScore * ComboMultiplier);
    }

    private void AddCombo()
    {
        ComboCount++;
        _clock = 0;

        if (ComboCount >= _minComboStart)
        {
            ComboMultiplier = 1 + ((ComboCount - _minComboStart) * _comboMultiplierScaler);
        }
    }

    public void MissCombo()
    {  
        ComboCount = 0;
        ComboMultiplier = 1;
    }

    private void ComboController()
    {
        if (ComboCount >= _minComboStart)
        {
            // In Combo
            _clock += Time.deltaTime;
        }
        else
        {
            // Not in combo or combo ended
        }

        if (_clock > ComboDuration)
        {
            // Combo ended
            print("caca");
            _clock = 0;
            MissCombo();
        }
    }
}

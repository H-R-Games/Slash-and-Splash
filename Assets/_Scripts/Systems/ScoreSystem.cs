using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DamageNumbersPro;
using Unity.VisualScripting;

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

    public float ComboDuration = 2;
    private float _clock = 0;

    [SerializeField] private TMP_Text _scoreText;

    public DamageNumber NumberPrefab;
    public RectTransform rectParent;


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
        var num = (KillEnemyScore * ComboMultiplier);
        Score += num;

        //Spawn new popup
        DamageNumber damageNumber = NumberPrefab.Spawn(transform.position, num);
        //Set the rect parent and anchored position.
        damageNumber.SetAnchoredPosition(rectParent, new Vector2(0, 0));
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

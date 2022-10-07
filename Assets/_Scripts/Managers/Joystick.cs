using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Joystick : MonoBehaviour
{
    [Header("Canvas")]
    private RectTransform _joystickBG; // Background (El Grande)
    private RectTransform _joystickFG; // Foreground (El que mueve el jugador)

    [Header("Joystick")]
    [SerializeField] private float _radius = 150;
    private int _index = -1;


    private int _joystickIndex = -1;

    void Start()
    {
        // Setting components
        _joystickBG = GetComponent<RectTransform>();
        _joystickFG = GetComponentInChildren<Transform>().Find("JoystickFG").GetComponent<RectTransform>();
    }

    void Update()
    {
        GetFinger();
    }

    private void GetFinger()
    {
        // When no finger is on screen detect if one touches
        if (_index == -1)
        {
            _joystickFG.localPosition = Vector3.zero;

            try
            {
                for (int i = 0; i < Input.touchCount; i++)
                {
                    Touch touch = Input.GetTouch(i);

                    if (touch.phase == TouchPhase.Began && Vector2.Distance(_joystickBG.position, touch.position) < _radius)
                    {
                        _index = i;
                        break;
                    }
                }
            }
            catch
            {

            }
        } else
        {
            // Update foreground joystick position
            Touch touch = Input.GetTouch(_index);
            _joystickFG.position = touch.position;

            // Reset index when finger is removed
            if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) _index = -1;
        }
    }
}

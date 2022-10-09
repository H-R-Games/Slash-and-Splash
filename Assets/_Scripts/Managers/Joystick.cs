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
    public Vector2 Direction { get; private set; }
    public float Distance { get; private set; }


    public TMP_Text canvasText;

    void Start()
    {
        // Setting components
        _joystickBG = GetComponent<RectTransform>();
        _joystickFG = GetComponentInChildren<Transform>().Find("JoystickFG").GetComponent<RectTransform>();
    }

    void Update()
    {
        GetFinger();
        GetJoystickInfo(_joystickFG);
    }

    /// <summary>
    /// Getting / removing the finger position that is on the joystick
    /// </summary>
    private void GetFinger()
    {
        // When no finger is on screen detect if one touches
        if (_index == -1)
        {
            _joystickFG.localPosition = Vector2.zero;

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
            try
            {
                // Update foreground joystick position
                Touch touch = Input.GetTouch(_index);
                _joystickFG.position = touch.position;

                // Reset index when finger is removed
                if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) _index = -1;
            } catch
            {
                _index = -1;
            }

        }
    }

    private void GetJoystickInfo(RectTransform joystick)
    {
        Direction = (_joystickBG.position - joystick.position).normalized;
        Distance = Vector2.Distance(Vector2.zero, joystick.localPosition);

        CanvasInfoStuff();
    }

    private void CanvasInfoStuff()
    {
        canvasText.text = "Direction: " + Direction + "<br>" +
            "Distance: " + Distance;
    }
}

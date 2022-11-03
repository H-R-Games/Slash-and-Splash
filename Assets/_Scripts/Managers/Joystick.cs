using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class Joystick : Singleton<Joystick>
{
    public enum AimType { Stretch, Pull }
    [Header("Canvas")]
    private RectTransform _joystickBG; // Background (El Grande)
    private RectTransform _joystickFG; // Foreground (El que mueve el jugador)

    [Header("Joystick")]
    public AimType TypeAim;
    private float _radius = 60;
    private float _recolocateRadius = 3000;
    private int _index = -1;

    private Vector2 _defaultJoystickBGPos;

    public Vector2 Direction { get; private set; }
    public float Distance { get; private set; }

    public TMP_Text canvasText;

    void Start()
    {
        // Setting components
        _joystickBG = GetComponent<RectTransform>();
        _joystickFG = GetComponentInChildren<Transform>().Find("JoystickFG").GetComponent<RectTransform>();

        _defaultJoystickBGPos = _joystickBG.position;
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

                    if (touch.phase == TouchPhase.Began && Vector2.Distance(_joystickBG.position, touch.position) < _recolocateRadius) // && Vector2.Distance(_joystickBG.position, touch.position) < _radius
                    {
                        _joystickBG.position = touch.position;

                        _index = i;
                        break;
                    }
                }
            }
            catch
            {

            }
        }
        else
        {
            try
            {
                // Update foreground joystick position
                Touch touch = Input.GetTouch(_index);
                _joystickFG.position = touch.position;

                // Lock joystick FG movement
                var inputLock = (_joystickFG.position - _joystickBG.position) / _radius;
                if (inputLock.magnitude > 1f) inputLock.Normalize();
                _joystickFG.position = _joystickBG.position + (Vector3)inputLock * _radius;

                // Reset index when finger is removed
                if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {
                    _index = -1;
                    _joystickBG.position = _defaultJoystickBGPos;
                }
            }
            catch
            {
                _index = -1;
            }

        }
    }

    private void GetJoystickInfo(RectTransform joystick)
    {
        if (TypeAim == AimType.Pull)
        {
            Direction = ((_joystickBG.position - joystick.position).normalized) * -1;
            Distance = Vector2.Distance(Vector2.zero, joystick.localPosition);
        }
        else
        {
            Direction = (_joystickBG.position - joystick.position).normalized;
            Distance = Vector2.Distance(Vector2.zero, joystick.localPosition);
        }

        CanvasInfoStuff();
    }

    private void CanvasInfoStuff()
    {
        canvasText.text = "Direction: " + Direction + "<br>" +
            "Distance: " + Distance;
    }
}

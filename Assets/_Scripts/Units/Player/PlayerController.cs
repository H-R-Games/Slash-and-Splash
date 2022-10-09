using System.Collections;
using System.Collections.Generic;
using static UnityEngine.GraphicsBuffer;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float _moveSpeed = 5f;

    [SerializeField] private float _minDashRange = 3f;
    [SerializeField] private float _maxDashRange = 9f;
    [SerializeField] private float _dashRange;
    [SerializeField] private bool _canDash = true;
    [SerializeField] private float _dashICD = 0.75f;

    [SerializeField] private Vector2 _finalPosition = Vector2.zero;

    [SerializeField] [Range(0.0f, 1.0f)] private float _slowmotionValue;

    [Header("Components")]
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private BoxCollider2D _boxCollider;

    [Header("Joystick")]
    [SerializeField] public Joystick JoystickScript;
    [SerializeField] private Vector2 _directionJoystick = Vector2.zero;

    [SerializeField] private bool _isAiming = false;

    [Header("References")]
    [SerializeField] private bool _inDash = false;

    void Start()
    {
        // Setting components
        _lineRenderer = GetComponent<LineRenderer>();
        _rb = GetComponent<Rigidbody2D>();
        _boxCollider = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        AimDash();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy" && _inDash)
        {
            print("JIJIJIA");
            _canDash = true;
        }
    }

    #region Aim Dash
    private void AimDash()
    {
        if (JoystickScript.Distance != 0 && _canDash == true)
        {
            _isAiming = true;

            // Calculate dash direction
            CalcDashRange();
            _finalPosition = JoystickScript.Direction * _dashRange + (Vector2)transform.position;
            _directionJoystick = JoystickScript.Direction;

            // Draw line renderer
            _lineRenderer.SetPosition(0, transform.position);
            _lineRenderer.SetPosition(1, _finalPosition);
        } else
        {
            _isAiming = false;
            Time.timeScale = 1; // Reseting time scale

            Dash();
            // Stop drawing line renderer
            _lineRenderer.SetPosition(0, transform.position);
            _lineRenderer.SetPosition(1, transform.position);
        }

        AimDashSlowmotion();
    }

    private void AimDashSlowmotion()
    {
        if (!_isAiming) return;

        Time.timeScale = _slowmotionValue;
    }
    #endregion

    /// <summary>
    /// Calculate the range of the dash depending how much the player is streaching the joystick
    /// </summary>
    private void CalcDashRange()
    {
        float max = 40;
        float min = 1;

        float joystickDistance = Mathf.Clamp(JoystickScript.Distance, min, max);
        float jsD_to_dashD = Helpers.FromRangeToRange(joystickDistance, min, max, _minDashRange, _maxDashRange);

        _dashRange = Mathf.Clamp(jsD_to_dashD, _minDashRange, _maxDashRange);
    }

    #region Dash
    /// <summary>
    /// When stop aiming SHOOT!
    /// </summary>
    private void Dash()
    {
        /* When joystick distance is 0 it means that the player is not aiming
           And if the _dash range is not equal to 0 it means that the player has aimed
           Then shoot and reset dash range to avoid dashing all the time while not aiming
        */
        if (JoystickScript.Distance == 0 && _dashRange != 0 && _canDash == true && _inDash == false)
        {
            // Do Dash
            StartCoroutine(DoDash(transform.position, _finalPosition, .1f, _directionJoystick));

            // Reseting vars
            _canDash = false;
            _dashRange = 0;
            StartCoroutine(ICD());
        }
    }

    /// <summary>
    /// Internal cooldown to stop things being spammed
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    private IEnumerator ICD()
    {
        yield return new WaitForSeconds(_dashICD);
        _canDash = true;
    }

    private IEnumerator DoDash(Vector2 init, Vector2 final, float time, Vector2 direction)
    {
        _boxCollider.size = new Vector2(2, 2);
        _rb.velocity = Vector2.zero;
        Vector2 curr = this.transform.position;
        float t = 0f;

        while (t < 1)
        {
            t += Time.deltaTime / time;
            //this.transform.position = Vector2.Lerp(curr, final, t);
            _rb.MovePosition(Vector2.Lerp(curr, final, t));
            yield return null;
        }

        float vel = Vector2.Distance(init, final) / time;
        _rb.AddForce((vel/10) * direction, ForceMode2D.Impulse);
        _boxCollider.size = new Vector2(1, 1);
    }
    #endregion
}

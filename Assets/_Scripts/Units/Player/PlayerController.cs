using System.Collections;
using System.Collections.Generic;
using static UnityEngine.GraphicsBuffer;
using UnityEngine;
using System;
using System.Reflection;
using TMPro;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float _moveSpeed = 5f;

    [SerializeField] private float _minDashRange = 3f;
    [SerializeField] private float _maxDashRange = 9f;
    [SerializeField] private float _dashRange;
    [SerializeField] private bool _canDash = true;
    [SerializeField] private float _dashDuration = 0.1f;
    [SerializeField] private float _dashICD = 0.75f;

    private bool _killedEnemy = false;

    public int MaxDashes = 3;
    [SerializeField] private int _dashes = 3;

    [SerializeField] private Vector2 _finalPosition = Vector2.zero;

    [SerializeField] [Range(0.0f, 1.0f)] private float _slowmotionValue;

    private Vector3 _lastFramePosition;

    private Vector3 _defaultScale = new Vector3 { x = 0.5f, y = 0.5f, z = 1.0f };
    private Vector3 _stretchScale = new Vector3 { x = 0.5f, y = 0.3f, z = 1.0f };
    private Vector3 _dashScale = Vector3.zero;

    private float _aimAssistAngle = 20f;
    private Vector2 _aimAssistTarget = Vector2.zero;

    [Header("Special Skill")]
    public Button SpecialSkillButton;
    public int Kills = 40;
    private bool _specialSkillReady = false;
    private bool _specialSkillActive = false;

    [Header("Components")]
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private BoxCollider2D _boxCollider;

    [SerializeField] private LayerMask _floorLayer;
    [SerializeField] private LayerMask _enemyLayer;

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

        JoystickScript = Joystick.Instance;

        _dashes = MaxDashes;
    }

    void Update()
    {
        DashesController();
        MegaBlasterButton();
        _lineRenderer.SetPosition(0, transform.position);

    }

    private void FixedUpdate()
    {
        AimDash();
        RotateCharacter();
        DeformCharacter();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy" && _inDash)
        {
            _canDash = true;
            _killedEnemy = true;
            ResetDases();

            collision.GetComponent<EnemyController>().Kill();
        }
    }

    #region Aim Dash
    private void AimDash()
    {
        if (JoystickScript.Distance != 0 && _canDash == true && _dashes >= 1 && !_specialSkillActive)
        {   
            _isAiming = true;
            //StopCoroutine(ReturnToDefaultScale(0.0f));
            _aimAssistTarget = GetNearestEnemy();

            // Calculate dash direction
            CalcDashRange();
            _finalPosition = JoystickScript.Direction * _dashRange + (Vector2)transform.position;
            var temp = _aimAssistTarget != Vector2.zero ? _aimAssistTarget : _finalPosition;
            _finalPosition = temp;
            _directionJoystick = JoystickScript.Direction;

            // Draw line renderer
            //_lineRenderer.SetPosition(0, transform.position);
            _lineRenderer.SetPosition(1, _finalPosition);
        } else
        {
            if (_specialSkillActive) print("JIJIJA");

            _isAiming = false;
            Time.timeScale = 1; // Reseting time scale

            Dash();
            // Stop drawing line renderer
            _lineRenderer.SetPosition(0, transform.position);
            _lineRenderer.SetPosition(1, transform.position);
        }

        // When aiming
        AimDashSlowmotion();
    }

    private void AimDashSlowmotion()
    {
        if (!_isAiming) return;

        // Reducing timescale when aiming
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

        Rigidbody2D rigid = GetComponent<Rigidbody2D>();

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
           And the special skill has not been activated
        */
        if (JoystickScript.Distance == 0 && _dashRange != 0 && _canDash && !_inDash && !_specialSkillActive)
        {
            // Do Dash
            StartCoroutine(DoDash(transform.position, _finalPosition, _dashDuration, _directionJoystick, _dashRange));
            _dashes--;

            // Reseting vars
            _canDash = false;
            _dashRange = 0;
            StartCoroutine(ICD());
        } else if (_specialSkillActive) StartCoroutine(DoSuperMegaBlaster());


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

    private IEnumerator DoDash(Vector2 init, Vector2 final, float time, Vector2 direction, float dashForce)
    {
        _lineRenderer.SetPosition(0, transform.position);
        _lineRenderer.SetPosition(1, transform.position);
        Time.timeScale = 1; // Reseting time scale

        _boxCollider.size = new Vector2(2, 2);
        _rb.velocity = Vector2.zero;
        Vector2 curr = this.transform.position;
        float t = 0f;
        _inDash = true;

        while (t < 1)
        {
            t += Time.deltaTime / time;
            //this.transform.position = Vector2.Lerp(curr, final, t);
            _rb.MovePosition(Vector2.Lerp(curr, final, t));
            yield return null;
        }

        if (_killedEnemy)
        {
            curr = this.transform.position;
            t = 0f;
            final = direction * dashForce + (Vector2)transform.position;

            while (t < 1)
            {
                t += Time.deltaTime / time;
                //this.transform.position = Vector2.Lerp(curr, final, t);
                
                _rb.MovePosition(Vector2.Lerp(curr, final, t));
                yield return null;
            }

            _killedEnemy = false;
        }
        _inDash = false;
        float vel = Vector2.Distance(init, final) / time;
        _rb.AddForce((vel/10) * direction, ForceMode2D.Impulse);
        _boxCollider.size = new Vector2(1, 1);
    }

    private void DashesController()
    {
        if (Grounded() && _dashes != MaxDashes) ResetDases();
    }

    public void ResetDases()
    {
        _dashes = MaxDashes;
    }

    public void SuperMegaSuperBlaster()
    {
        if (_isAiming) _specialSkillActive = true;
    }

    private void MegaBlasterButton()
    {
        if (_isAiming) SpecialSkillButton.interactable = true;
        else SpecialSkillButton.interactable = false;
    }

    private IEnumerator DoSuperMegaBlaster()
    {
        _specialSkillActive = false;
        
        // StartCoroutine(DoSuperMegaBlaster());
        _inDash = true;
        
        _boxCollider.size = new Vector2(2, 2);
        _rb.velocity = Vector2.zero;

        for (int i = 0; i < Kills; i++)
        {
            // Detect enemies in a radius of 100 units
            Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, 10, _enemyLayer);
            if (enemies.Length == 0) break;
            
            print(i);
            Vector2 curr = this.transform.position;
            Vector2 final = enemies[0].transform.position;
            float t = 0f;
            while (t < 1)
            {
                t += Time.deltaTime / _dashDuration;
                //this.transform.position = Vector2.Lerp(curr, final, t);
                _rb.MovePosition(Vector2.Lerp(curr, final, t));
                yield return null;
            }
            //enemiesList.Remove(enemies[i]);
        }
        
        _boxCollider.size = new Vector2(1, 1);
        _inDash = false;
    }
    
    
    #endregion

    private void RotateCharacter()
    {
        if (_isAiming)
        {
            // Rotating the player towards the direction is looking
            Vector2 dir = _directionJoystick;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        } else
        {
            // Is touching ground
            if (Grounded())
            {
                transform.rotation = Quaternion.AngleAxis(0, Vector3.forward);
            } else
            {
                var dir = (transform.position - _lastFramePosition).normalized;
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }

            _lastFramePosition = transform.position;
        }

    }

    private void DeformCharacter()
    {
        if (_isAiming)
        {
            float max = 40;
            float min = 1;
            transform.localScale = Vector3.Lerp(_defaultScale, _stretchScale, Helpers.FromRangeToPercentage(JoystickScript.Distance, min, max) / 100);
        } else if (_inDash && transform.localScale != _defaultScale)
        {
            if (_dashScale == Vector3.zero) StartCoroutine(ReturnToDefaultScale(_dashDuration));
        }
    }

    private IEnumerator ReturnToDefaultScale(float time)
    {
        yield return new WaitForSeconds(time * .5f);
        float t = 0f;
        _dashScale = transform.localScale;

        while (t < 1)
        {
            t += Time.deltaTime / time;
            transform.localScale = Vector3.Lerp(_dashScale, _defaultScale, t);
            yield return null;
        }

        _dashScale = Vector3.zero;
    }

    private bool Grounded()
    {
        return Physics2D.OverlapCircle(transform.position, .5f, _floorLayer);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, .5f);
        Gizmos.DrawWireSphere(transform.position, _dashRange);

        // add 15 degrees to _directionJoystick
        Vector2 dir = _directionJoystick;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        angle += _aimAssistAngle;
        dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

        // remove 15 degrees to _directionJoystick
        Vector2 dir2 = _directionJoystick;
        float angle2 = Mathf.Atan2(dir2.y, dir2.x) * Mathf.Rad2Deg;
        angle2 -= _aimAssistAngle;
        dir2 = new Vector2(Mathf.Cos(angle2 * Mathf.Deg2Rad), Mathf.Sin(angle2 * Mathf.Deg2Rad));

        Gizmos.DrawLine((Vector2)transform.position, (_directionJoystick) * _dashRange + (Vector2)transform.position);
        Gizmos.DrawLine((Vector2)transform.position, (dir) * _dashRange + (Vector2)transform.position);
        Gizmos.DrawLine((Vector2)transform.position, (dir2) * _dashRange + (Vector2)transform.position);
    }

    private Vector2 GetNearestEnemy()
    {
        // add 15 degrees to _directionJoystick
        Vector2 dir = _directionJoystick;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        angle += _aimAssistAngle;
        dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

        // remove 15 degrees to _directionJoystick
        Vector2 dir2 = _directionJoystick;
        float angle2 = Mathf.Atan2(dir2.y, dir2.x) * Mathf.Rad2Deg;
        angle2 -= _aimAssistAngle;
        dir2 = new Vector2(Mathf.Cos(angle2 * Mathf.Deg2Rad), Mathf.Sin(angle2 * Mathf.Deg2Rad));

        // Detect all enemies in a _dashRange radius
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, 10, _enemyLayer);
        List<GameObject> inEnemies = new List<GameObject>();
        
        foreach (var enemy in enemies)
        {
            if (enemy.transform.position != transform.position)
            {
                // Check if the enemy is in the _aimAssistAngle range degrees angle
                if (Vector2.Angle(_directionJoystick, enemy.transform.position - transform.position) < _aimAssistAngle * 1.5)
                {
                    if (Vector2.Distance(transform.position, enemy.transform.position) < 10)
                    {
                        inEnemies.Add(enemy.gameObject);
                    }
                }
            }
        }

        // Get nearest enemy in the inEnemies list to the player
        GameObject nearestEnemy = null;
        float nearestDistance = 0;
        foreach (var enemy in inEnemies)
        {
            if (nearestEnemy == null)
            {
                nearestEnemy = enemy;
                nearestDistance = Vector2.Distance(transform.position, enemy.transform.position);
            }
            else
            {
                if (Vector2.Distance(transform.position, enemy.transform.position) < nearestDistance)
                {
                    nearestEnemy = enemy;
                    nearestDistance = Vector2.Distance(transform.position, enemy.transform.position);
                }
            }
        }

        if (nearestEnemy != null) return nearestEnemy.transform.position;
        return Vector2.zero;
    }
}

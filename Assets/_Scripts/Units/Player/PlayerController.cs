using System.Collections;
using System.Collections.Generic;
using static UnityEngine.GraphicsBuffer;
using UnityEngine;
using System;
using System.Reflection;
using TMPro;
using UnityEngine.UI;
using TMPro;
using SpriteGlow;

public class PlayerController : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private ScriptableStats _stats;
    PlayerStats _pbs = new PlayerStats();

    [SerializeField] private float _lifes = 1;
    [SerializeField] private ParticleSystem _revivePs;
    private float _invulTime = 3;
    private bool _isInvul = false;

    [SerializeField] private float _moveSpeed = 5f;

    [SerializeField] private float _minDashRange = 3f;
    [SerializeField] private float _maxDashRange = 9f;
    [SerializeField] private float _dashRange;
    [SerializeField] private bool _canDash = true;
    [SerializeField] private float _dashDuration = 0.1f;
    [SerializeField] private float _dashICD = 0.75f;
    [SerializeField] private bool _coyoteDash = false;
    [SerializeField] private float _coyoteDashTime = 0.1f;

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
    [SerializeField] private bool _specialSkillReady = true;
    private bool _specialSkillActive = false;
    private bool _inSpecialSkill = false;
    private float _specialSkillCooldown = 20f;

    [Header("Components")]
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private BoxCollider2D _boxCollider;
    private ScoreSystem _scoreSystem;
    [SerializeField] private TMP_Text _uiText;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private SpriteGlowEffect _spriteGlowEffect;


    [SerializeField] private LayerMask _floorLayer;
    [SerializeField] private LayerMask _enemyLayer;

    GameManager _gm;

    [Header("Joystick")]
    [SerializeField] public Joystick JoystickScript;
    [SerializeField] private Vector2 _directionJoystick = Vector2.zero;

    [SerializeField] private bool _isAiming = false;

    [Header("Fall")]
    [SerializeField] private bool _isFalling = false;
    [SerializeField] private float _fallTimer = 0;
    [SerializeField] private float _fallTimerMax = 5f;
    private bool _willDie = false;

    [Header("References")]
    [SerializeField] private bool _inDash = false;

    [Header("Delegate")]
    public Action OnRevive;
    public Action OnDeath;

    public Action<EnemyController> KillEnemy;

    [Header("Other")]
    private bool _isActive = true;

    private void Awake()
    {
        GetAudioComponents();
        // On play
        PlayBackgroundMusic();
    }
    
    void Start()
    {
        // Setting components
        _lineRenderer = GetComponent<LineRenderer>();
        _rb = GetComponent<Rigidbody2D>();
        _boxCollider = GetComponent<BoxCollider2D>();
        _scoreSystem = GetComponent<ScoreSystem>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteGlowEffect = GetComponent<SpriteGlowEffect>();

        JoystickScript = Joystick.Instance;
        _gm = FindObjectOfType<GameManager>();

        PlayerSpawn();
        _gm.RestartGame += PlayerSpawn;
        _gm.Clear += Clear;


        JoystickScript.SuperMegaBlasterAction += OnDashCall;
    }

    void Update()
    {
        UI();
        if (!_isActive) return;

        FallDeath();
        DashesController();
        MegaBlasterButton();
        _lineRenderer.SetPosition(0, transform.position);

    }

    private void FixedUpdate()
    {
        if (!_isActive) return;
        
        AimDash();
        LowPassFilter();
        RotateCharacter();
        DeformCharacter();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy" && (_inDash || _coyoteDash))
        {
            _canDash = true;
            _killedEnemy = true;
            ResetDases();

            _scoreSystem.OnEnemyKilled();

            collision.GetComponent<EnemyController>().Kill();
            KillEnemy?.Invoke(collision.GetComponent<EnemyController>());
        }

        if (collision.gameObject.tag == "DeadZone") KillPlayer();
    }

    #region Health and Death
    private void FallDeath()
    {
        if (_rb.velocity.y < 0)
        {
            _isFalling = true;
            _fallTimer += Time.deltaTime;

            if (_fallTimer >= _fallTimerMax)
            {
                _willDie = true;
            }
        }
        else if (_rb.velocity.y > 0)
        {
            _isFalling = false;
            _fallTimer = 0;
            _willDie = false;
        }

        if (Grounded() && !_willDie)
        {
            _isFalling = false;
            _fallTimer = 0;
            _willDie = false;
        }

        if ((_willDie && _rb.velocity.y == 0 && !_inDash) || transform.position.y <= -5)
        {
            KillPlayer();
        }
    }
    
    private IEnumerator Invulnerability()
    {
        _isInvul = true;

        yield return new WaitForSeconds(_invulTime);
        _isInvul = false;
    }

    public void KillPlayer()
    {
        if (_isInvul)
        {
            _rb.velocity = new Vector2(_rb.velocity.x, 0);
            _rb.AddForce(Vector2.up * 10f, ForceMode2D.Impulse);
            ResetDases();
            _revivePs.Play();

            OnRevive?.Invoke();
            return;
        }
        _lifes--;
        
        if (_lifes <= 0)
        {
            _isActive = false;

            OnDeath?.Invoke();

            //UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        }
        else
        {
            _rb.velocity = new Vector2(_rb.velocity.x, 0);
            _rb.AddForce(Vector2.up * 10f, ForceMode2D.Impulse);
            _revivePs.Play();

            OnRevive?.Invoke();
        }

        StartCoroutine(Invulnerability());
        
    }
    #endregion

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
    #endregion

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
            if (!_specialSkillActive || !_inSpecialSkill) StartCoroutine(DoDash(transform.position, _finalPosition, _dashDuration, _directionJoystick, _dashRange));
            _dashes--;

            // Reseting vars
            _canDash = false;
            _dashRange = 0;
            StartCoroutine(ICD());
        } else if (_specialSkillActive && !_inSpecialSkill) StartCoroutine(DoSuperMegaBlaster());


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
        if (_specialSkillActive || _inSpecialSkill) yield break;
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
        } else
        {
            // Enemy not killed
            //this.GetComponent<ScoreSystem>().MissCombo();

            StartCoroutine(CoyoteDashHelp());
        }
        
        print("DASH FINISHED");
        _inDash = false;
        float vel = Vector2.Distance(init, final) / time;
        _rb.AddForce((vel/10) * direction, ForceMode2D.Impulse);
        _boxCollider.size = new Vector2(1, 1);
    }

    private IEnumerator CoyoteDashHelp()
    {
        _coyoteDash = true;
        yield return new WaitForSeconds(_coyoteDashTime);
        _coyoteDash = false;
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
        if (_isAiming && _specialSkillReady) SpecialSkillButton.interactable = true;
        else SpecialSkillButton.interactable = false;
    }

    private IEnumerator DoSuperMegaBlaster()
    {
        StartCoroutine(SpecialSkillCooldown());

        _specialSkillActive = true;
        _inSpecialSkill = true;

        // Stop drawing line renderer
        _lineRenderer.SetPosition(0, transform.position);
        _lineRenderer.SetPosition(1, transform.position);

        Time.timeScale = 1;

        // StartCoroutine(DoSuperMegaBlaster());
        _inDash = true;
        
        _boxCollider.size = new Vector2(2, 2);
        _rb.velocity = Vector2.zero;

        Vector2 direction = Vector2.zero;
        float vel = 0;

        for (int i = 0; i < Kills; i++)
        {
            // Detect enemies in a radius of 100 units
            Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, _maxDashRange * 1.5f, _enemyLayer);
            if (enemies.Length == 0) break;
            
            // print(i);
            Vector2 curr = this.transform.position;
            Vector2 final = enemies[0].transform.position;
            
            // Direction from curr to final
            direction = (final - curr).normalized;
            vel = Vector2.Distance(curr, final) / _dashDuration;

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

        _rb.velocity = Vector2.zero;
        _rb.AddForce((vel / 10) * direction, ForceMode2D.Impulse);

        _specialSkillActive = false;
        _inSpecialSkill = false;
        _boxCollider.size = new Vector2(1, 1);
        _inDash = false;
    }
    
    private IEnumerator SpecialSkillCooldown()
    {
        _specialSkillReady = false;
        yield return new WaitForSeconds(_specialSkillCooldown);
        _specialSkillReady = true;
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
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, _maxDashRange, _enemyLayer);
        List<GameObject> inEnemies = new List<GameObject>();

        foreach (var enemy in enemies)
        {
            if (enemy.transform.position != transform.position)
            {
                // Check if the enemy is in the _aimAssistAngle range degrees angle
                if (Vector2.Angle(_directionJoystick, enemy.transform.position - transform.position) < _aimAssistAngle * 1.5)
                {
                    if (Vector2.Distance(transform.position, enemy.transform.position) < _maxDashRange)
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

    private void OnDashCall(bool on)
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
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, _maxDashRange, _enemyLayer);
        List<GameObject> inEnemies = new List<GameObject>();

        foreach (var enemy in enemies)
        {
            if (enemy.transform.position != transform.position)
            {
                // Check if the enemy is in the _aimAssistAngle range degrees angle
                if (Vector2.Angle(_directionJoystick, enemy.transform.position - transform.position) < _aimAssistAngle * 1.5)
                {
                    if (Vector2.Distance(transform.position, enemy.transform.position) < _maxDashRange)
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
    }

    #endregion

    #region Animations

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
    #endregion

    private bool Grounded()
    {
        return Physics2D.OverlapCircle(transform.position, .5f, _floorLayer);
    }

    #region Audio
    [Header("Audio")]
    [SerializeField] private AudioSource _audioSourceSound;
    [SerializeField] private AudioSource _audioSourceMusic;
    [SerializeField] private AudioLowPassFilter _lowPassFilter;
    [SerializeField] private AudioClip _dashSound;
    [SerializeField] private AudioClip _backgroundMusic;

    private void GetAudioComponents()
    {
        // Get components in children
        _audioSourceSound = transform.GetChild(1).GetComponent<AudioSource>();
        _audioSourceMusic = transform.GetChild(0).GetComponent<AudioSource>();
        _lowPassFilter = transform.GetChild(0).GetComponent<AudioLowPassFilter>();
    }

    
    private void PlayDashSound()
    {
        _audioSourceSound.PlayOneShot(_dashSound);
    }

    private void PlayBackgroundMusic()
    {
        // Loop background music
        _audioSourceMusic.loop = true;
        _audioSourceMusic.clip = _backgroundMusic;
        _audioSourceMusic.Play();
    }

    private void StopBackgroundMusic()
    {
        _audioSourceMusic.Stop();
    }

    private void LowPassFilter()
    {
        if (_isAiming)
        {
            _lowPassFilter.cutoffFrequency = 5000;
        }
        else
        {
            _lowPassFilter.cutoffFrequency = 22000;
        }
    }

    #endregion

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

    #region Stats
    private void SetPlayerStats()
    {
        _pbs.MoveSpeed = _stats.MoveSpeed;
        _pbs.MinDashRange = _stats.MinDashRange;
        _pbs.MaxDashRange = _stats.MaxDashRange;
        _pbs.DashICD = _stats.DashICD;
        _pbs.DashSpeed = _stats.DashSpeed;
        _pbs.MaxDashes = _stats.MaxDashes;
        _pbs.SlowMotionValue = _stats.SlowMotionValue;
        _pbs.SpecialSkillCooldown = _stats.SpecialSkillCooldown;
        _pbs.Lifes = _stats.Lifes;

        // Calculate stats -- Powerups


        // Set _pbs to the player
        _moveSpeed = _pbs.MoveSpeed;
        _minDashRange = _pbs.MinDashRange;
        _maxDashRange = _pbs.MaxDashRange;
        _dashICD = _pbs.DashICD;
        _dashDuration = _pbs.DashSpeed;
        MaxDashes = _pbs.MaxDashes;
        _slowmotionValue = _pbs.SlowMotionValue;
        _specialSkillCooldown = _pbs.SpecialSkillCooldown;
        _lifes = _pbs.Lifes;
    }

    #endregion

    #region Setup
    private void Clear()
    {
        _isActive = false;
        _rb.bodyType = RigidbodyType2D.Static;
    }

    private void PlayerSpawn()
    {
        _isActive = true;
        _rb.bodyType = RigidbodyType2D.Dynamic;
        SetPlayerStats();

        // Positon and scale
        transform.position = new Vector2(0, 2);
        transform.localScale = _defaultScale;

        // Skin
        _spriteRenderer.sprite = _stats.PlayerSprite;
        _spriteGlowEffect.GlowColor = _stats.SpriteGlowColor;
        _spriteGlowEffect.AlphaThreshold = _stats.AlphaThreshold;
        _spriteGlowEffect.GlowBrightness = _stats.GlowBrightness;
        _spriteGlowEffect.OutlineWidth = _stats.OutlineWidth;

        // Trail
        _lineRenderer.startColor = _stats.TrailColor;
    }
    
    #endregion

    #region Getters and Setters
    public bool GetIsAiming()
    {
        return _isAiming;
    }

    public float GetJoystickDistance()
    {
        return JoystickScript.Distance;
    }
    #endregion

    #region UI
    public void UI()
    {
        _uiText.text = "Fall time: " + _fallTimer.ToString() +
            "\nIs falling: " + _isFalling.ToString() +
            "\nWill die: " + _willDie.ToString() +
            "\n Rigidbody velocity: " + _rb.velocity.ToString() +
            "\n Rb y velocity: " + _rb.velocity.y.ToString();
    }
    #endregion
}

class PlayerStats
{
    // Movement
    public float MoveSpeed;
    
    public float MinDashRange;
    public float MaxDashRange;
    
    public float DashICD;

    public float DashSpeed;
    public int MaxDashes;
    [Range(0, 1)] public float SlowMotionValue;

    public float SpecialSkillCooldown;

    // Health
    public int Lifes;


    public PlayerStats() { }
    
    public PlayerStats(float moveSpeed, float minDashRange, float maxDashRange, float dashICD, float dashSpeed, int maxDashes, float slowMotionValue, float specialSkillCoolDown, int lifes)
    {
        MoveSpeed = moveSpeed;
        MinDashRange = minDashRange;
        MaxDashRange = maxDashRange;
        DashICD = dashICD;
        DashSpeed = dashSpeed;
        MaxDashes = maxDashes;
        SlowMotionValue = slowMotionValue;
        SpecialSkillCooldown = specialSkillCoolDown;
        Lifes = lifes;
    }
}
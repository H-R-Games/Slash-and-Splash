using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Balls : MonoBehaviour
{
    [SerializeField][Range(0, 1f)] private float _sppedColor = 0.5f;
    [SerializeField] private List<Color> _colorsList;
    [SerializeField] private bool isRandonColor;

    Vector2 _limits;
    Rigidbody2D _rb;

    int _colorIndex = 0;
    float _time = 0;

    private void Awake()
    {
        _limits = new Vector2(GiroscopeSystem.limitsCameraX, GiroscopeSystem.limitsCameraY);
        _rb = GetComponent<Rigidbody2D>();
    }

    public void StartBall()
    {
        if (isRandonColor) GetComponent<SpriteRenderer>().color = _colorsList[_colorIndex];
        _rb.velocity = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * 5;
    }

    void Update()
    {
        LimitsController();
        MoveBall();
        if (isRandonColor) ColorRandom();
    }

    private void LimitsController()
    {
        Mathf.Clamp(_rb.velocity.x, 1, 10);
        Mathf.Clamp(_rb.velocity.y, 1, 10);

        if (this.transform.position.x > _limits.x)
        {
            this.transform.position = new Vector3(_limits.x, transform.position.y, transform.position.z);
            _rb.velocity = Vector2.zero;
            _rb.AddForce(new Vector2(-1, 0) * 10, ForceMode2D.Impulse);
        }
        else if (this.transform.position.x < -_limits.x)
        {
            this.transform.position = new Vector3(-_limits.x, transform.position.y, transform.position.z);
            _rb.velocity = Vector2.zero;
            _rb.AddForce(new Vector2(1, 0) * 10, ForceMode2D.Impulse);
        }
        else if (this.transform.position.y > _limits.y)
        {
            this.transform.position = new Vector3(transform.position.x, _limits.y, transform.position.z);
            _rb.velocity = Vector2.zero;
            _rb.AddForce(new Vector2(0, -1) * 10, ForceMode2D.Impulse);
        }
        else if (this.transform.position.y < -_limits.y)
        {
            this.transform.position = new Vector3(transform.position.x, -_limits.y, transform.position.z);
            _rb.velocity = Vector2.zero;
            _rb.AddForce(new Vector2(0, 1) * 10, ForceMode2D.Impulse);
        }
    }

    private void MoveBall()
    {
        if (Input.gyro.enabled)
        {
            _rb.AddForce(new Vector2(Input.gyro.rotationRateUnbiased.y, -Input.gyro.rotationRateUnbiased.x), ForceMode2D.Impulse);
        }
    }

    private void ColorRandom()
    {
        GetComponent<SpriteRenderer>().color = Color.Lerp(GetComponent<SpriteRenderer>().color, _colorsList[_colorIndex], _sppedColor * Time.deltaTime);

        _time = Mathf.Lerp(_time, 1, _sppedColor * Time.deltaTime);

        if (_time > 0.9f)
        {
            _time = 0;
            _colorIndex++;
            _colorIndex = (_colorIndex >= _colorsList.Count) ? 0 : _colorIndex;
        }
    }
}
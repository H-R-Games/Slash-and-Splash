using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Balls : MonoBehaviour
{
    Vector2 _limits;
    Rigidbody2D _rb;

    private void Awake()
    {
        _limits = new Vector2(GiroscopeSystem.limitsCameraX, GiroscopeSystem.limitsCameraY);
        _rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        LimitsController();
        MoveBall();
    }

    public void StartBall()
    {
        _rb.velocity = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * 5;
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
}
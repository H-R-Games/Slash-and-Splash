using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CameraFollow : MonoBehaviour
{
    [Header("Camera")]
    public Transform Target;
    [Range(0, 1)] public float Smoothing;

    public float DefaultCamera = 6;
    public float AddedAimCamera = 3;
    public float MaxAddedAimCamera = 5;
    
    private float _zoomSpeed = 2f;

    [SerializeField] private float _shakeMag;
    [SerializeField] private float _shakeTime;
    private bool _isShaking = false;

    private PlayerController _p;

    void Start()
    {
        _p = Target.GetComponent<PlayerController>();

        _p.OnRevive += ShakeCamera;
        _p.OnDeath += ShakeCamera;
    }

    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        CameraMovement();
        CameraZoom();
    }

    private void CameraMovement()
    {
        if (_isShaking) return;
        transform.position = Vector3.Lerp(transform.position, new Vector3(Target.position.x, Target.position.y, transform.position.z), Smoothing);

        if (transform.position.y < 0) transform.position = Vector3.Lerp(transform.position, new Vector3(Target.position.x, 0, transform.position.z), Smoothing);
    }

    // When the player is aiming, the camera zooms out smoothly
    private void CameraZoom()
    {
        if (_p.GetIsAiming())
        {
            if (Camera.main.orthographicSize < DefaultCamera + AddedAimCamera)
            {
                Camera.main.orthographicSize += _zoomSpeed * Helpers.FromRangeToRange(_p.GetJoystickDistance(), 3, 9, DefaultCamera, DefaultCamera + AddedAimCamera) * Time.deltaTime;
                Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, DefaultCamera, DefaultCamera + AddedAimCamera);
            }
            else if (Camera.main.orthographicSize < DefaultCamera + MaxAddedAimCamera)
            {
                Camera.main.orthographicSize += _zoomSpeed * Time.deltaTime;
                Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, DefaultCamera + AddedAimCamera, DefaultCamera + MaxAddedAimCamera);
            }
        }
        else
        {
            if (Camera.main.orthographicSize > DefaultCamera)
            {
                Camera.main.orthographicSize -= _zoomSpeed * Time.deltaTime;
            }
        }
    }

    // Shake camera for a certain amount of time
    public void ShakeCamera()
    {
        StartCoroutine(Shake(_shakeTime, _shakeMag));
    }

    private IEnumerator Shake(float duration, float magnitude)
    {
        _isShaking = true;
        
        Vector3 originalPos = transform.localPosition;

        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(transform.localPosition.x + x, transform.localPosition.y + y, transform.localPosition.z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = originalPos;
        _isShaking = false;
    }

}

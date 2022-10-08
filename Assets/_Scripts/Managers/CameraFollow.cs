using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CameraFollow : MonoBehaviour
{
    [Header("Camera")]
    public Transform Target;
    [Range(0, 1)] public float Smoothing;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        CameraMovement();
    }

    private void CameraMovement()
    {
        transform.position = Vector3.Lerp(transform.position, new Vector3(Target.position.x, Target.position.y, transform.position.z), Smoothing);

        if (transform.position.y < 0) transform.position = Vector3.Lerp(transform.position, new Vector3(Target.position.x, 0, transform.position.z), Smoothing);
    }
}

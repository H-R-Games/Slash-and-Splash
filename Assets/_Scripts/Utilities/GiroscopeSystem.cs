using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiroscopeSystem : MonoBehaviour
{
    [Header("Balls Settings")]
    [SerializeField] private GameObject _balls;
    [SerializeField][Range(1, 200)] private int _ballsInScene = 1;
    
    public static float limitsCameraX;
    public static float limitsCameraY;

    List<GameObject> _ballsList = new List<GameObject>();

    private void Awake()
    {
        // agarramos el tamaño de la camara y lo dividimos entre 2 para tener los limites de la camara
        limitsCameraX = Camera.main.orthographicSize * Camera.main.aspect;
        limitsCameraY = Camera.main.orthographicSize;
    }

    private void OnEnable()
    {
        Camera.main.gameObject.GetComponent<CameraFollow>().enabled = false;
        Camera.main.transform.position = new Vector3(0, 0, -10);
        SetGiroscope();
        CreateBalls();
    }

    private void OnDisable()
    {
        Input.gyro.enabled = false;
        DelteBalls();
        Camera.main.gameObject.GetComponent<CameraFollow>().enabled = true;
    }

    private void SetGiroscope()
    {
        Input.gyro.enabled = true;
        Input.gyro.updateInterval = 0.0167f;
    }

    private void CreateBalls()
    {
        for (int i = 0; i < _ballsInScene; i++)
        {
            float randomX = Random.Range(-limitsCameraX, limitsCameraX);
            float randomY = Random.Range(-limitsCameraY, limitsCameraY);
            Vector3 randomPosition = new Vector3(randomX, randomY, 0);
            GameObject balls = Instantiate(_balls, randomPosition, Quaternion.identity);
            balls.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            balls.gameObject.GetComponent<Balls>().StartBall();
            _ballsList.Add(balls);
        }
    }

    private void DelteBalls()
    {
        for (int i = _ballsList.Count - 1; i >= 0; i--)
        {
            Destroy(_ballsList[i]);
        }

        _ballsList = new List<GameObject>();
    }
}

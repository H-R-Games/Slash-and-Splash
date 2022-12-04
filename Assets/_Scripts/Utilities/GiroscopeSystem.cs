using System.Collections;
using System.Collections.Generic;
using UnityEngine.Pool;
using UnityEngine;

public class GiroscopeSystem : MonoBehaviour
{
    [Header("Balls Settings")]
    [SerializeField] private Balls _balls;
    [SerializeField][Range(1, 200)] private int _ballsInScene = 1;

    [Header("Pool Settings")]
    [SerializeField] private int _startingPool = 100;
    [SerializeField] private int _maxPool = 200;
    private ObjectPool<Balls> _pool;

    public static float limitsCameraX;
    public static float limitsCameraY;

    List<Balls> _ballsList = new List<Balls>();

    private void Awake()
    {
        limitsCameraX = Camera.main.orthographicSize * Camera.main.aspect;
        limitsCameraY = Camera.main.orthographicSize;

        _pool = new ObjectPool<Balls>(() =>
        {
            return Instantiate(_balls);
        }, ball =>
        {
            ball.gameObject.SetActive(true);
        }, ball =>
        {
            ball.gameObject.SetActive(false);
        }, ball =>
        {
            Destroy(ball.gameObject);
        }, false, _startingPool, _maxPool);
    }

    private void OnEnable()
    {
        //Camera.main.gameObject.GetComponent<CameraFollow>().enabled = false;
        Camera.main.transform.position = new Vector3(0, 0, -10);
        SetGiroscope();
        CreateBalls();
    }

    private void OnDisable()
    {
        Input.gyro.enabled = false;
        DeleteAllEnemy();
        _pool.Clear();
        //Camera.main.gameObject.GetComponent<CameraFollow>().enabled = true;
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

            Balls balls = _pool.Get();

            balls.transform.position = randomPosition;
            balls.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            balls.gameObject.transform.SetParent(this.transform);
            balls.gameObject.GetComponent<Balls>().StartBall();
            _ballsList.Add(balls);
        }
    }

    public void DeleteAllEnemy()
    {
        for (int i = _ballsList.Count - 1; i >= 0; i--)
        {
            _pool.Release(_ballsList[i]);
        }
        _ballsList.Clear();
    }

    public void StartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }
}
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class EnemySpawn : MonoBehaviour
{
    [Header("Set a prefab enemy")]
    public EnemyController NormalEnemy;
    private float _distance;
    private float _distanceTravelled;
    private float _min;
    private float _max;
    private float _spawnDistance = 15f;
    [SerializeField]private List<EnemyController> _entities;

    [Header("Radio of the spawn")]
    [Range(0, 100)]
    public float RadioGizmo = 20f;
    public int MaxEnemiesStart = 25;

    [Header("Pool Settings")]
    [SerializeField] private int _startingPool = 200;
    [SerializeField] private int _maxPool = 500;
    private ObjectPool<EnemyController> _pool;
    [SerializeField] private int _distanceToDelete = 100;

    private void Awake()
    {        
        _entities = new List<EnemyController>();
        _distance = 0f;
        _distanceTravelled = 0f;
        _min = 0f;
        _max = 0f;
    }

    private void Start()
    {
        
    }

    private void OnEnable()
    {
        // Subscribe to the event
        this.GetComponent<PlayerController>().OnDeath += DeleteAllEnemy;
        this.GetComponent<PlayerController>().KillEnemy += KillEnemy;

        
        // Create a new pool with a default size of 200 and a maximum size of 500
        _pool = new ObjectPool<EnemyController>(() =>
        {
            return Instantiate(NormalEnemy);
        }, enemy => {
            enemy.gameObject.SetActive(true);
        }, enemy =>
        {
            enemy.gameObject.SetActive(false);
        }, enemy =>
        {
            Destroy(enemy.gameObject);
        }, false, _startingPool, _maxPool);

        StartSpawn(_startingPool);
    }

    private void OnDisable()
    {
        // Unsubscribe to the event
        this.GetComponent<PlayerController>().OnDeath -= DeleteAllEnemy;
        this.GetComponent<PlayerController>().KillEnemy -= KillEnemy;

        // Clear the pool
        _pool.Clear();
    }

    private void FixedUpdate()
    {
        EnemyDeleteDistance();
    }

    private void Update()
    {
        CheckDistanceSpawn();
    }

    private void CheckDistanceSpawn()
    {
        _distance = transform.position.x;
        if (_distance - _max > 1f)
        {
            _distanceTravelled += 1f;
            SpawnEnemy(_spawnDistance);
            _max = _distance;
        }
        else if (_min - _distance > 1f)
        {
            _distanceTravelled += 1f;
            SpawnEnemy(_spawnDistance);
            _min = _distance;
        }
        if (_distance - _min > _spawnDistance)
        {
            _min = _distance - _spawnDistance;
        }
        if (_max - _distance > _spawnDistance)
        {
            _max = _distance + _spawnDistance;
        }
    }

    private void EnemyDeleteDistance()
    {
        for (int i = 0; i < _entities.Count; i++)
        {
            if (Vector2.Distance(transform.position, _entities[i].transform.position) > _distanceToDelete)
            {
                // Return the object to the pool
                _pool.Release(_entities[i]);
                _entities.Remove(_entities[i]);
            }
        }
    }

    private void SpawnEnemy(float offset)
    {
        if (_distance > 1130f && _distance < 1550f)
        {
            return;
        }
        if (_distance < -920f)
        {
            return;
        }
        float y = Mathf.Floor(Mathf.Abs(Random.Range(0f, 1f) - Random.Range(0f, 1f)) * 203f + -2f);
        Vector2 v = new Vector2(_distance + offset, y);

        // Get pooled enemy
        EnemyController enemy = _pool.Get();
        _entities.Add(enemy);

        enemy.transform.position = v;
    }

    private void StartSpawn(int _count)
    {
        for (int i = 0; i < _count; i++)
        {
            float x = new Vector3(Random.Range(-RadioGizmo, RadioGizmo), 0f, 0f).x;
            float y = new Vector3(0f, Mathf.Abs(Random.Range(-RadioGizmo, RadioGizmo)), 0f).y;

            Vector2 v = new Vector2(x, y);

            // Get pooled enemy
            EnemyController enemy = _pool.Get();
            _entities.Add(enemy);
            
            enemy.transform.position = v;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, RadioGizmo);
    }

    private void KillEnemy(EnemyController enemy)
    {
        _entities.Remove(enemy);
        _pool.Release(enemy);
    }

    public void DeleteAllEnemy()
    {
        for (int i = _entities.Count - 1; i >= 0; i--)
        {
            // Return the object to the pool
            _entities.RemoveAt(i);
            _pool.Release(_entities[i]);
        }
    }
}

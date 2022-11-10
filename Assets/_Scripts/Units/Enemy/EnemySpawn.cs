using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    [Header("Set a prefab enemy")]
    public GameObject normalEnemy;
    private float distance;
    private float distanceTravelled;
    private float min;
    private float max;
    private float spawnDistance = 15f;
    [SerializeField]private List<GameObject> entities;

    [Header("Radio of the spawn")]
    [Range(0, 100)]
    public float radioGizmo = 20f;
    public int maxEnemiesStart = 25;

    private void Awake()
    {
        entities = new List<GameObject>();
        distance = 0f;
        distanceTravelled = 0f;
        min = 0f;
        max = 0f;
    }

    private void Start()
    {
        StartSpawn(maxEnemiesStart);
    }

    private void Update()
    {
        CheckDistanceSpawn();
    }

    private void CheckDistanceSpawn()
    {
        distance = transform.position.x;
        if (distance - max > 1f)
        {
            distanceTravelled += 1f;
            SpawnEnemy(spawnDistance);
            max = distance;
        }
        else if (min - distance > 1f)
        {
            distanceTravelled += 1f;
            SpawnEnemy(spawnDistance);
            min = distance;
        }
        if (distance - min > spawnDistance)
        {
            min = distance - spawnDistance;
        }
        if (max - distance > spawnDistance)
        {
            max = distance + spawnDistance;
        }
    }

    private void SpawnEnemy(float offset)
    {
        if (distance > 1130f && distance < 1550f)
        {
            return;
        }
        if (distance < -920f)
        {
            return;
        }
        float y = Mathf.Floor(Mathf.Abs(Random.Range(0f, 1f) - Random.Range(0f, 1f)) * 203f + -2f);
        Vector2 v = new Vector2(distance + offset, y);
        entities.Add(Instantiate(normalEnemy, v, Quaternion.identity));
    }

    private void StartSpawn(int _count)
    {
        for (int i = 0; i < _count; i++)
        {
            float x = new Vector3(Random.Range(-radioGizmo, radioGizmo), 0f, 0f).x;
            float y = new Vector3(0f, Mathf.Abs(Random.Range(-radioGizmo, radioGizmo)), 0f).y;

            Vector2 v = new Vector2(x, y);
            GameObject enemy = Instantiate(normalEnemy, v, Quaternion.identity);
            entities.Add(enemy);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radioGizmo);
    }

    public void DeleteAllEnemy()
    {
        for (int i = entities.Count - 1; i >= 0; i--)
        {
            Destroy(entities[i]);
        }
    }
}

using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    public GameObject normalEnemy;
    private float distance;
    private float distanceTravelled;
    private float min;
    private float max;
    private float spawnDistance = 30f;
    public List<GameObject> entities;
    private bool gravity;

    private void Awake()
    {
        entities = new List<GameObject>();
        distance = 0f;
        distanceTravelled = 0f;
        min = 0f;
        max = 0f;
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
}

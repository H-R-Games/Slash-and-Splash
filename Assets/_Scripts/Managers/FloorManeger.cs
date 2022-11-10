using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorManeger : MonoBehaviour
{
    [Header("Floor Settings")]
    public GameObject floorPrefab;
    public GameObject floorDeadPrefab;
    [Range(0, 100)]
    public int floorCount = 120;
    private float _distance = 15;

    void Start()
    {
        SpawnFloor();
        DeadZone();
    }

    private void SpawnFloor()
    {
        StartFloor();
        for (int i = 0; i < floorCount; i++)
        {
            _distance += Random.Range(20, 30);
            int _scaleX = Mathf.Abs(Random.Range(5, 10));
            float x = new Vector3(Random.Range(-_distance, _distance), 0f, 0f).x;
            Vector2 v = new Vector2(x, -5);

            GameObject _floor = Instantiate(floorPrefab, v, transform.rotation);
            _floor.transform.localScale = new Vector3(_scaleX, 1, 1);
        }
    }

    private void StartFloor()
    {
        GameObject floor = Instantiate(floorPrefab, new Vector3(0, -5, 0), Quaternion.identity);
        int _scaleX = Mathf.Abs(Random.Range(5, 10));
        floor.transform.localScale = new Vector3(_scaleX, 1, 1);
    }

    private void DeadZone()
    {
        GameObject _DeadZone = Instantiate(floorDeadPrefab, new Vector3(0, -30, 0), Quaternion.identity);
        _DeadZone.transform.localScale = new Vector3(100000, 50, 1);
    }
}

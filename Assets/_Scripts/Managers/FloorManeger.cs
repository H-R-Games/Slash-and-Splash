using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorManeger : MonoBehaviour
{
    [Header("Floor Settings")]
    public GameObject floorPrefab;
    public float floorWidth = 10f;


    private List<GameObject> floors;

    void Start()
    {
        floors = new List<GameObject>();
    }

    void Update()
    {
        SpawnFloor();
    }

    public void SpawnFloor()
    {
        int _floorWihth = Mathf.Abs(Random.Range(5, 20));
        int _floorSpace = Mathf.Abs(Random.Range(5, 10));
        GameObject _floor = Instantiate(floorPrefab, new Vector3(Random.Range(-floorWidth, floorWidth), 0, transform.position.z), Quaternion.identity);
        _floor.transform.localScale = new Vector3(_floorWihth, 1, 1);
        floors.Add(_floor);
    }
}

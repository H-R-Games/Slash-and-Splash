using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorManeger : MonoBehaviour
{
    [Header("Floor Settings")]
    [SerializeField] private GameObject _floorPrefab;
    [SerializeField] private GameObject _floorDeadPrefab;
    [SerializeField][Range(0, 150)] private int _floorCount = 120;
    private float _distance = 7;
    
    List<GameObject> _floors = new List<GameObject>();

    private GameManager _gm;

    void Start()
    {
        _gm = FindObjectOfType<GameManager>();
        _gm.RestartGame += SpawnFloor;
        _gm.Clear += DestroyFloor;

        SpawnFloor();
        DeadZone();
    }

    private void OnEnable()
    {
        GameObject.FindGameObjectWithTag("Player").gameObject.GetComponent<PlayerController>().OnDeath += DestroyFloor;
    }

    /// <summary>
    /// Creamos una funcion para crear el suelo ene el mapa y lo guardamos en una lista
    /// </summary>
    private void SpawnFloor()
    {
        StartFloor();
        
        for (int i = 0; i < _floorCount; i++)
        {
            _distance += Random.Range(20, 30);
            int _scaleX = Mathf.Abs(Random.Range(8, 15));
            float x = new Vector3(Random.Range(-_distance, _distance), 0f, 0f).x;
            Vector2 v = new Vector2(x, -5);

            GameObject _floor = Instantiate(_floorPrefab, v, transform.rotation);
            _floor.transform.localScale = new Vector3(_scaleX, 1, 1);
            _floors.Add(_floor);
        }
    }

    /// <summary>
    /// Spawnear el primer piso en la pocicion 0 del mundo y lo guarda en la lista
    /// </summary>
    private void StartFloor()
    {
        GameObject floor = Instantiate(_floorPrefab, new Vector3(0, -5, 0), Quaternion.identity);
        int _scaleX = Mathf.Abs(Random.Range(5, 10));
        floor.transform.localScale = new Vector3(_scaleX, 1, 1);
        _floors.Add(floor);
    }

    /// <summary>
    /// Creamos una funcion para poder crear una zona de muerte del jugador en el juego
    /// </summary>
    private void DeadZone()
    {
        GameObject _DeadZone = Instantiate(_floorDeadPrefab, new Vector3(0, -30, 0), Quaternion.identity);
        _DeadZone.transform.localScale = new Vector3(100000, 50, 1);
    }

    /// <summary>
    /// Creamos una funcion para poder eliminar todos los pisos que estan en la lista
    /// </summary>
    public void DestroyFloor()
    {
        for (int i = _floors.Count - 1; i >= 0; i--)
        {
            Destroy(_floors[i]);
        }
        
        _floors = new List<GameObject>();
    }
}

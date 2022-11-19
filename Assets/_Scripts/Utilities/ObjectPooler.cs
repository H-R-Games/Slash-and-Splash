using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    private List<GameObject> _pool;

    public ObjectPooler()
    {
        _pool = new List<GameObject>();
    }

    public void StorePoolObject(int _Number, GameObject _Object)
    {
        for (int i = 0; i < _Number; i++)
        {
            GameObject _poolObject = Instantiate(_Object);
            _poolObject.SetActive(false);
            _pool.Add(_poolObject);
        }
    }

    public GameObject GetPoolerObject(GameObject _Obj)
    {
        for (int i = 0; i < _pool.Count; i++) if (!_pool[i].activeInHierarchy) return _pool[i];

        GameObject _newObj = Instantiate(_Obj);
        _pool.Add(_newObj);
        _newObj.SetActive(false);

        return _newObj;
    }
}

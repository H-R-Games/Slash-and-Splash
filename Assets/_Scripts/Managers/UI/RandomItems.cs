using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomItems : MonoBehaviour
{
    public GameObject[] PowerUps;

    // 0 = Doble Tiempo
    // 1 = Curacion
    // 2 = Mas Velocidad
    // 3 = Mas tiempo de SlowMotion
    // 4 = Mas alcance de dash

    // wtf va como un gacha

    int n, u, d;

    public void OnEnable()
    {
        n = Random.Range(0, PowerUps.Length);
        u = Random.Range(0, PowerUps.Length);
        d = Random.Range(0, PowerUps.Length);

        PowerUps[n].SetActive(true);
        PowerUps[u].SetActive(true);
        PowerUps[d].SetActive(true);
    }

    public void OnDisable()
    {
        PowerUps[n].SetActive(false);
        PowerUps[u].SetActive(false);
        PowerUps[d].SetActive(false);
    }

}

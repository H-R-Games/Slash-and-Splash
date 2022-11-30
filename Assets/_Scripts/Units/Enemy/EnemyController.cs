using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Components")]
    public GameObject ExplosionPS;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            //print("OH NO MORI");
        }
    }

    public void Kill()
    {
        GameObject gameObjectps = Instantiate(ExplosionPS, this.transform.position, Quaternion.identity);
        Destroy(gameObjectps, 1.5f);
    }
}

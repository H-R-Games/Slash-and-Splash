using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.ParticleSystem;

public class TiendaScript : MonoBehaviour
{
    public int[,] shopItems = new int[6, 6];
    public float coins;
    public Text CoinsTXT;


    void Start()
    {
        CoinsTXT.text = "MONEDAS:" + coins.ToString();

        //          ID
        shopItems[1, 1] = 1;
        shopItems[1, 2] = 2;
        shopItems[1, 3] = 3;
        shopItems[1, 4] = 4;
        shopItems[1, 5] = 5;

        //    PRECIO POWER UPS
        shopItems[2, 1] = 200;
        shopItems[2, 2] = 200;
        shopItems[2, 3] = 200;
        shopItems[2, 4] = 200;
        shopItems[2, 5] = 200;

        //    Cantidad de power ups comprados
        shopItems[3, 1] = 0;
        shopItems[3, 2] = 0;
        shopItems[3, 3] = 0;
        shopItems[3, 4] = 0;
        shopItems[3, 5] = 0;

    }


    public void Buy()
    {
        GameObject ButtonRef = GameObject.FindGameObjectWithTag("Event").GetComponent<EventSystem>().currentSelectedGameObject;

        if(coins >= shopItems[2, ButtonRef.GetComponent<BotonPrueba>().ItemID])
        {
            coins -= shopItems[2, ButtonRef.GetComponent<BotonPrueba>().ItemID];
            shopItems[3, ButtonRef.GetComponent<BotonPrueba>().ItemID]++;

            CoinsTXT.text = "MONEDAS:" + coins.ToString();

            ButtonRef.GetComponent<BotonPrueba>().QuantityTxt.text = shopItems[3, ButtonRef.GetComponent<BotonPrueba>().ItemID].ToString();
        }

        //---------------- ITEMS ---------------------------

        if (ButtonRef.GetComponent<BotonPrueba>().ItemID == 1)
        {
            print("DOBLE TIEMPO");  //RELOJ ROJO
        }
        if (ButtonRef.GetComponent<BotonPrueba>().ItemID == 2)
        {
            print("TE CURA VIDA");  //CORAZÓN VERDE
        }
        if (ButtonRef.GetComponent<BotonPrueba>().ItemID == 3)
        {
            print("MAS VELOCIDAD");  //tIPO QUE CORRE AZUL
        }
        if (ButtonRef.GetComponent<BotonPrueba>().ItemID == 4)
        {
            print("MAS TIEMPO DE SLOWMOTION");   //RELOJ MORADO
        }
        if (ButtonRef.GetComponent<BotonPrueba>().ItemID == 5)
        {
            print("DASH MAS LARGO");  //FLECHAS AMARILLAS
        }
    }

}

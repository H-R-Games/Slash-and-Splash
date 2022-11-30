using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BotonPrueba : MonoBehaviour
{
    public int ItemID;

    public Text PierceTxt;
    public Text QuantityTxt;
    public GameObject ShopManager;


    void Update()
    {
        PierceTxt.text = "Price: $" + ShopManager.GetComponent<TiendaScript>().shopItems[2,ItemID].ToString();
        QuantityTxt.text = ShopManager.GetComponent<TiendaScript>().shopItems[3, ItemID].ToString();
    }



}

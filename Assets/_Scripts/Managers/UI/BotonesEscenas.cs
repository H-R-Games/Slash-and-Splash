using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BotonesEscenas : MonoBehaviour
{
    public GameObject[] Escenas;

    public GameObject[] TextTDL;

    public bool TLBool = true;

    public void Menu()
    {
        Escenas[0].SetActive(true);
    }

    public void Ajustes()
    {
        Escenas[1].SetActive(true);
        Escenas[0].SetActive(false);
    }

    public void Juego()
    {
        //Escenas[2].SetActive(true);          //Pantalla juego

        Escenas[0].SetActive(false);

        //Aparecer tienda

        Escenas[3].SetActive(true);
    }


    public void TiendaDONE()
    {
        Escenas[3].SetActive(false);
        Escenas[2].SetActive(true);

    }

    public void Pausa()
    {
        Escenas[4].SetActive(true);
        Escenas[2].SetActive(false);
        Escenas[3].SetActive(false);
    }

    public void GO()
    {
        Escenas[5].SetActive(true);
        Escenas[3].SetActive(false);
    }


    //----------------------------------

    public void BackToMenu()
    {
        Escenas[0].SetActive(true);
        Escenas[1].SetActive(false);
        Escenas[2].SetActive(false);
        Escenas[3].SetActive(false);
        Escenas[4].SetActive(false);
    }

    public void BackFromTienda()
    {
        Escenas[3].SetActive(false);
        Escenas[2].SetActive(true);
    }

    public void BackFromPausa()
    {
        Escenas[2].SetActive(true);
        Escenas[4].SetActive(false);
    }

    //--------------- CONTROLES -------------

    public void LanzaminetoNormal()
    {
        if (TLBool == true)
        {
            TextTDL[0].SetActive(false);
            TextTDL[1].SetActive(true);
            TLBool = false;
        }
        else
        {
            TextTDL[0].SetActive(true);
            TextTDL[1].SetActive(false);
            TLBool = true;
        }
    }

}

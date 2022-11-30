using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PruebaFB : MonoBehaviour
{
    [SerializeField] private GameObject botonPausa;
    [SerializeField] private GameObject menuPausa;


    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject GOPantalla;
    [SerializeField] private GameObject PJuego;
    [SerializeField] private GameObject PTienda;

    private bool juegoPausado = false;

    //private bool juegoTienda = false;

    private void Update()
    {

        // Pausa
        if(Input.GetKeyUp(KeyCode.Escape))
        {
            if (juegoPausado)
            {
                Reanudar();
            }
            else
            {
                GameOver();
            }
        }

        // Tienda

        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (juegoPausado)
            {
                TiendaOn();
            }
            else
            {
                TiendaOff();
            }
        }


    }

    //-------------- Menu Pausa ------------------

    public void Pausa()
    {
        Time.timeScale = 0f;
        botonPausa.SetActive(false);
        PJuego.SetActive(false);
        menuPausa.SetActive(true);
    }

    public void Reanudar()
    {
        Time.timeScale = 1f;
        botonPausa.SetActive(true);
        menuPausa.SetActive(false);
        PJuego.SetActive(true);
    }

    //--------------- Tienda -------------------

    public void TiendaOn()
    {
        Time.timeScale = 0f;
        botonPausa.SetActive(false);
        PTienda.SetActive(true);
        PJuego.SetActive(false);
    }

    public void TiendaOff()
    {
        Time.timeScale = 1f;
        botonPausa.SetActive(true);
        PTienda.SetActive(false);
        PJuego.SetActive(true);
    }

    //----------------------------------------

    public void Reiniciar()
    {
        Time.timeScale = 1f;
        botonPausa.SetActive(true);
        PJuego.SetActive(true);
        GOPantalla.SetActive(false);

        menuPausa.SetActive(false);

        //Algo para reiniciar el juego...
    }

    public void GameOver()
    {
        Time.timeScale = 1f;
        botonPausa.SetActive(false);
        PJuego.SetActive(false);

        GOPantalla.SetActive(true);
    }

    public void VolverAlMenu()
    {
        botonPausa.SetActive(true);
        Time.timeScale = 1f;

        mainMenu.SetActive(true);
        PJuego.SetActive(false);
        GOPantalla.SetActive(false);
        menuPausa.SetActive(false);


    }

}

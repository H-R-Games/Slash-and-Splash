using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SonidosUI : MonoBehaviour
{
    public AudioClip[] Sonidos;
    private AudioSystem audioSystem;

    void Start()
    {
        audioSystem = FindObjectOfType<AudioSystem>();
    }

    public void SonidoCompra()
    {
        audioSystem.PlaySound(Sonidos[0], 1);
    }

    public void SonidoBack()
    {
        audioSystem.PlaySound(Sonidos[1], 1);
    }

    public void SonidoCantBuy()
    {
        audioSystem.PlaySound(Sonidos[2], 1);
    }

    public void SonidoTienda()
    {
        audioSystem.PlaySound(Sonidos[3], 1);
    }

    public void SonidoBurst()
    {
        audioSystem.PlaySound(Sonidos[4], 1);
    }

    public void SonidoClick()
    {
        audioSystem.PlaySound(Sonidos[5], 1);
    }

    public void SonidoPlay()
    {
        audioSystem.PlaySound(Sonidos[6], 1);
    }
}

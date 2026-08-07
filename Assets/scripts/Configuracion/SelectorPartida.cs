using UnityEngine;
using UnityEngine.UI;

public class SelectorPartida : MonoBehaviour
{
    [Header("Dificultad")]
    public Button botonFacil;
    public Button botonDificil;

    [Header("Vidas")]
    public Button boton1;
    public Button boton2;
    public Button boton3;

    [Header("Colores")]
    public Color colorNormal = Color.white;
    public Color colorSeleccionado = Color.green;

    private void Start()
    {
        SeleccionarFacil();
        Seleccionar3Vidas();
    }

    public void SeleccionarFacil()
    {
        PintarBoton(botonFacil, true);
        PintarBoton(botonDificil, false);
    }

    public void SeleccionarDificil()
    {
        PintarBoton(botonFacil, false);
        PintarBoton(botonDificil, true);
    }

    public void Seleccionar1Vidas()
    {
        PintarVidas(boton1);
    }

    public void Seleccionar2Vidas()
    {
        PintarVidas(boton2);
    }

    public void Seleccionar3Vidas()
    {
        PintarVidas(boton3);
    }

    void PintarVidas(Button seleccionado)
    {
        PintarBoton(boton1, seleccionado == boton1);
        PintarBoton(boton2, seleccionado == boton2);
        PintarBoton(boton3, seleccionado == boton3);
    }

    void PintarBoton(Button boton, bool seleccionado)
    {
        ColorBlock cb = boton.colors;

        cb.normalColor = seleccionado ? colorSeleccionado : colorNormal;
        cb.selectedColor = cb.normalColor;
        cb.highlightedColor = cb.normalColor;
        cb.pressedColor = cb.normalColor;

        boton.colors = cb;
    }
}
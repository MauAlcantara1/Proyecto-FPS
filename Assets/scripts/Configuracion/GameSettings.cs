using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameSettings : MonoBehaviour
{
    [Header("Botones de dificultad")]
    public Button btnFacil;
    public Button btnDificil;

    [Header("Botones de vidas")]
    public Button btn1Vida;
    public Button btn2Vidas;
    public Button btn3Vidas;

    [Header("Colores")]
    public Color colorSeleccionado = Color.green;
    public Color colorNormal = Color.white;

    public enum Dificultad
    {
        Facil,
        Dificil
    }

    public static Dificultad dificultad = Dificultad.Facil;
    public static int vidas = 3;

    void Start()
    {
        SeleccionarFacil();
        Seleccionar3Vidas();
    }

    //=========================
    // DIFICULTAD
    //=========================

    public void SeleccionarFacil()
    {
        dificultad = Dificultad.Facil;

        CambiarColor(btnFacil, true);
        CambiarColor(btnDificil, false);
    }

    public void SeleccionarDificultad()
    {
        dificultad = Dificultad.Dificil;

        CambiarColor(btnFacil, false);
        CambiarColor(btnDificil, true);
    }

    //=========================
    // VIDAS
    //=========================

    public void Seleccionar1Vida()
    {
        vidas = 1;

        ActualizarBotonesVidas(btn1Vida);
    }

    public void Seleccionar2Vidas()
    {
        vidas = 2;

        ActualizarBotonesVidas(btn2Vidas);
    }

    public void Seleccionar3Vidas()
    {
        vidas = 3;

        ActualizarBotonesVidas(btn3Vidas);
    }

    //=========================
    // JUGAR
    //=========================

    public void Jugar()
    {
        SceneManager.LoadScene("JuegoSolitario");
    }

    //=========================
    // AUXILIARES
    //=========================

    void ActualizarBotonesVidas(Button seleccionado)
    {
        CambiarColor(btn1Vida, seleccionado == btn1Vida);
        CambiarColor(btn2Vidas, seleccionado == btn2Vidas);
        CambiarColor(btn3Vidas, seleccionado == btn3Vidas);
    }

    void CambiarColor(Button boton, bool seleccionado)
    {
        ColorBlock colores = boton.colors;

        if (seleccionado)
            colores.normalColor = colorSeleccionado;
        else
            colores.normalColor = colorNormal;

        colores.selectedColor = colores.normalColor;
        colores.highlightedColor = colores.normalColor;
        colores.pressedColor = colores.normalColor;

        boton.colors = colores;
    }
}

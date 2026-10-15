
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class SelectorPartida : MonoBehaviour
{

    [Header("Dificultad")]
    [SerializeField] private Button botonFacil;
    [SerializeField] private Button botonDificil;

    [Header("Sprites botón Fácil")]
    [SerializeField] private Sprite elegidoFacil;
    [SerializeField] private Sprite noSeleccionadoFacil;

    [Header("Sprites botón Difícil")]
    [SerializeField] private Sprite elegidoDificil;
    [SerializeField] private Sprite noSeleccionadoDificil;

    [SerializeField] private TMP_InputField inputNombre;    
    private static int dificultad = 0;
    private static string nombreJugador = "";


    [SerializeField] private Scene escenarioSolo;


    private void Start()
    {
        SeleccionarDificultad(0);
        inputNombre.characterLimit = 10;

    }

    public static int Dificultad
    {
        get { return dificultad; }
    }

    public void SeleccionarDificultad(int nuevaDificultad)
    {
        dificultad = nuevaDificultad;

        if (dificultad == 0)
        {
            botonFacil.image.sprite = elegidoFacil;
            botonDificil.image.sprite = noSeleccionadoDificil;

            Debug.Log("Facil");
        }

        if (dificultad == 1)
        {
            botonFacil.image.sprite = noSeleccionadoFacil;
            botonDificil.image.sprite = elegidoDificil;

            Debug.Log("Dificil");
        }
    }

    public void Facil()
    {
        SeleccionarDificultad(0);
    }

    public void Difícil()
    {
        SeleccionarDificultad(1);
    }

    public void GuardarNombre(string nuevoNombre)
    {
        nombreJugador = nuevoNombre;
        Debug.Log("Nombre del jugador: " + nombreJugador);
    }

    public void EscenaSolitario()
    {
        SceneManager.LoadScene("SampleScene");
    }
}
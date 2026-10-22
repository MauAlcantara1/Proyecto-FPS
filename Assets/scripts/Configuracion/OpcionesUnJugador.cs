
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class SelectorPartida : MonoBehaviour
{

    [Header("Jugador y Escena")]
    [SerializeField] private TMP_InputField inputNombre;
    [SerializeField] private Scene escenarioSolo;

    private static int dificultad = 0;
    private static string nombreJugador = "";

    public static int Dificultad => dificultad;
    public static string NombreJugador => nombreJugador;
   


    private void Start()
    {
        SeleccionarDificultad(0);
        if (inputNombre != null)
        {
            inputNombre.characterLimit = 10;
        }

    }


    public void SeleccionarDificultad(int nuevaDificultad)
    {
        dificultad = nuevaDificultad;

        if (dificultad == 0)
        {
            Debug.Log("Facil");
        }

        if (dificultad == 1)
        {
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
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class MenuMultijugador : MonoBehaviour
{
    [Header("Paneles")]
    [SerializeField] private GameObject menuPrincipal;
    [SerializeField] private GameObject menuCrearSala;
    [SerializeField] private GameObject menuUnirseSala;


    [Header("Crear Sala")]
    [SerializeField] private TMP_InputField inputNombreCrear;

    [SerializeField] private Button botonFacil;
    [SerializeField] private Button botonDificil;

    [SerializeField] private Sprite spriteSeleccionado;
    [SerializeField] private Sprite spriteNoSeleccionado;


    [Header("Unirse a Sala")]
    [SerializeField] private TMP_InputField inputNombreUnirse;
    [SerializeField] private TMP_InputField inputCodigoSala;

    [Header("Sala de espera")]
    [SerializeField] private GameObject menuSala;

    [Header("Photon")]
    [SerializeField] private PhotonMultijugador photonMultijugador;


    // 0 = Fácil
    // 1 = Difícil
    private int dificultadSeleccionada = 0;


    private void Start()
    {
        MostrarMenuPrincipal();

        // Límites de caracteres
        inputNombreCrear.characterLimit = 10;
        inputNombreUnirse.characterLimit = 10;

        // Por defecto Fácil
        SeleccionarFacil();
    }


    // =========================================================
    // NAVEGACIÓN
    // =========================================================

    public void MostrarMenuPrincipal()
    {
        menuPrincipal.SetActive(true);
        menuCrearSala.SetActive(false);
        menuUnirseSala.SetActive(false);
        menuSala.SetActive(false);
    }

    public void MostrarCrearSala()
    {
        menuPrincipal.SetActive(false);
        menuCrearSala.SetActive(true);
        menuUnirseSala.SetActive(false);
        menuSala.SetActive(false);

        SeleccionarFacil();
    }

    public void MostrarUnirseSala()
    {
        menuPrincipal.SetActive(false);
        menuCrearSala.SetActive(false);
        menuUnirseSala.SetActive(true);
        menuSala.SetActive(false);
    }

    public void MostrarSala()
    {
        menuPrincipal.SetActive(false);
        menuCrearSala.SetActive(false);
        menuUnirseSala.SetActive(false);
        menuSala.SetActive(true);
    }


    public void Regresar()
    {
        MostrarMenuPrincipal();
    }


    // =========================================================
    // DIFICULTAD
    // =========================================================

    public void SeleccionarFacil()
    {
        dificultadSeleccionada = 0;

        botonFacil.image.sprite = spriteSeleccionado;
        botonDificil.image.sprite = spriteNoSeleccionado;
    }


    public void SeleccionarDificil()
    {
        dificultadSeleccionada = 1;

        botonFacil.image.sprite = spriteNoSeleccionado;
        botonDificil.image.sprite = spriteSeleccionado;
    }


    // =========================================================
    // CREAR SALA
    // =========================================================

    public void ListoCrearSala()
    {
        string nombre = inputNombreCrear.text.Trim();

        if (string.IsNullOrEmpty(nombre))
        {
            Debug.LogWarning("Debes ingresar un nombre.");
            return;
        }

        Debug.Log("CREAR SALA");
        Debug.Log("Jugador: " + nombre);
        Debug.Log(
            "Dificultad: " +
            (dificultadSeleccionada == 0 ? "Fácil" : "Difícil")
        );

        photonMultijugador.CrearSala(
            nombre,
            dificultadSeleccionada
        );
    }


    // =========================================================
    // UNIRSE A SALA
    // =========================================================

    public void ListoUnirseSala()
    {
        string nombre = inputNombreUnirse.text.Trim();
        string codigo = inputCodigoSala.text.Trim();

        if (string.IsNullOrEmpty(nombre))
        {
            Debug.LogWarning("Debes ingresar un nombre.");
            return;
        }

        if (string.IsNullOrEmpty(codigo))
        {
            Debug.LogWarning("Debes ingresar el código de la sala.");
            return;
        }

        Debug.Log("UNIRSE A SALA");
        Debug.Log("Jugador: " + nombre);
        Debug.Log("Código: " + codigo);

        photonMultijugador.UnirseASala(
            nombre,
            codigo
        );
    }
}
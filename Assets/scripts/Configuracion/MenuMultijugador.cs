using System.Collections;
using UnityEngine;
using TMPro;

public class MenuMultijugador : MonoBehaviour
{
    [Header("Paneles")]
    [SerializeField] private GameObject menuPrincipal;
    [SerializeField] private GameObject menuCrearSala;
    [SerializeField] private GameObject menuUnirseSala;

    [Header("Crear Sala")]
    [SerializeField] private TMP_InputField inputNombreCrear;

    [Header("Unirse a Sala")]
    [SerializeField] private TMP_InputField inputNombreUnirse;
    [SerializeField] private TMP_InputField inputCodigoSala;

    [Header("Sala de espera y Loader")]
    [SerializeField] private GameObject menuSala;
    [Tooltip("Arrastra aquí el objeto o animación de carga (Loader)")]
    [SerializeField] private GameObject loader;
    [Tooltip("Opcional: Texto dentro del loader para indicar qué está cargando")]
    [SerializeField] private TMP_Text textoEstadoLoader;

    [Header("Mensajes de Error (UI)")]
    [SerializeField] private TMP_Text textoError;
    [SerializeField] private float duracionMensajeError = 3.5f;
    private Coroutine corrutinaError;

    [Header("Photon")]
    [SerializeField] private PhotonMultijugador photonMultijugador;

    // 0 = Fácil
    // 1 = Difícil
    private int dificultadSeleccionada = 0;

    private void Start()
    {
        LimpiarError();
        OcultarLoader();
        MostrarMenuPrincipal();

        if (inputNombreCrear != null) inputNombreCrear.characterLimit = 10;
        if (inputNombreUnirse != null) inputNombreUnirse.characterLimit = 10;

        SeleccionarFacil();
    }

    // =========================================================
    // NAVEGACIÓN
    // =========================================================

    public void MostrarMenuPrincipal()
    {
        LimpiarError();
        OcultarLoader();
        if (menuPrincipal != null) menuPrincipal.SetActive(true);
        if (menuCrearSala != null) menuCrearSala.SetActive(false);
        if (menuUnirseSala != null) menuUnirseSala.SetActive(false);
        if (menuSala != null) menuSala.SetActive(false);
    }

    public void MostrarCrearSala()
    {
        LimpiarError();
        OcultarLoader();
        if (menuPrincipal != null) menuPrincipal.SetActive(false);
        if (menuCrearSala != null) menuCrearSala.SetActive(true);
        if (menuUnirseSala != null) menuUnirseSala.SetActive(false);
        if (menuSala != null) menuSala.SetActive(false);

        SeleccionarFacil();
    }

    public void MostrarUnirseSala()
    {
        LimpiarError();
        OcultarLoader();
        if (menuPrincipal != null) menuPrincipal.SetActive(false);
        if (menuCrearSala != null) menuCrearSala.SetActive(false);
        if (menuUnirseSala != null) menuUnirseSala.SetActive(true);
        if (menuSala != null) menuSala.SetActive(false);
    }

    public void MostrarSala()
    {
        LimpiarError();
        if (menuPrincipal != null) menuPrincipal.SetActive(false);
        if (menuCrearSala != null) menuCrearSala.SetActive(false);
        if (menuUnirseSala != null) menuUnirseSala.SetActive(false);
        if (menuSala != null) menuSala.SetActive(true);

        // Mantiene el loader activo indicando que espera al segundo jugador
        MostrarLoader("Esperando a otro jugador...");
    }

    public void Regresar()
    {
        MostrarMenuPrincipal();
    }

    // =========================================================
    // CONTROL DEL LOADER
    // =========================================================

    public void MostrarLoader(string mensajeEstado = "Cargando...")
    {
        if (loader != null)
        {
            loader.SetActive(true);
        }

        if (textoEstadoLoader != null)
        {
            textoEstadoLoader.text = mensajeEstado;
        }
    }

    public void OcultarLoader()
    {
        if (loader != null)
        {
            loader.SetActive(false);
        }
    }

    // =========================================================
    // SISTEMA DE ERRORES EN UI
    // =========================================================

   public void MostrarError(string mensaje)
    {
        Debug.LogWarning(mensaje);

        // Solo apagamos el loader si NO estamos ya dentro de la sala de espera
        // (Así, si alguien se sale de la sala, sale el aviso pero el loader sigue esperando a otro jugador)
        if (menuSala == null || !menuSala.activeSelf)
        {
            OcultarLoader();
        }

        if (textoError == null) return;

        if (corrutinaError != null)
        {
            StopCoroutine(corrutinaError);
        }

        corrutinaError = StartCoroutine(RutinaMostrarError(mensaje));
    }

    private IEnumerator RutinaMostrarError(string mensaje)
    {
        textoError.text = mensaje;
        textoError.gameObject.SetActive(true);

        yield return new WaitForSeconds(duracionMensajeError);

        LimpiarError();
    }

    public void LimpiarError()
    {
        if (corrutinaError != null)
        {
            StopCoroutine(corrutinaError);
            corrutinaError = null;
        }

        if (textoError != null)
        {
            textoError.text = "";
            textoError.gameObject.SetActive(false);
        }
    }

    // =========================================================
    // DIFICULTAD
    // =========================================================

    public void SeleccionarFacil()
    {
        dificultadSeleccionada = 0;
    }

    public void SeleccionarDificil()
    {
        dificultadSeleccionada = 1;
    }

    // =========================================================
    // CREAR SALA
    // =========================================================

    public void ListoCrearSala()
    {
        if (inputNombreCrear == null) return;

        string nombre = inputNombreCrear.text.Trim();

        if (string.IsNullOrEmpty(nombre))
        {
            MostrarError("Debes ingresar un nombre de jugador.");
            return;
        }

        if (photonMultijugador == null)
        {
            MostrarError("Error interno: Gestor de red no asignado.");
            return;
        }

        LimpiarError();
        MostrarLoader("Creando sala...");
        photonMultijugador.CrearSala(nombre, dificultadSeleccionada);
    }

    // =========================================================
    // UNIRSE A SALA
    // =========================================================

    public void ListoUnirseSala()
    {
        if (inputNombreUnirse == null || inputCodigoSala == null) return;

        string nombre = inputNombreUnirse.text.Trim();
        string codigo = inputCodigoSala.text.Trim();

        if (string.IsNullOrEmpty(nombre))
        {
            MostrarError("Debes ingresar un nombre de jugador.");
            return;
        }

        if (string.IsNullOrEmpty(codigo))
        {
            MostrarError("Debes ingresar el código de la sala.");
            return;
        }

        if (photonMultijugador == null)
        {
            MostrarError("Error interno: Gestor de red no asignado.");
            return;
        }

        LimpiarError();
        MostrarLoader("Uniéndose a la sala...");
        photonMultijugador.UnirseASala(nombre, codigo);
    }
}
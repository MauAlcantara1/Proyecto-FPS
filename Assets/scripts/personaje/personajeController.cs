
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

public class personajeController : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float velocidadCaminar = 4f;
    [SerializeField] private float velocidadCorrer = 7f;
    [SerializeField] private float fuerzaSalto = 3f;
    [SerializeField] private float gravedad = -9.81f;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] public AudioClip pasosCaminar;
    [SerializeField] public AudioClip pasosCorrer;

    [Header("Referencias")]
    [SerializeField] public Animator animator;
    [SerializeField] private GameObject linterna;
    [SerializeField] private Puntuacion puntuacion;
    [SerializeField] private personajeVida saludJugador;

    [Header("Pausa")]
    [SerializeField] private MenuPausa menuPausa;

    [Header("Interfaz de compra")]
    [SerializeField] private GameObject textoComprar;

    private CharacterController controller;
    private PhotonView vistaJugador;

    private Vector2 movimientoInput;
    private Vector3 velocidadVertical;

    private bool saltar;
    private bool correr;
    private bool pausa;

    private ZonaCompra zonaCompraActual;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        vistaJugador = GetComponentInParent<PhotonView>();

#if UNITY_EDITOR
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
#endif
    }

    private void Start()
    {
        if (!EsJugadorLocal())
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = false;
                rb.isKinematic = true;
            }
            return;
        }

        pausa = false;
        Time.timeScale = 1f;

        pausa = false;
        Time.timeScale = 1f;

        if (menuPausa == null)
        {
            menuPausa = FindFirstObjectByType<MenuPausa>();
        }

        if (menuPausa != null)
        {
            menuPausa.AsignarJugador(this);
            menuPausa.OcultarPaneles();
        }
        else
        {
            Debug.LogWarning(
                "No se encontró MenuPausa en la escena."
            );
        }

        if (textoComprar != null)
            textoComprar.SetActive(false);

        puntuacion = FindFirstObjectByType<Puntuacion>();

        if (puntuacion == null)
        {
            Debug.LogError(
                "No se encontró Puntuacion en la escena.");
            return;
        }

        armaController armas =
            GetComponentInChildren<armaController>(true);

        if (armas == null)
        {
            Debug.LogError(
                "No se encontró armaController en JugadorOnline.");
            return;
        }

        puntuacion.AsignarArmaJugador(armas);

        Debug.Log("Puntuación y armas locales conectadas.");
    }

    private bool EsJugadorLocal()
    {
        return !PhotonNetwork.InRoom ||
            (vistaJugador != null && vistaJugador.IsMine);
    }

    private void Update()
    {
        if (!EsJugadorLocal())
            return;

        if (pausa)
            return;

        if (saludJugador != null && saludJugador.EstaCurandose())
        {
            if (movimientoInput.magnitude > 0.1f || saltar)
                saludJugador.CancelarCuracion();
        }

        MoverJugador();
        SonidosPasos();
    }

    public void Movimiento(InputAction.CallbackContext context)
    {
        if (!EsJugadorLocal() || pausa)
            return;

        movimientoInput = context.ReadValue<Vector2>();
    }

    public void Linterna(InputAction.CallbackContext context)
    {
        if (!EsJugadorLocal() || pausa)
            return;

        if (context.performed && linterna != null)
            linterna.SetActive(!linterna.activeSelf);
    }

    public void Salto(InputAction.CallbackContext context)
    {
        if (!EsJugadorLocal() || pausa)
            return;

        if (context.performed)
            saltar = true;
    }

    public void Correr(InputAction.CallbackContext context)
    {
        if (!EsJugadorLocal() || pausa)
            return;

        correr = context.ReadValueAsButton();
    }

    public void Pausa(InputAction.CallbackContext context)
    {
        if (!EsJugadorLocal() || !context.performed)
            return;

        if (pausa)
        {
            ReanudarJuego();
            return;
        }

        pausa = true;

        // En multijugador NO detenemos el tiempo.
        // En un jugador conservamos la pausa tradicional.
        if (!PhotonNetwork.InRoom)
        {
            Time.timeScale = 0f;
        }

        movimientoInput = Vector2.zero;
        saltar = false;
        correr = false;

        // Liberar el cursor para utilizar los botones
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        if (menuPausa != null)
        {
            menuPausa.MostrarPausa();
        }

    }

    public void ReanudarJuego()
    {
        if (!EsJugadorLocal())
            return;

        pausa = false;

        if (!PhotonNetwork.InRoom)
        {
            Time.timeScale = 1f;
        }

        if (menuPausa != null)
        {
            menuPausa.OcultarPaneles();
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        movimientoInput = Vector2.zero;
        saltar = false;
        correr = false;

        Debug.Log("Jugador local reanudó la partida.");
    }

    private void MoverJugador()
    {
        if (controller == null || !controller.enabled)
            return;

        float deltaTime = Time.deltaTime;

        // Verificar si el personaje está en el suelo
        bool estaEnSuelo = controller.isGrounded;

        if (estaEnSuelo && velocidadVertical.y < 0f)
        {
            velocidadVertical.y = -2f;
        }

        // Salto
        if (estaEnSuelo && saltar)
        {
            velocidadVertical.y = Mathf.Sqrt(
                fuerzaSalto * -2f * gravedad
            );

            saltar = false;
        }

        // Aplicar gravedad
        velocidadVertical.y += gravedad * deltaTime;

        // Dirección horizontal
        Vector3 direccion =
            transform.right * movimientoInput.x +
            transform.forward * movimientoInput.y;

        float velocidad = correr
            ? velocidadCorrer
            : velocidadCaminar;

        // Combinar movimiento horizontal y vertical
        Vector3 movimientoFinal =
            direccion * velocidad +
            Vector3.up * velocidadVertical.y;

        // Un solo Move por fotograma
        controller.Move(movimientoFinal * deltaTime);
    }

    private void SonidosPasos()
    {
        if (audioSource == null || controller == null)
            return;

        if (!controller.isGrounded ||
            movimientoInput.magnitude < 0.1f)
        {
            if (audioSource.isPlaying)
                audioSource.Stop();

            return;
        }

        AudioClip clipActual = correr
            ? pasosCorrer
            : pasosCaminar;

        if (audioSource.clip != clipActual)
        {
            audioSource.clip = clipActual;

            if (clipActual != null)
                audioSource.Play();
        }
        else if (!audioSource.isPlaying && clipActual != null)
        {
            audioSource.Play();
        }
    }

    public void Curar(InputAction.CallbackContext context)
    {
        if (!EsJugadorLocal() || pausa || saludJugador == null)
            return;

        if (context.performed)
            saludJugador.IniciarCuracion();
        else if (context.canceled)
            saludJugador.CancelarCuracion();
    }

    public void EntrarZonaCompra(ZonaCompra zona)
    {
        if (!EsJugadorLocal() || zona == null)
            return;

        zonaCompraActual = zona;

        if (textoComprar != null)
            textoComprar.SetActive(true);
    }

    public void SalirZonaCompra(ZonaCompra zona)
    {
        if (!EsJugadorLocal())
            return;

        if (zonaCompraActual != zona)
            return;

        zonaCompraActual = null;

        if (textoComprar != null)
            textoComprar.SetActive(false);
    }

    public void comprar(InputAction.CallbackContext context)
    {
        if (!EsJugadorLocal() || pausa || !context.performed)
            return;

        if (zonaCompraActual == null)
        {
            Debug.Log("No estás dentro de una zona de compra.");
            return;
        }

        zonaCompraActual.IntentarComprar();
    }

    private void OnDestroy()
    {
        if (EsJugadorLocal() && pausa)
            Time.timeScale = 1f;
    }

    public bool EstaEnPausa()
    {
        return pausa;
    }
}

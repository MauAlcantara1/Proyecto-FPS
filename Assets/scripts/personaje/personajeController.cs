
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
    [SerializeField] private GameObject panelPausa;

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
            return;

        pausa = false;
        Time.timeScale = 1f;

        if (panelPausa != null)
        {
            panelPausa.SetActive(false);
        }
        else
        {
            Debug.LogWarning(
                "No se ha asignado PanelPausa en el Inspector.");
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
        AplicarGravedad();
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

        if (panelPausa != null)
            panelPausa.SetActive(true);

        Time.timeScale = 0f;

        movimientoInput = Vector2.zero;
        saltar = false;
        correr = false;

        if (audioSource != null && audioSource.isPlaying)
            audioSource.Stop();
    }

    public void ReanudarJuego()
    {
        if (!EsJugadorLocal())
            return;

        pausa = false;
        Time.timeScale = 1f;

        if (panelPausa != null)
            panelPausa.SetActive(false);
    }

    private void MoverJugador()
    {
        if (controller == null)
            return;

        Vector3 direccion =
            transform.right * movimientoInput.x +
            transform.forward * movimientoInput.y;

        float velocidad = correr
            ? velocidadCorrer
            : velocidadCaminar;

        controller.Move(
            direccion * velocidad * Time.deltaTime);

        if (controller.isGrounded && saltar)
        {
            velocidadVertical.y =
                Mathf.Sqrt(fuerzaSalto * -2f * gravedad);

            saltar = false;
        }
    }

    private void AplicarGravedad()
    {
        if (controller == null)
            return;

        if (controller.isGrounded && velocidadVertical.y < 0)
            velocidadVertical.y = -2f;

        velocidadVertical.y += gravedad * Time.deltaTime;

        controller.Move(
            velocidadVertical * Time.deltaTime);
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
}

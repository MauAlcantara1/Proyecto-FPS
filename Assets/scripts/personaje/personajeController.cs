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
    [SerializeField] private AudioSource audioSource;
    [SerializeField] public AudioClip pasosCaminar;
    [SerializeField] public AudioClip pasosCorrer;
    [SerializeField] public Animator animator;
    [SerializeField] private GameObject linterna;
    [SerializeField] private Puntuacion puntuacion;

    
    [SerializeField] private personajeVida saludJugador;   // <-- agregar esto


    private CharacterController controller;

    private Vector2 movimientoInput;
    private bool saltar;
    private bool correr;
    private bool pausa;
    private Vector3 velocidadVertical;


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

        // Buscar la puntuación del HUD local.
        puntuacion = FindFirstObjectByType<Puntuacion>();

        if (puntuacion == null)
        {
            Debug.LogError(
                "No se encontró Puntuacion en la escena."
            );
            return;
        }

        // Buscar el controlador de armas del jugador.
        armaController armas =
            GetComponentInChildren<armaController>(true);

        if (armas == null)
        {
            Debug.LogError(
                "No se encontró armaController en JugadorOnline."
            );
            return;
        }

        puntuacion.AsignarArmaJugador(armas);

        Debug.Log("Puntuación y armas locales conectadas.");
    } 

    private void Update()
    {
        if (!EsJugadorLocal())
        return;

        if (saludJugador != null && saludJugador.EstaCurandose())
        {
            if (movimientoInput.magnitude > 0.1f || saltar)
            {
                saludJugador.CancelarCuracion();
            }
        }
        MoverJugador();
        AplicarGravedad();
        SonidosPasos();

    }

    private PhotonView vistaJugador;

    private bool EsJugadorLocal()
    {
        return !PhotonNetwork.InRoom ||
            (vistaJugador != null && vistaJugador.IsMine);
    }


    public void Movimiento(InputAction.CallbackContext context)
    {
        movimientoInput = context.ReadValue<Vector2>();

    }


    public void Linterna(InputAction.CallbackContext context)
    {
        if (!EsJugadorLocal())
        return;
        if (context.performed)
        {
            linterna.SetActive(!linterna.activeSelf);
        }
    }
    
    public void Salto(InputAction.CallbackContext context)
    {
        if (!EsJugadorLocal())
        return;

        if(context.performed)
        {
            saltar = true;
        }
    }

    public void Correr(InputAction.CallbackContext context)
    {
        if (!EsJugadorLocal())
        return;

        correr = context.ReadValueAsButton();
    }

    public void Pausa(InputAction.CallbackContext context)
    {
        pausa = context.ReadValueAsButton();
    } //crear funcion para pausa



    private void MoverJugador()
    {
        Vector3 direccion =
            transform.right * movimientoInput.x +
            transform.forward * movimientoInput.y;


        float velocidad = correr
            ? velocidadCorrer
            : velocidadCaminar;


        controller.Move(
            direccion * velocidad * Time.deltaTime
        );


        if(controller.isGrounded && saltar)
        {
            velocidadVertical.y =
                Mathf.Sqrt(fuerzaSalto * -2f * gravedad);

            saltar = false;
        }
    }



    private void AplicarGravedad()
    {
        if(controller.isGrounded && velocidadVertical.y < 0)
        {
            velocidadVertical.y = -2f;
        }


        velocidadVertical.y += gravedad * Time.deltaTime;


        controller.Move(
            velocidadVertical * Time.deltaTime
        );
    }

    private void SonidosPasos()
    {
        if (!controller.isGrounded || movimientoInput.magnitude < 0.1f)
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }

            return;
        }

        AudioClip clipActual = correr
            ? pasosCorrer
            : pasosCaminar;

        if (audioSource.clip != clipActual)
        {
            audioSource.clip = clipActual;
            audioSource.Play();
        }
        else if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    public void Curar(InputAction.CallbackContext context)
    {
        if (!EsJugadorLocal())
        return;
        if (saludJugador == null) return;

        // Al PRESIONAR la tecla -> Inicia la curación
        if (context.performed)
        {
            saludJugador.IniciarCuracion();
        }
        // Al SOLTAR la tecla -> Cancela la curación
        else if (context.canceled)
        {
            saludJugador.CancelarCuracion();
        }
    }

   
    public void comprar(InputAction.CallbackContext context)
    {
        if (!EsJugadorLocal())
            return;

        if (!context.performed)
            return;

        if (puntuacion == null)
        {
            Debug.LogError(
                "No hay una referencia de Puntuacion."
            );
            return;
        }

        puntuacion.ComprarArma();
    }

}
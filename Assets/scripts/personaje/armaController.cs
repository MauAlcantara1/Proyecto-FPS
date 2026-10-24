using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;

public class armaController : MonoBehaviour
{

    [Header("Imagen del arma primaria")]
    private Image imagenArmaHUD;
    private Sprite spritePistolaHUD;
    private Sprite spriteFusilHUD;
    
    private Animator animatorActual;
    private CharacterController controller;
    private MunicionArma municionActual;

    private TMP_Text textoCargadorHUD;
    private TMP_Text textoMunicionHUD;

    public enum TipoArma
    {
        pistola = 0,
        fusil = 1,
        uzi = 2
    }

    [Header("Arma primaria")]
    [SerializeField] private TipoArma armaActiva = TipoArma.pistola;

    [Header("Armas")]
    [SerializeField] private GameObject pistola;
    [SerializeField] private GameObject fusil;
    [SerializeField] private GameObject uzi;

    [Header("Segunda arma")]
    [SerializeField] private GameObject segundaArma;

    [Header("Indicadores de selección")]
    [SerializeField] private GameObject UIseleccionadaUno;
    [SerializeField] private GameObject UIseleccionadaDos;

    [Tooltip("Indicador visual de botiquines disponibles.")]
    [SerializeField] private GameObject UIseleccionadaBotiquin;

    [Header("Sistema de botiquines")]
    [SerializeField] private personajeVida saludJugador;

    [SerializeField] private bool primariaActiva = true;

    private bool disparando = false;

    [Header("Desbloqueo")]
    [SerializeField] private bool fusilDesbloqueado = false;

    public bool FusilDesbloqueado => fusilDesbloqueado;

    private PhotonView vistaJugador;

    private bool EsJugadorLocal()
    {
        return !PhotonNetwork.InRoom ||
            (vistaJugador != null && vistaJugador.IsMine);
    }

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        vistaJugador = GetComponentInParent<PhotonView>();

        if (saludJugador == null)
            saludJugador = GetComponentInParent<personajeVida>();

        if (saludJugador == null)
            saludJugador = GetComponentInChildren<personajeVida>(true);
    }

    private void Start()
    {
        if (EsJugadorLocal())
        {
            HUDArmasLocal hud =
                FindFirstObjectByType<HUDArmasLocal>();

            if (hud != null)
            {
                hud.Conectar(this);
            }
            else
            {
                Debug.LogWarning(
                    "No se encontró HUDArmasLocal en la escena."
                );
            }
        }

        ActualizarArmaVisible();
        ActualizarAnimator();
        ActualizarIndicadorBotiquin();
    }

    private void Update()
    {
        if (!EsJugadorLocal())
        return;
        ActualizarIndicadorBotiquin();

        if (EstaIncapacitado())
        {
            disparando = false;

            if (animatorActual != null)
                animatorActual.SetBool("Disparo", false);

            return;
        }

        if (!disparando)
            return;

        if (municionActual == null || !municionActual.PuedeDisparar())
        {
            if (animatorActual != null)
                animatorActual.SetBool("Disparo", false);

            disparando = false;
            return;
        }

        if (animatorActual != null)
            animatorActual.SetBool("Disparo", true);
    }

    public void ActualizarArmaVisible()
    {
        if (pistola != null)
            pistola.SetActive(
                primariaActiva && armaActiva == TipoArma.pistola
            );

        if (fusil != null)
            fusil.SetActive(
                primariaActiva && armaActiva == TipoArma.fusil
            );

        if (uzi != null)
            uzi.SetActive(
                primariaActiva && armaActiva == TipoArma.uzi
            );

        if (segundaArma != null)
            segundaArma.SetActive(!primariaActiva);

        // La selección cambia entre primaria y machete.
        // La imagen corresponde siempre al arma primaria actual.
        ActualizarIndicadoresArmas();
        ActualizarIndicadorBotiquin();
        ActualizarImagenArmaHUD();
    }

    private void ActualizarIndicadorBotiquin()
    {
        if (!EsJugadorLocal() || UIseleccionadaBotiquin == null)
            return;

        bool tieneBotiquines =
            saludJugador != null &&
            saludJugador.ObtenerCantidadBotiquines() > 0;

        UIseleccionadaBotiquin.SetActive(tieneBotiquines);
    }

    private void ActualizarAnimator()
    {
        if (primariaActiva)
        {
            animatorActual = ObtenerAnimatorArmaPrimaria();
        }
        else
        {
            animatorActual = segundaArma != null
                ? segundaArma.GetComponent<Animator>()
                : null;
        }

        ActualizarMunicion();
    }

    private void ActualizarMunicion()
    {
        MunicionArma municionAnterior = municionActual;

        // Siempre mostramos la munición de la pistola/fusil/Uzi,
        // incluso cuando se equipa el machete.
        GameObject armaPrimaria = ObtenerArmaPrimaria();
        municionActual = armaPrimaria != null
            ? armaPrimaria.GetComponentInChildren<MunicionArma>(true)
            : null;

        if (!EsJugadorLocal())
            return;

        if (municionAnterior != null && municionAnterior != municionActual)
            municionAnterior.DesconectarInterfaz();

        ActualizarHUDMunicion();
    }

    private Animator ObtenerAnimatorArmaPrimaria()
    {
        GameObject armaActual = ObtenerArmaPrimaria();

        return armaActual != null
            ? armaActual.GetComponent<Animator>()
            : null;
    }

    private GameObject ObtenerArmaPrimaria()
    {
        switch (armaActiva)
        {
            case TipoArma.pistola:
                return pistola;

            case TipoArma.fusil:
                return fusil;

            case TipoArma.uzi:
                return uzi;

            default:
                return null;
        }
    }

    public MunicionArma ObtenerMunicionArmaPrimaria()
    {
        if (!EsJugadorLocal())
            return null;

        GameObject arma = ObtenerArmaPrimaria();

        if (arma == null)
        {
            Debug.LogWarning(
                "No se encontró el arma primaria seleccionada."
            );

            return null;
        }

        MunicionArma municion =
            arma.GetComponentInChildren<MunicionArma>(true);

        if (municion == null)
        {
            Debug.LogWarning(
                "El arma " + arma.name +
                " no tiene componente MunicionArma."
            );
        }

        return municion;
    }

    public void Cambio(InputAction.CallbackContext context)
    {
    if (!EsJugadorLocal() || EstaIncapacitado())
        return;

        if (context.performed && animatorActual != null)
        {
            animatorActual.SetTrigger("Cambio");
            Debug.Log("Cambio de arma: armaController");
        }
    }

    public void Recarga(InputAction.CallbackContext context)
    {
        if (!EsJugadorLocal() || EstaIncapacitado())
            return;
        if (!context.performed || !primariaActiva)
            return;

        if (municionActual == null)
        {
            return;
        }

        if (!municionActual.PuedeRecargar())
        {
            return;
        }

        municionActual.IniciarRecarga();

        if (animatorActual != null)
            animatorActual.SetTrigger("Recarga");
    }

    public void Disparo(InputAction.CallbackContext context)
    {
        if (!EsJugadorLocal() || EstaIncapacitado())
            return;
        if (context.performed)
        {
            if (!primariaActiva)
            {
                if (animatorActual != null)
                    animatorActual.SetTrigger("Disparo");

                return;
            }

            disparando = true;
        }
        else if (context.canceled)
        {
            disparando = false;

            if (animatorActual != null)
                animatorActual.SetBool("Disparo", false);
        }
    }

    public void CambiarArma()
    {
        if (!EsJugadorLocal() || EstaIncapacitado())
            return;

    
        if (municionActual != null)
        {
            municionActual.CancelarRecarga();
        }

        if (animatorActual != null)
        {
            animatorActual.SetBool("Disparo", false);
        }

        disparando = false;

        primariaActiva = !primariaActiva;

        ActualizarArmaVisible();
        ActualizarAnimator();

        if (animatorActual != null)
        {
            animatorActual.Rebind();
            animatorActual.Update(0f);
        }

        Debug.Log("Cambio de arma completado.");
    }

    public Animator ObtenerAnimatorActual()
    {
        return animatorActual;
    }

    public void DesbloquearFusil()
    {
        if (!EsJugadorLocal() || EstaIncapacitado())
            return;

        if (fusil == null)
        {
            return;
        }

        fusilDesbloqueado = true;
        armaActiva = TipoArma.fusil;
        primariaActiva = true;

        ActualizarArmaVisible();
        ActualizarAnimator();

    }

    public void AsignarInterfaz(
        GameObject seleccionUno,
        GameObject seleccionDos,
        GameObject seleccionBotiquin)
    {
        if (!EsJugadorLocal())
            return;

        UIseleccionadaUno = seleccionUno;
        UIseleccionadaDos = seleccionDos;
        UIseleccionadaBotiquin = seleccionBotiquin;

        ActualizarIndicadoresArmas();
        ActualizarIndicadorBotiquin();
    }

    private void ActualizarIndicadoresArmas()
    {
        if (!EsJugadorLocal())
            return;

        if (UIseleccionadaUno != null)
            UIseleccionadaUno.SetActive(primariaActiva);

        if (UIseleccionadaDos != null)
            UIseleccionadaDos.SetActive(!primariaActiva);
    }

    public void AsignarInterfazMunicion(
        TMP_Text cargador,
        TMP_Text municion)
    {
        if (!EsJugadorLocal())
            return;

        textoCargadorHUD = cargador;
        textoMunicionHUD = municion;

        ActualizarHUDMunicion();
    }

    private void ActualizarHUDMunicion()
    {
        if (!EsJugadorLocal())
            return;

        if (municionActual != null)
        {
            municionActual.AsignarInterfaz(
                textoCargadorHUD,
                textoMunicionHUD
            );

            return;
        }
        if (textoCargadorHUD != null)
            textoCargadorHUD.text = "--";

        if (textoMunicionHUD != null)
            textoMunicionHUD.text = "--";
    }

    private bool EstaIncapacitado()
    {
        return saludJugador != null &&
            saludJugador.EstaIncapacitado();
    }

    public void AsignarImagenHUD(
        Image imagen,
        Sprite spritePistola,
        Sprite spriteFusil)
    {
        if (!EsJugadorLocal())
            return;

        imagenArmaHUD = imagen;
        spritePistolaHUD = spritePistola;
        spriteFusilHUD = spriteFusil;

        ActualizarImagenArmaHUD();
    }

    private void ActualizarImagenArmaHUD()
    {
        if (!EsJugadorLocal() || imagenArmaHUD == null)
            return;

        switch (armaActiva)
        {
            case TipoArma.pistola:
                imagenArmaHUD.sprite = spritePistolaHUD;
                break;

            case TipoArma.fusil:
                imagenArmaHUD.sprite = spriteFusilHUD;
                break;
        }
    }
}
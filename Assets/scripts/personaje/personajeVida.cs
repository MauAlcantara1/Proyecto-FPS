
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
public class personajeVida : MonoBehaviourPun
{
    [Header("UI TextMeshPro")]
    [SerializeField] private TMP_Text textoVida;

    [Header("Ajustes de Vida")]
    [SerializeField] private int vidaActual = 100;
    [SerializeField] private int vidaMaxima = 100;

    [Header("Ajustes de Inmunidad")]
    [SerializeField] private float segundosInmunidad = 2f;
    private float siguienteTiempoDaño = 0f;

    [Header("Imagenes Vida")]
    [SerializeField] private Image imagenVida;
    [SerializeField] private Sprite spriteVidaLlena;
    [SerializeField] private Sprite spriteVidaMedia;
    [SerializeField] private Sprite spriteVidaBaja;

    [Header("Sistema de Botiquín")]
    [SerializeField] private int cantidadBotiquines = 1;
    [SerializeField] private float tiempoCuracion = 3f;
    [SerializeField] private Slider barraProgresoCuracion;
    [SerializeField] private GameObject panelCuracionUI;

    private bool estaCurandose = false;
    private Coroutine corrutinaCuracion;

    [Header("Pantalla de Muerte")]
    [SerializeField] private PantallaMuerte pantallaMuerte;

    private bool pantallaMuerteMostrada = false;

    private bool EsJugadorLocal()
    {
        return !PhotonNetwork.InRoom || photonView.IsMine;
    }

    private void Start()
    {
        if (!EsJugadorLocal())
            return;

        if (panelCuracionUI != null)
            panelCuracionUI.SetActive(false);

        if (barraProgresoCuracion != null)
            barraProgresoCuracion.value = 0f;

        ActualizarInterfaz();
    }

    // Asignar la interfaz del jugador local desde PhotonManager.
    public void AsignarInterfaz(
        TMP_Text texto,
        Image imagen,
        Slider barraCuracion,
        GameObject panelCuracion)
    {
        if (!EsJugadorLocal())
            return;

        textoVida = texto;
        imagenVida = imagen;
        barraProgresoCuracion = barraCuracion;
        panelCuracionUI = panelCuracion;

        if (panelCuracionUI != null)
            panelCuracionUI.SetActive(false);

        if (barraProgresoCuracion != null)
            barraProgresoCuracion.value = 0f;

        ActualizarInterfaz();
    }

    public bool EstaVivo()
    {
        return vidaActual > 0;
    }

    public void RecibirDaño(int cantidad)
    {
        if (cantidad <= 0)
            return;

        if (!PhotonNetwork.InRoom)
        {
            AplicarDaño(cantidad);
            return;
        }

        if (photonView.IsMine)
        {
            AplicarDaño(cantidad);
        }
        else if (photonView.Owner != null)
        {
            photonView.RPC(
                nameof(RPC_RecibirDaño),
                photonView.Owner,
                cantidad
            );
        }
    }

    [PunRPC]
    private void RPC_RecibirDaño(int cantidad)
    {
        if (!photonView.IsMine)
            return;

        AplicarDaño(cantidad);
    }

    private void AplicarDaño(int cantidad)
    {
        if (!EstaVivo())
            return;

        if (Time.time < siguienteTiempoDaño)
            return;

        siguienteTiempoDaño =
            Time.time + segundosInmunidad;

        if (estaCurandose)
            CancelarCuracion();

        vidaActual = Mathf.Clamp(
            vidaActual - cantidad,
            0,
            vidaMaxima
        );

        ActualizarInterfaz();
        SincronizarVida();
        ComprobarMuerte();
    }

    private void SincronizarVida()
    {
        if (!PhotonNetwork.InRoom || !photonView.IsMine)
            return;

        photonView.RPC(
            nameof(RPC_SincronizarVida),
            RpcTarget.Others,
            vidaActual
        );
    }

    [PunRPC]
    private void RPC_SincronizarVida(int nuevaVida)
    {
        if (photonView.IsMine)
            return;

        vidaActual = Mathf.Clamp(
            nuevaVida,
            0,
            vidaMaxima
        );
    }

    // SISTEMA DE BOTIQUÍN

    public void IniciarCuracion()
    {
        if (!EsJugadorLocal())
            return;

        if (cantidadBotiquines > 0 &&
            !estaCurandose &&
            EstaVivo() &&
            vidaActual < vidaMaxima)
        {
            corrutinaCuracion =
                StartCoroutine(ProcesoCuracion());
        }
    }

    public void CancelarCuracion()
    {
        if (estaCurandose && corrutinaCuracion != null)
        {
            StopCoroutine(corrutinaCuracion);

            corrutinaCuracion = null;
            estaCurandose = false;

            if (panelCuracionUI != null)
                panelCuracionUI.SetActive(false);

            if (barraProgresoCuracion != null)
                barraProgresoCuracion.value = 0f;
        }
    }

    private IEnumerator ProcesoCuracion()
    {
        estaCurandose = true;

        if (panelCuracionUI != null)
            panelCuracionUI.SetActive(true);

        float tiempoTranscurrido = 0f;

        while (tiempoTranscurrido < tiempoCuracion)
        {
            tiempoTranscurrido += Time.deltaTime;

            if (barraProgresoCuracion != null)
            {
                barraProgresoCuracion.value =
                    tiempoTranscurrido / tiempoCuracion;
            }

            yield return null;
        }

        int vidaFaltante = vidaMaxima - vidaActual;

        int cantidadACurar = Mathf.RoundToInt(
            vidaFaltante * 0.80f
        );

        if (cantidadACurar < 1)
            cantidadACurar = 1;

        vidaActual = Mathf.Clamp(
            vidaActual + cantidadACurar,
            0,
            vidaMaxima
        );

        cantidadBotiquines--;

        estaCurandose = false;
        corrutinaCuracion = null;

        if (barraProgresoCuracion != null)
            barraProgresoCuracion.value = 0f;

        if (panelCuracionUI != null)
            panelCuracionUI.SetActive(false);

        ActualizarInterfaz();
        SincronizarVida();
    }

    // NUEVO: permite comprar botiquines adicionales.
    public bool ComprarBotiquines(int cantidad)
    {
        if (!EsJugadorLocal())
            return false;

        if (cantidad <= 0 || !EstaVivo())
            return false;

        cantidadBotiquines += cantidad;

        Debug.Log(
            "Botiquines disponibles: " + cantidadBotiquines
        );

        return true;
    }

    private void ActualizarInterfaz()
    {
        if (!EsJugadorLocal())
            return;

        if (textoVida != null)
        {
            textoVida.SetText(" + {0}", vidaActual);
        }
        else
        {
            Debug.LogWarning(
                "No está asignado textoVida en el jugador local.",
                this
            );
        }

        ActualizarColorVida();
    }

    private void ActualizarColorVida()
    {
        if (!EsJugadorLocal())
            return;

        Color nuevoColor;
        Sprite nuevoSprite;

        float porcentajeVida = vidaMaxima > 0
            ? (float)vidaActual / vidaMaxima
            : 0f;

        if (porcentajeVida >= 0.8f)
        {
            nuevoColor = new Color(0.54f, 1f, 0.53f);
            nuevoSprite = spriteVidaLlena;
        }
        else if (porcentajeVida >= 0.4f)
        {
            nuevoColor = new Color(1f, 0.5f, 0f);
            nuevoSprite = spriteVidaMedia;
        }
        else
        {
            nuevoColor = Color.red;
            nuevoSprite = spriteVidaBaja;
        }

        if (textoVida != null)
            textoVida.color = nuevoColor;

        if (imagenVida != null)
            imagenVida.sprite = nuevoSprite;
    }

    public bool EstaCurandose()
    {
        return estaCurandose;
    }

    private void ComprobarMuerte()
    {
        if (pantallaMuerteMostrada || EstaVivo())
            return;

        if (!EsJugadorLocal())
            return;

        pantallaMuerteMostrada = true;

        if (estaCurandose)
            CancelarCuracion();

        int puntuacionFinal = 0;

        Puntuacion puntuacion =
            FindFirstObjectByType<Puntuacion>();

        if (puntuacion != null)
        {
            puntuacionFinal =
                puntuacion.ObtenerPuntuacionTotalPartida();

            puntuacion.RegistrarFinPartida();
        }

        if (pantallaMuerte != null)
        {
            pantallaMuerte.MostrarPantallaMuerte(
                puntuacionFinal
            );
        }
        else
        {
            Debug.LogWarning(
                "No está asignada PantallaMuerte en personajeVida.",
                this
            );
        }
    }
}

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class HUDVidaCompanero : MonoBehaviour
{
    [Header("Interfaz del compañero")]
    [SerializeField] private TMP_Text textoVidaCompanero;
    [SerializeField] private Image imagenVidaCompanero;

    [Header("Contenedor de vida")]
    [SerializeField] private GameObject panelVidaCompanero;

    [Header("Sprites de vida")]
    [SerializeField] private Sprite spriteVidaLlena;
    [SerializeField] private Sprite spriteVidaMedia;
    [SerializeField] private Sprite spriteVidaBaja;

    private personajeVida saludCompanero;

    private int ultimaVida = -1;
    private float proximaBusqueda = 0f;

    private void Start()
    {
        if (panelVidaCompanero != null)
            panelVidaCompanero.SetActive(false);
    }

    private void Update()
    {
        // Este HUD solo se utiliza en multijugador.
        if (!PhotonNetwork.InRoom)
        {
            if (panelVidaCompanero != null &&
                panelVidaCompanero.activeSelf)
            {
                panelVidaCompanero.SetActive(false);
            }

            saludCompanero = null;
            ultimaVida = -1;
            return;
        }

        // Buscar al jugador remoto.
        if (saludCompanero == null)
        {
            if (Time.unscaledTime < proximaBusqueda)
                return;

            proximaBusqueda = Time.unscaledTime + 0.5f;

            BuscarCompanero();
        }

        if (saludCompanero == null)
            return;

        // Actualizar únicamente cuando cambie la vida.
        int vidaActual = saludCompanero.ObtenerVidaActual();

        if (vidaActual != ultimaVida)
        {
            ultimaVida = vidaActual;
            ActualizarInterfaz();
        }
    }

    private void BuscarCompanero()
    {
        personajeVida[] jugadores =
            FindObjectsByType<personajeVida>(
                FindObjectsSortMode.None
            );

        foreach (personajeVida jugador in jugadores)
        {
            if (jugador == null)
                continue;

            PhotonView vista = jugador.photonView;

            if (vista == null)
                continue;

            // Ignorar a nuestro propio jugador.
            if (vista.IsMine)
                continue;

            saludCompanero = jugador;
            ultimaVida = -1;

            if (panelVidaCompanero != null)
                panelVidaCompanero.SetActive(true);

            ActualizarInterfaz();

            ultimaVida = saludCompanero.ObtenerVidaActual();

            Debug.Log(
                "HUD conectado a la vida del compañero: " +
                vista.ViewID
            );

            return;
        }

        // No hay compañero conectado.
        if (panelVidaCompanero != null)
            panelVidaCompanero.SetActive(false);
    }

    private void ActualizarInterfaz()
    {
        if (saludCompanero == null)
            return;

        int vidaActual = saludCompanero.ObtenerVidaActual();
        int vidaMaxima = saludCompanero.ObtenerVidaMaxima();

        float porcentajeVida = vidaMaxima > 0
            ? (float)vidaActual / vidaMaxima
            : 0f;

        Color nuevoColor;
        Sprite nuevoSprite;

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

        // Actualizar texto.
        if (textoVidaCompanero != null)
        {
            textoVidaCompanero.SetText(
                " + {0}",
                vidaActual
            );

            textoVidaCompanero.color = nuevoColor;
        }

        // Actualizar imagen.
        if (imagenVidaCompanero != null)
        {
            imagenVidaCompanero.sprite = nuevoSprite;
        }
    }
}
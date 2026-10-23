using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class GameOverMultijugador : MonoBehaviourPunCallbacks
{
    [Header("Panel Game Over")]
    [SerializeField] private GameObject panelGameOver;

    [Header("Panel de incapacitación")]
    [SerializeField] private GameObject panelIncapacitado;

    [Header("Jugador 1")]
    [SerializeField] private TMP_Text textoNombreJugador1;
    [SerializeField] private TMP_Text textoPuntuacionJugador1;

    [Header("Jugador 2")]
    [SerializeField] private TMP_Text textoNombreJugador2;
    [SerializeField] private TMP_Text textoPuntuacionJugador2;

    [Header("Escena")]
    [SerializeField] private string escenaMenu = "Menu";

    private const string CLAVE_INCAPACITADO = "MZ_Incapacitado";
    private const string CLAVE_PUNTOS = "MZ_Puntos";
    private const string CLAVE_GAMEOVER = "MZ_GameOver";

    public static bool PartidaTerminada { get; private set; }

    private bool solicitudFinEnviada;
    private bool saliendoAlMenu;

    private void Awake()
    {
        PartidaTerminada = false;

        if (panelGameOver != null)
            panelGameOver.SetActive(false);
    }

    private void Start()
    {
        if (!PhotonNetwork.InRoom)
            return;

        // Permitir que Photon continúe procesando
        // mensajes incluso con Time.timeScale en cero.
        PhotonNetwork.MinimalTimeScaleToDispatchInFixedUpdate = 0.01f;

        // Reiniciar las propiedades del jugador
        // para esta nueva partida.
        Hashtable propiedadesJugador = new Hashtable
        {
            { CLAVE_INCAPACITADO, false },
            { CLAVE_PUNTOS, 0 }
        };

        PhotonNetwork.LocalPlayer.SetCustomProperties(
            propiedadesJugador
        );

        // Inicializar el estado de la sala.
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.SetCustomProperties(
                new Hashtable
                {
                    { CLAVE_GAMEOVER, false }
                }
            );
        }
    }

    // Se llama cuando nuestro jugador cae a 0 HP.
    public void RegistrarIncapacitado()
    {
        if (!PhotonNetwork.InRoom || PartidaTerminada)
            return;

        int puntos = ObtenerPuntuacionLocal();

        Hashtable propiedades = new Hashtable
        {
            { CLAVE_INCAPACITADO, true },
            { CLAVE_PUNTOS, puntos }
        };

        PhotonNetwork.LocalPlayer.SetCustomProperties(
            propiedades
        );

        Debug.Log(
            "[GAME OVER] Jugador incapacitado. Puntos: " +
            puntos
        );
    }

    // Se llama cuando nuestro jugador es reanimado.
    public void RegistrarReanimado()
    {
        if (!PhotonNetwork.InRoom || PartidaTerminada)
            return;

        PhotonNetwork.LocalPlayer.SetCustomProperties(
            new Hashtable
            {
                { CLAVE_INCAPACITADO, false }
            }
        );

        Debug.Log("[GAME OVER] Jugador reanimado.");
    }

    private int ObtenerPuntuacionLocal()
    {
        Puntuacion puntuacion =
            FindFirstObjectByType<Puntuacion>();

        if (puntuacion == null)
        {
            Debug.LogWarning(
                "No se encontró Puntuacion para el jugador local."
            );

            return 0;
        }

        return puntuacion.ObtenerPuntuacionTotalPartida();
    }

    public override void OnPlayerPropertiesUpdate(
        Player jugador,
        Hashtable propiedadesActualizadas)
    {
        if (PartidaTerminada)
            return;

        VerificarFinPartida();
    }

    private void VerificarFinPartida()
    {
        // El anfitrión confirma el Game Over.
        if (!PhotonNetwork.InRoom ||
            !PhotonNetwork.IsMasterClient ||
            solicitudFinEnviada ||
            PartidaTerminada)
        {
            return;
        }

        Player[] jugadores = PhotonNetwork.PlayerList;

        // La partida cooperativa requiere dos jugadores.
        if (jugadores.Length != 2)
            return;

        foreach (Player jugador in jugadores)
        {
            if (!jugador.CustomProperties.TryGetValue(
                    CLAVE_INCAPACITADO,
                    out object estado))
            {
                return;
            }

            if (!(estado is bool incapacitado) ||
                !incapacitado)
            {
                return;
            }

            // Esperar también la puntuación.
            if (!jugador.CustomProperties.TryGetValue(
                    CLAVE_PUNTOS,
                    out object puntos))
            {
                return;
            }

            if (!(puntos is int))
                return;
        }

        // Ambos están incapacitados.
        solicitudFinEnviada = true;

        Debug.Log(
            "[GAME OVER] Ambos jugadores incapacitados."
        );

        PhotonNetwork.CurrentRoom.SetCustomProperties(
            new Hashtable
            {
                { CLAVE_GAMEOVER, true }
            }
        );
    }

    public override void OnRoomPropertiesUpdate(
        Hashtable propiedadesActualizadas)
    {
        if (!propiedadesActualizadas.TryGetValue(
                CLAVE_GAMEOVER,
                out object resultado))
        {
            return;
        }

        if (resultado is bool terminado && terminado)
        {
            MostrarGameOver();
        }
    }

    private void MostrarGameOver()
    {
        if (PartidaTerminada)
            return;

        PartidaTerminada = true;

        Player[] jugadores = PhotonNetwork.PlayerList;

        if (jugadores.Length != 2)
        {
            Debug.LogError(
                "No hay exactamente dos jugadores para mostrar resultados."
            );
            return;
        }

        // Ordenar por entrada a la sala.
        // El primero será Jugador 1.
        System.Array.Sort(
            jugadores,
            (a, b) => a.ActorNumber.CompareTo(b.ActorNumber)
        );

        MostrarDatosJugador(
            jugadores[0],
            textoNombreJugador1,
            textoPuntuacionJugador1,
            1
        );

        MostrarDatosJugador(
            jugadores[1],
            textoNombreJugador2,
            textoPuntuacionJugador2,
            2
        );

        // Ocultar panel anterior de incapacitación.
        if (panelIncapacitado != null)
            panelIncapacitado.SetActive(false);

        // Mostrar panel final.
        if (panelGameOver != null)
        {
            panelGameOver.SetActive(true);
            panelGameOver.transform.SetAsLastSibling();
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Detener el juego en este cliente.
        Time.timeScale = 0f;

        Debug.Log("[GAME OVER] Partida finalizada.");
    }

    private void MostrarDatosJugador(
        Player jugador,
        TMP_Text textoNombre,
        TMP_Text textoPuntos,
        int numeroJugador)
    {
        string nombre = jugador.NickName;

        if (string.IsNullOrWhiteSpace(nombre))
            nombre = "Jugador " + numeroJugador;

        int puntos = 0;

        if (jugador.CustomProperties.TryGetValue(
                CLAVE_PUNTOS,
                out object valor))
        {
            if (valor is int)
                puntos = (int)valor;
        }

        if (textoNombre != null)
            textoNombre.text = nombre;

        if (textoPuntos != null)
            textoPuntos.text = puntos.ToString();
    }

    public void VolverAlMenu()
    {
        if (saliendoAlMenu)
            return;

        saliendoAlMenu = true;

        // Restaurar el tiempo antes de abandonar Photon.
        Time.timeScale = 1f;

        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            CargarMenu();
        }
    }

    public override void OnLeftRoom()
    {
        if (saliendoAlMenu)
            CargarMenu();
    }

    private void CargarMenu()
    {
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene(escenaMenu);
    }
}
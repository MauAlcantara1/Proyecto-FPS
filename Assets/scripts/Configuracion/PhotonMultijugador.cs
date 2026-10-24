using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using TMPro;
using UnityEngine.UI;

public class PhotonMultijugador : MonoBehaviourPunCallbacks
{
    [Header("Configuración")]
    [SerializeField] private byte maxJugadores = 2;

    private bool conectado = false;

    // Datos temporales por si el código generado ya existe
    private string nombreCreador;
    private int dificultadCreador;

    [Header("Menú")]
    [SerializeField] private MenuMultijugador menuMultijugador;

    [Header("Sala de espera")]
    [SerializeField] private TMP_Text textoCodigo;
    [SerializeField] private TMP_Text textoJugador1;
    [SerializeField] private TMP_Text textoJugador2;
    [SerializeField] private TMP_Text textoEstado;

    [Header("Inicio de partida")]
    [SerializeField] private Button botonIniciar;
    [SerializeField] private string escenaMultijugador;

    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        Debug.Log("Conectando a Photon...");
        PhotonNetwork.ConnectUsingSettings();
    }

    // =========================================================
    // CONEXIÓN
    // =========================================================

    public override void OnConnectedToMaster()
    {
        conectado = true;

        Debug.Log("[PHOTON] Conectado al servidor maestro.");

        DiagnosticarConexion("CONEXIÓN AL SERVIDOR");

        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedRoom()
    {
        DiagnosticarConexion("ENTRADA A SALA CONFIRMADA");
        Debug.Log("==========================");
        Debug.Log("ENTRAMOS A LA SALA");
        Debug.Log("Código: " + PhotonNetwork.CurrentRoom.Name);
        Debug.Log("Jugador: " + PhotonNetwork.NickName);
        Debug.Log("Jugadores: " + PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers);

        // Mostrar el menú de sala de espera
        if (menuMultijugador != null)
            menuMultijugador.MostrarSala();

        // Mostrar código en pantalla
        if (textoCodigo != null)
            textoCodigo.text = PhotonNetwork.CurrentRoom.Name;

        // Mostrar dificultad elegida por el creador
        MostrarDificultad();

        // Mostrar jugadores conectados y actualizar el estado del Loader
        ActualizarListaJugadores();

        Debug.Log("==========================");
    }

    public void CrearSala(string nombreJugador, int dificultad)
    {
        if (!conectado)
        {
            // 1. AVISO SI AÚN NO CONECTA AL SERVIDOR (Apaga el loader automáticamente)
            if (menuMultijugador != null)
                menuMultijugador.MostrarError("Conectando al servidor, intenta de nuevo en unos segundos.");
            
            return;
        }

        nombreCreador = nombreJugador;
        dificultadCreador = dificultad;

        PhotonNetwork.NickName = nombreJugador;

        CrearSalaConCodigo();
    }

    private void CrearSalaConCodigo()
    {
        string codigoSala = Random.Range(100000, 1000000).ToString();

        Hashtable propiedades = new Hashtable
        {
            { "Dificultad", dificultadCreador }
        };

        RoomOptions opciones = new RoomOptions
        {
            MaxPlayers = maxJugadores,
            IsOpen = true,
            IsVisible = false,
            CustomRoomProperties = propiedades
        };

        Debug.Log("Intentando crear sala: " + codigoSala);

        PhotonNetwork.CreateRoom(codigoSala, opciones);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log(
            "[PHOTON HOST] SALA CREADA CORRECTAMENTE." +
            "\nCódigo: " + PhotonNetwork.CurrentRoom.Name +
            "\nJugadores: " + PhotonNetwork.CurrentRoom.PlayerCount +
            "\nSala abierta: " + PhotonNetwork.CurrentRoom.IsOpen
        );

        DiagnosticarConexion("SALA CREADA");
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogWarning("No se pudo crear la sala. Generando otro código...");
        CrearSalaConCodigo();
    }

    // =========================================================
    // UNIRSE
    // =========================================================

    public void UnirseASala(string nombreJugador, string codigo)
    {
        if (!conectado || !PhotonNetwork.IsConnectedAndReady)
        {
            if (menuMultijugador != null)
            {
                menuMultijugador.MostrarError(
                    "Aún no está lista la conexión a Photon."
                );
            }

            return;
        }

        PhotonNetwork.NickName = nombreJugador;

        codigo = codigo.Trim();

        DiagnosticarConexion("ANTES DE UNIRSE");

        Debug.Log(
            "[PHOTON INVITADO] Intentando entrar." +
            "\nCódigo solicitado: [" + codigo + "]"
        );

        bool solicitudEnviada = PhotonNetwork.JoinRoom(codigo);

        Debug.Log(
            "[PHOTON INVITADO] Solicitud enviada: " +
            solicitudEnviada
        );

        if (!solicitudEnviada && menuMultijugador != null)
        {
            menuMultijugador.MostrarError(
                "Photon todavía no permite solicitar la entrada."
            );
        }
    }
    public override void OnJoinRoomFailed(
        short returnCode,
        string message)
    {
        Debug.LogError(
            "[PHOTON] ERROR AL UNIRSE." +
            "\nCódigo Photon: " + returnCode +
            "\nMensaje: " + message
        );

        DiagnosticarConexion("ERROR AL UNIRSE");

        if (menuMultijugador == null)
            return;

        if (returnCode == 32758)
        {
            menuMultijugador.MostrarError(
                "La sala no existe en este servidor. " +
                "Verifica el código y la región."
            );
        }
        else
        {
            menuMultijugador.MostrarError(
                "No se pudo entrar: " + message
            );
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        conectado = false;
        Debug.LogError("Desconectado de Photon: " + cause);

        // 4. ERROR DE CAÍDA DE RED
        if (menuMultijugador != null)
        {
            menuMultijugador.MostrarMenuPrincipal();
            menuMultijugador.MostrarError("Se perdió la conexión con el servidor.");
        }
    }

    private void MostrarDificultad()
    {
        if (!PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("Dificultad"))
            return;

        int dificultad = (int)PhotonNetwork.CurrentRoom.CustomProperties["Dificultad"];
        Debug.Log("Dificultad: " + (dificultad == 0 ? "Fácil" : "Difícil"));
    }

    private void ActualizarListaJugadores()
    {
        Player[] jugadores = PhotonNetwork.PlayerList;

        if (textoJugador1 != null)
            textoJugador1.text = "Esperando jugador...";

        if (textoJugador2 != null)
            textoJugador2.text = "Esperando jugador...";

        if (jugadores.Length >= 1 && textoJugador1 != null)
            textoJugador1.text = jugadores[0].NickName;

        if (jugadores.Length >= 2 && textoJugador2 != null)
            textoJugador2.text = jugadores[1].NickName;

        if (textoEstado != null)
        {
            textoEstado.text =
                jugadores.Length +
                " / " +
                PhotonNetwork.CurrentRoom.MaxPlayers +
                " jugadores";
        }

        // 5. CONTROL AUTOMÁTICO DEL LOADER EN LA SALA DE ESPERA
        if (menuMultijugador != null)
        {
            if (jugadores.Length < PhotonNetwork.CurrentRoom.MaxPlayers)
            {
                // Si falta un jugador, se muestra el loader esperando
                menuMultijugador.MostrarLoader("Esperando a otro jugador...");
            }
            else
            {
                // En cuanto están los 2 jugadores listos, ocultamos el loader
                menuMultijugador.OcultarLoader();
            }
        }

        ActualizarBotonIniciar();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(newPlayer.NickName + " entró a la sala.");
        ActualizarListaJugadores();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log(otherPlayer.NickName + " abandonó la sala.");
        ActualizarListaJugadores();

        if (menuMultijugador != null)
        {
            menuMultijugador.MostrarError(otherPlayer.NickName + " abandonó la sala.");
        }
    }

    // =========================================================
    // SALIR DE LA SALA
    // =========================================================

    public void SalirDeSala()
    {
        if (!PhotonNetwork.InRoom)
            return;

        Debug.Log("Saliendo de la sala...");
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        Debug.Log("Salimos de la sala.");

        if (menuMultijugador != null)
            menuMultijugador.MostrarMenuPrincipal();

        if (textoCodigo != null) textoCodigo.text = "";
        if (textoJugador1 != null) textoJugador1.text = "";
        if (textoJugador2 != null) textoJugador2.text = "";
        if (textoEstado != null) textoEstado.text = "";
    }

    private void ActualizarBotonIniciar()
    {
        if (botonIniciar == null)
            return;

        bool soyMaster = PhotonNetwork.IsMasterClient;
        bool salaCompleta =
            PhotonNetwork.CurrentRoom.PlayerCount >=
            PhotonNetwork.CurrentRoom.MaxPlayers;

        // Solo el creador ve el botón
        botonIniciar.gameObject.SetActive(soyMaster);

        // Y solo puede pulsarlo cuando estén los 2 jugadores
        botonIniciar.interactable = soyMaster && salaCompleta;
    }

    public void IniciarPartida()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            if (menuMultijugador != null)
                menuMultijugador.MostrarError("Solo el anfitrión puede iniciar la partida.");
            return;
        }

        if (PhotonNetwork.CurrentRoom.PlayerCount < 2)
        {
            if (menuMultijugador != null)
                menuMultijugador.MostrarError("Falta un jugador para iniciar.");
            return;
        }

        Debug.Log("Iniciando partida...");

        if (menuMultijugador != null)
            menuMultijugador.MostrarLoader("Iniciando partida...");

        // Ya no permitimos que entren más jugadores
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;

        PhotonNetwork.LoadLevel(escenaMultijugador);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        Debug.Log("Nuevo MasterClient: " + newMasterClient.NickName);
        ActualizarListaJugadores();
    }

    private void DiagnosticarConexion(string etapa)
    {
        Debug.Log(
            "\n========== PHOTON ==========" +
            "\nEtapa: " + etapa +
            "\nRegión: " + PhotonNetwork.CloudRegion +
            "\nCluster: " + PhotonNetwork.CurrentCluster +
            "\nGameVersion: " + PhotonNetwork.GameVersion +
            "\nAppVersion: " + PhotonNetwork.AppVersion +
            "\nEstado: " + PhotonNetwork.NetworkClientState +
            "\nConexión lista: " + PhotonNetwork.IsConnectedAndReady +
            "\nEn sala: " + PhotonNetwork.InRoom +
            "\nSala actual: " +
                (PhotonNetwork.CurrentRoom != null
                    ? PhotonNetwork.CurrentRoom.Name
                    : "NINGUNA") +
            "\n============================"
        );
    }
}
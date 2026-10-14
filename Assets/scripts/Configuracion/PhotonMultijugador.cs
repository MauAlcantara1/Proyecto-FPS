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

        Debug.Log("Conectado a Photon.");

        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("==========================");
        Debug.Log("ENTRAMOS A LA SALA");

        Debug.Log("Código: " + PhotonNetwork.CurrentRoom.Name);
        Debug.Log("Jugador: " + PhotonNetwork.NickName);

        Debug.Log(
            "Jugadores: " +
            PhotonNetwork.CurrentRoom.PlayerCount +
            "/" +
            PhotonNetwork.CurrentRoom.MaxPlayers
        );

        // Mostrar el menú de sala de espera
        if (menuMultijugador != null)
            menuMultijugador.MostrarSala();

        // Mostrar código en pantalla
        if (textoCodigo != null)
            textoCodigo.text = PhotonNetwork.CurrentRoom.Name;

        // Mostrar dificultad elegida por el creador
        MostrarDificultad();

        // Mostrar jugadores conectados
        ActualizarListaJugadores();

        Debug.Log("==========================");
    }


    public void CrearSala(string nombreJugador, int dificultad)
    {
        if (!conectado)
        {
            Debug.LogWarning("Todavía no estamos conectados a Photon.");
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
        Debug.Log("Sala creada correctamente.");
        Debug.Log("Código: " + PhotonNetwork.CurrentRoom.Name);
    }


    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogWarning(
            "No se pudo crear la sala. Generando otro código..."
        );

        CrearSalaConCodigo();
    }


    // =========================================================
    // UNIRSE
    // =========================================================

    public void UnirseASala(string nombreJugador, string codigo)
    {
        if (!conectado)
        {
            Debug.LogWarning("Todavía no estamos conectados a Photon.");
            return;
        }

        PhotonNetwork.NickName = nombreJugador;

        Debug.Log("Intentando entrar a la sala: " + codigo);

        PhotonNetwork.JoinRoom(codigo);
    }


    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogWarning(
            "No se pudo entrar a la sala: " + message
        );
    }



    public override void OnDisconnected(DisconnectCause cause)
    {
        conectado = false;

        Debug.LogError(
            "Desconectado de Photon: " + cause
        );
    }


    private void MostrarDificultad()
    {
        if (!PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("Dificultad"))
            return;

        int dificultad =
            (int)PhotonNetwork.CurrentRoom.CustomProperties["Dificultad"];

        Debug.Log(
            "Dificultad: " +
            (dificultad == 0 ? "Fácil" : "Difícil")
        );
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
        ActualizarBotonIniciar();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(
            newPlayer.NickName +
            " entró a la sala."
        );

        ActualizarListaJugadores();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log(
            otherPlayer.NickName +
            " abandonó la sala."
        );

        ActualizarListaJugadores();
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

        if (textoCodigo != null)
            textoCodigo.text = "";

        if (textoJugador1 != null)
            textoJugador1.text = "";

        if (textoJugador2 != null)
            textoJugador2.text = "";

        if (textoEstado != null)
            textoEstado.text = "";
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
            Debug.LogWarning("Solo el MasterClient puede iniciar la partida.");
            return;
        }

        if (PhotonNetwork.CurrentRoom.PlayerCount < 2)
        {
            Debug.LogWarning("Falta un jugador para iniciar.");
            return;
        }

        Debug.Log("Iniciando partida...");

        // Ya no permitimos que entren más jugadores
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;

        PhotonNetwork.LoadLevel(escenaMultijugador);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        Debug.Log(
            "Nuevo MasterClient: " +
            newMasterClient.NickName
        );

        ActualizarListaJugadores();
    }

}
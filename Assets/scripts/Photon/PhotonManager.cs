using System.Collections;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PhotonManager : MonoBehaviour
{
    [Header("Jugador")]
    [SerializeField] private GameObject prefabJugador;

    [Header("Spawn")]
    [SerializeField] private Transform[] puntosSpawn;

    private bool jugadorCreado = false;

    private IEnumerator Start()
    {
        Debug.Log("Esperando a Photon para crear jugador...");

        // Al cambiar de escena seguimos dentro de la misma sala.
        // Esperamos únicamente a que Photon esté listo.
        yield return new WaitUntil(() =>
            PhotonNetwork.IsConnectedAndReady &&
            PhotonNetwork.InRoom
        );

        CrearJugador();
    }

    private void CrearJugador()
    {
        if (jugadorCreado)
            return;

        if (prefabJugador == null)
        {
            Debug.LogError("No asignaste el prefab del jugador.");
            return;
        }

        if (puntosSpawn == null || puntosSpawn.Length == 0)
        {
            Debug.LogError("No asignaste puntos de Spawn.");
            return;
        }

        int indice = ObtenerIndiceJugador();

        if (indice >= puntosSpawn.Length)
            indice = indice % puntosSpawn.Length;

        Transform puntoSpawn = puntosSpawn[indice];

        GameObject jugador = PhotonNetwork.Instantiate(
            prefabJugador.name,
            puntoSpawn.position,
            puntoSpawn.rotation
        );

        jugadorCreado = true;

        Debug.Log(
            "Jugador creado correctamente | " +
            "Nombre: " + PhotonNetwork.NickName +
            " | Actor: " + PhotonNetwork.LocalPlayer.ActorNumber +
            " | Spawn: " + indice +
            " | Objeto: " + jugador.name
        );
    }

    private int ObtenerIndiceJugador()
    {
        Player[] jugadores = PhotonNetwork.PlayerList;

        for (int i = 0; i < jugadores.Length; i++)
        {
            if (jugadores[i].ActorNumber ==
                PhotonNetwork.LocalPlayer.ActorNumber)
            {
                return i;
            }
        }

        return 0;
    }
}
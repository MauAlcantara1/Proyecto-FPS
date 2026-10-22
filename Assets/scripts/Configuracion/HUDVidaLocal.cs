
using UnityEngine;
using TMPro;
using Photon.Pun;

public class HUDVidaJugadorLocal : MonoBehaviour
{
    [Header("Interfaz del jugador local")]
    [SerializeField] private TMP_Text textoVidaLocal;

    private personajeVida vidaLocal;
    private float proximaBusqueda = 0f;

    private void Update()
    {
        // Si ya encontramos al jugador, no buscar nuevamente.
        if (vidaLocal != null)
            return;

        if (Time.unscaledTime < proximaBusqueda)
            return;

        proximaBusqueda = Time.unscaledTime + 0.5f;

        BuscarJugadorLocal();
    }

    private void BuscarJugadorLocal()
    {
        if (textoVidaLocal == null)
        {
            Debug.LogError(
                "[HUD] No está asignado el texto de vida local."
            );

            enabled = false;
            return;
        }

        personajeVida[] jugadores =
            FindObjectsByType<personajeVida>();

        foreach (personajeVida jugador in jugadores)
        {
            if (jugador == null)
                continue;

            PhotonView vista =
                jugador.GetComponentInParent<PhotonView>();

            // En multijugador solo conectar nuestro jugador.
            if (PhotonNetwork.InRoom)
            {
                if (vista == null || !vista.IsMine)
                    continue;
            }

            vidaLocal = jugador;

            // Conectar el HUD al jugador.
            vidaLocal.AsignarTextoVida(textoVidaLocal);

            Debug.Log(
                "[HUD] Vida local conectada al jugador: " +
                jugador.gameObject.name
            );

            return;
        }
    }
}

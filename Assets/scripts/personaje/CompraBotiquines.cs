
using UnityEngine;
using Photon.Pun;

public class CompraBotiquines : MonoBehaviour
{
    [Header("Configuración de compra")]
    [SerializeField] private int costo = 25;
    [SerializeField] private int cantidadBotiquines = 1;

    public void Comprar(
        personajeController comprador,
        Puntuacion puntuacionManager)
    {
        if (comprador == null || puntuacionManager == null)
        {
            Debug.LogWarning(
                "[COMPRA BOTIQUÍN] Falta el comprador o Puntuacion.",
                this
            );
            return;
        }

        // Verificar que el comprador sea local.
        PhotonView vista =
            comprador.GetComponentInParent<PhotonView>();

        if (PhotonNetwork.InRoom &&
            (vista == null || !vista.IsMine))
        {
            Debug.LogWarning(
                "[COMPRA BOTIQUÍN] El comprador no es local."
            );
            return;
        }

        if (costo <= 0 || cantidadBotiquines <= 0)
        {
            Debug.LogWarning(
                "[COMPRA BOTIQUÍN] Configuración inválida.",
                this
            );
            return;
        }

        // Buscar la vida del jugador comprador.
        personajeVida vidaJugador =
            comprador.GetComponentInParent<personajeVida>();

        if (vidaJugador == null)
        {
            vidaJugador =
                comprador.GetComponentInChildren<personajeVida>(true);
        }

        if (vidaJugador == null)
        {
            Debug.LogWarning(
                "[COMPRA BOTIQUÍN] No se encontró personajeVida " +
                "en " + comprador.name
            );
            return;
        }

        if (!puntuacionManager.GastarPuntos(costo))
        {
            Debug.Log(
                "[COMPRA BOTIQUÍN] Puntos insuficientes."
            );
            return;
        }

        if (!vidaJugador.ComprarBotiquines(cantidadBotiquines))
        {
            puntuacionManager.DevolverPuntos(costo);

            Debug.LogWarning(
                "[COMPRA BOTIQUÍN] Compra cancelada. " +
                "Puntos devueltos."
            );
            return;
        }

        Debug.Log(
            "[COMPRA BOTIQUÍN] Compra realizada." +
            "\nJugador: " + comprador.name +
            "\nBotiquines comprados: " + cantidadBotiquines +
            "\nBotiquines disponibles: " +
            vidaJugador.ObtenerCantidadBotiquines() +
            "\nPuntos restantes: " +
            puntuacionManager.ObtenerPuntuacion()
        );
    }
}


using UnityEngine;
using Photon.Pun;

public class CompraMunicion : MonoBehaviour
{
    [Header("Configuración de compra")]
    [SerializeField] private int costo = 20;
    [SerializeField] private int cantidadMunicion = 30;

    public void Comprar(
        personajeController comprador,
        Puntuacion puntuacionManager)
    {
        if (comprador == null || puntuacionManager == null)
        {
            Debug.LogWarning(
                "[COMPRA MUNICIÓN] Falta el comprador o Puntuacion.",
                this
            );
            return;
        }

        // Solo el jugador local puede comprar.
        PhotonView vista =
            comprador.GetComponentInParent<PhotonView>();

        if (PhotonNetwork.InRoom &&
            (vista == null || !vista.IsMine))
        {
            Debug.LogWarning(
                "[COMPRA MUNICIÓN] El comprador no es local."
            );
            return;
        }

        if (costo <= 0 || cantidadMunicion <= 0)
        {
            Debug.LogWarning(
                "[COMPRA MUNICIÓN] Configuración inválida.",
                this
            );
            return;
        }

    armaController controladorArmas =
        comprador.GetComponentInChildren<armaController>(true);

    if (controladorArmas == null)
    {
        Debug.LogWarning(
            "[COMPRA MUNICIÓN] No se encontró armaController."
        );

        return;
    }

    MunicionArma armaJugador =
    controladorArmas.ObtenerMunicionArmaPrimaria();

        if (armaJugador == null)
        {
            Debug.LogWarning(
                "[COMPRA MUNICIÓN] No se encontró MunicionArma " +
                "en " + comprador.name
            );
            return;
        }

        if (!puntuacionManager.GastarPuntos(costo))
        {
            Debug.Log(
                "[COMPRA MUNICIÓN] Puntos insuficientes."
            );
            return;
        }

        if (!armaJugador.AgregarMunicionAlmacenada(
            cantidadMunicion))
        {
            puntuacionManager.DevolverPuntos(costo);

            Debug.LogWarning(
                "[COMPRA MUNICIÓN] Compra cancelada. " +
                "Puntos devueltos."
            );
            return;
        }

        Debug.Log(
            "[COMPRA MUNICIÓN] Compra realizada." +
            "\nJugador: " + comprador.name +
            "\nBalas compradas: " + cantidadMunicion +
            "\nMunición total: " +
            armaJugador.ObtenerMunicionAlmacenada() +
            "\nPuntos restantes: " +
            puntuacionManager.ObtenerPuntuacion()
        );
    }
}

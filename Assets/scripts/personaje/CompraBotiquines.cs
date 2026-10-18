using UnityEngine;

public class CompraBotiquines : MonoBehaviour
{
    [Header("Configuración de compra")]
    [SerializeField] private int costo = 25;
    [SerializeField] private int cantidadBotiquines = 1;

    [Header("Referencias")]
    [SerializeField] private Puntuacion puntuacionManager;
    [SerializeField] private personajeVida vidaJugador;

    public void Comprar()
    {
        if (puntuacionManager == null || vidaJugador == null)
        {
            Debug.LogWarning(
                "Asigna Puntuacion y personajeVida en el Inspector.", this);
            return;
        }

        if (costo <= 0 || cantidadBotiquines <= 0)
        {
            Debug.LogWarning(
                "El costo y la cantidad deben ser mayores que cero.", this);
            return;
        }

        if (!puntuacionManager.GastarPuntos(costo))
        {
            Debug.Log("No tienes suficientes puntos para comprar botiquines.");
            return;
        }

        if (!vidaJugador.ComprarBotiquines(cantidadBotiquines))
        {
            puntuacionManager.DevolverPuntos(costo);
            Debug.LogWarning(
                "No se pudo completar la compra. Se devolvieron los puntos.", this);
            return;
        }

        Debug.Log(
            "Compraste " + cantidadBotiquines +
            " botiquín(es) por $" + costo);
    }
}
using UnityEngine;

public class CompraMunicion : MonoBehaviour
{
    [Header("Configuración de compra")]
    [SerializeField] private int costo = 20;
    [SerializeField] private int cantidadMunicion = 30;

    [Header("Referencias")]
    [SerializeField] private Puntuacion puntuacionManager;
    [SerializeField] private MunicionArma municionArma1;
    [SerializeField] private MunicionArma municionArma2;

    public void Comprar()
    {
        if (puntuacionManager == null)
        {
            Debug.LogWarning("No está asignado Puntuacion.", this);
            return;
        }

        if (costo <= 0 || cantidadMunicion <= 0)
        {
            Debug.LogWarning(
                "El costo y la cantidad deben ser mayores que cero.", this);
            return;
        }

        MunicionArma armaEquipada = ObtenerArmaEquipada();

        if (armaEquipada == null)
        {
            Debug.LogWarning(
                "No se encontró un arma equipada para comprar munición.", this);
            return;
        }

        if (!puntuacionManager.GastarPuntos(costo))
        {
            Debug.Log("No tienes suficientes puntos para comprar munición.");
            return;
        }

        if (!armaEquipada.AgregarMunicionAlmacenada(cantidadMunicion))
        {
            puntuacionManager.DevolverPuntos(costo);
            Debug.Log(
                "No se pudo agregar munición. Se devolvieron los puntos.");
            return;
        }

        Debug.Log(
            "Compraste " + cantidadMunicion +
            " balas para " + armaEquipada.name +
            " por $" + costo);
    }

    private MunicionArma ObtenerArmaEquipada()
    {
        bool arma1Activa =
            municionArma1 != null &&
            municionArma1.gameObject.activeInHierarchy;

        bool arma2Activa =
            municionArma2 != null &&
            municionArma2.gameObject.activeInHierarchy;

        if (arma1Activa && !arma2Activa)
            return municionArma1;

        if (arma2Activa && !arma1Activa)
            return municionArma2;

        if (arma1Activa && arma2Activa)
        {
            Debug.LogWarning(
                "Ambas armas están activas. No se compró munición.", this);
        }

        return null;
    }
}
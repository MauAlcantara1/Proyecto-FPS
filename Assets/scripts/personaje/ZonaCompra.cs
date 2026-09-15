using UnityEngine;

public class ZonaCompra : MonoBehaviour
{
    [SerializeField] private Puntuacion puntuacionManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Jugador"))
        {
            if (puntuacionManager != null)
                puntuacionManager.EntrarZonaCompra();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Jugador"))
        {
            if (puntuacionManager != null)
                puntuacionManager.SalirZonaCompra();
        }
    }
}
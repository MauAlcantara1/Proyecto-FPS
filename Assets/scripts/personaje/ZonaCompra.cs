
using UnityEngine;

public class ZonaCompra : MonoBehaviour
{
    public enum TipoCompra
    {
        Arma,
        Municion,
        Botiquin
    }

    [Header("Tipo de artículo")]
    [SerializeField] private TipoCompra tipoCompra;

    [Header("Referencias")]
    [SerializeField] private Puntuacion puntuacionManager;
    [SerializeField] private CompraMunicion compraMunicion;
    [SerializeField] private CompraBotiquines compraBotiquines;

    public TipoCompra ObtenerTipoCompra()
    {
        return tipoCompra;
    }

    private void OnTriggerEnter(Collider other)
    {
        personajeController jugador =
            other.GetComponentInParent<personajeController>();

        if (jugador == null)
            return;

        jugador.EntrarZonaCompra(this);

        if (tipoCompra == TipoCompra.Arma &&
            puntuacionManager != null)
        {
            puntuacionManager.EntrarZonaCompra();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        personajeController jugador =
            other.GetComponentInParent<personajeController>();

        if (jugador == null)
            return;

        jugador.SalirZonaCompra(this);

        if (tipoCompra == TipoCompra.Arma &&
            puntuacionManager != null)
        {
            puntuacionManager.SalirZonaCompra();
        }
    }

    public void IntentarComprar()
    {
        switch (tipoCompra)
        {
            case TipoCompra.Arma:
                if (puntuacionManager == null)
                {
                    Debug.LogWarning(
                        "Asigna Puntuacion en la zona de armas.", this);
                    return;
                }

                puntuacionManager.ComprarArma();
                break;

            case TipoCompra.Municion:
                if (compraMunicion == null)
                {
                    Debug.LogWarning(
                        "Asigna CompraMunicion en la zona de munición.", this);
                    return;
                }

                compraMunicion.Comprar();
                break;

            case TipoCompra.Botiquin:
                if (compraBotiquines == null)
                {
                    Debug.LogWarning(
                        "Asigna CompraBotiquines en la zona de botiquines.", this);
                    return;
                }

                compraBotiquines.Comprar();
                break;
        }
    }
}

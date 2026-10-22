
using UnityEngine;
using Photon.Pun;

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

    private void Awake()
    {
        BuscarReferencias();
    }

    private void BuscarReferencias()
    {
        if (puntuacionManager == null)
        {
            puntuacionManager =
                FindAnyObjectByType<Puntuacion>();
        }

        if (compraMunicion == null)
        {
            compraMunicion =
                GetComponent<CompraMunicion>();
        }

        if (compraBotiquines == null)
        {
            compraBotiquines =
                GetComponent<CompraBotiquines>();
        }
    }

    public TipoCompra ObtenerTipoCompra()
    {
        return tipoCompra;
    }

    private bool EsJugadorLocal(personajeController jugador)
    {
        if (jugador == null)
            return false;

        if (!PhotonNetwork.InRoom)
            return true;

        PhotonView vista =
            jugador.GetComponentInParent<PhotonView>();

        return vista != null && vista.IsMine;
    }


    private void OnTriggerEnter(Collider other)
    {
        personajeController jugador =
            other.GetComponentInParent<personajeController>();

        if (!EsJugadorLocal(jugador))
            return;

        BuscarReferencias();

        Debug.Log(
            "[ZONA COMPRA] Entrando en zona: " +
            tipoCompra +
            " | Jugador: " + jugador.name
        );

        jugador.EntrarZonaCompra(this);

        if (puntuacionManager == null)
        {
            Debug.LogWarning(
                "[ZONA COMPRA] No se encontró Puntuacion.",
                this
            );
            return;
        }

        // Conservamos la lógica original para armas.
        if (tipoCompra == TipoCompra.Arma)
        {
            puntuacionManager.EntrarZonaCompra();
        }
        else
        {
            // Munición y botiquines ahora también
            // muestran el panel de compra.
            puntuacionManager.MostrarPanelCompra();
        }
    }



    private void OnTriggerExit(Collider other)
    {
        personajeController jugador =
            other.GetComponentInParent<personajeController>();

        if (!EsJugadorLocal(jugador))
            return;

        BuscarReferencias();

        Debug.Log(
            "[ZONA COMPRA] Saliendo de zona: " +
            tipoCompra
        );

        jugador.SalirZonaCompra(this);

        if (puntuacionManager == null)
            return;

        if (tipoCompra == TipoCompra.Arma)
        {
            puntuacionManager.SalirZonaCompra();
        }
        else
        {
            puntuacionManager.OcultarPanelCompra();
        }
    }

    public void IntentarComprar(personajeController comprador)
    {
        if (!EsJugadorLocal(comprador))
        {
            Debug.LogWarning(
                "[ZONA COMPRA] Compra rechazada: " +
                "jugador no local."
            );
            return;
        }

        BuscarReferencias();

        if (puntuacionManager == null)
        {
            Debug.LogError(
                "[ZONA COMPRA] No se encontró Puntuacion.",
                this
            );
            return;
        }

        Debug.Log(
            "[ZONA COMPRA] Procesando compra: " +
            tipoCompra +
            "\nComprador: " + comprador.name
        );

        switch (tipoCompra)
        {
            case TipoCompra.Arma:

                puntuacionManager.ComprarArma();
                break;

            case TipoCompra.Municion:

                if (compraMunicion == null)
                {
                    Debug.LogWarning(
                        "[ZONA COMPRA] Falta CompraMunicion.",
                        this
                    );
                    return;
                }

                compraMunicion.Comprar(
                    comprador,
                    puntuacionManager
                );
                break;

            case TipoCompra.Botiquin:

                if (compraBotiquines == null)
                {
                    Debug.LogWarning(
                        "[ZONA COMPRA] Falta CompraBotiquines.",
                        this
                    );
                    return;
                }

                compraBotiquines.Comprar(
                    comprador,
                    puntuacionManager
                );
                break;
        }
    }
}

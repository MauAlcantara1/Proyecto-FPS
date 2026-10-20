
using TMPro;
using UnityEngine;
using Photon.Pun;

public class MunicionArma : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private int capacidadCargador = 16;
    [SerializeField] private int cargadorActual = 16;
    [SerializeField] private int municionAlmacenada = 116;

    [Header("UI")]
    [SerializeField] private TMP_Text textoCargador;
    [SerializeField] private TMP_Text textoMunicion;

    [Header("Bala")]
    [SerializeField] private GameObject bala;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private int velocidadBala = 10;

    [SerializeField] private ParticleSystem efectoDisparo;

    private bool recargando = false;
    private PhotonView vistaJugador;

    private void Awake()
    {
        vistaJugador = GetComponentInParent<PhotonView>();
    }

    private bool EsJugadorLocal()
    {
        if (!PhotonNetwork.InRoom)
            return true;

        return vistaJugador != null && vistaJugador.IsMine;
    }

    private void Start()
    {
        if (!EsJugadorLocal())
            return;

        ActualizarUI();
    }

    public bool PuedeDisparar()
    {
        return cargadorActual > 0 && !recargando;
    }

    public bool PuedeRecargar()
    {
        return cargadorActual < capacidadCargador &&
               municionAlmacenada > 0 &&
               !recargando;
    }

    public void GastarBala()
    {
        if (!EsJugadorLocal() || !PuedeDisparar())
            return;

        if (bala == null || spawnPoint == null)
        {
            Debug.LogError(
                "Falta asignar Bala o SpawnPoint.", this);
            return;
        }

        cargadorActual--;
        ActualizarUI();

        GameObject nuevaBala = Instantiate(
            bala,
            spawnPoint.position,
            spawnPoint.rotation
        );

        if (efectoDisparo != null)
        {
            efectoDisparo.Stop(
                true,
                ParticleSystemStopBehavior.StopEmittingAndClear
            );

            efectoDisparo.Play();
        }

        Rigidbody rb = nuevaBala.GetComponent<Rigidbody>();

        if (rb != null)
            rb.linearVelocity = spawnPoint.forward * velocidadBala;
    }

    public void IniciarRecarga()
    {
        if (!EsJugadorLocal())
            return;

        if (!PuedeRecargar())
            return;

        recargando = true;
    }

    public void Recargar()
    {
        if (!EsJugadorLocal())
            return;

        if (!recargando)
            return;

        int balasNecesarias = capacidadCargador - cargadorActual;

        int balasARecargar = Mathf.Min(
            balasNecesarias,
            municionAlmacenada
        );

        cargadorActual += balasARecargar;
        municionAlmacenada -= balasARecargar;
        recargando = false;

        ActualizarUI();
    }

    // Agrega munición comprada al inventario de esta arma.
    public bool AgregarMunicionAlmacenada(int cantidad)
    {
        if (!EsJugadorLocal())
            return false;

        if (cantidad <= 0)
            return false;

        municionAlmacenada += cantidad;
        ActualizarUI();

        return true;
    }

    private void ActualizarUI()
    {
        if (!EsJugadorLocal())
            return;

        if (textoCargador != null)
            textoCargador.text = cargadorActual.ToString();

        if (textoMunicion != null)
            textoMunicion.text = municionAlmacenada.ToString();
    }

    public int ObtenerBalasCargador()
    {
        return cargadorActual;
    }

    public int ObtenerMunicionAlmacenada()
    {
        return municionAlmacenada;
    }

    public bool EstaRecargando()
    {
        return recargando;
    }

    public void AsignarInterfaz(
        TMP_Text cargador,
        TMP_Text municion)
    {
        if (!EsJugadorLocal())
            return;

        textoCargador = cargador;
        textoMunicion = municion;

        ActualizarUI();
    }

    public void DesconectarInterfaz()
    {
        if (!EsJugadorLocal())
            return;

        textoCargador = null;
        textoMunicion = null;
    }
}

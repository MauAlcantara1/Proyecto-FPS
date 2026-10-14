
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
        return cargadorActual == 0 &&
               municionAlmacenada > 0 &&
               !recargando;
    }

    // Llamado desde el Animation Event
    public void GastarBala()
    {
        // No generar balas desde jugadores remotos.
        if (!EsJugadorLocal())
            return;

        if (!PuedeDisparar())
            return;

        if (bala == null || spawnPoint == null)
        {
            Debug.LogError(
                "Falta asignar Bala o SpawnPoint.",
                this
            );
            return;
        }

        cargadorActual--;
        ActualizarUI();

        // La bala es local al jugador que dispara.
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
        {
            rb.linearVelocity =
                spawnPoint.forward * velocidadBala;
        }
    }

    public void IniciarRecarga()
    {
        if (!EsJugadorLocal())
            return;

        if (cargadorActual > 0)
            return;

        if (municionAlmacenada <= 0)
            return;

        recargando = true;
    }

    // También puede llamarse desde Animation Event
    public void Recargar()
    {
        if (!EsJugadorLocal())
            return;

        int balasNecesarias =
            capacidadCargador - cargadorActual;

        int balasARecargar = Mathf.Min(
            balasNecesarias,
            municionAlmacenada
        );

        cargadorActual += balasARecargar;
        municionAlmacenada -= balasARecargar;

        recargando = false;
        ActualizarUI();
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
}

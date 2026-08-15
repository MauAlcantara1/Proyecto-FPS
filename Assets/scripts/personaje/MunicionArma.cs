using TMPro;
using UnityEngine;

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
    [SerializeField] GameObject bala;
    [SerializeField] Transform spawnPoint;
    [SerializeField] private int velocidadBala = 10;

    [SerializeField] private ParticleSystem efectoDisparo;



    private bool recargando = false;

    private void Start()
    {
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

    public void GastarBala()
    {

        if (!PuedeDisparar())
        {
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
            efectoDisparo.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            efectoDisparo.Play();
        }

        Rigidbody rb = nuevaBala.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity = spawnPoint.forward * velocidadBala;
        }
    }
    public void IniciarRecarga()
    {
        if (cargadorActual > 0)
            return;

        if (municionAlmacenada <= 0)
            return;

        recargando = true;
    }

    public void Recargar()
    {

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
    private void ActualizarUI()
    {
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
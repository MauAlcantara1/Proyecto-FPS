using System.Collections;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class ReanimacionCooperativa : MonoBehaviour
{
    [Header("Aviso del compañero")]
    [SerializeField] private TMP_Text textoAvisoAliado;

    [SerializeField] private float duracionAviso = 3f;

    [Header("Comprobaciones")]
    [SerializeField] private float intervaloBusqueda = 0.5f;
    [SerializeField] private float intervaloReanimacion = 0.5f;

    private personajeVida jugadorLocal;
    private personajeVida compañero;

    private bool avisoMostrado = false;

    private float proximaBusqueda;
    private float proximoIntento;

    private Coroutine rutinaAviso;

    private void Awake()
    {
        if (textoAvisoAliado != null)
            textoAvisoAliado.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (GameOverMultijugador.PartidaTerminada)
            return;
        if (!PhotonNetwork.InRoom)
            return;

        if (Time.unscaledTime >= proximaBusqueda)
        {
            proximaBusqueda =
                Time.unscaledTime + intervaloBusqueda;

            BuscarJugadores();
        }

        if (jugadorLocal == null || compañero == null)
            return;

        if (!compañero.EstaIncapacitado())
        {
            avisoMostrado = false;
            return;
        }

        if (!jugadorLocal.EstaVivo())
            return;

        if (!avisoMostrado)
        {
            avisoMostrado = true;
            MostrarAviso();
        }

        if (Time.unscaledTime < proximoIntento)
            return;

        float distancia = Vector3.Distance(
            jugadorLocal.transform.position,
            compañero.transform.position
        );

        if (distancia <= compañero.ObtenerDistanciaReanimacion())
        {
            proximoIntento =
                Time.unscaledTime + intervaloReanimacion;

            compañero.SolicitarReanimacionAutomatica();
        }
    }

    private void BuscarJugadores()
    {
        personajeVida[] jugadores =
            FindObjectsByType<personajeVida>(
                FindObjectsSortMode.None
            );

        personajeVida nuevoLocal = null;
        personajeVida nuevoCompanero = null;

        foreach (personajeVida jugador in jugadores)
        {
            if (jugador == null)
                continue;

            PhotonView vista = jugador.photonView;

            if (vista == null)
                continue;

            if (vista.IsMine)
            {
                nuevoLocal = jugador;
            }
            else
            {
                nuevoCompanero = jugador;
            }
        }

        if (compañero != nuevoCompanero)
            avisoMostrado = false;

        jugadorLocal = nuevoLocal;
        compañero = nuevoCompanero;
    }

    private void MostrarAviso()
    {
        if (textoAvisoAliado == null)
            return;

        if (rutinaAviso != null)
            StopCoroutine(rutinaAviso);

        textoAvisoAliado.text =
            "TU ALIADO ESTÁ INCAPACITADO";

        textoAvisoAliado.gameObject.SetActive(true);

        rutinaAviso = StartCoroutine(OcultarAviso());
    }

    private IEnumerator OcultarAviso()
    {
        yield return new WaitForSecondsRealtime(duracionAviso);

        if (textoAvisoAliado != null)
            textoAvisoAliado.gameObject.SetActive(false);

        rutinaAviso = null;
    }
}
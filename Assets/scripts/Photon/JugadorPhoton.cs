using UnityEngine;
using Photon.Pun;

public class JugadorPhoton : MonoBehaviourPun
{
    [Header("Objetos exclusivos del jugador local")]
    [SerializeField] private GameObject[] objetosSoloLocal;

    [Header("Scripts exclusivos del jugador local")]
    [SerializeField] private MonoBehaviour[] scriptsSoloLocal;

    private void Start()
    {
        ConfigurarJugador();
    }

    private void ConfigurarJugador()
    {
        bool esMiJugador = photonView.IsMine;

        // Cámara, armas FPS, audio, etc.
        foreach (GameObject objeto in objetosSoloLocal)
        {
            if (objeto != null)
                objeto.SetActive(esMiJugador);
        }

        // Movimiento, mouse, disparo, interacción, etc.
        foreach (MonoBehaviour script in scriptsSoloLocal)
        {
            if (script != null)
                script.enabled = esMiJugador;
        }

        if (esMiJugador)
        {
            Debug.Log("Este es MI jugador");
        }
        else
        {
            Debug.Log("Este es un jugador REMOTO");
        }
    }
}
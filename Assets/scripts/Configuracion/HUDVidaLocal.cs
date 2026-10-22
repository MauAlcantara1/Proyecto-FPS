
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class HUDVidaJugadorLocal : MonoBehaviour
{
    [Header("Interfaz del jugador local")]
    [SerializeField] private TMP_Text textoVidaLocal;
    [SerializeField] private Image imagenVidaLocal;

    [Header("Curación")]
    [SerializeField] private Slider barraProgresoCuracion;
    [SerializeField] private GameObject panelCuracionUI;

    private personajeVida vidaLocal;
    private float proximaBusqueda = 0f;

    private void Update()
    {
        if (vidaLocal != null)
            return;

        if (Time.unscaledTime < proximaBusqueda)
            return;

        proximaBusqueda = Time.unscaledTime + 0.5f;

        BuscarJugadorLocal();
    }

    private void BuscarJugadorLocal()
    {
        if (textoVidaLocal == null ||
            imagenVidaLocal == null ||
            barraProgresoCuracion == null ||
            panelCuracionUI == null)
        {
            Debug.LogError(
                "[HUD VIDA] Faltan referencias en el Inspector.",
                this
            );
            return;
        }

        personajeVida[] jugadores =
            FindObjectsByType<personajeVida>();

        foreach (personajeVida jugador in jugadores)
        {
            if (jugador == null)
                continue;

            PhotonView vista =
                jugador.GetComponentInParent<PhotonView>();

            if (PhotonNetwork.InRoom)
            {
                if (vista == null || !vista.IsMine)
                    continue;
            }

            vidaLocal = jugador;

            // Asignar toda la interfaz local.
            vidaLocal.AsignarInterfaz(
                textoVidaLocal,
                imagenVidaLocal,
                barraProgresoCuracion,
                panelCuracionUI
            );

            Debug.Log(
                "[HUD VIDA] Interfaz conectada correctamente." +
                "\nJugador: " + jugador.name +
                "\nTexto: " + textoVidaLocal.name +
                "\nImagen: " + imagenVidaLocal.name +
                "\nBarra curación: " +
                barraProgresoCuracion.name +
                "\nPanel curación: " +
                panelCuracionUI.name
            );

            return;
        }
    }
}

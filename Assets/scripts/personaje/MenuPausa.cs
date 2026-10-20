using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class MenuPausa : MonoBehaviourPunCallbacks
{
    [Header("Paneles")]
    [SerializeField] private GameObject panelPausa;
    [SerializeField] private GameObject panelConfiguracion;

    [Header("Escenas")]
    [SerializeField] private string escenaMenu = "Menu";

    private personajeController jugador;
    private bool saliendoAlMenu = false;

    private void Awake()
    {
        OcultarPaneles();
    }

    // Conectar al jugador local creado por Photon
    public void AsignarJugador(personajeController jugadorLocal)
    {
        jugador = jugadorLocal;

        Debug.Log("Menú de pausa conectado al jugador local.");
    }

    public void MostrarPausa()
    {
        if (panelConfiguracion != null)
            panelConfiguracion.SetActive(false);

        if (panelPausa != null)
            panelPausa.SetActive(true);
    }

    public void OcultarPaneles()
    {
        if (panelPausa != null)
            panelPausa.SetActive(false);

        if (panelConfiguracion != null)
            panelConfiguracion.SetActive(false);
    }

    public void Continuar()
    {
        if (jugador != null)
        {
            jugador.ReanudarJuego();
        }
        else
        {
            Debug.LogWarning(
                "No hay un jugador local asignado al menú de pausa."
            );
        }
    }

    public void AbrirConfiguracion()
    {
        if (panelPausa != null)
            panelPausa.SetActive(false);

        if (panelConfiguracion != null)
            panelConfiguracion.SetActive(true);
    }

    public void CerrarConfiguracion()
    {
        if (panelConfiguracion != null)
            panelConfiguracion.SetActive(false);

        if (panelPausa != null)
            panelPausa.SetActive(true);
    }

    public void IrAlMenu()
    {
        if (jugador != null)
            jugador.ReanudarJuego();

        Time.timeScale = 1f;

        // Cada jugador abandona la sala por separado.
        if (PhotonNetwork.InRoom)
        {
            saliendoAlMenu = true;
            PhotonNetwork.LeaveRoom();
            return;
        }

        CargarMenu();
    }

    public override void OnLeftRoom()
    {
        if (saliendoAlMenu)
            CargarMenu();
    }

    private void CargarMenu()
    {
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene(escenaMenu);
    }
}
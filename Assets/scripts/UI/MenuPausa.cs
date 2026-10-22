using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class MenuPausa : MonoBehaviourPunCallbacks
{
    [Header("Contenedor General")]
    [SerializeField] private GameObject contenedorMenuPausa;

    [Header("Ventanas de las 3 Opciones")]
    [SerializeField] private GameObject panelPausa;
    [SerializeField] private GameObject panelConfiguracion;
    [SerializeField] private GameObject panelSalir;

    [Header("Escenas")]
    [SerializeField] private string escenaMenu = "Menu";

    private personajeController jugador;
    private bool saliendoAlMenu = false;

    private void Awake()
    {
        OcultarPaneles();
    }

    public void AsignarJugador(personajeController jugadorLocal)
    {
        jugador = jugadorLocal;
        Debug.Log("Menú de pausa conectado al jugador local.");
    }

    public void MostrarPausa()
    {
        if (contenedorMenuPausa != null)
            contenedorMenuPausa.SetActive(true);

        MostrarVentanaContinuar();
    }

    public void OcultarPaneles()
    {
        if (contenedorMenuPausa != null)
            contenedorMenuPausa.SetActive(false);

        if (panelPausa != null)
            panelPausa.SetActive(false);

        if (panelConfiguracion != null)
            panelConfiguracion.SetActive(false);

        if (panelSalir != null)
            panelSalir.SetActive(false);
    }


    public void MostrarVentanaContinuar()
    {
        if (panelPausa != null) panelPausa.SetActive(true);
        if (panelConfiguracion != null) panelConfiguracion.SetActive(false);
        if (panelSalir != null) panelSalir.SetActive(false);
    }

    public void AbrirConfiguracion()
    {
        if (panelPausa != null) panelPausa.SetActive(false);
        if (panelConfiguracion != null) panelConfiguracion.SetActive(true);
        if (panelSalir != null) panelSalir.SetActive(false);
    }

    public void AbrirSalir()
    {
        if (panelPausa != null) panelPausa.SetActive(false);
        if (panelConfiguracion != null) panelConfiguracion.SetActive(false);
        if (panelSalir != null) panelSalir.SetActive(true);
    }

    public void CerrarConfiguracion()
    {
        MostrarVentanaContinuar();
    }

    public void Continuar()
    {
        if (jugador != null)
        {
            jugador.ReanudarJuego();
        }
        else
        {
            OcultarPaneles();
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Debug.LogWarning("No había jugador asignado, pero se reanudó el juego manualmente.");
        }
    }

    public void IrAlMenu()
    {
        if (jugador != null)
            jugador.ReanudarJuego();

        Time.timeScale = 1f;

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
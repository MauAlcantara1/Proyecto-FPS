using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPausa : MonoBehaviour
{
    [Header("Paneles")]
    [SerializeField] private GameObject panelPausa;
    [SerializeField] private GameObject panelConfiguracion;

    [Header("Jugador")]
    [SerializeField] private personajeController jugador;

    [Header("Escenas")]
    [SerializeField] private string escenaMenu = "Menu";

    public void Continuar()
    {
        if (jugador != null)
        {
            jugador.ReanudarJuego();
        }
        else
        {
            Time.timeScale = 1f;

            if (panelPausa != null)
                panelPausa.SetActive(false);
        }

        if (panelConfiguracion != null)
            panelConfiguracion.SetActive(false);
    }

    public void IrAlMenu()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(escenaMenu);
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
}
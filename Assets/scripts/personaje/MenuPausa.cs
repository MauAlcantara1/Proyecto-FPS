using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPausa : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private GameObject panelPausa;
    [SerializeField] private personajeController jugador;

    [Header("Escenas")]
    [SerializeField] private string escenaMenu = "Menu";
    [SerializeField] private string escenaConfiguracion = "Configuracion";

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
    }

    public void IrAlMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(escenaMenu);
    }

    public void IrAConfiguracion()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(escenaConfiguracion);
    }
}
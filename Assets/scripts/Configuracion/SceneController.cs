using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    // Menú
    public void IrMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    // Juego Solitario
    public void IrJuegoSolitario()
    {
        SceneManager.LoadScene("JuegoSolitario");
    }

    // Juego Cooperativo
    public void IrJuegoCoop()
    {
        SceneManager.LoadScene("JuegoCoop");
    }

    // Configuración
    public void IrConfiguracion()
    {
        SceneManager.LoadScene("Configuracion");
    }

    // Créditos
    public void IrCreditos()
    {
        SceneManager.LoadScene("Creditos");
    }

    // Salir del juego (opcional)
    public void Salir()
    {
        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
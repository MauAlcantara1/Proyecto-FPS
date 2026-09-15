using TMPro;
using UnityEngine;

public class Puntuacion : MonoBehaviour
{
    [Header("Puntuación")]
    [SerializeField] private int puntosPorDaño = 7;
    [SerializeField] private TMP_Text textoPuntuacion;
    [SerializeField] private int costo = 10;
    [SerializeField] private GameObject panelCompra;
    [SerializeField] private armaController armaController;

    private int puntuacionActual = 0;
    private bool puedeComprar = false;

    private void Start()
    {
        ActualizarUI();
    }

    public void SumarPuntosPorDaño()
    {
        puntuacionActual += puntosPorDaño;
        ActualizarUI();
    }

    private void ActualizarUI()
    {
        if (textoPuntuacion != null)
            textoPuntuacion.text = "  $ " + puntuacionActual.ToString();
    }

    public int ObtenerPuntuacion()
    {
        return puntuacionActual;
    }

    // --- Métodos llamados desde el script ZonaCompra ---
    public void EntrarZonaCompra()
    {
        puedeComprar = true;
        if (panelCompra != null)
            panelCompra.SetActive(true);
    }

    public void SalirZonaCompra()
    {
        puedeComprar = false;
        if (panelCompra != null)
            panelCompra.SetActive(false);
    }

    public void ComprarArma()
    {
        if (!puedeComprar)
            return;

        if (puntuacionActual < costo)
        {
            Debug.Log("No tienes suficientes puntos");
            return;
        }

        puntuacionActual -= costo;
        ActualizarUI();

        if (armaController != null)
            armaController.DesbloquearFusil();

        if (panelCompra != null)
            panelCompra.SetActive(false);

        puedeComprar = false;
    }
}
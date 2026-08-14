using TMPro;
using UnityEngine;

public class Puntuacion : MonoBehaviour
{
    [Header("Puntuación")]
    [SerializeField] private int puntosPorDaño = 7;
    [SerializeField] private TMP_Text textoPuntuacion;

    private int puntuacionActual = 0;

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
        {
            textoPuntuacion.text = "  $ " + puntuacionActual.ToString();
        }
    }

    public int ObtenerPuntuacion()
    {
        return puntuacionActual;
    }
}

using TMPro;
using UnityEngine;

public class Puntuacion : MonoBehaviour
{
    [Header("Puntuación")]
    [SerializeField] private int puntosPorDaño = 7;
    [SerializeField] private TMP_Text textoPuntuacion;

    [Header("Compra de armas")]
    [SerializeField] private int costo = 10;
    [SerializeField] private GameObject panelCompra;
    [SerializeField] private armaController armaController;

    private int puntuacionActual = 0;
    private int puntuacionTotalPartida = 0;
    private bool puedeComprar = false;
    private bool partidaRegistrada = false;

    private const string CLAVE_RECORD = "RecordPuntuacion";
    private const string CLAVE_NOMBRE_RECORD = "RecordNombre";
    private const string CLAVE_NOMBRE_JUGADOR = "NombreJugador";
    private const string CLAVE_NUEVO_RECORD = "NuevoRecord";

    private void Start()
    {
        PlayerPrefs.SetInt(CLAVE_NUEVO_RECORD, 0);
        PlayerPrefs.Save();

        ActualizarUI();
    }

    public void SumarPuntosPorDaño()
    {
        puntuacionActual += puntosPorDaño;
        puntuacionTotalPartida += puntosPorDaño;

        ActualizarUI();
    }

    public void AsignarArmaJugador(armaController armaLocal)
    {
        armaController = armaLocal;
    }

    private void ActualizarUI()
    {
        if (textoPuntuacion != null)
        {
            textoPuntuacion.text = "  $ " + puntuacionActual;
        }
    }

    // Puntuación disponible para gastar.
    public int ObtenerPuntuacion()
    {
        return puntuacionActual;
    }

    // Total acumulado durante la partida, sin restar las compras.
    public int ObtenerPuntuacionTotalPartida()
    {
        return puntuacionTotalPartida;
    }

    // Guarda el récord cuando termina la partida.
    public void RegistrarFinPartida()
    {
        if (partidaRegistrada)
            return;

        partidaRegistrada = true;

        int recordAnterior = PlayerPrefs.GetInt(
            CLAVE_RECORD,
            0
        );

        if (puntuacionTotalPartida > recordAnterior)
        {
            string nombreJugador = PlayerPrefs.GetString(
                CLAVE_NOMBRE_JUGADOR,
                "Jugador"
            );

            if (string.IsNullOrWhiteSpace(nombreJugador))
                nombreJugador = "Jugador";

            PlayerPrefs.SetInt(
                CLAVE_RECORD,
                puntuacionTotalPartida
            );

            PlayerPrefs.SetString(
                CLAVE_NOMBRE_RECORD,
                nombreJugador
            );

            PlayerPrefs.SetInt(
                CLAVE_NUEVO_RECORD,
                1
            );

            PlayerPrefs.Save();

            Debug.Log(
                "Nuevo récord: " + nombreJugador
                + " - " + puntuacionTotalPartida
            );
        }
        else
        {
            PlayerPrefs.SetInt(CLAVE_NUEVO_RECORD, 0);
            PlayerPrefs.Save();
        }
    }

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

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

    [Header("Récord local")]
    [SerializeField] private string nombreJugador = "Jugador";

    private int puntuacionActual = 0;
    private int puntuacionTotalPartida = 0;
    private bool puedeComprar = false;
    private bool partidaFinalizada = false;

    private const string CLAVE_RECORD = "RecordPuntuacion";
    private const string CLAVE_NOMBRE_RECORD = "RecordNombre";
    private const string CLAVE_ULTIMA_PUNTUACION = "UltimaPuntuacion";
    private const string CLAVE_NOMBRE_JUGADOR = "NombreJugador";
    private const string CLAVE_NUEVO_RECORD = "NuevoRecord";

    private void Start()
    {
        ActualizarUI();
    }

    public void SumarPuntosPorDaño()
    {
        if (partidaFinalizada)
            return;

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
            textoPuntuacion.text = "  $ " + puntuacionActual.ToString();
    }

    public int ObtenerPuntuacion()
    {
        return puntuacionActual;
    }

    public int ObtenerPuntuacionTotalPartida()
    {
        return puntuacionTotalPartida;
    }

    public void EstablecerNombreJugador(string nombre)
    {
        if (!string.IsNullOrWhiteSpace(nombre))
            nombreJugador = nombre.Trim();
    }

    // Guarda el resultado cuando el jugador muere.
    public void RegistrarFinPartida()
    {
        if (partidaFinalizada)
            return;

        partidaFinalizada = true;

        int recordAnterior = PlayerPrefs.GetInt(CLAVE_RECORD, 0);
        bool nuevoRecord = puntuacionTotalPartida > recordAnterior;

        PlayerPrefs.SetInt(
            CLAVE_ULTIMA_PUNTUACION,
            puntuacionTotalPartida
        );

        PlayerPrefs.SetString(
            CLAVE_NOMBRE_JUGADOR,
            nombreJugador
        );

        PlayerPrefs.SetInt(
            CLAVE_NUEVO_RECORD,
            nuevoRecord ? 1 : 0
        );

        if (nuevoRecord)
        {
            PlayerPrefs.SetInt(
                CLAVE_RECORD,
                puntuacionTotalPartida
            );

            PlayerPrefs.SetString(
                CLAVE_NOMBRE_RECORD,
                nombreJugador
            );
        }

        PlayerPrefs.Save();

        Debug.Log(
            "Partida finalizada. Puntuación total: "
            + puntuacionTotalPartida
            + ". Nuevo récord: "
            + nuevoRecord
        );
    }

    // --- Métodos llamados desde ZonaCompra ---

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
        if (!puedeComprar || partidaFinalizada)
            return;

        if (puntuacionActual < costo)
        {
            Debug.Log("No tienes suficientes puntos");
            return;
        }

        // Solo disminuyen las monedas disponibles.
        // La puntuación total de la partida no cambia.
        puntuacionActual -= costo;

        ActualizarUI();

        if (armaController != null)
            armaController.DesbloquearFusil();

        if (panelCompra != null)
            panelCompra.SetActive(false);

        puedeComprar = false;
    }
}
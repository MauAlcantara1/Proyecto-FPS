
using TMPro;
using UnityEngine;

public class PantallaInicio : MonoBehaviour
{
    [Header("Elementos de la interfaz")]
    [SerializeField] private TMP_InputField inputNombre;
    [SerializeField] private TMP_Text textoRecord;

    private const string CLAVE_RECORD = "RecordPuntuacion";
    private const string CLAVE_NOMBRE_RECORD = "RecordNombre";
    private const string CLAVE_NOMBRE_JUGADOR = "NombreJugador";

    private void Start()
    {
        // Recuperar el nombre guardado anteriormente.
        if (inputNombre != null)
        {
            inputNombre.text = PlayerPrefs.GetString(
                CLAVE_NOMBRE_JUGADOR,
                ""
            );

            inputNombre.onEndEdit.AddListener(
                GuardarNombreDesdeInput
            );
        }

        MostrarRecord();
    }

    private void OnDestroy()
    {
        if (inputNombre != null)
        {
            inputNombre.onEndEdit.RemoveListener(
                GuardarNombreDesdeInput
            );
        }
    }

    public void MostrarRecord()
    {
        if (textoRecord == null)
        {
            return;
        }

        int puntuacionMaxima = PlayerPrefs.GetInt(
            CLAVE_RECORD,
            0
        );

        string nombreRecord = PlayerPrefs.GetString(
            CLAVE_NOMBRE_RECORD,
            "Nadie"
        );

        if (puntuacionMaxima > 0)
        {
            textoRecord.text =
                "El récord es de " + nombreRecord
                + " con " + puntuacionMaxima + " puntos";
        }
        else
        {
            textoRecord.text = "Aún no hay ningún récord";
        }
    }

    private void GuardarNombreDesdeInput(string nombre)
    {
        GuardarNombre(nombre);
    }

    // Conectar este método al botón Jugar antes de cargar la partida.
    public void GuardarNombreJugador()
    {
        if (inputNombre == null)
        {
            Debug.LogWarning("No está asignado el campo del nombre.");
            return;
        }

        GuardarNombre(inputNombre.text);
    }

    private void GuardarNombre(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            Debug.LogWarning("Ingresa un nombre para jugar.");
            return;
        }

        nombre = nombre.Trim();

        PlayerPrefs.SetString(
            CLAVE_NOMBRE_JUGADOR,
            nombre
        );

        PlayerPrefs.Save();

        Debug.Log("Nombre guardado: " + nombre);
    }

    public string ObtenerNombreJugador()
    {
        return PlayerPrefs.GetString(
            CLAVE_NOMBRE_JUGADOR,
            "Jugador"
        );
    }
}
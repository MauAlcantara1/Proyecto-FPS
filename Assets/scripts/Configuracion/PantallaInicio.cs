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
        MostrarRecord();

        // Recuperar el nombre guardado anteriormente.
        if (inputNombre != null)
        {
            inputNombre.text = PlayerPrefs.GetString(
                CLAVE_NOMBRE_JUGADOR,
                ""
            );

            // Guardar automáticamente cuando termine de editar.
            inputNombre.onEndEdit.AddListener(
                GuardarNombreDesdeInput
            );
        }
    }

    private void OnDestroy()
    {
        // Evitar dejar registrado el evento.
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
            return;

        int puntuacionMaxima =
            PlayerPrefs.GetInt(CLAVE_RECORD, 0);

        string nombreRecord =
            PlayerPrefs.GetString(CLAVE_NOMBRE_RECORD, "Nadie");

        if (puntuacionMaxima > 0)
        {
            textoRecord.text =
                "El record es de " + nombreRecord
                + " por " + puntuacionMaxima;
        }
        else
        {
            textoRecord.text = "Aún no hay ningún récord";
        }
    }

    // Se ejecuta al terminar de editar el Input Field.
    private void GuardarNombreDesdeInput(string nombre)
    {
        GuardarNombre(nombre);
    }

    // Se conserva por si necesitas llamarlo desde otro script.
    public void GuardarNombreJugador()
    {
        if (inputNombre == null)
            return;

        GuardarNombre(inputNombre.text);
    }

    private void GuardarNombre(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            Debug.Log("Ingresa un nombre para jugar.");
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
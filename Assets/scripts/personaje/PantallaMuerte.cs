
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class PantallaMuerte : MonoBehaviourPunCallbacks
{
    [Header("Interfaz")]
    [SerializeField] private GameObject panelMuerte;
    [SerializeField] private TMP_Text textoGameOver;
    [SerializeField] private TMP_Text textoPuntuacionFinal;
    [SerializeField] private TMP_Text textoRecord;

    [Header("Escenas")]
    [SerializeField] private string escenaMenu = "Menu";

    private bool jugadorMuerto = false;
    private bool salirAlMenu = false;

    private const string CLAVE_RECORD = "RecordPuntuacion";
    private const string CLAVE_NOMBRE_RECORD = "RecordNombre";
    private const string CLAVE_NUEVO_RECORD = "NuevoRecord";

    private void Start()
    {
        if (panelMuerte != null)
            panelMuerte.SetActive(false);
    }

    public void MostrarPantallaMuerte(int puntuacionFinal)
    {
        if (jugadorMuerto)
            return;

        jugadorMuerto = true;

        if (panelMuerte != null)
            panelMuerte.SetActive(true);

        if (textoGameOver != null)
            textoGameOver.text = "GAME OVER";

        if (textoPuntuacionFinal != null)
        {
            textoPuntuacionFinal.text =
                "Puntuación final: " + puntuacionFinal;
        }

        if (textoRecord != null)
        {
            int record = PlayerPrefs.GetInt(CLAVE_RECORD, 0);

            string nombreRecord = PlayerPrefs.GetString(
                CLAVE_NOMBRE_RECORD,
                "Jugador"
            );

            bool nuevoRecord =
                PlayerPrefs.GetInt(CLAVE_NUEVO_RECORD, 0) == 1;

            if (nuevoRecord)
            {
                textoRecord.text =
                    "¡NUEVO RÉCORD!\n" +
                    "Récord: " + record + "\n" +
                    "Jugador: " + nombreRecord;
            }
            else
            {
                textoRecord.text =
                    "Récord: " + record + "\n" +
                    "Jugador: " + nombreRecord;
            }
        }

        // Mostrar y desbloquear el cursor.
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Pausar el tiempo del juego.
        Time.timeScale = 0f;
    }

    public void VolverAlMenu()
    {
        if (salirAlMenu)
            return;

        if (PhotonNetwork.InRoom)
        {
            salirAlMenu = true;

            // Restaurar el tiempo antes de salir de la sala.
            Time.timeScale = 1f;

            PhotonNetwork.LeaveRoom();
        }
        else
        {
            CargarMenu();
        }
    }

    public override void OnLeftRoom()
    {
        if (!salirAlMenu)
            return;

        salirAlMenu = false;
        CargarMenu();
    }

    private void CargarMenu()
    {
        // Asegurar que el menú no se cargue pausado.
        Time.timeScale = 1f;

        SceneManager.LoadScene(escenaMenu);
    }

    public bool EstaMuerto()
    {
        return jugadorMuerto;
    }
}

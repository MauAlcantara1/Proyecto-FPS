using UnityEngine;
using TMPro;

public class HUDArmasLocal : MonoBehaviour
{
    [Header("Indicadores de armas")]
    [SerializeField] private GameObject UIseleccionadaUno;
    [SerializeField] private GameObject UIseleccionadaDos;

    [Header("Indicador de botiquines")]
    [SerializeField] private GameObject UIseleccionadaBotiquin;

    [Header("Interfaz de munición")]
    [SerializeField] private TMP_Text textoCargador;
    [SerializeField] private TMP_Text textoMunicion;

    public void Conectar(armaController armas)
    {
        if (armas == null)
        {
            Debug.LogWarning(
                "No se encontró armaController para conectar el HUD."
            );
            return;
        }

        // Conectar indicadores de selección
        armas.AsignarInterfaz(
            UIseleccionadaUno,
            UIseleccionadaDos,
            UIseleccionadaBotiquin
        );

        // Conectar textos de munición
        armas.AsignarInterfazMunicion(
            textoCargador,
            textoMunicion
        );

        Debug.Log(
            "HUD de armas y munición conectado al jugador local."
        );
    }
}
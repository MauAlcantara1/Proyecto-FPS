using UnityEngine;
using UnityEngine.UI;
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

    [Header("Imagen del arma primaria")]
    [SerializeField] private Image imagenArmaPrimaria;
    [SerializeField] private Sprite spritePistola;
    [SerializeField] private Sprite spriteFusil;

    public void Conectar(armaController armas)
    {
        if (armas == null)
        {
            Debug.LogWarning("No se encontró armaController para conectar el HUD.");
            return;
        }

        armas.AsignarInterfaz(
            UIseleccionadaUno,
            UIseleccionadaDos,
            UIseleccionadaBotiquin
        );

        armas.AsignarInterfazMunicion(
            textoCargador,
            textoMunicion
        );

        armas.AsignarImagenHUD(
            imagenArmaPrimaria,
            spritePistola,
            spriteFusil
        );
    }
}

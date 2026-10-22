
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class HUDRetratosCoop : MonoBehaviourPunCallbacks
{
    [Header("Retratos del HUD")]
    [SerializeField] private Image retratoJugadorLocal;
    [SerializeField] private Image retratoCompanero;

    [Header("Sprites de Personajes")]
    [SerializeField] private Sprite spriteHombre;
    [SerializeField] private Sprite spriteSecretaria;

    private void Start()
    {
        if (PhotonNetwork.InRoom)
        {
            ConfigurarRetratos();
        }
    }

    public override void OnJoinedRoom()
    {
        ConfigurarRetratos();
    }

    private void ConfigurarRetratos()
    {
        if (!PhotonNetwork.InRoom)
            return;

        if (retratoJugadorLocal == null ||
            retratoCompanero == null)
        {
            Debug.LogError(
                "[HUD] Faltan imágenes de los personajes."
            );
            return;
        }

        if (spriteHombre == null ||
            spriteSecretaria == null)
        {
            Debug.LogError(
                "[HUD] Faltan sprites de los personajes."
            );
            return;
        }

        bool esHost = PhotonNetwork.IsMasterClient;

        if (esHost)
        {
            // Host: hombre a la derecha.
            retratoJugadorLocal.sprite = spriteHombre;

            // Compañera: secretaria a la izquierda.
            retratoCompanero.sprite = spriteSecretaria;
        }
        else
        {
            // Invitado: secretaria a la derecha.
            retratoJugadorLocal.sprite = spriteSecretaria;

            // Compañero: hombre a la izquierda.
            retratoCompanero.sprite = spriteHombre;
        }

        Debug.Log(
            "[HUD] Retratos configurados." +
            "\nRol: " + (esHost ? "HOST" : "INVITADO") +
            "\nLocal: " + retratoJugadorLocal.sprite.name +
            "\nCompañero: " + retratoCompanero.sprite.name
        );
    }
}

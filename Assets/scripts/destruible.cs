
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
public class Destruible : MonoBehaviourPun
{
    [Header("Objeto destruido")]
    [SerializeField] private GameObject versionPrefab;

    [Header("Vida")]
    [SerializeField] private int vidaMaxima = 100;
    [SerializeField] private int dañoPorImpacto = 25;

    [Header("Explosión")]
    [SerializeField] private float fuerzaExplosion = 500f;
    [SerializeField] private float radioExplosion = 2f;

    private int vidaActual;

    // Evita procesar la destrucción más de una vez.
    private bool destruido = false;
    private bool destruccionVisualAplicada = false;

    private void Awake()
    {
        vidaActual = vidaMaxima;
    }

    // ==========================================
    // RECIBIR DAÑO
    // ==========================================

    public void TomarDaño(int daño)
    {
        if (daño <= 0 || destruido)
            return;

        // MODO INDIVIDUAL
        if (!PhotonNetwork.InRoom)
        {
            AplicarDaño(daño);
            return;
        }

        // MULTIJUGADOR
        if (photonView.ViewID == 0)
        {
            Debug.LogError(
                "[DESTRUIBLE] PhotonView sin ViewID: " +
                gameObject.name,
                this
            );
            return;
        }

        // El anfitrión administra directamente la vida.
        if (PhotonNetwork.IsMasterClient)
        {
            AplicarDaño(daño);
        }
        else
        {
            // El invitado solicita que el anfitrión
            // aplique el daño sobre el objeto.
            photonView.RPC(
                nameof(RPC_SolicitarDaño),
                RpcTarget.MasterClient,
                daño
            );

            Debug.Log(
                "[DESTRUIBLE] Daño enviado al anfitrión. " +
                "Objeto: " + gameObject.name +
                " | Daño: " + daño
            );
        }
    }

    // ==========================================
    // PHOTON - RECIBIR SOLICITUD DE DAÑO
    // ==========================================

    [PunRPC]
    private void RPC_SolicitarDaño(
        int daño,
        PhotonMessageInfo info)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        if (destruido || daño <= 0)
            return;

        Debug.Log(
            "[DESTRUIBLE] Solicitud recibida." +
            "\nObjeto: " + gameObject.name +
            "\nJugador: " + info.Sender.NickName +
            "\nDaño: " + daño
        );

        AplicarDaño(daño);
    }

    // ==========================================
    // CONTROL AUTORITATIVO DE VIDA
    // ==========================================

    private void AplicarDaño(int daño)
    {
        if (destruido)
            return;

        vidaActual = Mathf.Max(
            0,
            vidaActual - daño
        );

        Debug.Log(
            "[DESTRUIBLE] " + gameObject.name +
            " | Vida restante: " + vidaActual
        );

        if (vidaActual > 0)
            return;

        destruido = true;

        if (PhotonNetwork.InRoom)
        {
            Debug.Log(
                "[DESTRUIBLE] Objeto destruido. " +
                "Sincronizando con ambos jugadores."
            );

            photonView.RPC(
                nameof(RPC_DestruirObjeto),
                RpcTarget.AllViaServer
            );
        }
        else
        {
            // Mantener el funcionamiento individual.
            DestruirVisualmente();
        }
    }

    // ==========================================
    // PHOTON - DESTRUCCIÓN SINCRONIZADA
    // ==========================================

    [PunRPC]
    private void RPC_DestruirObjeto()
    {
        if (destruccionVisualAplicada)
            return;

        Debug.Log(
            "[DESTRUIBLE] Señal de destrucción recibida. " +
            "Objeto: " + gameObject.name
        );

        DestruirVisualmente();
    }

    // ==========================================
    // EFECTO VISUAL DE DESTRUCCIÓN
    // ==========================================

    private void DestruirVisualmente()
    {
        if (destruccionVisualAplicada)
            return;

        destruccionVisualAplicada = true;
        destruido = true;

        if (versionPrefab != null)
        {
            GameObject brokenInstance = Instantiate(
                versionPrefab,
                transform.position,
                transform.rotation
            );

            Rigidbody[] fragments =
                brokenInstance.GetComponentsInChildren<Rigidbody>();

            foreach (Rigidbody rb in fragments)
            {
                rb.AddExplosionForce(
                    fuerzaExplosion,
                    transform.position,
                    radioExplosion
                );
            }

            Destroy(brokenInstance, 10f);
        }
        else
        {
            Debug.LogWarning(
                "[DESTRUIBLE] Falta asignar versionPrefab.",
                this
            );
        }

        if (!PhotonNetwork.InRoom)
        {
            // Modo individual: destrucción original.
            Destroy(gameObject);
            return;
        }

        // Multiplayer: ocultar el objeto intacto
        // sin eliminar su PhotonView.
        Renderer[] renderers =
            GetComponentsInChildren<Renderer>(true);

        foreach (Renderer renderizador in renderers)
        {
            renderizador.enabled = false;
        }

        Collider[] colliders =
            GetComponentsInChildren<Collider>(true);

        foreach (Collider col in colliders)
        {
            col.enabled = false;
        }

        Debug.Log(
            "[DESTRUIBLE] Objeto ocultado correctamente."
        );
    }

    // ==========================================
    // EVITAR GOLPES DE JUGADORES REMOTOS
    // ==========================================

    private bool AtaqueEsLocal(Collider colliderAtaque)
    {
        if (!PhotonNetwork.InRoom)
            return true;

        PhotonView vistaAtacante =
            colliderAtaque.GetComponentInParent<PhotonView>();

        // Los proyectiles creados localmente con
        // Instantiate no necesitan PhotonView propio.
        if (vistaAtacante == null)
            return true;

        return vistaAtacante.IsMine;
    }

    // ==========================================
    // IMPACTO DE BALAS
    // ==========================================

    private void OnCollisionEnter(Collision collision)
    {
        if (destruido)
            return;

        if (!collision.gameObject.CompareTag("Bala"))
            return;

        if (!AtaqueEsLocal(collision.collider))
            return;

        TomarDaño(dañoPorImpacto);

        // Las balas actuales se crean con Instantiate.
        Destroy(collision.gameObject);
    }

    // ==========================================
    // IMPACTO DEL MACHETE (TRIGGER)
    // ==========================================

    private void OnTriggerEnter(Collider other)
    {
        if (destruido)
            return;

        if (!other.CompareTag("Bala") || !other.isTrigger)
            return;

        if (!AtaqueEsLocal(other))
            return;

        Debug.Log(
            "[DESTRUIBLE] Impacto local de machete."
        );

        TomarDaño(dañoPorImpacto);
    }
}

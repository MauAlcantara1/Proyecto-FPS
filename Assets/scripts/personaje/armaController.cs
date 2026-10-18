
using UnityEngine;
using UnityEngine.InputSystem;

public class armaController : MonoBehaviour
{
    private Animator animatorActual;
    private CharacterController controller;
    private MunicionArma municionActual;

    public enum TipoArma
    {
        pistola = 0,
        fusil = 1,
        uzi = 2
    }

    [Header("Arma primaria")]
    [SerializeField] private TipoArma armaActiva = TipoArma.pistola;

    [Header("Armas")]
    [SerializeField] private GameObject pistola;
    [SerializeField] private GameObject fusil;
    [SerializeField] private GameObject uzi;

    [Header("Segunda arma")]
    [SerializeField] private GameObject segundaArma;
    [SerializeField] private GameObject UIseleccionadaUno;
    [SerializeField] private GameObject UIseleccionadaDos;

    [SerializeField] private bool primariaActiva = true;
    private bool disparando = false;

    [Header("Desbloqueo")]
    [SerializeField] private bool fusilDesbloqueado = false;

    public bool FusilDesbloqueado => fusilDesbloqueado;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Start()
    {
        ActualizarArmaVisible();
        ActualizarAnimator();
    }

    private void Update()
    {
        if (!disparando)
            return;

        if (municionActual == null || !municionActual.PuedeDisparar())
        {
            if (animatorActual != null)
                animatorActual.SetBool("Disparo", false);

            disparando = false;
            return;
        }

        if (animatorActual != null)
            animatorActual.SetBool("Disparo", true);
    }

    public void ActualizarArmaVisible()
    {
        if (pistola != null)
            pistola.SetActive(
                primariaActiva && armaActiva == TipoArma.pistola);

        if (fusil != null)
            fusil.SetActive(
                primariaActiva && armaActiva == TipoArma.fusil);

        if (uzi != null)
            uzi.SetActive(
                primariaActiva && armaActiva == TipoArma.uzi);

        if (segundaArma != null)
            segundaArma.SetActive(!primariaActiva);

        if (UIseleccionadaUno != null)
            UIseleccionadaUno.SetActive(primariaActiva);

        if (UIseleccionadaDos != null)
            UIseleccionadaDos.SetActive(!primariaActiva);
    }

    private void ActualizarAnimator()
    {
        if (primariaActiva)
        {
            animatorActual = ObtenerAnimatorArmaPrimaria();
        }
        else
        {
            animatorActual = segundaArma != null
                ? segundaArma.GetComponent<Animator>()
                : null;
        }

        ActualizarMunicion();
    }

    private void ActualizarMunicion()
    {
        if (primariaActiva)
        {
            GameObject armaActual = ObtenerArmaPrimaria();

            municionActual = armaActual != null
                ? armaActual.GetComponent<MunicionArma>()
                : null;
        }
        else
        {
            municionActual = segundaArma != null
                ? segundaArma.GetComponent<MunicionArma>()
                : null;
        }
    }

    private Animator ObtenerAnimatorArmaPrimaria()
    {
        GameObject armaActual = ObtenerArmaPrimaria();

        return armaActual != null
            ? armaActual.GetComponent<Animator>()
            : null;
    }

    private GameObject ObtenerArmaPrimaria()
    {
        switch (armaActiva)
        {
            case TipoArma.pistola:
                return pistola;

            case TipoArma.fusil:
                return fusil;

            case TipoArma.uzi:
                return uzi;

            default:
                return null;
        }
    }

    public void Cambio(InputAction.CallbackContext context)
    {
        if (context.performed && animatorActual != null)
        {
            animatorActual.SetTrigger("Cambio");
            Debug.Log("Cambio de arma: armaController");
        }
    }

    public void Recarga(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        if (municionActual == null)
        {
            Debug.LogWarning("No hay un componente MunicionArma en el arma activa.");
            return;
        }

        // Permite recargar aunque queden balas en el cargador.
        if (!municionActual.PuedeRecargar())
        {
            Debug.Log("No se puede recargar: cargador lleno, sin reserva o recarga en curso.");
            return;
        }

        municionActual.IniciarRecarga();

        if (animatorActual != null)
            animatorActual.SetTrigger("Recarga");
    }

    public void Disparo(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!primariaActiva)
            {
                if (animatorActual != null)
                    animatorActual.SetTrigger("Disparo");

                return;
            }

            disparando = true;
        }
        else if (context.canceled)
        {
            disparando = false;

            if (animatorActual != null)
                animatorActual.SetBool("Disparo", false);
        }
    }

    public void CambiarArma()
    {
        primariaActiva = !primariaActiva;

        Debug.Log("Cambio de arma2");

        // Detiene el estado de disparo al cambiar de arma.
        disparando = false;

        ActualizarArmaVisible();
        ActualizarAnimator();

        if (animatorActual != null)
        {
            animatorActual.Rebind();
            animatorActual.Update(0f);
        }
    }

    public Animator ObtenerAnimatorActual()
    {
        return animatorActual;
    }

    public void DesbloquearFusil()
    {
        if (fusilDesbloqueado)
        {
            Debug.Log("El fusil ya está desbloqueado.");
            return;
        }

        if (fusil == null)
        {
            Debug.LogError("No se ha asignado el objeto Fusil.");
            return;
        }

        fusilDesbloqueado = true;
        armaActiva = TipoArma.fusil;
        primariaActiva = true;

        ActualizarArmaVisible();
        ActualizarAnimator();

        Debug.Log("Fusil desbloqueado y equipado.");
    }
}

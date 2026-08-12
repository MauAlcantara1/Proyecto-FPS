using UnityEngine;
using UnityEngine.InputSystem;

public class armaController : MonoBehaviour
{
    private Animator animatorActual;
    private CharacterController controller;

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
    private int balas = 0;

    [Header("Segunda arma")]
    [SerializeField] private GameObject segundaArma;
    [SerializeField] private GameObject UIseleccionadaUno;
    [SerializeField] private GameObject UIseleccionadaDos;


    [SerializeField] private bool primariaActiva = true;

    private void Start()
    {
        ActualizarArmaVisible();
        ActualizarAnimator();
    }

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    public void ActualizarArmaVisible()
    {
        if (pistola != null)
            pistola.SetActive(primariaActiva && armaActiva == TipoArma.pistola);

        if (fusil != null)
            fusil.SetActive(primariaActiva && armaActiva == TipoArma.fusil);

        if (uzi != null)
            uzi.SetActive(primariaActiva && armaActiva == TipoArma.uzi);

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
            animatorActual = segundaArma.GetComponent<Animator>();
        }
    }

    private Animator ObtenerAnimatorArmaPrimaria()
    {
        switch (armaActiva)
        {
            case TipoArma.pistola:
                return pistola.GetComponent<Animator>();

            case TipoArma.fusil:
                return fusil.GetComponent<Animator>();

            case TipoArma.uzi:
                return uzi.GetComponent<Animator>();

            default:
                return null;
        }
    }

    public void Cambio(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            animatorActual.SetTrigger("Cambio");
        }
    }

    public void Recarga(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            animatorActual.SetTrigger("Recarga");
        }
    }

    public void Disparo(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            animatorActual.SetTrigger("Disparo");
        }
    }

    public void CambiarArma()
    {
        primariaActiva = !primariaActiva;

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
}
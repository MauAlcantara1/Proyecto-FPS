using UnityEngine;
using UnityEngine.InputSystem;

public class personajeController : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float velocidadCaminar = 4f;
    [SerializeField] private float velocidadCorrer = 7f;
    [SerializeField] private float fuerzaSalto = 3f;
    [SerializeField] private float gravedad = -9.81f;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] public AudioClip pasosCaminar;
    [SerializeField] public AudioClip pasosCorrer;
    [SerializeField] public Animator animator;



    private CharacterController controller;

    private Vector2 movimientoInput;
    private bool saltar;
    private bool correr;

    private Vector3 velocidadVertical;


    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }


    private void Update()
    {
        MoverJugador();
        AplicarGravedad();
        SonidosPasos();

    }


    public void Movimiento(InputAction.CallbackContext context)
    {
        movimientoInput = context.ReadValue<Vector2>();

    }




    
    public void Salto(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            saltar = true;
        }
    }

    public void Correr(InputAction.CallbackContext context)
    {
        correr = context.ReadValueAsButton();
    }



    private void MoverJugador()
    {
        Vector3 direccion =
            transform.right * movimientoInput.x +
            transform.forward * movimientoInput.y;


        float velocidad = correr
            ? velocidadCorrer
            : velocidadCaminar;


        controller.Move(
            direccion * velocidad * Time.deltaTime
        );


        if(controller.isGrounded && saltar)
        {
            velocidadVertical.y =
                Mathf.Sqrt(fuerzaSalto * -2f * gravedad);

            saltar = false;
        }
    }



    private void AplicarGravedad()
    {
        if(controller.isGrounded && velocidadVertical.y < 0)
        {
            velocidadVertical.y = -2f;
        }


        velocidadVertical.y += gravedad * Time.deltaTime;


        controller.Move(
            velocidadVertical * Time.deltaTime
        );
    }

    private void SonidosPasos()
    {
        if (!controller.isGrounded || movimientoInput.magnitude < 0.1f)
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }

            return;
        }

        AudioClip clipActual = correr
            ? pasosCorrer
            : pasosCaminar;

        if (audioSource.clip != clipActual)
        {
            audioSource.clip = clipActual;
            audioSource.Play();
        }
        else if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }
}
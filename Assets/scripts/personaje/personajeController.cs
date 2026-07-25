using UnityEngine;
using UnityEngine.InputSystem;

public class personajeController : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float velocidadCaminar = 4f;
    [SerializeField] private float velocidadCorrer = 7f;
    [SerializeField] private float fuerzaSalto = 3f;
    [SerializeField] private float gravedad = -9.81f;


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
    }


    // Recibe Move desde Input System
    public void Movimiento(InputAction.CallbackContext context)
    {
        movimientoInput = context.ReadValue<Vector2>();

        Debug.Log(movimientoInput);
    }


    // Recibe Jump
    public void Salto(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            saltar = true;
        }
    }


    // Recibe Sprint
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
}
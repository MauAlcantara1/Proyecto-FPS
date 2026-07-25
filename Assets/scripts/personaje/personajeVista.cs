using UnityEngine;
using UnityEngine.InputSystem;

public class personajeVista : MonoBehaviour
{
    [SerializeField] private Transform cuerpoJugador;
    [SerializeField] private float sensibilidad = 100f;

    private Vector2 lookInput;

    private float rotacionX = 0f;


    public void Mirar(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }


    void Update()
    {
        float mouseX = lookInput.x * sensibilidad * Time.deltaTime;
        float mouseY = lookInput.y * sensibilidad * Time.deltaTime;


        rotacionX -= mouseY;
        rotacionX = Mathf.Clamp(rotacionX, -90f, 90f);


        transform.localRotation =
            Quaternion.Euler(rotacionX, 0f, 0f);


        cuerpoJugador.Rotate(
            Vector3.up * mouseX
        );
    }
}
using UnityEngine;
using UnityEngine.InputSystem;

public class personajeVista : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform cuerpoJugador;
    [SerializeField] private personajeController jugador;

    [Header("Configuración de cámara")]
    [SerializeField] private float sensibilidad = 100f;

    private Vector2 lookInput;
    private float rotacionX = 0f;

    private void Awake()
    {
        // Buscar al personaje al que pertenece la cámara.
        if (jugador == null)
        {
            jugador =
                GetComponentInParent<personajeController>();
        }
    }

    private void OnEnable()
    {
        sensibilidad = PlayerPrefs.GetFloat(
            "SensibilidadMouseGuardada",
            sensibilidad
        );

        ConfiguracionSensibilidad.OnSensibilidadCambiada +=
            ActualizarSensibilidad;
    }

    private void OnDisable()
    {
        ConfiguracionSensibilidad.OnSensibilidadCambiada -=
            ActualizarSensibilidad;

        lookInput = Vector2.zero;
    }

    private void ActualizarSensibilidad(float nuevaSensibilidad)
    {
        sensibilidad = nuevaSensibilidad;
    }

    public void Mirar(InputAction.CallbackContext context)
    {
        if (jugador != null && jugador.EstaEnPausa())
        {
            lookInput = Vector2.zero;
            return;
        }

        lookInput = context.ReadValue<Vector2>();
    }

    private void Update()
    {
        if (jugador != null && jugador.EstaEnPausa())
        {
            lookInput = Vector2.zero;
            return;
        }

        float mouseX =
            lookInput.x * sensibilidad * Time.deltaTime;

        float mouseY =
            lookInput.y * sensibilidad * Time.deltaTime;

        rotacionX -= mouseY;
        rotacionX = Mathf.Clamp(rotacionX, -90f, 90f);

        transform.localRotation =
            Quaternion.Euler(rotacionX, 0f, 0f);

        if (cuerpoJugador != null)
        {
            cuerpoJugador.Rotate(
                Vector3.up * mouseX
            );
        }
    }
}
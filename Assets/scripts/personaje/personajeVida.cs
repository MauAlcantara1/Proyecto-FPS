using UnityEngine;
using TMPro;
using UnityEngine.UI; 
public class personajeVida : MonoBehaviour
{
    [Header("UI TextMeshPro")]
    [SerializeField] private TMP_Text textoVida;

    [Header("Ajustes de Vida")]
    [SerializeField] private int vidaActual = 100;

    [Header("Ajustes de Inmunidad")]
    [SerializeField] private float segundosInmunidad = 2f;
    private float siguienteTiempoDaño = 0f;
    private bool esInmune = false; 

    [Header("Imagenes Vida")]
    [SerializeField] private Image imagenVida; 
    [SerializeField] private Sprite spriteVidaLlena;
    [SerializeField] private Sprite spriteVidaMedia;
    [SerializeField] private Sprite spriteVidaBaja;


    void Start()
    {
        ActualizarInterfaz();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemigo"))
        {
            RecibirDaño(5);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Enemigo"))
        {
            RecibirDaño(5);
        }
    }

    public void RecibirDaño(int cantidad)
    {
        if (Time.time < siguienteTiempoDaño) return;

        siguienteTiempoDaño = Time.time + segundosInmunidad;

        vidaActual -= cantidad;
        vidaActual = Mathf.Clamp(vidaActual, 0, 100);

        ActualizarInterfaz();
    }

    private void ActualizarInterfaz()
    {
        textoVida.SetText(" + {0}", vidaActual);
        ActualizarColorVida();
    }

    private void ActualizarColorVida()
    {
        if (vidaActual >= 80)
        {
            textoVida.color = new Color(0.54f, 1f, 0.53f);
            imagenVida.sprite = spriteVidaLlena;
        }
        else if (vidaActual >= 40)
        {
            textoVida.color = new Color(1f, 0.5f, 0f); 
            imagenVida.sprite = spriteVidaMedia;
        }
        else
        {
            textoVida.color = Color.red;
            imagenVida.sprite = spriteVidaBaja;
        }
    }
}
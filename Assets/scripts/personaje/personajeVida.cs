using System.Collections; // <--- Corroborado para solucionar CS0246 (IEnumerator)
using UnityEngine;
using TMPro;
using UnityEngine.UI; 

public class personajeVida : MonoBehaviour
{
    [Header("UI TextMeshPro")]
    [SerializeField] private TMP_Text textoVida;

    [Header("Ajustes de Vida")]
    [SerializeField] private int vidaActual = 100;
    [SerializeField] private int vidaMaxima = 100;

    [Header("Ajustes de Inmunidad")]
    [SerializeField] private float segundosInmunidad = 2f;
    private float siguienteTiempoDaño = 0f;

    [Header("Imagenes Vida")]
    [SerializeField] private Image imagenVida; 
    [SerializeField] private Sprite spriteVidaLlena;
    [SerializeField] private Sprite spriteVidaMedia;
    [SerializeField] private Sprite spriteVidaBaja;

    [Header("Sistema de Botiquín")]
    [SerializeField] private int cantidadBotiquines = 1;
    [SerializeField] private float tiempoCuracion = 3f; 
    [SerializeField] private Slider barraProgresoCuracion; 
    [SerializeField] private GameObject panelCuracionUI; 
    
    private bool estaCurandose = false;
    private Coroutine corrutinaCuracion;

    void Start()
    {
        if (panelCuracionUI != null) panelCuracionUI.SetActive(false);
        if (barraProgresoCuracion != null) barraProgresoCuracion.value = 0f;
        
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

        if (estaCurandose)
        {
            CancelarCuracion();
        }

        vidaActual -= cantidad;
        vidaActual = Mathf.Clamp(vidaActual, 0, vidaMaxima);

        ActualizarInterfaz();
    }

    // --- LÓGICA DEL BOTIQUÍN ---

    public void IniciarCuracion()
    {
        if (cantidadBotiquines > 0 && !estaCurandose && vidaActual < vidaMaxima)
        {
            corrutinaCuracion = StartCoroutine(ProcesoCuracion());
        }
    }

    public void CancelarCuracion()
    {
        if (estaCurandose && corrutinaCuracion != null)
        {
            StopCoroutine(corrutinaCuracion);
            estaCurandose = false;

            if (panelCuracionUI != null) panelCuracionUI.SetActive(false);
            if (barraProgresoCuracion != null) barraProgresoCuracion.value = 0f;
        }
    }

    private IEnumerator ProcesoCuracion()
    {
        estaCurandose = true;
        if (panelCuracionUI != null) panelCuracionUI.SetActive(true);

        float tiempoTranscurrido = 0f;

        while (tiempoTranscurrido < tiempoCuracion)
        {
            tiempoTranscurrido += Time.deltaTime;

            if (barraProgresoCuracion != null)
            {
                barraProgresoCuracion.value = tiempoTranscurrido / tiempoCuracion;
            }

            yield return null;
        }

        // Cura el 80% de la salud faltante estilo Left 4 Dead
        int vidaFaltante = vidaMaxima - vidaActual;
        int cantidadACurar = Mathf.RoundToInt(vidaFaltante * 0.80f); 

        if (cantidadACurar < 1) cantidadACurar = 1;

        vidaActual = Mathf.Clamp(vidaActual + cantidadACurar, 0, vidaMaxima);
        cantidadBotiquines--;

        estaCurandose = false;
        if (barraProgresoCuracion != null) barraProgresoCuracion.value = 0f;
        if (panelCuracionUI != null) panelCuracionUI.SetActive(false);

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

    public bool EstaCurandose() => estaCurandose;
}
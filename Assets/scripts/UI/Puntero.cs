using UnityEngine;
using UnityEngine.EventSystems;

public class PunteroMenu : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("El triángulo de este botón")]
    [Tooltip("Arrastra aquí el triángulo rojo que le corresponde a este botón")]
    [SerializeField] private GameObject miPuntero;

    [Header("Sonidos de Interfaz")]
    [Tooltip("Arrastra aquí el objeto que tiene el AudioSource (ej. tu AudioManager)")]
    [SerializeField] private AudioSource fuenteDeAudio;
    [SerializeField] private AudioClip sonidoHover;
    [SerializeField] private AudioClip sonidoClic;

    private void Start()
    {
        if (miPuntero != null)
        {
            miPuntero.SetActive(false);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (miPuntero != null) miPuntero.SetActive(true);

        if (fuenteDeAudio != null && sonidoHover != null)
        {
            fuenteDeAudio.PlayOneShot(sonidoHover);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (miPuntero != null) miPuntero.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (fuenteDeAudio != null && sonidoClic != null)
        {
            fuenteDeAudio.PlayOneShot(sonidoClic);
        }
    }
}
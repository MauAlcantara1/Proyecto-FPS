using UnityEngine;
using UnityEngine.EventSystems;

public class PunteroMenu : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [Header("Puntero")]
    [SerializeField] private GameObject miPuntero;

    [Header("Sonidos de Interfaz")]
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

    public void OnPointerDown(PointerEventData eventData)
        {
            if (fuenteDeAudio != null && sonidoClic != null)
            {
                fuenteDeAudio.PlayOneShot(sonidoClic);
            }
        }
}
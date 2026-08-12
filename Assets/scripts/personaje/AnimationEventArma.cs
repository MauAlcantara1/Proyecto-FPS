using UnityEngine;

public class AnimationEventArma : MonoBehaviour
{
    private armaController armaController;
    [SerializeField] public AudioClip disparoM9;
    [SerializeField] public AudioClip recarga;
    [SerializeField] public AudioClip equipar;
    [SerializeField] private AudioSource audioSource;

    private void Awake()
    {
        armaController = GetComponentInParent<armaController>();
    }

    public void CambiarArma()
    {
        armaController.CambiarArma();
    }

    public void SonidoDisparo()
    {
        ReproducirSonido(disparoM9);
    }

    public void Recarga()
    {
        ReproducirSonido(recarga);
    }

    public void Equipar()
    {
        ReproducirSonido(equipar);
    }

    private void ReproducirSonido(AudioClip clip)
    {
        if(audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
using UnityEngine;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    [Header("Sliders")]
    public Slider volumenGeneral;
    public Slider volumenMusica;
    public Slider volumenSFX;

    void Start()
    {
        // Cargar configuración guardada
        volumenGeneral.value = PlayerPrefs.GetFloat("VolumenGeneral", 1f);
        volumenMusica.value = PlayerPrefs.GetFloat("VolumenMusica", 1f);
        volumenSFX.value = PlayerPrefs.GetFloat("VolumenSFX", 1f);

        // Aplicar volumen
        ActualizarVolumenGeneral(volumenGeneral.value);
        ActualizarVolumenMusica(volumenMusica.value);
        ActualizarVolumenSFX(volumenSFX.value);

        // Detectar cambios
        volumenGeneral.onValueChanged.AddListener(ActualizarVolumenGeneral);
        volumenMusica.onValueChanged.AddListener(ActualizarVolumenMusica);
        volumenSFX.onValueChanged.AddListener(ActualizarVolumenSFX);
    }

    public void ActualizarVolumenGeneral(float volumen)
    {
        AudioListener.volume = volumen;
        PlayerPrefs.SetFloat("VolumenGeneral", volumen);
    }

    public void ActualizarVolumenMusica(float volumen)
    {
        PlayerPrefs.SetFloat("VolumenMusica", volumen);

        // Aquí irá el AudioSource de la música
        // musicaSource.volume = volumen * volumenGeneral.value;
    }

    public void ActualizarVolumenSFX(float volumen)
    {
        PlayerPrefs.SetFloat("VolumenSFX", volumen);

        // Aquí irán los AudioSource de efectos
        // efectoSource.volume = volumen * volumenGeneral.value;
    }
}
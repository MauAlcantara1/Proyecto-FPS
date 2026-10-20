
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioSettings : MonoBehaviour
{
    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("Sliders")]
    public Slider volumenGeneral;
    public Slider volumenMusica;
    public Slider volumenSFX;

    [Header("Parámetros del Mixer")]
    [SerializeField] private string parametroGeneral = "MasterVolume";
    [SerializeField] private string parametroMusica = "MusicVolume";
    [SerializeField] private string parametroSFX = "SFXVolume";

    private void Start()
    {
        if (audioMixer == null)
        {
            return;
        }

        if (volumenGeneral == null ||
            volumenMusica == null ||
            volumenSFX == null)
        {
            return;
        }


        // El volumen general lo controlará el Mixer.
        AudioListener.volume = 1f;

        // Recuperar valores guardados.
        float general = PlayerPrefs.GetFloat("VolumenGeneral", 1f);
        float musica = PlayerPrefs.GetFloat("VolumenMusica", 1f);
        float sfx = PlayerPrefs.GetFloat("VolumenSFX", 1f);

        // Actualizar sliders sin ejecutar eventos.
        volumenGeneral.SetValueWithoutNotify(Mathf.Clamp01(general));
        volumenMusica.SetValueWithoutNotify(Mathf.Clamp01(musica));
        volumenSFX.SetValueWithoutNotify(Mathf.Clamp01(sfx));

        // Aplicar los valores al Audio Mixer.
        ActualizarVolumenGeneral(volumenGeneral.value);
        ActualizarVolumenMusica(volumenMusica.value);
        ActualizarVolumenSFX(volumenSFX.value);

        // Detectar cambios.
        volumenGeneral.onValueChanged.AddListener(
            ActualizarVolumenGeneral
        );

        volumenMusica.onValueChanged.AddListener(
            ActualizarVolumenMusica
        );

        volumenSFX.onValueChanged.AddListener(
            ActualizarVolumenSFX
        );

    }

    public void ActualizarVolumenGeneral(float volumen)
    {
        CambiarVolumenMixer(parametroGeneral, volumen);

        PlayerPrefs.SetFloat("VolumenGeneral", volumen);
    }

    public void ActualizarVolumenMusica(float volumen)
    {
        CambiarVolumenMixer(parametroMusica, volumen);

        PlayerPrefs.SetFloat("VolumenMusica", volumen);
    }

    public void ActualizarVolumenSFX(float volumen)
    {
        CambiarVolumenMixer(parametroSFX, volumen);

        PlayerPrefs.SetFloat("VolumenSFX", volumen);
    }

    private void CambiarVolumenMixer(string parametro, float volumen)
    {
        if (audioMixer == null)
            return;

        // Convertir el volumen de 0-1 a decibelios.
        float volumenDB = volumen <= 0.0001f
            ? -80f
            : Mathf.Log10(Mathf.Clamp01(volumen)) * 20f;

        bool aplicado = audioMixer.SetFloat(
            parametro,
            volumenDB
        );

    }

    private void OnDestroy()
    {
        if (volumenGeneral != null)
        {
            volumenGeneral.onValueChanged.RemoveListener(
                ActualizarVolumenGeneral
            );
        }

        if (volumenMusica != null)
        {
            volumenMusica.onValueChanged.RemoveListener(
                ActualizarVolumenMusica
            );
        }

        if (volumenSFX != null)
        {
            volumenSFX.onValueChanged.RemoveListener(
                ActualizarVolumenSFX
            );
        }

        PlayerPrefs.Save();
    }
}
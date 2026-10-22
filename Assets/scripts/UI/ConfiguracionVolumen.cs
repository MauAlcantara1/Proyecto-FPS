
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class ConfiguracionVolumen : MonoBehaviour
{
    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("Sliders")]
    [SerializeField] private Slider sliderMaster;
    [SerializeField] private Slider sliderMusica;
    [SerializeField] private Slider sliderSFX;

    [Header("Parámetros expuestos del Mixer")]
    [SerializeField] private string parametroMaster = "MasterVolume";
    [SerializeField] private string parametroMusica = "MusicVolume";
    [SerializeField] private string parametroSFX = "SFXVolume";

    // Guardado de preferencias
    private const string PREF_MASTER = "VolumenMasterGuardado";
    private const string PREF_MUSICA = "VolumenMusicaGuardado";
    private const string PREF_SFX = "VolumenSFXGuardado";

    private void Start()
    {
        if (audioMixer == null)
        {
            Debug.LogError(
                "[AUDIO] No está asignado MainMixer."
            );
            return;
        }

        AudioListener.volume = 1f;


        float master = PlayerPrefs.GetFloat(
            PREF_MASTER,
            PlayerPrefs.GetFloat("VolumenSistemaGuardado", 0.75f)
        );

        float musica = PlayerPrefs.GetFloat(
            PREF_MUSICA, 0.75f
        );

        float sfx = PlayerPrefs.GetFloat(
            PREF_SFX, 0.75f
        );

        // Configurar sliders.
        ConfigurarSlider(sliderMaster, master);
        ConfigurarSlider(sliderMusica, musica);
        ConfigurarSlider(sliderSFX, sfx);

        // Aplicar volumen inicial.
        AplicarVolumen(parametroMaster, master);
        AplicarVolumen(parametroMusica, musica);
        AplicarVolumen(parametroSFX, sfx);

        // Registrar eventos.
        if (sliderMaster != null)
        {
            sliderMaster.onValueChanged.AddListener(
                CambiarVolumenMaster
            );
        }

        if (sliderMusica != null)
        {
            sliderMusica.onValueChanged.AddListener(
                CambiarVolumenMusica
            );
        }

        if (sliderSFX != null)
        {
            sliderSFX.onValueChanged.AddListener(
                CambiarVolumenSFX
            );
        }

        Debug.Log(
            "[AUDIO] MainMixer configurado correctamente."
        );
    }

    private void ConfigurarSlider(Slider slider, float volumen)
    {
        if (slider == null)
        {
            Debug.LogWarning(
                "[AUDIO] Falta asignar un Slider."
            );
            return;
        }

        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.wholeNumbers = false;

        slider.SetValueWithoutNotify(
            Mathf.Clamp01(volumen)
        );
    }

    public void CambiarVolumenMaster(float valor)
    {
        AplicarVolumen(parametroMaster, valor);

        PlayerPrefs.SetFloat(
            PREF_MASTER,
            Mathf.Clamp01(valor)
        );

        PlayerPrefs.Save();
    }

    public void CambiarVolumenMusica(float valor)
    {
        AplicarVolumen(parametroMusica, valor);

        PlayerPrefs.SetFloat(
            PREF_MUSICA,
            Mathf.Clamp01(valor)
        );

        PlayerPrefs.Save();
    }

    public void CambiarVolumenSFX(float valor)
    {
        AplicarVolumen(parametroSFX, valor);

        PlayerPrefs.SetFloat(
            PREF_SFX,
            Mathf.Clamp01(valor)
        );

        PlayerPrefs.Save();
    }

    private void AplicarVolumen(
        string parametro,
        float volumenLineal)
    {
        if (audioMixer == null)
            return;

        float volumen = Mathf.Clamp01(volumenLineal);

        // Convertir rango 0-1 a decibelios.
        float decibelios = volumen <= 0.0001f
            ? -80f
            : Mathf.Log10(volumen) * 20f;

        bool aplicado = audioMixer.SetFloat(
            parametro,
            decibelios
        );

        if (aplicado)
        {
            Debug.Log(
                "[AUDIO] Parámetro: " + parametro +
                " | Volumen: " + volumen +
                " | dB: " + decibelios
            );
        }
        else
        {
            Debug.LogError(
                "[AUDIO] No se encontró el parámetro: " +
                parametro +
                ". Revisa Exposed Parameters en MainMixer."
            );
        }
    }

    private void OnDestroy()
    {
        if (sliderMaster != null)
        {
            sliderMaster.onValueChanged.RemoveListener(
                CambiarVolumenMaster
            );
        }

        if (sliderMusica != null)
        {
            sliderMusica.onValueChanged.RemoveListener(
                CambiarVolumenMusica
            );
        }

        if (sliderSFX != null)
        {
            sliderSFX.onValueChanged.RemoveListener(
                CambiarVolumenSFX
            );
        }
    }
}

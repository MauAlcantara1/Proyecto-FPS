using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class ConfiguracionVolumen : MonoBehaviour
{
    [Header("Referencia al Mixer")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("Sliders de UI")]
    [SerializeField] private Slider sliderMusica;
    [SerializeField] private Slider sliderSFX;
    [SerializeField] private Slider sliderSistema;

    private const string PREF_MUSICA = "VolumenMusicaGuardado";
    private const string PREF_SFX = "VolumenSFXGuardado";
    private const string PREF_SISTEMA = "VolumenSistemaGuardado";

    private void Start()
    {
        float volMusica = PlayerPrefs.GetFloat(PREF_MUSICA, 0.75f);
        float volSFX = PlayerPrefs.GetFloat(PREF_SFX, 0.75f);
        float volSis = PlayerPrefs.GetFloat(PREF_SISTEMA, 0.75f);

        if (sliderMusica != null)
        {
            sliderMusica.onValueChanged.RemoveAllListeners();
            sliderMusica.minValue = 0.0001f;
            sliderMusica.maxValue = 1f;
            sliderMusica.SetValueWithoutNotify(volMusica);
            sliderMusica.onValueChanged.AddListener(CambiarVolumenMusica);
        }

        if (sliderSFX != null)
        {
            sliderSFX.onValueChanged.RemoveAllListeners();
            sliderSFX.minValue = 0.0001f;
            sliderSFX.maxValue = 1f;
            sliderSFX.SetValueWithoutNotify(volSFX);
            sliderSFX.onValueChanged.AddListener(CambiarVolumenSFX);
        }

        if (sliderSistema != null)
        {
            sliderSistema.onValueChanged.RemoveAllListeners();
            sliderSistema.minValue = 0.0001f;
            sliderSistema.maxValue = 1f;
            sliderSistema.SetValueWithoutNotify(volSis);
            sliderSistema.onValueChanged.AddListener(CambiarVolumenSis);
        }

        AplicarMusicaAlMixer(volMusica);
        AplicarSFXAlMixer(volSFX);
        AplicarSisAlMixer(volSis);
    }

    public void CambiarVolumenMusica(float valor)
    {
        AplicarMusicaAlMixer(valor);
        PlayerPrefs.SetFloat(PREF_MUSICA, valor);
        PlayerPrefs.Save();
    }

    public void CambiarVolumenSFX(float valor)
    {
        AplicarSFXAlMixer(valor);
        PlayerPrefs.SetFloat(PREF_SFX, valor);
        PlayerPrefs.Save();
    }

    public void CambiarVolumenSis(float valor)
    {
        AplicarSisAlMixer(valor);
        PlayerPrefs.SetFloat(PREF_SISTEMA, valor);
        PlayerPrefs.Save();
    }

    private void AplicarMusicaAlMixer(float valorLineal)
    {
        if (audioMixer != null)
        {
            float dB = Mathf.Log10(Mathf.Clamp(valorLineal, 0.0001f, 1f)) * 20f;
            audioMixer.SetFloat("MusicVolume", dB);
        }
    }

    private void AplicarSFXAlMixer(float valorLineal)
    {
        if (audioMixer != null)
        {
            float dB = Mathf.Log10(Mathf.Clamp(valorLineal, 0.0001f, 1f)) * 20f;
            audioMixer.SetFloat("SFXVolume", dB);
        }
    }

    private void AplicarSisAlMixer(float valorLineal)
    {
        if (audioMixer != null)
        {
            float dB = Mathf.Log10(Mathf.Clamp(valorLineal, 0.0001f, 1f)) * 20f;
            audioMixer.SetFloat("SystemVolume", dB);
        }
    }
}
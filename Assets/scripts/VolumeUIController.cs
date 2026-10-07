using UnityEngine;
using UnityEngine.UI;

public class VolumeUIController : MonoBehaviour
{
    [Header("UI Sliders")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    private void OnEnable()
    {
        // 1. Configurar límites de los sliders (0.0001 evita el error de Log10 en 0)
        ConfigureSlider(masterSlider);
        ConfigureSlider(musicSlider);
        ConfigureSlider(sfxSlider);

        // 2. Sincronizar la UI con los valores guardados
        if (AudioManager.Instance != null)
        {
            masterSlider.value = AudioManager.Instance.GetMasterVolume();
            musicSlider.value = AudioManager.Instance.GetMusicVolume();
            sfxSlider.value = AudioManager.Instance.GetSFXVolume();
        }

        // 3. Suscribir eventos cuando mueves el slider
        if (masterSlider != null) masterSlider.onValueChanged.AddListener(OnMasterSliderChanged);
        if (musicSlider != null) musicSlider.onValueChanged.AddListener(OnMusicSliderChanged);
        if (sfxSlider != null) sfxSlider.onValueChanged.AddListener(OnSFXSliderChanged);
    }

    private void OnDisable()
    {
        // Limpiar escuchadores para evitar perdidas de memoria al cerrar/destruir el menú
        if (masterSlider != null) masterSlider.onValueChanged.RemoveListener(OnMasterSliderChanged);
        if (musicSlider != null) musicSlider.onValueChanged.RemoveListener(OnMusicSliderChanged);
        if (sfxSlider != null) sfxSlider.onValueChanged.RemoveListener(OnSFXSliderChanged);
    }

    private void ConfigureSlider(Slider slider)
    {
        if (slider != null)
        {
            slider.minValue = 0.0001f;
            slider.maxValue = 1f;
        }
    }

    private void OnMasterSliderChanged(float value) => AudioManager.Instance?.SetMasterVolume(value);
    private void OnMusicSliderChanged(float value) => AudioManager.Instance?.SetMusicVolume(value);
    private void OnSFXSliderChanged(float value) => AudioManager.Instance?.SetSFXVolume(value);
}
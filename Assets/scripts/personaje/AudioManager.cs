using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer mainMixer;

    // Claves para PlayerPrefs
    private const string MASTER_KEY = "MasterVolume";
    private const string MUSIC_KEY = "MusicVolume";
    private const string SFX_KEY = "SFXVolume";

    private void Awake()
    {
        // 1. Persistencia entre escenas (Singleton)
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // 2. Carga e inicialización correcta al arrancar
        LoadVolumeSettings();
    }

    // Carga los volúmenes guardados (por defecto 0.75f si es la primera vez)
    private void LoadVolumeSettings()
    {
        SetMasterVolume(PlayerPrefs.GetFloat(MASTER_KEY, 0.75f));
        SetMusicVolume(PlayerPrefs.GetFloat(MUSIC_KEY, 0.75f));
        SetSFXVolume(PlayerPrefs.GetFloat(SFX_KEY, 0.75f));
    }

    // Convertimos el valor del slider (0.0001 a 1) a Decibeles (-80dB a 0dB)
    public void SetMasterVolume(float value)
    {
        float db = Mathf.Log10(Mathf.Max(0.0001f, value)) * 20f;
        mainMixer.SetFloat(MASTER_KEY, db);
        PlayerPrefs.SetFloat(MASTER_KEY, value);
    }

    public void SetMusicVolume(float value)
    {
        float db = Mathf.Log10(Mathf.Max(0.0001f, value)) * 20f;
        mainMixer.SetFloat(MUSIC_KEY, db);
        PlayerPrefs.SetFloat(MUSIC_KEY, value);
    }

    public void SetSFXVolume(float value)
    {
        float db = Mathf.Log10(Mathf.Max(0.0001f, value)) * 20f;
        mainMixer.SetFloat(SFX_KEY, db);
        PlayerPrefs.SetFloat(SFX_KEY, value);
    }

    // Métodos para la sincronización de la UI
    public float GetMasterVolume() => PlayerPrefs.GetFloat(MASTER_KEY, 0.75f);
    public float GetMusicVolume() => PlayerPrefs.GetFloat(MUSIC_KEY, 0.75f);
    public float GetSFXVolume() => PlayerPrefs.GetFloat(SFX_KEY, 0.75f);
}
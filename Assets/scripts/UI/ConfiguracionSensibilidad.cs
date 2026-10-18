using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConfiguracionSensibilidad : MonoBehaviour
{
    [Header("UI de Sensibilidad")]
    [SerializeField] private Slider sliderSensibilidad;
  

    [Header("Rango de Sensibilidad")]
    [SerializeField] private float sensibilidadMinima = 10f;
    [SerializeField] private float sensibilidadMaxima = 500f;
    [SerializeField] private float sensibilidadPorDefecto = 100f;

    private const string PREF_SENSIBILIDAD = "SensibilidadMouseGuardada";
    public static float SensibilidadActual { get; private set; } = 100f;
    public static event Action<float> OnSensibilidadCambiada;

    private void Awake()
    {
        SensibilidadActual = PlayerPrefs.GetFloat(PREF_SENSIBILIDAD, sensibilidadPorDefecto);
    }

    private void Start()
    {
        if (sliderSensibilidad != null)
        {
            sliderSensibilidad.minValue = sensibilidadMinima;
            sliderSensibilidad.maxValue = sensibilidadMaxima;
            sliderSensibilidad.value = SensibilidadActual;

            sliderSensibilidad.onValueChanged.AddListener(CambiarSensibilidad);
        }

    }

    public void CambiarSensibilidad(float nuevoValor)
    {
        SensibilidadActual = nuevoValor;
        PlayerPrefs.SetFloat(PREF_SENSIBILIDAD, nuevoValor);
        PlayerPrefs.Save();
        OnSensibilidadCambiada?.Invoke(nuevoValor);
    }

}
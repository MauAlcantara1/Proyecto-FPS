using UnityEngine;

public class HUDIncapacitadoLocal : MonoBehaviour
{
    [Header("Panel de incapacitación")]
    [SerializeField] private GameObject panelIncapacitado;

    private void Awake()
    {
        if (panelIncapacitado != null)
            panelIncapacitado.SetActive(false);
    }

    public void Conectar(personajeVida jugador)
    {
        if (jugador == null)
            return;

        jugador.AsignarPanelIncapacitado(panelIncapacitado);

        Debug.Log(
            "Panel de incapacitación conectado al jugador local."
        );
    }
}
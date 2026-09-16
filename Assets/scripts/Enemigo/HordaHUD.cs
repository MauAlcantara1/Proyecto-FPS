using UnityEngine;
using TMPro;

public class HordaHUD : MonoBehaviour
{
    [Header("Referencias UI (TextMeshPro)")]
    [SerializeField] private TMP_Text txtRonda;
    [SerializeField] private TMP_Text txtZombiesRestantes;

    public void ActualizarRonda(int numeroRonda)
    {
        if (txtRonda != null)
        {
            txtRonda.SetText("RONDA: {0}", numeroRonda);
        }
    }
    public void ActualizarEnemigosRestantes(int restantes)
    {
        if (txtZombiesRestantes != null)
        {
            txtZombiesRestantes.SetText("ZOMBIES: {0}", restantes);
        }
    }
}
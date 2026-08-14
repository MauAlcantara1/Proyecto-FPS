using UnityEngine;

public class AnimationEventMunicion : MonoBehaviour
{
    private MunicionArma municionArma;

    private void Awake()
    {
        municionArma = GetComponentInParent<MunicionArma>();
    }



    public void IniciarRecarga()
    {
        if (municionArma != null)
        {
            municionArma.IniciarRecarga();
        }
    }


}
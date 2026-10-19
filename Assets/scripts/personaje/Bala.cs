
using UnityEngine;

public class BalaImpacto : MonoBehaviour
{
    [Header("Tipo de Arma")]
    [SerializeField] private bool esMachete = false;

    [Header("Daño")]
    [SerializeField] private int dañoBase = 25;

    [Header("Efectos (Solo Balas)")]
    [SerializeField] private GameObject efectoImpacto;
    [SerializeField] private GameObject marcaImpacto;

    private bool impactoProcesado = false;

    private void Start()
    {

        Collider col = GetComponentInChildren<Collider>(true);


    }

    private void OnCollisionEnter(Collision collision)
    {
        ProcesarImpacto(
            collision.collider,
            collision
        );
    }

    private void OnTriggerEnter(Collider other)
    {

        if (!esMachete)
            return;

        ProcesarImpacto(other, null);
    }

    private void ProcesarImpacto(
        Collider colliderImpactado,
        Collision collision)
    {
       

        if (!esMachete && impactoProcesado)
        {
            return;
        }

        EnemigoController zombie =
            colliderImpactado.GetComponentInParent<EnemigoController>();

        if (zombie != null)
        {
            

            

            zombie.RecibirImpacto(
                dañoBase,
                colliderImpactado
            );

            
        }
        else
        {
           
        }

        // El machete no genera efectos
        // ni se destruye.
        if (esMachete)
        {
            
            return;
        }

        impactoProcesado = true;

        if (collision != null && collision.contactCount > 0)
        {
            ContactPoint contacto = collision.GetContact(0);

            Quaternion rotacion =
                Quaternion.LookRotation(contacto.normal);

            if (efectoImpacto != null)
            {
                GameObject impacto = Instantiate(
                    efectoImpacto,
                    contacto.point,
                    rotacion
                );

                Destroy(impacto, 2f);
            }

            if (marcaImpacto != null)
            {
                GameObject marca = Instantiate(
                    marcaImpacto,
                    contacto.point +
                    contacto.normal * 0.001f,
                    rotacion
                );

                Destroy(marca, 10f);
            }
        }



        Destroy(gameObject);
    }
}

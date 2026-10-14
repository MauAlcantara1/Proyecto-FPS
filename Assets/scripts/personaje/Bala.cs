
using UnityEngine;

public class BalaImpacto : MonoBehaviour
{
    [Header("Daño")]
    [SerializeField] private int dañoBase = 25;

    [Header("Efectos")]
    [SerializeField] private GameObject efectoImpacto;
    [SerializeField] private GameObject marcaImpacto;

    private bool impactoProcesado = false;

    private void OnCollisionEnter(Collision collision)
    {
        // Evitar registrar dos impactos con la misma bala.
        if (impactoProcesado)
            return;

        impactoProcesado = true;

        // Buscar el zombi, incluso si golpeamos
        // un collider hijo, como la cabeza.
        EnemigoController zombie =
            collision.collider.GetComponentInParent<EnemigoController>();

        if (zombie != null)
        {
            zombie.RecibirImpacto(
                dañoBase,
                collision.collider
            );
        }

        if (collision.contactCount > 0)
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

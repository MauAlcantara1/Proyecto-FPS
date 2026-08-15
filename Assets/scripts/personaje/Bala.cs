using UnityEngine;

public class BalaImpacto : MonoBehaviour
{
    [SerializeField] private GameObject efectoImpacto;
    [SerializeField] private GameObject marcaImpacto;

    private void OnCollisionEnter(Collision collision)
    {
        ContactPoint contacto = collision.GetContact(0);

        Quaternion rotacion = Quaternion.LookRotation(contacto.normal);

        // Partícula de impacto
        if (efectoImpacto != null)
        {
            GameObject impacto = Instantiate(
                efectoImpacto,
                contacto.point,
                rotacion
            );

            Destroy(impacto, 2f);
        }

        // Marca de impacto
        if (marcaImpacto != null)
        {
            GameObject marca = Instantiate(
                marcaImpacto,
                contacto.point + contacto.normal * 0.001f,
                rotacion
            );

            Destroy(marca, 10f);
        }

        Destroy(gameObject);
    }
}
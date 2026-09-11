using UnityEngine;

public class Destruible : MonoBehaviour
{
    [SerializeField] private GameObject versionPrefab;
    [SerializeField] private int vidaMaxima = 100;
    [SerializeField] private float fuerzaExplosion = 500f;
    [SerializeField] private float radioExplosion = 2f;

    private int vidaActual;

    private void Start()
    {
        vidaActual = vidaMaxima;
    }

    public void TomarDaño(int daño)
    {
        vidaActual -= daño;

        if (vidaActual <= 0)
        {
            DestruirObjeto();
        }
    }

    private void DestruirObjeto()
    {
        GameObject brokenInstance = Instantiate(versionPrefab, transform.position, transform.rotation);

        Rigidbody[] fragments = brokenInstance.GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in fragments)
        {
            rb.AddExplosionForce(fuerzaExplosion, transform.position, radioExplosion);
        }

        Destroy(brokenInstance, 10f);

        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bala"))
        {
            TomarDaño(25);
            Destroy(collision.gameObject);
        }
    }
}
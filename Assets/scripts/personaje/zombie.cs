using UnityEngine;

public class Zombie : MonoBehaviour
{
    [Header("Vida")]
    [SerializeField] private int vidaMaxima = 100;
    private int vidaActual;
    [SerializeField] private Puntuacion puntuacion;

    private void Start()
    {
        vidaActual = vidaMaxima;
    }

    public void RecibirDaño(int daño)
    {
        vidaActual -= daño;

        if (puntuacion != null)
        {
            puntuacion.SumarPuntosPorDaño();
        }


        if (vidaActual <= 0)
        {
            Morir();
        }
    }

    private void Morir()
    {
        Debug.Log("Zombie muerto");


        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bala"))
        {
            RecibirDaño(25);

            Destroy(collision.gameObject);
        }
    }
}
using UnityEngine;
using UnityEngine.AI;


public class EnemigoRango : MonoBehaviour
{
    public NavMeshAgent enemigo;
    private Transform objetivo; 

    [Header("Vida")]
    [SerializeField] private int vidaMaxima = 100;
    private int vidaActual;
    [SerializeField] private Puntuacion puntuacion;

    [Header("Características")]
    public float velocidad;
    public float rango;
    float distancia;
    private HordaManager hordaManager;


    private void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        
        if (playerObject != null)
        {
            objetivo = playerObject.transform;
        }

        hordaManager = FindFirstObjectByType<HordaManager>();

        vidaActual = vidaMaxima;

    }

    private void Update()
    {
        distancia = Vector3.Distance(enemigo.transform.position, objetivo.position);

        if(distancia < rango)
        {
            Perseguir();
        }
        else if(distancia > rango + 3)
        {
            PararPerseguir();
        }

    }

    private void Perseguir()
    {
         if (enemigo.isOnNavMesh)
        {
            enemigo.SetDestination(objetivo.position);
        }
    }

    private void PararPerseguir()
    {
        if (enemigo.isOnNavMesh)
        {
            enemigo.ResetPath();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(enemigo.transform.position,rango);
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

        if(hordaManager != null)
        {
            hordaManager.EnemyDied(gameObject);
        }

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

using UnityEngine;
using UnityEngine.AI;


public class EnemigoRango : MonoBehaviour
{
    public NavMeshAgent enemigo;
    private Transform objetivo; 

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

    public void Muere()
    {
        if(hordaManager != null)
        {
            hordaManager.EnemyDied(gameObject);
        }
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(enemigo.transform.position,rango);
    }
    
}

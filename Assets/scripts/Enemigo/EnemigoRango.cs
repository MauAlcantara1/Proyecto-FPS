using UnityEngine;
using UnityEngine.AI;


public class EnemigoRango : MonoBehaviour
{
    public NavMeshAgent enemigo;
    public float velocidad;
    public bool perseguir;
    public float rango;
    float distancia;

    public Transform objetivo; 

    private void Update()
    {
        distancia = Vector3.Distance(enemigo.transform.position, objetivo.position);

        if(distancia < rango)
        {
            perseguir = true;
        }else if(distancia > rango + 3)
        {
            perseguir = false;
        }

        if(perseguir == false)
        {
            enemigo.speed = 0;
        }else if (perseguir == true)
        {
            enemigo.speed=velocidad;
            enemigo.SetDestination(objetivo.position);
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(enemigo.transform.position,rango);
    }
    
}

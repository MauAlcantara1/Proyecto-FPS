using UnityEngine;
using UnityEngine.AI;

public class EnemigoController : MonoBehaviour
{
    public NavMeshAgent enemigo;
    private Transform objetivo;
    private personajeVida jugadorVida;

    [Header("Vida")]
    [SerializeField] private int vidaMaxima = 100;
    private int vidaActual;
    [SerializeField] private Puntuacion puntuacion;

    [Header("Características")]
    public float velocidad = 3.5f;
    public float rango = 15f;
    private float distancia;
    private HordaManager hordaManager;

    [Header("Combate")]
    [SerializeField] private int dañoAtaque = 10;
    [SerializeField] private float distanciaAtaque = 2f;
    [SerializeField] private float cadenciaAtaque = 1.2f;
    private float siguienteTiempoAtaque = 0f;

    private void Awake()
    {

        vidaActual = vidaMaxima;

        if (enemigo == null)
        {
            enemigo = GetComponent<NavMeshAgent>();
        }
    }

    private void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        
        if (playerObject != null)
        {
            objetivo = playerObject.transform;
            jugadorVida = playerObject.GetComponent<personajeVida>();
        }

        if (hordaManager == null)
        {
            hordaManager = FindFirstObjectByType<HordaManager>();
        }

        if (puntuacion == null)
        {
            puntuacion = FindFirstObjectByType<Puntuacion>();
        }

        if (enemigo != null)
        {
            enemigo.speed = velocidad;
        }
    }

    public void ConfigurarAtributos(int nuevaVida, int nuevoDaño, float nuevaVelocidad, HordaManager manager)
    {
        vidaMaxima = nuevaVida;
        vidaActual = nuevaVida;
        dañoAtaque = nuevoDaño;
        velocidad = nuevaVelocidad;
        hordaManager = manager;

        if (enemigo != null)
        {
            enemigo.speed = velocidad;
        }
    }

    private void Update()
    {
        if (objetivo == null || enemigo == null) return;

        distancia = Vector3.Distance(enemigo.transform.position, objetivo.position);

        if (distancia <= distanciaAtaque)
        {
            Atacar();
        }
        else if (distancia < rango)
        {
            Perseguir();
        }
        else if (distancia > rango + 3f)
        {
            PararPerseguir();
        }
    }

    private void Perseguir()
    {
        if (enemigo.isOnNavMesh)
        {
            enemigo.isStopped = false;
            enemigo.SetDestination(objetivo.position);
        }
    }

    private void PararPerseguir()
    {
        if (enemigo.isOnNavMesh)
        {
            enemigo.isStopped = true;
            enemigo.ResetPath();
        }
    }

    private void Atacar()
    {
        if (enemigo.isOnNavMesh)
        {
            enemigo.isStopped = true;
        }

        Vector3 direccion = (objetivo.position - transform.position).normalized;
        direccion.y = 0;
        if (direccion != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direccion), Time.deltaTime * 5f);
        }

        if (Time.time >= siguienteTiempoAtaque)
        {
            siguienteTiempoAtaque = Time.time + cadenciaAtaque;

            if (jugadorVida != null)
            {
                jugadorVida.RecibirDaño(dañoAtaque);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 pos = enemigo != null ? enemigo.transform.position : transform.position;
        Gizmos.DrawWireSphere(pos, rango);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(pos, distanciaAtaque);
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
        if (hordaManager != null)
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
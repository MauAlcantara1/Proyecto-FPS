using UnityEngine;
using UnityEngine.AI;

public class EnemigoController : MonoBehaviour
{
    public NavMeshAgent enemigo;
    [SerializeField] private Animator animator;
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
    private bool estaMuerto = false;

    [Header("Puntos Débiles")]
    [SerializeField] private Collider colliderCabeza;
    [SerializeField] private float multiplicadorCabeza = 2.5f;

    [Header("Audio del Zombie")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] sonidosGrunido;
    [SerializeField] private AudioClip sonidoAtaque;
    [SerializeField] private AudioClip sonidoMuerte;
    [SerializeField] private float intervaloGrunidoMin = 4f;
    [SerializeField] private float intervaloGrunidoMax = 9f;
    private float siguienteTiempoGrunido = 0f;

    private void Awake()
    {
        vidaActual = vidaMaxima;

        if (enemigo == null) enemigo = GetComponent<NavMeshAgent>();
        if (animator == null) animator = GetComponentInChildren<Animator>();
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            objetivo = playerObject.transform;
            jugadorVida = playerObject.GetComponent<personajeVida>();
        }

        if (hordaManager == null) hordaManager = FindFirstObjectByType<HordaManager>();
        if (puntuacion == null) puntuacion = FindFirstObjectByType<Puntuacion>();

        if (enemigo != null) enemigo.speed = velocidad;
        siguienteTiempoGrunido = Time.time + Random.Range(1f, 3f);
    }

    public void ConfigurarAtributos(int nuevaVida, int nuevoDaño, float nuevaVelocidad, HordaManager manager)
    {
        vidaMaxima = nuevaVida;
        vidaActual = nuevaVida;
        dañoAtaque = nuevoDaño;
        velocidad = nuevaVelocidad;
        hordaManager = manager;

        if (enemigo != null) enemigo.speed = velocidad;
    }

    private void Update()
    {
        if (estaMuerto || objetivo == null || enemigo == null) return;

        if (animator != null)
        {
            animator.SetFloat("Velocidad", enemigo.velocity.magnitude);
        }

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

        ControlarGrunido();
    }

    private void ControlarGrunido()
    {
        if (Time.time >= siguienteTiempoGrunido)
        {
            siguienteTiempoGrunido = Time.time + Random.Range(intervaloGrunidoMin, intervaloGrunidoMax);

            if (sonidosGrunido != null && sonidosGrunido.Length > 0 && audioSource != null)
            {
                AudioClip clipAleatorio = sonidosGrunido[Random.Range(0, sonidosGrunido.Length)];
                audioSource.pitch = Random.Range(0.85f, 1.15f);
                audioSource.PlayOneShot(clipAleatorio);
            }
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

            if (animator != null)
            {
                animator.SetTrigger("Atacar");
            }

            if (audioSource != null && sonidoAtaque != null)
            {
                audioSource.pitch = Random.Range(0.9f, 1.1f);
                audioSource.PlayOneShot(sonidoAtaque);
            }

            if (jugadorVida != null)
            {
                jugadorVida.RecibirDaño(dañoAtaque);
            }
        }
    }

    public void RecibirDaño(int daño)
    {
        if (estaMuerto) return;

        vidaActual -= daño;

        if (puntuacion != null) puntuacion.SumarPuntosPorDaño();

        if (vidaActual <= 0)
        {
            Morir();
        }
    }

    private void OnCollisionEnter(Collision collision)
{
    if (collision.gameObject.CompareTag("Bala"))
    {
        int dañoFinal = 25;

        if (colliderCabeza != null && collision.collider == colliderCabeza)
        {
            dañoFinal = Mathf.RoundToInt(dañoFinal * multiplicadorCabeza);
            Debug.Log("<color=red>¡HEADSHOT!</color> Daño crítico aplicado: " + dañoFinal);
        }
        else
        {
            Debug.Log("Impacto al cuerpo. Daño: " + dañoFinal);
        }

        RecibirDaño(dañoFinal);

        Destroy(collision.gameObject);
    }
}

    private void Morir()
    {
        estaMuerto = true;

        if (enemigo != null && enemigo.isOnNavMesh)
        {
            enemigo.isStopped = true;
            enemigo.enabled = false;
        }

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        if (animator != null)
        {
            animator.SetTrigger("Morir");
        }

        if (audioSource != null)
        {
            audioSource.Stop();

            if (sonidoMuerte != null)
            {
                audioSource.pitch = Random.Range(0.85f, 1.05f);
                audioSource.PlayOneShot(sonidoMuerte);
            }
        }

        if (hordaManager != null)
        {
            hordaManager.EnemyDied(gameObject);
        }

        Destroy(gameObject, 3.5f);
    }

/*    private void OnDrawGizmos()
    {
        Vector3 centro = enemigo != null ? enemigo.transform.position : transform.position;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(centro, rango);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(centro, distanciaAtaque);
    }
*/
}
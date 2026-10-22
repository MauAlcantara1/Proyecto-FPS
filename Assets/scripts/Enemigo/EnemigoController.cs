
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using System.Collections;
using Photon.Realtime;

public class EnemigoController : MonoBehaviourPun, IPunInstantiateMagicCallback
{
    public NavMeshAgent enemigo;
    [SerializeField] private Animator animator;

    private Transform objetivo;
    private personajeVida jugadorVida;

    [Header("Comportamiento")]
    public bool esZombiDeHorda = false;

    [SerializeField] private float intervaloBusqueda = 0.4f;
    private float siguienteBusqueda = 0f;

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
    [SerializeField] private int dañoAtaque = 3;
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

        if (enemigo == null)
            enemigo = GetComponent<NavMeshAgent>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        int dificultad = SelectorPartida.Dificultad;
        if (hordaManager == null)
            hordaManager = FindFirstObjectByType<HordaManager>();

        if (puntuacion == null)
            puntuacion = FindFirstObjectByType<Puntuacion>();

        if (enemigo != null)
            enemigo.speed = velocidad;

        siguienteTiempoGrunido =
            Time.time + Random.Range(1f, 3f);

        BuscarJugadorMasCercano();
    }

    private bool TieneAutoridad()
    {
        return !PhotonNetwork.InRoom ||
               PhotonNetwork.IsMasterClient;
    }

    private void BuscarJugadorMasCercano()
    {
        GameObject[] jugadores =
            GameObject.FindGameObjectsWithTag("Player");

        Transform nuevoObjetivo = null;
        personajeVida nuevaVida = null;

        float distanciaMinima = Mathf.Infinity;

        foreach (GameObject jugador in jugadores)
        {
            personajeVida vida =
                jugador.GetComponent<personajeVida>();

            if (vida == null)
                vida = jugador.GetComponentInParent<personajeVida>();

            if (vida == null || !vida.EstaVivo())
                continue;

            float distanciaJugador = Vector3.Distance(
                transform.position,
                vida.transform.position
            );

            if (distanciaJugador < distanciaMinima)
            {
                distanciaMinima = distanciaJugador;
                nuevoObjetivo = vida.transform;
                nuevaVida = vida;
            }
        }

        objetivo = nuevoObjetivo;
        jugadorVida = nuevaVida;
    }

    public void RecibirImpacto(
        int dañoBase,
        Collider colliderImpactado)
    {
        if (estaMuerto)
            return;

        int dañoFinal = dañoBase;

        if (colliderCabeza != null &&
            colliderImpactado == colliderCabeza)
        {
            dañoFinal = Mathf.RoundToInt(
                dañoBase * multiplicadorCabeza
            );

            Debug.Log(
                "¡HEADSHOT! Daño: " + dañoFinal
            );
        }


        RecibirDaño(dañoFinal);
    }

    public void ConfigurarAtributos(
        int nuevaVida,
        int nuevoDaño,
        float nuevaVelocidad,
        HordaManager manager,
        bool deHorda)
    {
        vidaMaxima = nuevaVida;
        vidaActual = nuevaVida;
        dañoAtaque = nuevoDaño;
        velocidad = nuevaVelocidad;
        hordaManager = manager;
        esZombiDeHorda = deHorda;

        if (enemigo != null)
            enemigo.speed = velocidad;
    }

    private void Update()
    {
        if (estaMuerto || enemigo == null)
            return;

        if (!TieneAutoridad())
            return;

        if (Time.time >= siguienteBusqueda)
        {
            siguienteBusqueda =
                Time.time + intervaloBusqueda;

            BuscarJugadorMasCercano();
        }

        if (objetivo == null)
        {
            PararPerseguir();
            return;
        }

        distancia = Vector3.Distance(
            transform.position,
            objetivo.position
        );

        if (distancia <= distanciaAtaque)
        {
            Atacar();
        }
        else if (esZombiDeHorda || distancia <= rango)
        {
            esZombiDeHorda = true;
            Perseguir();
        }
        else
        {
            PararPerseguir();
        }

        if (animator != null)
        {
            animator.SetFloat(
                "Velocidad",
                enemigo.velocity.magnitude
            );
        }

        if (esZombiDeHorda)
            ControlarGrunido();
    }

    private void ControlarGrunido()
    {
        if (Time.time < siguienteTiempoGrunido)
            return;

        siguienteTiempoGrunido = Time.time +
            Random.Range(
                intervaloGrunidoMin,
                intervaloGrunidoMax
            );

        if (sonidosGrunido == null ||
            sonidosGrunido.Length == 0 ||
            audioSource == null)
            return;

        AudioClip clip = sonidosGrunido[
            Random.Range(0, sonidosGrunido.Length)
        ];

        audioSource.pitch =
            Random.Range(0.85f, 1.15f);

        audioSource.PlayOneShot(clip);
    }

    private void Perseguir()
    {
        if (objetivo == null || !enemigo.isOnNavMesh)
            return;

        enemigo.isStopped = false;
        enemigo.SetDestination(objetivo.position);
    }

    private void PararPerseguir()
    {
        if (enemigo == null || !enemigo.isOnNavMesh)
            return;

        enemigo.isStopped = true;
        enemigo.ResetPath();
    }

    private void Atacar()
    {
        if (enemigo.isOnNavMesh)
            enemigo.isStopped = true;

        Vector3 direccion =
            objetivo.position - transform.position;

        direccion.y = 0f;

        if (direccion.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(direccion),
                Time.deltaTime * 5f
            );
        }

        if (Time.time < siguienteTiempoAtaque)
            return;

        siguienteTiempoAtaque =
            Time.time + cadenciaAtaque;

        if (animator != null)
            animator.SetTrigger("Atacar");

        if (audioSource != null && sonidoAtaque != null)
        {
            audioSource.pitch =
                Random.Range(0.9f, 1.1f);

            audioSource.PlayOneShot(sonidoAtaque);
        }

        if (jugadorVida != null && jugadorVida.EstaVivo())
        {
            jugadorVida.RecibirDaño(dañoAtaque);
        }
    }

    public void RecibirDaño(int daño)
    {
        if (estaMuerto || daño <= 0)
            return;

        if (!PhotonNetwork.InRoom)
        {
            AplicarDaño(daño, null);
            return;
        }

        if (PhotonNetwork.IsMasterClient)
        {
            AplicarDaño(daño, PhotonNetwork.LocalPlayer);
        }
        else
        {
            photonView.RPC(
                nameof(RPC_SolicitarDaño),
                RpcTarget.MasterClient,
                daño
            );
        }
    }

    [PunRPC]
    private void RPC_SolicitarDaño(
        int daño,
        PhotonMessageInfo info)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        AplicarDaño(daño, info.Sender);
    }

    private void AplicarDaño(int daño, Player jugadorQueDisparo)
    {
        if (estaMuerto || daño <= 0)
            return;

        esZombiDeHorda = true;
        vidaActual = Mathf.Max(0, vidaActual - daño);

        if (PhotonNetwork.InRoom)
        {
            if (jugadorQueDisparo != null)
            {
                photonView.RPC(
                    nameof(RPC_DarPuntos),
                    jugadorQueDisparo
                );
            }
        }
        else
        {
            // Compatible con un jugador.
            Puntuacion puntos =
                FindFirstObjectByType<Puntuacion>();

            if (puntos != null)
                puntos.SumarPuntosPorDaño();
        }

        if (vidaActual <= 0)
            Morir();
    }

    private void Morir()
    {
        if (estaMuerto)
            return;

        if (PhotonNetwork.InRoom)
        {
            if (!PhotonNetwork.IsMasterClient)
                return;

            photonView.RPC(
                nameof(RPC_Morir),
                RpcTarget.All
            );
        }
        else
        {
            EjecutarMuerteVisual();
            StartCoroutine(DestruirZombie());
        }
    }

    [PunRPC]
    private void RPC_Morir()
    {
        EjecutarMuerteVisual();

        // Solo el Master Client controla
        // la destrucción del objeto de red.
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(DestruirZombie());
        }
    }

    private void EjecutarMuerteVisual()
    {
        if (estaMuerto)
            return;

        estaMuerto = true;
        vidaActual = 0;

        if (enemigo != null && enemigo.enabled)
        {
            if (enemigo.isOnNavMesh)
                enemigo.isStopped = true;

            enemigo.enabled = false;
        }

        // Desactivar los colliders del zombi,
        // incluyendo los de las partes del cuerpo.
        Collider[] colliders =
            GetComponentsInChildren<Collider>();

        foreach (Collider col in colliders)
        {
            col.enabled = false;
        }

        if (animator != null)
            animator.SetTrigger("Morir");

        if (audioSource != null)
        {
            audioSource.Stop();

            if (sonidoMuerte != null)
            {
                audioSource.pitch =
                    Random.Range(0.85f, 1.05f);

                audioSource.PlayOneShot(sonidoMuerte);
            }
        }

        // Solo el líder actualiza el contador.
        if (hordaManager != null &&
            (!PhotonNetwork.InRoom ||
            PhotonNetwork.IsMasterClient))
        {
            hordaManager.EnemyDied(gameObject);
        }
    }

    private IEnumerator DestruirZombie()
    {
        yield return new WaitForSeconds(3.5f);

        if (PhotonNetwork.InRoom)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        object[] datos = info.photonView.InstantiationData;

        if (datos == null || datos.Length < 3)
            return;

        int vida = (int)datos[0];
        int daño = (int)datos[1];
        float velocidadNueva = (float)datos[2];

        HordaManager manager =
            FindFirstObjectByType<HordaManager>();

        ConfigurarAtributos(
            vida,
            daño,
            velocidadNueva,
            manager,
            true
        );


    }

    [PunRPC]
    private void RPC_DarPuntos(PhotonMessageInfo info)
    {
        // Aceptar recompensas únicamente del líder.
        if (!PhotonNetwork.InRoom ||
            info.Sender != PhotonNetwork.MasterClient)
            return;

        Puntuacion puntosLocales =
            FindFirstObjectByType<Puntuacion>();

        if (puntosLocales != null)
        {
            puntosLocales.SumarPuntosPorDaño();
        }
        else
        {
            Debug.LogWarning(
                "No se encontró Puntuacion local."
            );
        }
    }

}

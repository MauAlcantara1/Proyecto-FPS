using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public class HordaManager : MonoBehaviourPunCallbacks
{
    public enum Dificultad
    {
        Facil = 0,
        Dificil = 1
    }

    [Header("Dificultad")]
    private Dificultad dificultadActual;

    [Header("Enemigo Prefab")]
    [SerializeField] private GameObject enemigoPrefab;

    [Header("Puntos de aparición")]
    [SerializeField] private Transform[] spawnPoints;

    [Header("Límites y Oleadas")]
    [SerializeField] private int baseEnemigosPorWave = 6;
    [SerializeField] private int incrementoPorRonda = 2;
    [SerializeField] private int maxZombiesSimultaneos = 12;
    [SerializeField] private float spawnDelay = 0.8f;
    [SerializeField] private float tiempoEntreWaves = 5f;

    [Header("Stats Base del Zombie")]
    [SerializeField] private int vidaBaseZombie = 50;
    [SerializeField] private int vidaExtraPorBloque = 25;
    [SerializeField] private int dañoBaseZombie = 5;

    [Header("Interfaz y Audio")]
    [SerializeField] private HordaHUD hordaHUD;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip sonidoInicioRonda;
    [SerializeField] private AudioClip sonidoFinRonda;

    private List<GameObject> enemigosVivosEnMapa =
        new List<GameObject>();

    private int waveActual = 0;
    private int enemigosPendientesPorSpawnear = 0;
    private int totalEnemigosRestantesRonda = 0;
    private bool waveEnProgreso = false;

    private bool EsAutoridad()
    {
        return !PhotonNetwork.InRoom ||
               PhotonNetwork.IsMasterClient;
    }

    private void Start()
    {
        if (hordaHUD == null)
            hordaHUD = FindFirstObjectByType<HordaHUD>();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (SelectorPartida.Dificultad == 0)
        {
            dificultadActual = Dificultad.Facil;
            Debug.Log("Dificultad: FÁCIL");
        }
        else
        {
            dificultadActual = Dificultad.Dificil;
            Debug.Log("Dificultad: DIFÍCIL");
        }

        // Solo el Master Client genera hordas.
        if (!EsAutoridad())
        {
            Debug.Log(
                "Cliente secundario: esperando zombis de Photon."
            );
            return;
        }

        if (PhotonNetwork.InRoom)
        {
            Debug.Log(
                "Master Client: controlando las hordas."
            );
        }

        StartCoroutine(StartNextWave());
    }

    private IEnumerator StartNextWave()
    {
        if (!EsAutoridad())
            yield break;

        waveEnProgreso = false;

        yield return new WaitForSeconds(tiempoEntreWaves);

        if (!EsAutoridad())
            yield break;

        waveActual++;
        waveEnProgreso = true;

        if (audioSource != null &&
            sonidoInicioRonda != null)
        {
            audioSource.PlayOneShot(sonidoInicioRonda);
        }

        int totalEnemigosOleada =
            CalcularCantidadZombies();

        enemigosPendientesPorSpawnear =
            totalEnemigosOleada;

        totalEnemigosRestantesRonda =
            totalEnemigosOleada;

        SincronizarHUD();

        StartCoroutine(GestorDeSpawn());
    }

    private IEnumerator GestorDeSpawn()
    {
        while (enemigosPendientesPorSpawnear > 0)
        {
            if (!EsAutoridad())
                yield break;

            enemigosVivosEnMapa.RemoveAll(
                e => e == null
            );

            if (enemigosVivosEnMapa.Count <
                maxZombiesSimultaneos)
            {
                if (!SpawnEnemy())
                {
                    Debug.LogError(
                        "No se pudo crear el zombie."
                    );
                    yield break;
                }

                enemigosPendientesPorSpawnear--;

                yield return new WaitForSeconds(
                    spawnDelay
                );
            }
            else
            {
                yield return new WaitForSeconds(0.5f);
            }
        }
    }

    private bool SpawnEnemy()
    {
        if (!EsAutoridad())
            return false;

        if (enemigoPrefab == null ||
            spawnPoints == null ||
            spawnPoints.Length == 0)
        {
            Debug.LogError(
                "Falta prefab o puntos de aparición."
            );
            return false;
        }

        Transform spawnPoint = spawnPoints[
            Random.Range(0, spawnPoints.Length)
        ];

        Vector3 posicionSegura = spawnPoint.position;

        if (NavMesh.SamplePosition(
            spawnPoint.position,
            out NavMeshHit hit,
            5.0f,
            NavMesh.AllAreas))
        {
            posicionSegura = hit.position;
        }

        int vidaCalculada = CalcularVidaZombie();
        int dañoCalculado = CalcularDañoZombie();
        float velocidadCalculada =
            CalcularVelocidadZombie();

        GameObject nuevoEnemigo;

        if (PhotonNetwork.InRoom)
        {
            // Estadísticas que recibe cada cliente.
            object[] datosZombie =
            {
                vidaCalculada,
                dañoCalculado,
                velocidadCalculada
            };

            nuevoEnemigo =
                PhotonNetwork.InstantiateRoomObject(
                    enemigoPrefab.name,
                    posicionSegura,
                    spawnPoint.rotation,
                    0,
                    datosZombie
                );
        }
        else
        {
            // Conservamos funcionamiento sin Photon.
            nuevoEnemigo = Instantiate(
                enemigoPrefab,
                posicionSegura,
                spawnPoint.rotation
            );

            EnemigoController controller =
                nuevoEnemigo.GetComponent<EnemigoController>();

            if (controller != null)
            {
                controller.ConfigurarAtributos(
                    vidaCalculada,
                    dañoCalculado,
                    velocidadCalculada,
                    this,
                    true
                );
            }
        }

        if (nuevoEnemigo == null)
            return false;

        NavMeshAgent agente =
            nuevoEnemigo.GetComponent<NavMeshAgent>();

        if (agente != null &&
            agente.enabled &&
            agente.isOnNavMesh)
        {
            agente.Warp(posicionSegura);
        }

        enemigosVivosEnMapa.Add(nuevoEnemigo);

        Debug.Log(
            "Zombie generado en la ronda " + waveActual
        );

        return true;
    }

    public void EnemyDied(GameObject enemy)
    {
        if (!EsAutoridad())
            return;

        if (!enemigosVivosEnMapa.Remove(enemy))
            return;

        totalEnemigosRestantesRonda =
            Mathf.Max(
                0,
                totalEnemigosRestantesRonda - 1
            );

        SincronizarHUD();

        if (enemigosPendientesPorSpawnear <= 0 &&
            enemigosVivosEnMapa.Count == 0 &&
            waveEnProgreso)
        {
            waveEnProgreso = false;

            if (audioSource != null &&
                sonidoFinRonda != null)
            {
                audioSource.PlayOneShot(sonidoFinRonda);
            }

            StartCoroutine(StartNextWave());
        }
    }

    private void Update()
    {
        if (!EsAutoridad())
            return;

        enemigosVivosEnMapa.RemoveAll(
            e => e == null
        );
    }

    private int CalcularCantidadZombies()
    {
        float multDificultad = dificultadActual switch
        {
            Dificultad.Facil => 0.8f,
            Dificultad.Dificil => 1.3f,
            _ => 0.8f
        };

        return Mathf.Max(
            1,
            Mathf.RoundToInt(
                (baseEnemigosPorWave +
                (waveActual - 1) * incrementoPorRonda)
                * multDificultad
            )
        );
    }

    private int CalcularVidaZombie()
    {
        float multDificultad = dificultadActual switch
        {
            Dificultad.Facil => 0.8f,
            Dificultad.Dificil => 1.4f,
            _ => 0.8f
        };

        int bloquesDeCinco =
            (waveActual - 1) / 5;

        return Mathf.RoundToInt(
            (vidaBaseZombie +
            bloquesDeCinco * vidaExtraPorBloque)
            * multDificultad
        );
    }

    private int CalcularDañoZombie()
    {
        float multDificultad = dificultadActual switch
        {
            Dificultad.Facil => 1.0f,
            Dificultad.Dificil => 3.0f,
            _ => 1.0f
        };

        return Mathf.Max(
            1,
            Mathf.RoundToInt(
                dañoBaseZombie * multDificultad
            )
        );
    }

    private float CalcularVelocidadZombie()
    {
        return dificultadActual switch
        {
            Dificultad.Facil => 3.5f,
            Dificultad.Dificil => 5f,
            _ => 3.5f
        };
    }

    public void EstablecerDificultad(
        Dificultad nuevaDificultad)
    {
        dificultadActual = nuevaDificultad;
    }

    private void SincronizarHUD()
    {
        if (hordaHUD == null)
            hordaHUD = FindFirstObjectByType<HordaHUD>();

        // El líder actualiza su propia interfaz.
        if (hordaHUD != null)
        {
            hordaHUD.ActualizarRonda(waveActual);

            hordaHUD.ActualizarEnemigosRestantes(
                totalEnemigosRestantesRonda
            );
        }

        // Solo el Master Client envía los valores.
        if (PhotonNetwork.InRoom &&
            PhotonNetwork.IsMasterClient)
        {
            photonView.RPC(
                nameof(RPC_ActualizarHUD),
                RpcTarget.Others,
                waveActual,
                totalEnemigosRestantesRonda
            );
        }
    }

    [PunRPC]
    private void RPC_ActualizarHUD(int ronda, int restantes)
    {
        // El invitado recibe los valores del líder.
        waveActual = ronda;
        totalEnemigosRestantesRonda = restantes;

        if (hordaHUD == null)
            hordaHUD = FindFirstObjectByType<HordaHUD>();

        if (hordaHUD != null)
        {
            hordaHUD.ActualizarRonda(ronda);
            hordaHUD.ActualizarEnemigosRestantes(restantes);
        }
    }
}

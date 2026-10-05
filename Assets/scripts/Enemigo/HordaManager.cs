using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HordaManager : MonoBehaviour
{
    public enum Dificultad { Facil, Normal, Dificil, Pesadilla }

    [Header("Dificultad")]
    [SerializeField] private Dificultad dificultadActual = Dificultad.Normal;

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
    [SerializeField] private int dañoBaseZombie = 10;

    [Header("Interfaz y Audio")]
    [SerializeField] private HordaHUD hordaHUD;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip sonidoInicioRonda;
    [SerializeField] private AudioClip sonidoFinRonda;

    // Control interno de la horda
    private List<GameObject> enemigosVivosEnMapa = new List<GameObject>();
    private int waveActual = 0;
    private int enemigosPendientesPorSpawnear = 0;
    private int totalEnemigosRestantesRonda = 0;
    private bool waveEnProgreso = false;
    

    private void Start()
    {
        if (hordaHUD == null) hordaHUD = FindFirstObjectByType<HordaHUD>();
        if (audioSource == null) audioSource = GetComponent<AudioSource>();

        StartCoroutine(StartNextWave());
    }

    private IEnumerator StartNextWave()
    {
        waveEnProgreso = false;
        yield return new WaitForSeconds(tiempoEntreWaves);

        waveActual++;
        waveEnProgreso = true;

        if (audioSource != null && sonidoInicioRonda != null)
        {
            audioSource.PlayOneShot(sonidoInicioRonda);
        }

        int totalEnemigosOleada = CalcularCantidadZombies();
        enemigosPendientesPorSpawnear = totalEnemigosOleada;
        totalEnemigosRestantesRonda = totalEnemigosOleada;

        if (hordaHUD != null)
        {
            hordaHUD.ActualizarRonda(waveActual);
            hordaHUD.ActualizarEnemigosRestantes(totalEnemigosRestantesRonda);
        }

        StartCoroutine(GestorDeSpawn());
    }

    private IEnumerator GestorDeSpawn()
    {
        while (enemigosPendientesPorSpawnear > 0)
        {
            if (enemigosVivosEnMapa.Count < maxZombiesSimultaneos)
            {
                SpawnEnemy();
                enemigosPendientesPorSpawnear--;
                yield return new WaitForSeconds(spawnDelay);
            }
            else
            {
                yield return new WaitForSeconds(0.5f);
            }
        }
    }

   private void SpawnEnemy()
    {
        if (enemigoPrefab == null || spawnPoints == null || spawnPoints.Length == 0) return;

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

       Vector3 posicionSegura = spawnPoint.position;
        // Busca el punto más cercano de la malla azul en un radio de 5 metros
        if (UnityEngine.AI.NavMesh.SamplePosition(spawnPoint.position, out UnityEngine.AI.NavMeshHit hit, 5.0f, UnityEngine.AI.NavMesh.AllAreas))
        {
            posicionSegura = hit.position;
        }

        GameObject nuevoEnemigo = Instantiate(
            enemigoPrefab,
            posicionSegura,
            spawnPoint.rotation
        );
        
        // Forzar al agente a acoplarse
        nuevoEnemigo.GetComponent<UnityEngine.AI.NavMeshAgent>().Warp(posicionSegura);

        int vidaCalculada = CalcularVidaZombie();
        int dañoCalculado = CalcularDañoZombie();
        float velocidadCalculada = 3.5f;

        EnemigoController controller = nuevoEnemigo.GetComponent<EnemigoController>();
        if (controller != null)
        {
            controller.ConfigurarAtributos(vidaCalculada, dañoCalculado, velocidadCalculada, this, true);
        }

        enemigosVivosEnMapa.Add(nuevoEnemigo);
    }
    public void EnemyDied(GameObject enemy)
    {
        if (enemigosVivosEnMapa.Contains(enemy))
        {
            enemigosVivosEnMapa.Remove(enemy);
        }

        totalEnemigosRestantesRonda--;
        if (totalEnemigosRestantesRonda < 0) totalEnemigosRestantesRonda = 0;

        if (hordaHUD != null)
        {
            hordaHUD.ActualizarEnemigosRestantes(totalEnemigosRestantesRonda);
        }

        if (enemigosPendientesPorSpawnear <= 0 && enemigosVivosEnMapa.Count == 0 && waveEnProgreso)
        {
            waveEnProgreso = false;

            if (audioSource != null && sonidoFinRonda != null)
            {
                audioSource.PlayOneShot(sonidoFinRonda);
            }

            StartCoroutine(StartNextWave());
        }
    }

    private void Update()
    {
        enemigosVivosEnMapa.RemoveAll(e => e == null);
    }

    private int CalcularCantidadZombies()
    {
        float multDificultad = dificultadActual switch
        {
            Dificultad.Facil => 0.8f,
            Dificultad.Normal => 1.0f,
            Dificultad.Dificil => 1.3f,
            Dificultad.Pesadilla => 1.6f,
            _ => 1.0f
        };

        return Mathf.Max(1, Mathf.RoundToInt((baseEnemigosPorWave + (waveActual - 1) * incrementoPorRonda) * multDificultad));
    }

    private int CalcularVidaZombie()
    {
        float multDificultad = dificultadActual switch
        {
            Dificultad.Facil => 0.8f,
            Dificultad.Normal => 1.0f,
            Dificultad.Dificil => 1.4f,
            Dificultad.Pesadilla => 2.0f,
            _ => 1.0f
        };

       int bloquesDeCinco = (waveActual - 1) / 5;

        int vidaCalculada = Mathf.RoundToInt((vidaBaseZombie + (bloquesDeCinco * vidaExtraPorBloque)) * multDificultad);

    return vidaCalculada;
    }

    private int CalcularDañoZombie()
    {
        float multDificultad = dificultadActual switch
        {
            Dificultad.Facil => 0.7f,
            Dificultad.Normal => 1.0f,
            Dificultad.Dificil => 1.5f,
            Dificultad.Pesadilla => 2.2f,
            _ => 1.0f
        };

        return Mathf.Max(1, Mathf.RoundToInt(dañoBaseZombie * multDificultad));
    }

    public void EstablecerDificultad(Dificultad nuevaDificultad) => dificultadActual = nuevaDificultad;
}
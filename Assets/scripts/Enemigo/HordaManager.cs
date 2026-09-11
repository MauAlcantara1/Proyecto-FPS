using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HordaManager : MonoBehaviour
{
    [Header("Enemigo")]
    [SerializeField] private GameObject enemigo;

    [Header("Puntos de aparición")]
    [SerializeField] private Transform[] spawnPoints;

    [Header("Configuración de oleadas")]
    [SerializeField] private int enemigosPorWave = 5;
    [SerializeField] private float spawnDelay = 0.5f;
    [SerializeField] private float tiempoEntreWaves = 5f;

    private List<GameObject> enemigosActivos = new List<GameObject>();

    private int waveActual = 0;
    private bool waveEnProgreso = false;

    private void Start()
    {
        StartCoroutine(StartNextWave());
    }

    private IEnumerator StartNextWave()
    {
        waveEnProgreso = false;

        Debug.Log("Siguiente oleada en " + tiempoEntreWaves + " segundos.");

        yield return new WaitForSeconds(tiempoEntreWaves);

        waveActual++;

        Debug.Log("===== OLEADA " + waveActual + " =====");

        waveEnProgreso = true;

        for (int i = 0; i < enemigosPorWave; i++)
        {
            SpawnEnemy();

            yield return new WaitForSeconds(spawnDelay);
        }
    }

    private void SpawnEnemy()
    {
        if (enemigo == null)
        {
            Debug.LogError("HordeManager: No se ha asignado el Enemy Prefab.");
            return;
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("HordeManager: No hay Spawn Points.");
            return;
        }

        Transform spawnPoint =
            spawnPoints[Random.Range(0, spawnPoints.Length)];

        GameObject enemy = Instantiate(
            enemigo,
            spawnPoint.position,
            spawnPoint.rotation
        );

        enemigosActivos.Add(enemy);

        Debug.Log("Enemigo generado. Enemigos activos: " + enemigosActivos.Count);
    }

    private void Update()
    {
        enemigosActivos.RemoveAll(enemy => enemy == null);

        if (waveEnProgreso && enemigosActivos.Count == 0)
        {
            waveEnProgreso = false;

            Debug.Log("Oleada " + waveActual + " terminada.");

            StartCoroutine(StartNextWave());
        }
    }

    public void EnemyDied(GameObject enemy)
    {
        if (enemigosActivos.Contains(enemy))
        {
            enemigosActivos.Remove(enemy);
        }

        Debug.Log(
            "Enemigo eliminado. Quedan: " + enemigosActivos.Count
        );
    }
}
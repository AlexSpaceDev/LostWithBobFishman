using System.Collections;
using UnityEngine;

public class FishSpawner : MonoBehaviour
{
    [Header("Prefabs de peces")]
    public GameObject goodFishPrefab; // antes era 'fishes'
    public GameObject badFishPrefab;  // nuevo prefab

    [Header("Puntos de aparición")]
    public Transform[] leftSpawnPoints;
    public Transform[] rightSpawnPoints;

    [Header("Tiempos de spawn")]
    public float minSpawnDelay = 0.5f;
    public float maxSpawnDelay = 1.0f;

    [Header("Escala forzada de peces")]
    public Vector3 forcedFishScale = new Vector3(1f, 1f, 1f);

    void Start()
    {
    }

    public void StartSpawning()
    {
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            SpawnFish();
            float wait = Random.Range(minSpawnDelay, maxSpawnDelay);
            yield return new WaitForSeconds(wait);
        }
    }

    void SpawnFish()
    {
        bool spawnLeft = Random.value > 0.5f; // 50/50 izquierda o derecha
        Transform[] spawnArray = spawnLeft ? leftSpawnPoints : rightSpawnPoints;
        Transform[] targetArray = spawnLeft ? rightSpawnPoints : leftSpawnPoints;

        Transform spawnPoint = spawnArray[Random.Range(0, spawnArray.Length)];
        Transform targetPoint = targetArray[Random.Range(0, targetArray.Length)];

        // 🧠 50% probabilidad de pez bueno o malo
        GameObject prefabToSpawn = Random.value > 0.5f ? goodFishPrefab : badFishPrefab;

        GameObject spawnedFish = Instantiate(prefabToSpawn, spawnPoint.position, Quaternion.identity);

        // 🐟 Forzar escala fija independientemente del prefab o jerarquía
        spawnedFish.transform.localScale = forcedFishScale;

        // 🧭 Enviar objetivo
        spawnedFish.GetComponent<Fish>().SetTarget(targetPoint.position);
    }
}



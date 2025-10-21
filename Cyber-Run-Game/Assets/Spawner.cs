using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject[] obstaclePrefabs;   // candidate obstacles
    [SerializeField] private Transform obstacleParent;       // where spawned obstacles live in the hierarchy
    [SerializeField] private float obstacleSpawnTime = 3f;   // base time between spawns
    [Range(0, 1)] public float obstacleSpawnTimeFactor = 0.1f; // how strongly spawn time scales over run time
    [SerializeField] private float obstacleSpeed = 7f;       // base obstacle speed
    [Range(0, 1)] public float obstacleSpeedFactor = 0.2f;   // how strongly speed scales over run time

    // scaled values computed each frame based on timeAlive
    private float _obstacleSpawnTime;
    private float _obstacleSpeed;

    private float timeUntilObstacleSpawn; // per-spawn countdown
    private float timeAlive;              // seconds since run started (starts at 1 to avoid div-by-zero)

    private void Start()
    {
        // clear all obstacles on game over; reset scaling at game start
        GameManager.Instance.onGameOver.AddListener(ClearObstacle);
        GameManager.Instance.onPlay.AddListener(ResetFactors);
    }

    private void Update()
    {
        // only run spawning while the game is active
        if (!GameManager.Instance.isPlaying) return;

        timeAlive += Time.deltaTime;   // advance run time
        CalculateFactors();            // update scaled spawn time and speed
        SpawnLoop();                   // drive spawn cadence
    }

    private void SpawnLoop()
    {
        timeUntilObstacleSpawn += Time.deltaTime;

        if (timeUntilObstacleSpawn >= _obstacleSpawnTime)
        {
            Spawn();
            timeUntilObstacleSpawn = 0f;
        }
    }

    // Destroy all existing obstacles under the parent container
    private void ClearObstacle()
    {
        foreach (Transform child in obstacleParent)
            Destroy(child.gameObject);
    }

    // Recompute scaled spawn time and speed from elapsed run time
    private void CalculateFactors()
    {
        // spawn interval gets shorter over time; speed increases over time
        _obstacleSpawnTime = obstacleSpawnTime / Mathf.Pow(timeAlive, obstacleSpawnTimeFactor);
        _obstacleSpeed = obstacleSpeed * Mathf.Pow(timeAlive, obstacleSpeedFactor);
    }

    // Reset run-time scaling and timers at the start of a new run
    private void ResetFactors()
    {
        timeAlive = 1f; // start at 1 to keep Pow() stable and avoid division by zero
        _obstacleSpawnTime = obstacleSpawnTime;
        _obstacleSpeed = obstacleSpeed;
        timeUntilObstacleSpawn = 0f;
    }

    private void Spawn()
    {
        // pick a random obstacle prefab
        GameObject prefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];

        // instantiate at spawner position, parent under obstacleParent
        GameObject instance = Instantiate(prefab, transform.position, Quaternion.identity);
        instance.transform.parent = obstacleParent;

        // give it a leftward velocity using the scaled speed
        Rigidbody2D rb = instance.GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.left * _obstacleSpeed;
    }
}

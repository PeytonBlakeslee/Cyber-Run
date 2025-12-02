using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject[] obstaclePrefabs;   // candidate obstacles
    [SerializeField] private Transform obstacleParent;       // where spawned obstacles live in the hierarchy
    [SerializeField] private float obstacleSpawnTime = 3f;   // base time between spawns (Normal mode)
    [Range(0, 1)] public float obstacleSpawnTimeFactor = 0.1f; // how strongly spawn time scales over run time
    [SerializeField] private float obstacleSpeed = 7f;       // base obstacle speed (Normal mode)
    [Range(0, 1)] public float obstacleSpeedFactor = 0.2f;   // how strongly speed scales over run time

    [Header("Hard Mode Multipliers")]
    [Tooltip("< 1 = faster spawns (less time between obstacles) in Hard")]
    [SerializeField] private float hardSpawnMultiplier   = 0.7f;
    [Tooltip("> 1 = faster obstacles in Hard")]
    [SerializeField] private float hardSpeedMultiplier   = 1.2f;
    [Tooltip("> 1 = ramps faster over time in Hard")]
    [SerializeField] private float hardRampMultiplier    = 1.5f;

    [Header("Caps (to avoid impossible infinite scaling)")]
    [SerializeField] private float minSpawnTime = 0.25f;   // fastest allowed spawn interval (seconds)
    [SerializeField] private float maxObstacleSpeed = 25f; // fastest allowed obstacle speed

    // scaled values computed each frame based on timeAlive + difficulty
    private float _obstacleSpawnTime;
    private float _obstacleSpeed;

    private float timeUntilObstacleSpawn; // per-spawn countdown
    private float timeAlive;              // seconds since run started

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
        CalculateFactors();            // update scaled spawn time and speed (with difficulty)
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

    // Recompute scaled spawn time and speed from elapsed run time + difficulty
    private void CalculateFactors()
    {
        // default multipliers (Normal mode = base behavior)
        float spawnMult = 1f;
        float speedMult = 1f;
        float rampMult  = 1f;

        // In Hard mode, make spawn interval smaller, speed higher, and ramp faster
        if (GameManager.Instance != null &&
            GameManager.Instance.difficulty == GameManager.DifficultyMode.Hard)
        {
            spawnMult = hardSpawnMultiplier;
            speedMult = hardSpeedMultiplier;
            rampMult  = hardRampMultiplier;
        }

        // t is basically "seconds alive" but scaled for harder ramp in Hard mode
        float t = timeAlive * rampMult;

        // base scaling: spawn interval gets shorter over time; speed increases over time
        // use (t + 1) to keep Pow() stable and avoid issues near zero
        float scaledSpawnDivisor = Mathf.Pow(t + 1f, obstacleSpawnTimeFactor);
        float scaledSpeedFactor  = Mathf.Pow(t + 1f, obstacleSpeedFactor);

        _obstacleSpawnTime = (obstacleSpawnTime * spawnMult) / scaledSpawnDivisor;
        _obstacleSpeed     = (obstacleSpeed * speedMult) * scaledSpeedFactor;

        // clamp so the game does not become literally impossible
        _obstacleSpawnTime = Mathf.Max(_obstacleSpawnTime, minSpawnTime);
        _obstacleSpeed     = Mathf.Min(_obstacleSpeed, maxObstacleSpeed);

        // report this frame's effective speed to GameManager for stats as a MULTIPLIER of base speed
        if (GameManager.Instance != null && obstacleSpeed > 0f)
        {
            float speedMultiplier = _obstacleSpeed / obstacleSpeed; // 1.0 = base, 2.0 = twice as fast, etc.
            GameManager.Instance.RegisterSpeedSample(speedMultiplier);
        }
    }

    // Reset run-time scaling and timers at the start of a new run
    private void ResetFactors()
    {
        timeAlive = 0f;
        timeUntilObstacleSpawn = 0f;

        // recompute scaled values immediately for the new run
        CalculateFactors();
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

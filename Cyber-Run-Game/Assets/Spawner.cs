using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject[] obstaclePrefabs; // obstacles that can spawn
    [SerializeField] private Transform obstacleParent;
    [SerializeField] private float obstacleSpawnTime = 3f; // time between each spawn
    [Range(0, 1)] public float obstacleSpawnTimeFactor = 0.1f;
    [SerializeField] private float obstacleSpeed = 7f;     // how fast obstacles move
    [Range(0, 1)] public float obstacleSpeedFactor = 0.2f;

    private float _obstacleSpawnTime;
    private float _obstacleSpeed;

    private float timeUntilObstacleSpawn; // spawn timer

    private float timeAlive;

    private void Start()
    {
        GameManager.Instance.onGameOver.AddListener(ClearObstacle);
        GameManager.Instance.onPlay.AddListener(ResetFactors);
    }

    private void Update()
    {
        // only run spawning while the game is active
        if (GameManager.Instance.isPlaying)
        {
            timeAlive += Time.deltaTime;

            CalculateFactors();

            SpawnLoop();
        }
    }

    private void SpawnLoop()
    {
        // count time until next obstacle spawn
        timeUntilObstacleSpawn += Time.deltaTime;

        // if enough time passed, spawn an obstacle
        if (timeUntilObstacleSpawn >= _obstacleSpawnTime)
        {
            Spawn();
            timeUntilObstacleSpawn = 0f;
        }
    }

    private void ClearObstacle()
    {
        foreach(Transform child in obstacleParent)
        {
            Destroy(child.gameObject);
        }
    }

    private void CalculateFactors()
    {
        _obstacleSpawnTime = obstacleSpawnTime / Mathf.Pow(timeAlive, obstacleSpawnTimeFactor);
        _obstacleSpeed = obstacleSpeed * Mathf.Pow(timeAlive, obstacleSpeedFactor);

    }
    private void ResetFactors()
    {
        timeAlive = 1f;
        _obstacleSpawnTime = obstacleSpawnTime;
        _obstacleSpeed = obstacleSpeed;
    }
    private void Spawn()
    {
        // pick a random obstacle
        GameObject obstacle = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];

        // create it at this spawner's position
        GameObject newObstacle = Instantiate(obstacle, transform.position, Quaternion.identity);

        newObstacle.transform.parent = obstacleParent;

        // make it move left at a set speed
        Rigidbody2D rb = newObstacle.GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.left * _obstacleSpeed;
    }
}

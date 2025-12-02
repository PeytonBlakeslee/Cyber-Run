using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    [Header("Speed Settings")]
    public float baseScrollSpeed = 4f;          // starting scroll speed
    [Range(0f, 1f)] public float speedFactor = 0.2f;  // how strongly speed ramps over time

    private float width;
    private Vector3 startPosition;

    void Start()
    {
        // remember where this tile started so we can reset on new runs
        startPosition = transform.position;

        // grab width from any child SpriteRenderer (FarBG / City)
        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        width = sr.bounds.size.x;

        // optional: reset position whenever a new game starts
        if (GameManager.Instance != null)
            GameManager.Instance.onPlay.AddListener(ResetScroll);
    }

    void Update()
    {
        // no GameManager or not currently playing = do nothing
        if (GameManager.Instance == null || !GameManager.Instance.isPlaying)
            return;

        // use currentScore as "time alive" since it already increments with Time.deltaTime
        // t is basically "seconds since run started"
        float t = GameManager.Instance.currentScore;

        // ramp speed up over time. t+1 so we never raise 0 to a power
        float scrollSpeed = baseScrollSpeed * Mathf.Pow(t + 1f, speedFactor);

        // move left using the ramped speed
        transform.position += Vector3.left * scrollSpeed * Time.deltaTime;

        // seamless looping
        if (transform.position.x <= -width)
        {
            transform.position += new Vector3(width * 2f, 0f, 0f);
        }
    }

    private void ResetScroll()
    {
        // put this tile back to its starting spot when a new run begins
        transform.position = startPosition;
    }
}

using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    [Header("Speed Settings")]
    public float baseScrollSpeed = 4f;                 // starting scroll speed
    [Range(0f, 1f)] public float speedFactor = 0.2f;   // how strongly speed ramps over time

    [Header("Tiling")]
    [Tooltip("If > 0, this overrides the auto width detection.")]
    [SerializeField] private float overrideWidth = 0f;

    [Tooltip("Small positive value to force a tiny overlap and avoid visible gaps.")]
    [SerializeField] private float seamFixOffset = 0.02f;

    private float width;
    private Vector3 startPosition;

    void Start()
    {
        // remember where this tile started so we can reset on new runs
        startPosition = transform.position;

        // either use manual width or auto-detect
        if (overrideWidth > 0f)
        {
            width = overrideWidth;
        }
        else
        {
            // grab width from any child SpriteRenderer (FarBG / City)
            SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
            if (sr != null)
                width = sr.bounds.size.x;
            else
                width = 10f; // fallback so it's never zero
        }

        // optional: reset position whenever a new game starts
        if (GameManager.Instance != null)
            GameManager.Instance.onPlay.AddListener(ResetScroll);
    }

    void Update()
    {
        if (GameManager.Instance == null || !GameManager.Instance.isPlaying)
            return;

        // use currentScore as "time alive"
        float t = GameManager.Instance.currentScore;

        // ramp speed up over time
        float scrollSpeed = baseScrollSpeed * Mathf.Pow(t + 1f, speedFactor);

        // move left using the ramped speed
        transform.position += Vector3.left * scrollSpeed * Time.deltaTime;

        // seamless looping with small overlap to hide any seam
        if (transform.position.x <= -width)
        {
            transform.position += new Vector3(width * 2f - seamFixOffset, 0f, 0f);
        }
    }

    private void ResetScroll()
    {
        transform.position = startPosition;
    }
}

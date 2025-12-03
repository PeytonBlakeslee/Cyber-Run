using UnityEngine;

/// <summary>
/// Attach this to the ObstaclePassTrigger object (a BoxCollider2D with IsTrigger = true)
/// placed slightly behind the player. When an obstacle's collider crosses this trigger,
/// we count that obstacle as "dodged" exactly once.
/// </summary>
public class ObstaclePassCounter : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Find the ObstacleRoot on this collider or one of its parents.
        // This works even if the collider is on a child (e.g., one spike in a row).
        ObstacleRoot root = other.GetComponentInParent<ObstacleRoot>();
        if (root == null)
            return;

        // Make sure this really is an obstacle (root GameObject tagged "Obstacle")
        if (!root.CompareTag("Obstacle"))
            return;

        // Already counted this obstacle? Then skip.
        if (root.counted)
            return;

        // Mark as counted and notify GameManager that one obstacle was dodged.
        root.counted = true;

        if (GameManager.Instance != null)
            GameManager.Instance.RegisterObstacleDodged();
    }
}

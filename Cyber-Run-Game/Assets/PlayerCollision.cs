using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    private void Start()
    {
        // When the game starts, ensure the player is enabled
        GameManager.Instance.onPlay.AddListener(ActivatePlayer);
    }

    // Enable the player object (called when the game starts)
    private void ActivatePlayer()
    {
        gameObject.SetActive(true);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        // If we hit an obstacle: disable player and trigger Game Over
        if (other.transform.tag == "Obstacle")
        {
            gameObject.SetActive(false);
            GameManager.Instance.GameOver();
        }
    }
}

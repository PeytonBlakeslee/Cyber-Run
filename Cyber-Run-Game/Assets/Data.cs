using UnityEngine;

// Serializable container for persistent game data.
// Add fields here to track data that needs to be saved.
[System.Serializable]
public class Data
{
    // Highest score by difficulty (seconds survived / points)
    public float normalHighscore;
    public float hardHighscore;

    // Best streak = obstacles dodged in the highest scoring run (per difficulty)
    public int normalBestStreak;
    public int hardBestStreak;

    // Fastest effective speed reached across all runs
    public float fastestSpeedEver;

    // Lifetime stats
    public int totalObstaclesCleared; // all-time dodged obstacles
    public float totalScoreEver;      // sum of all scores
    public int totalRunsPlayed;       // how many runs started
}

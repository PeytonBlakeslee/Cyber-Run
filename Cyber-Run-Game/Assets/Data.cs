using UnityEngine;

// Serializable container for persistent game data.
// Add fields here to track data that needs to be saved.
[System.Serializable]
public class Data
{
    // Highest score achieved in Normal difficulty
    public float normalHighscore;

    // Highest score achieved in Hard difficulty
    public float hardHighscore;
}

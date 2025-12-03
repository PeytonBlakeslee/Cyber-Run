using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    // Global difficulty for the whole game.
    // Spawner + PlayerMovement + UI all read this.
    public enum DifficultyMode { Normal, Hard }

    #region Singleton
    public static GameManager Instance;

    private void Awake()
    {
        // basic singleton set
        if (Instance == null)
        {
            Instance = this;
            // Optional: persist across scenes
            // DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // load save data early so UI can read highscores/stats in Start()
        string loadedData = SaveSystem.Load("save");
        if (!string.IsNullOrEmpty(loadedData))
            data = JsonUtility.FromJson<Data>(loadedData);
        else
            data = new Data();
    }
    #endregion

    [Header("Runtime State")]
    public float currentScore = 0f;   // increases while playing
    public Data data;                 // persistent data (highscores + stats)
    public bool isPlaying = false;    // game loop active flag

    [Header("Difficulty")]
    public DifficultyMode difficulty = DifficultyMode.Normal; // default difficulty

    [Header("Events")]
    public UnityEvent onPlay = new UnityEvent();       // fired when game starts
    public UnityEvent onGameOver = new UnityEvent();   // fired on game over

    // Per-run stats (reset every StartGame)
    private int obstaclesDodgedThisRun = 0;
    private float maxSpeedThisRun = 0f;

    private void Update()
    {
        // accumulate score only while actively playing
        if (isPlaying)
            currentScore += Time.deltaTime;
    }

    // Called by UI to toggle between Normal and Hard before a run
    public void ToggleDifficulty()
    {
        difficulty = (difficulty == DifficultyMode.Normal)
            ? DifficultyMode.Hard
            : DifficultyMode.Normal;
    }

    // Called by UI Play button
    public void StartGame()
    {
        // notify listeners that a new run started
        onPlay.Invoke();

        // enable scoring and reset run state
        isPlaying = true;
        currentScore = 0f;

        // reset per-run stats
        obstaclesDodgedThisRun = 0;
        maxSpeedThisRun = 0f;

        // lifetime stat: track total runs
        data.totalRunsPlayed++;

        // switch to gameplay music
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopMusic();
            AudioManager.Instance.PlayGameMusic();
        }
    }

    // Called when the player dies
    public void GameOver()
    {
        // notify listeners that the run ended
        onGameOver.Invoke();

        // lifetime stat: add this run's score to total
        data.totalScoreEver += currentScore;

        // update fastest speed ever if this run beat it
        if (maxSpeedThisRun > data.fastestSpeedEver)
            data.fastestSpeedEver = maxSpeedThisRun;

        // update and save highscore / best streak for current difficulty
        float currentHigh = GetCurrentDifficultyHighscore();
        if (currentScore > currentHigh)
        {
            // new best score for this difficulty
            SetCurrentDifficultyHighscore(currentScore);

            // best streak = obstacles dodged in the highest scoring run
            SetCurrentDifficultyBestStreak(obstaclesDodgedThisRun);
        }

        // save everything
        string saveString = JsonUtility.ToJson(data);
        SaveSystem.Save("save", saveString);

        // immediately refresh start-menu highscores so they are correct
        UIManager ui = FindFirstObjectByType<UIManager>();
        if (ui != null)
            ui.RefreshStartMenuHighscores();

        // stop scoring and freeze gameplay
        isPlaying = false;

        // switch back to menu music
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopMusic();
            AudioManager.Instance.PlayMenuMusic();
        }
    }

    // Called whenever an obstacle successfully passes the player
    public void RegisterObstacleDodged()
    {
        obstaclesDodgedThisRun++;
        data.totalObstaclesCleared++;
    }

    // Called by Spawner / movement logic to sample the effective obstacle speed
    public void RegisterSpeedSample(float effectiveSpeed)
    {
        if (effectiveSpeed > maxSpeedThisRun)
            maxSpeedThisRun = effectiveSpeed;
    }

    // Helpers to get/set highscores based on current difficulty
    private float GetCurrentDifficultyHighscore()
    {
        return difficulty == DifficultyMode.Hard
            ? data.hardHighscore
            : data.normalHighscore;
    }

    private void SetCurrentDifficultyHighscore(float value)
    {
        if (difficulty == DifficultyMode.Hard)
            data.hardHighscore = value;
        else
            data.normalHighscore = value;
    }

    private void SetCurrentDifficultyBestStreak(int value)
    {
        if (difficulty == DifficultyMode.Hard)
            data.hardBestStreak = value;
        else
            data.normalBestStreak = value;
    }

    // Pretty-print helpers for UI

    // current run score as int string
    public string PrettyScore() =>
        Mathf.RoundToInt(currentScore).ToString();

    // current difficulty's best score
    public string PrettyHighscore() =>
        Mathf.RoundToInt(GetCurrentDifficultyHighscore()).ToString();

    // explicit per-mode highscores for the start menu
    public string PrettyNormalHighscore() =>
        Mathf.RoundToInt(data.normalHighscore).ToString();

    public string PrettyHardHighscore() =>
        Mathf.RoundToInt(data.hardHighscore).ToString();

    // per-mode best streaks
    public string PrettyNormalBestStreak() =>
        data.normalBestStreak.ToString();

    public string PrettyHardBestStreak() =>
        data.hardBestStreak.ToString();

    // global stats
    public string PrettyFastestSpeedEver() =>
        data.fastestSpeedEver.ToString("0.0");

    public string PrettyTotalObstaclesCleared() =>
        data.totalObstaclesCleared.ToString();

    public string PrettyTotalScoreEver() =>
        Mathf.RoundToInt(data.totalScoreEver).ToString();

    public string PrettyTotalRunsPlayed() =>
        data.totalRunsPlayed.ToString();
}

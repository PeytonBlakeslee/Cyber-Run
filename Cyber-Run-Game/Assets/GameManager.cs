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
            // Optional: keep this across scene loads
            // DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // load save data early so UI can read highscores in Start()
        string loadedData = SaveSystem.Load("save");
        if (!string.IsNullOrEmpty(loadedData))
            data = JsonUtility.FromJson<Data>(loadedData);
        else
            data = new Data();
    }
    #endregion

    [Header("Runtime State")]
    public float currentScore = 0f;         // increases while playing
    public Data data;                       // persistent data (e.g., highscores)
    public bool isPlaying = false;          // game loop active flag

    [Header("Difficulty")]
    public DifficultyMode difficulty = DifficultyMode.Normal; // current difficulty (Normal by default)

    [Header("Events")]
    public UnityEvent onPlay = new UnityEvent();       // fired when game starts
    public UnityEvent onGameOver = new UnityEvent();   // fired on game over 

    private void Update()
    {
        // accumulate score only while actively playing
        if (isPlaying)
            currentScore += Time.deltaTime;
    }

    // Called by UI before starting a run
    public void SetNormalDifficulty()
    {
        difficulty = DifficultyMode.Normal;
    }

    // Called by UI before starting a run
    public void SetHardDifficulty()
    {
        difficulty = DifficultyMode.Hard;
    }

    public void StartGame()
    {
        // notify listeners that a new run started
        onPlay.Invoke();

        // enable scoring and reset run score
        isPlaying = true;
        currentScore = 0f;

        // switch to in-game music
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopMusic();
            AudioManager.Instance.PlayGameMusic();
        }
    }

    public void GameOver()
    {
        // notify listeners that the run ended
        onGameOver.Invoke();

        // update and save highscore for the CURRENT difficulty if beaten
        float currentHigh = GetCurrentDifficultyHighscore();
        if (currentHigh < currentScore)
        {
            SetCurrentDifficultyHighscore(currentScore);
            string saveString = JsonUtility.ToJson(data);
            SaveSystem.Save("save", saveString);
        }

        // stop scoring and freeze gameplay
        isPlaying = false;

        // switch back to menu music
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopMusic();
            AudioManager.Instance.PlayMenuMusic();
        }
    }

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

    // helper display methods
    public string PrettyScore() => Mathf.RoundToInt(currentScore).ToString();

    // for generic "current mode" use
    public string PrettyHighscore() =>
        Mathf.RoundToInt(GetCurrentDifficultyHighscore()).ToString();

    // explicit per-mode highscores for the start menu
    public string PrettyNormalHighscore() =>
        Mathf.RoundToInt(data.normalHighscore).ToString();

    public string PrettyHardHighscore() =>
        Mathf.RoundToInt(data.hardHighscore).ToString();

    public void ToggleDifficulty()
    {
        difficulty = (difficulty == DifficultyMode.Normal)
            ? DifficultyMode.Hard
            : DifficultyMode.Normal;
    }
}

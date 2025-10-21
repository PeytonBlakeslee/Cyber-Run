using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager Instance;

    private void Awake()
    {
        // basic singleton set
        if (Instance == null) Instance = this;

        // load save data early so UI can read highscores in Start()
        string loadedData = SaveSystem.Load("save");
        if (!string.IsNullOrEmpty(loadedData))
            data = JsonUtility.FromJson<Data>(loadedData);
        else
            data = new Data();
    }
    #endregion

    [Header("Runtime State")]
    public float currentScore = 0f;   // increases while playing
    public Data data;                 // persistent data (e.g., highscore)
    public bool isPlaying = false;    // game loop active flag

    [Header("Events")]
    public UnityEvent onPlay = new UnityEvent();       // fired when game starts
    public UnityEvent onGameOver = new UnityEvent();   // fired on game over 

    private void Update()
    {
        // accumulate score only while actively playing
        if (isPlaying)
            currentScore += Time.deltaTime;
    }

    public void StartGame()
    {
        onPlay.Invoke();    // notify listeners
        isPlaying = true;   // enable scoring
        currentScore = 0f;  // reset run score
    }

    public void GameOver()
    {
        onGameOver.Invoke();  // first notify 

        // update and save highscore if beaten
        if (data.highscore < currentScore)
        {
            data.highscore = currentScore;
            string saveString = JsonUtility.ToJson(data);
            SaveSystem.Save("save", saveString);
        }

        isPlaying = false;    // stop scoring

    }

    // helper display methods
    public string PrettyScore() => Mathf.RoundToInt(currentScore).ToString();
    public string PrettyHighscore() => Mathf.RoundToInt(data.highscore).ToString();
}

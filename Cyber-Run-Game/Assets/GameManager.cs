using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager Instance;
    private void Awake()
    {
        if (Instance == null) Instance = this;

        // Load data EARLY so UI can read highscore on its Start()
        string loadedData = SaveSystem.Load("save");
        if (!string.IsNullOrEmpty(loadedData))
            data = JsonUtility.FromJson<Data>(loadedData);
        else
            data = new Data();
    }
    #endregion

    public float currentScore = 0f;
    public Data data;
    public bool isPlaying = false;

    public UnityEvent onPlay = new UnityEvent();
    public UnityEvent onGameOver = new UnityEvent();

    private void Update()
    {
        if (isPlaying)
            currentScore += Time.deltaTime;
    }

    public void StartGame()
    {
        onPlay.Invoke();
        isPlaying = true;
        currentScore = 0f;
    }

    public void GameOver()
    {
        onGameOver.Invoke();

        if (data.highscore < currentScore)
        {
            data.highscore = currentScore;
            string saveString = JsonUtility.ToJson(data);
            SaveSystem.Save("save", saveString);
        }

        isPlaying = false;

        // you had this twice; keeping your original behavior
        onGameOver.Invoke();
    }

    public string PrettyScore() => Mathf.RoundToInt(currentScore).ToString();
    public string PrettyHighscore() => Mathf.RoundToInt(data.highscore).ToString();
}

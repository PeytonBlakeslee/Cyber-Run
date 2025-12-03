using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject startMenuUI;          // main start menu root
    [SerializeField] private GameObject gameOverUI;           // game over panel root
    [SerializeField] private TextMeshProUGUI ScoreUI;         // in-game live score
    [SerializeField] private TextMeshProUGUI gameOverScoreUI; // score text on game over
    [SerializeField] private TextMeshProUGUI gameOverHighscoreUI; // highscore text on game over

    [Header("Start Menu Highscores")]
    [SerializeField] private TextMeshProUGUI normalHighscoreUI; // "Normal Highscore: X"
    [SerializeField] private TextMeshProUGUI hardHighscoreUI;   // "Hard Highscore: X"

    [Header("Difficulty UI")]
    [SerializeField] private TextMeshProUGUI difficultyLabelUI; // "Difficulty: Normal/Hard"

    [Header("Info Panel")]
    [SerializeField] private GameObject modeInfoPanel;          // panel explaining modes
    [SerializeField] private TextMeshProUGUI infoLabelUI;       // optional extra text (not required)

    [Header("Audio UI")]
    [SerializeField] private TextMeshProUGUI muteLabelUI;       // "Sound: On/Off"

    [Header("Stats Panel")]
    [SerializeField] private GameObject statsPanel;             // stats root panel
    [SerializeField] private TextMeshProUGUI statsNormalBestText;
    [SerializeField] private TextMeshProUGUI statsHardBestText;
    [SerializeField] private TextMeshProUGUI statsFastestSpeedText;
    [SerializeField] private TextMeshProUGUI statsTotalObstaclesText;
    [SerializeField] private TextMeshProUGUI statsTotalScoreText;
    [SerializeField] private TextMeshProUGUI statsTotalRunsText;

    private GameManager gm;

    private void Start()
    {
        gm = GameManager.Instance;

        // start menu music when scene loads
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayMenuMusic();

        // subscribe to game over event
        gm.onGameOver.AddListener(ActivateGameOverUI);

        // initial visibility
        if (startMenuUI) startMenuUI.SetActive(true);
        if (gameOverUI) gameOverUI.SetActive(false);
        if (ScoreUI) ScoreUI.gameObject.SetActive(false);
        if (statsPanel) statsPanel.SetActive(false);
        if (modeInfoPanel) modeInfoPanel.SetActive(false);

        // initial text
        RefreshStartMenuHighscores();
        UpdateDifficultyLabel();
        UpdateMuteLabel();
    }

    // Called by Play button
    public void PlayButtonHandler()
    {
        gm.StartGame();

        if (startMenuUI) startMenuUI.SetActive(false);
        if (gameOverUI) gameOverUI.SetActive(false);
        if (ScoreUI) ScoreUI.gameObject.SetActive(true);
        if (statsPanel) statsPanel.SetActive(false);
        if (modeInfoPanel) modeInfoPanel.SetActive(false);
    }

    // Called by Difficulty button
    public void DifficultyButtonHandler()
    {
        gm.ToggleDifficulty();
        UpdateDifficultyLabel();
    }

    private void UpdateDifficultyLabel()
    {
        if (!difficultyLabelUI) return;

        string mode =
            gm.difficulty == GameManager.DifficultyMode.Hard ? "Hard" : "Normal";

        difficultyLabelUI.text = "Difficulty: " + mode;
    }

    // Called via GameManager.onGameOver event
    public void ActivateGameOverUI()
    {
        if (gameOverUI) gameOverUI.SetActive(true);
        if (ScoreUI) ScoreUI.gameObject.SetActive(false);
        if (statsPanel) statsPanel.SetActive(false);

        if (gameOverScoreUI)
            gameOverScoreUI.text = "Score: " + gm.PrettyScore();

        if (gameOverHighscoreUI)
            gameOverHighscoreUI.text = "Highscore: " + gm.PrettyHighscore();

        // keep start menu labels in sync for when player returns
        RefreshStartMenuHighscores();
    }

    // Public so GameManager can force a refresh right after saving data
    public void RefreshStartMenuHighscores()
    {
        if (normalHighscoreUI)
            normalHighscoreUI.text = "Normal Highscore: " + gm.PrettyNormalHighscore();

        if (hardHighscoreUI)
            hardHighscoreUI.text = "Hard Highscore: " + gm.PrettyHardHighscore();
    }

    // Info "?" button
    public void ToggleModeInfoPanel()
    {
        if (!modeInfoPanel) return;
        modeInfoPanel.SetActive(!modeInfoPanel.activeSelf);
    }

    // Mute button
    public void MuteButtonHandler()
    {
        if (AudioManager.Instance == null) return;

        AudioManager.Instance.ToggleMute();
        UpdateMuteLabel();
    }

    private void UpdateMuteLabel()
    {
        if (!muteLabelUI) return;

        bool muted = AudioManager.Instance != null && AudioManager.Instance.IsMuted;
        muteLabelUI.text = muted ? "Sound: Off" : "Sound: On";
    }

    // Stats button on main menu
    public void StatsButtonHandler()
    {
        if (!statsPanel) return;

        statsPanel.SetActive(true);
        UpdateStatsPanel();
    }

    // Close button on stats panel
    public void CloseStatsPanel()
    {
        if (statsPanel)
            statsPanel.SetActive(false);
    }

    // Fill stats texts from GameManager / Data
    private void UpdateStatsPanel()
    {
        if (statsNormalBestText)
        {
            statsNormalBestText.text =
                $"Normal Best Run: {gm.PrettyNormalHighscore()}s  ({gm.PrettyNormalBestStreak()} dodged)";
        }

        if (statsHardBestText)
        {
            statsHardBestText.text =
                $"Hard Best Run: {gm.PrettyHardHighscore()}s  ({gm.PrettyHardBestStreak()} dodged)";
        }

        if (statsFastestSpeedText)
        {
            statsFastestSpeedText.text =
                $"Fastest Speed Reached: {gm.PrettyFastestSpeedEver()}x";
        }

        if (statsTotalObstaclesText)
        {
            statsTotalObstaclesText.text =
                $"Obstacles Cleared (All Time): {gm.PrettyTotalObstaclesCleared()}";
        }

        if (statsTotalScoreText)
        {
            statsTotalScoreText.text =
                $"Total Score (All Time): {gm.PrettyTotalScoreEver()}";
        }

        if (statsTotalRunsText)
        {
            statsTotalRunsText.text =
                $"Total Runs Played: {gm.PrettyTotalRunsPlayed()}";
        }
    }

    // Live in-game score label
    private void OnGUI()
    {
        if (ScoreUI && ScoreUI.gameObject.activeSelf)
            ScoreUI.text = gm.PrettyScore();
    }
}

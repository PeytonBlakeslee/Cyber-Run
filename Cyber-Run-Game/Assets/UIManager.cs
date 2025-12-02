using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject startMenuUI;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private TextMeshProUGUI ScoreUI;
    [SerializeField] private TextMeshProUGUI gameOverScoreUI;
    [SerializeField] private TextMeshProUGUI gameOverHighscoreUI;

    [Header("Start Menu Highscores")]
    [SerializeField] private TextMeshProUGUI normalHighscoreUI;
    [SerializeField] private TextMeshProUGUI hardHighscoreUI;

    [Header("Difficulty UI")]
    [SerializeField] private TextMeshProUGUI difficultyLabelUI;

    [Header("Info Panel")]
    [SerializeField] private GameObject modeInfoPanel;
    [SerializeField] private TextMeshProUGUI infoLabelUI;   // optional

    [Header("Audio UI")]
    [SerializeField] private TextMeshProUGUI muteLabelUI;

    [Header("Stats Panel")]
    [SerializeField] private GameObject statsPanel;
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

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayMenuMusic();

        gm.onGameOver.AddListener(ActivateGameOverUI);

        startMenuUI.SetActive(true);
        gameOverUI.SetActive(false);
        ScoreUI.gameObject.SetActive(false);
        if (statsPanel) statsPanel.SetActive(false);
        if (modeInfoPanel) modeInfoPanel.SetActive(false);

        RefreshStartMenuHighscores();
        UpdateDifficultyLabel();
        UpdateMuteLabel();
    }

    // PLAY button
    public void PlayButtonHandler()
    {
        gm.StartGame();

        startMenuUI.SetActive(false);
        gameOverUI.SetActive(false);
        ScoreUI.gameObject.SetActive(true);
        if (statsPanel) statsPanel.SetActive(false);
        if (modeInfoPanel) modeInfoPanel.SetActive(false);
    }

    // DIFFICULTY button
    public void DifficultyButtonHandler()
    {
        gm.ToggleDifficulty();
        UpdateDifficultyLabel();
    }

    private void UpdateDifficultyLabel()
    {
        if (!difficultyLabelUI) return;

        difficultyLabelUI.text =
            "Difficulty: " + (gm.difficulty == GameManager.DifficultyMode.Hard ? "Hard" : "Normal");
    }

    // GAME OVER UI
    public void ActivateGameOverUI()
    {
        gameOverUI.SetActive(true);
        ScoreUI.gameObject.SetActive(false);
        if (statsPanel) statsPanel.SetActive(false);

        gameOverScoreUI.text = "Score: " + gm.PrettyScore();
        gameOverHighscoreUI.text = "Highscore: " + gm.PrettyHighscore();

        RefreshStartMenuHighscores();
    }

    // Start menu highscores
    private void RefreshStartMenuHighscores()
    {
        if (normalHighscoreUI)
            normalHighscoreUI.text = "Normal Highscore: " + gm.PrettyNormalHighscore();

        if (hardHighscoreUI)
            hardHighscoreUI.text = "Hard Highscore: " + gm.PrettyHardHighscore();
    }

    // INFO button ("?")
    public void ToggleModeInfoPanel()
    {
        if (!modeInfoPanel) return;
        modeInfoPanel.SetActive(!modeInfoPanel.activeSelf);
    }

    // MUTE button
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

    // STATS button
    public void StatsButtonHandler()
    {
        statsPanel.SetActive(true);
        UpdateStatsPanel();
    }

    public void CloseStatsPanel()
    {
        statsPanel.SetActive(false);
    }

    // Fill stats panel
    private void UpdateStatsPanel()
    {
        statsNormalBestText.text =
            $"Normal Best Run: {gm.PrettyNormalHighscore()}s  ({gm.PrettyNormalBestStreak()} dodged)";

        statsHardBestText.text =
            $"Hard Best Run: {gm.PrettyHardHighscore()}s  ({gm.PrettyHardBestStreak()} dodged)";

        statsFastestSpeedText.text =
            $"Fastest Speed Reached: {gm.PrettyFastestSpeedEver()}x";

        statsTotalObstaclesText.text =
            $"Obstacles Cleared (All Time): {gm.PrettyTotalObstaclesCleared()}";

        statsTotalScoreText.text =
            $"Total Score (All Time): {gm.PrettyTotalScoreEver()}";

        statsTotalRunsText.text =
            $"Total Runs Played: {gm.PrettyTotalRunsPlayed()}";
    }

    // LIVE SCORE
    private void OnGUI()
    {
        if (ScoreUI.gameObject.activeSelf)
            ScoreUI.text = gm.PrettyScore();
    }
}

using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject startMenuUI;                 // start menu root
    [SerializeField] private GameObject gameOverUI;                  // game over root
    [SerializeField] private TextMeshProUGUI ScoreUI;                // in-game score text
    [SerializeField] private TextMeshProUGUI gameOverScoreUI;        // score label on game over
    [SerializeField] private TextMeshProUGUI gameOverHighscoreUI;    // highscore label on game over

    [Header("Start Menu Highscores")]
    [SerializeField] private TextMeshProUGUI normalHighscoreUI;      // e.g. "Normal Highscore: 0"
    [SerializeField] private TextMeshProUGUI hardHighscoreUI;        // e.g. "Hard Highscore: 0"

    [Header("Difficulty UI")]
    [SerializeField] private TextMeshProUGUI difficultyLabelUI;      // label on the difficulty button

    [Header("Info Panel")]
    [SerializeField] private GameObject modeInfoPanel;   // panel that explains Normal vs Hard

    [Header("Audio UI")]
    [SerializeField] private TextMeshProUGUI muteLabelUI;  // label on the mute button



    private GameManager gm;

    private void Start()
    {
        gm = GameManager.Instance;

        // start menu music when the scene loads (if you wired AudioManager)
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayMenuMusic();

        // update game over screen when the game ends
        gm.onGameOver.AddListener(ActivateGameOverUI);

        // initial UI state: show start menu, hide gameplay + game over
        if (startMenuUI) startMenuUI.SetActive(true);
        if (ScoreUI) ScoreUI.gameObject.SetActive(false);
        if (gameOverUI) gameOverUI.SetActive(false);

        // fill start menu labels
        RefreshStartMenuHighscores();
        UpdateDifficultyLabel();
        UpdateMuteLabel();

    }

    // Called by the Play button
    public void PlayButtonHandler()
    {
        gm.StartGame();

        // switch to gameplay HUD
        if (startMenuUI) startMenuUI.SetActive(false);
        if (ScoreUI) ScoreUI.gameObject.SetActive(true);
        if (gameOverUI) gameOverUI.SetActive(false);
    }

    // Called by the Difficulty button
    public void DifficultyButtonHandler()
    {
        gm.ToggleDifficulty();
        UpdateDifficultyLabel();
        // highscores shown are per-mode, but we display both,
        // so no need to change those here.
    }

    private void UpdateDifficultyLabel()
    {
        if (!difficultyLabelUI) return;

        string modeText = gm.difficulty == GameManager.DifficultyMode.Hard
            ? "Hard"
            : "Normal";

        difficultyLabelUI.text = "Difficulty: " + modeText;
    }

    // Show both highscores on the start menu
    private void RefreshStartMenuHighscores()
    {
        if (normalHighscoreUI)
            normalHighscoreUI.text = "Normal Highscore: " + gm.PrettyNormalHighscore();

        if (hardHighscoreUI)
            hardHighscoreUI.text = "Hard Highscore: " + gm.PrettyHardHighscore();
    }

    // Show game over UI and fill in score labels
    public void ActivateGameOverUI()
    {
        if (gameOverUI) gameOverUI.SetActive(true);
        if (ScoreUI) ScoreUI.gameObject.SetActive(false);

        if (gameOverScoreUI)
            gameOverScoreUI.text = "Score: " + gm.PrettyScore();

        if (gameOverHighscoreUI)
            gameOverHighscoreUI.text = "Highscore: " + gm.PrettyHighscore();

        // update start menu highscores so when you go back they are fresh
        RefreshStartMenuHighscores();
    }

    // Update the live score text only while the score UI is visible
    private void OnGUI()
    {
        if (ScoreUI && ScoreUI.gameObject.activeSelf)
            ScoreUI.text = gm.PrettyScore();
    }

    // Called by the small "?" button to show / hide the info panel
    public void ToggleModeInfoPanel()
    {
    if (!modeInfoPanel) return;
    bool newState = !modeInfoPanel.activeSelf;
    modeInfoPanel.SetActive(newState);
    }

    // Called by the mute button
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


}

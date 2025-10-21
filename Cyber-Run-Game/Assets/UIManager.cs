using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject startMenuUI;                 // start menu root
    [SerializeField] private GameObject gameOverUI;                  // game over root
    [SerializeField] private TextMeshProUGUI ScoreUI;                // in-game score text
    [SerializeField] private TextMeshProUGUI startMenuHighscoreUI;   // highscore text under title (start menu)
    [SerializeField] private TextMeshProUGUI gameOverScoreUI;        // score label on game over
    [SerializeField] private TextMeshProUGUI gameOverHighscoreUI;    // highscore label on game over

    private GameManager gm;

    private void Start()
    {
        gm = GameManager.Instance;

        // update game over screen when the game ends
        gm.onGameOver.AddListener(ActivateGameOverUI);

        // initial UI state: show start menu, hide gameplay + game over
        if (startMenuUI) startMenuUI.SetActive(true);
        if (ScoreUI) ScoreUI.gameObject.SetActive(false);
        if (gameOverUI) gameOverUI.SetActive(false);

        // populate start menu highscore once at launch
        if (startMenuHighscoreUI)
            startMenuHighscoreUI.text = "Highscore: " + gm.PrettyHighscore();
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

    // Show game over UI and fill in score labels
    public void ActivateGameOverUI()
    {
        if (gameOverUI) gameOverUI.SetActive(true);
        if (ScoreUI) ScoreUI.gameObject.SetActive(false);

        gameOverScoreUI.text = "Score: " + gm.PrettyScore();
        gameOverHighscoreUI.text = "Highscore: " + gm.PrettyHighscore();
    }

    // Utility hook for a "Back to Menu" button to refresh the start menu highscore label
    public void RefreshStartMenuHighscore()
    {
        if (startMenuHighscoreUI)
            startMenuHighscoreUI.text = "Highscore: " + gm.PrettyHighscore();
    }

    // Update the live score text only while the score UI is visible
    private void OnGUI()
    {
        if (ScoreUI && ScoreUI.gameObject.activeSelf)
            ScoreUI.text = gm.PrettyScore();
    }
}

using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ScoreUI;
    [SerializeField] private GameObject startMenuUI;
    [SerializeField] private GameObject gameOverUI;

    [SerializeField] private TextMeshProUGUI startMenuHighscoreUI; // NEW
    [SerializeField] private TextMeshProUGUI gameOverScoreUI;
    [SerializeField] private TextMeshProUGUI gameOverHighscoreUI;

    private GameManager gm;

    private void Start()
    {
        gm = GameManager.Instance;
        gm.onGameOver.AddListener(ActivateGameOverUI);

        // ensure start menu shows, others hidden (optional but helpful)
        if (startMenuUI) startMenuUI.SetActive(true);
        if (ScoreUI) ScoreUI.gameObject.SetActive(false);
        if (gameOverUI) gameOverUI.SetActive(false);

        // show saved highscore under the title
        if (startMenuHighscoreUI)
            startMenuHighscoreUI.text = "Highscore: " + gm.PrettyHighscore();
    }

    public void PlayButtonHandler()
    {
        gm.StartGame();

        // basic toggle when starting
        if (startMenuUI) startMenuUI.SetActive(false);
        if (ScoreUI) ScoreUI.gameObject.SetActive(true);
        if (gameOverUI) gameOverUI.SetActive(false);
    }

    public void ActivateGameOverUI()
    {
        if (gameOverUI) gameOverUI.SetActive(true);
        if (ScoreUI) ScoreUI.gameObject.SetActive(false);

        gameOverScoreUI.text = "Score: " + gm.PrettyScore();
        gameOverHighscoreUI.text = "Highscore: " + gm.PrettyHighscore();
    }
    public void RefreshStartMenuHighscore()
    {
        if (startMenuHighscoreUI != null)
            startMenuHighscoreUI.text = "Highscore: " + gm.PrettyHighscore();
    }

    private void OnGUI()
    {
        if (ScoreUI && ScoreUI.gameObject.activeSelf)
            ScoreUI.text = gm.PrettyScore();
    }
}


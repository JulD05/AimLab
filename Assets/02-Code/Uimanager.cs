using UnityEngine;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject exitButton;
    [SerializeField] private GameObject optionButton;

    [Header("Panels")]
    [SerializeField] private GameObject difficultyPanel;

    [Header("Gameplay")]
    [SerializeField] private MouseLook mouseLook;
    [SerializeField] private TargetSpawner targetSpawner;
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private GameTimer gameTimer;
    [SerializeField] private GameObject[] gameplayUI;

    private RoundSummaryUI roundSummaryUI;
    private PauseMenuUI pauseMenuUI;
    private LeaderboardDatabase leaderboardDatabase;
    private LeaderboardUI leaderboardUI;
    private bool gameplayActive;
    private bool pauseActive;

    void Awake()
    {
        if (!HasAnyMenuReference())
        {
            enabled = false;
            return;
        }

        roundSummaryUI = FindObjectOfType<RoundSummaryUI>();
        if (roundSummaryUI == null)
        {
            GameObject summaryObject = new GameObject("Summary");
            roundSummaryUI = summaryObject.AddComponent<RoundSummaryUI>();
        }

        roundSummaryUI.Initialize(this);

        pauseMenuUI = FindObjectOfType<PauseMenuUI>();
        if (pauseMenuUI == null)
        {
            GameObject pauseObject = new GameObject("PauseMenu");
            pauseMenuUI = pauseObject.AddComponent<PauseMenuUI>();
        }

        pauseMenuUI.Initialize(this);

        leaderboardDatabase = LeaderboardDatabase.LoadRuntimeDatabase();
        leaderboardUI = FindObjectOfType<LeaderboardUI>();
        if (leaderboardUI == null)
        {
            GameObject leaderboardObject = new GameObject("LeaderboardUI");
            leaderboardUI = leaderboardObject.AddComponent<LeaderboardUI>();
        }

        leaderboardUI.Initialize(this, leaderboardDatabase);
    }

    // Etat initial : Play + Exit, pas d'Option, pas de difficulté
    void Start()
    {
        ResetToMain();
    }

    void Update()
    {
        if (Keyboard.current == null) return;
        if (!Keyboard.current.escapeKey.wasPressedThisFrame) return;
        if (!gameplayActive) return;

        if (pauseActive)
        {
            ResumeGame();
            return;
        }

        PauseGame();
    }

    // Play : cache Play, montre Option
    public void OnPlayClicked()
    {
        leaderboardUI?.HideAll();
        SetMainMenuButtonsVisible(false);
        if (playButton != null) playButton.SetActive(false);
        if (exitButton != null) exitButton.SetActive(true);
        if (optionButton != null) optionButton.SetActive(true);
        if (difficultyPanel != null) difficultyPanel.SetActive(false);
        SetGameplayUIVisible(false);
        mouseLook?.SetCursorLockedState(false);
    }

    // Option : toggle difficulté
    public void OnOptionClicked()
    {
        if (difficultyPanel == null) return;
        difficultyPanel.SetActive(!difficultyPanel.activeSelf);
    }

    // Quand une difficulté est choisie : on cache tout l'UI
    public void HideAllUI()
    {
        if (playButton != null) playButton.SetActive(false);
        if (exitButton != null) exitButton.SetActive(false);
        if (optionButton != null) optionButton.SetActive(false);
        if (difficultyPanel != null) difficultyPanel.SetActive(false);

        gameplayActive = true;
        pauseActive = false;
        Time.timeScale = 1f;
        leaderboardUI?.HideAll();
        pauseMenuUI?.HidePause();
        scoreManager?.ResetScore();
        SetGameplayUIVisible(true);
        mouseLook?.SetCursorLockedState(true);
    }

    // Exit : revient à l'état initial
    public void ResetToMain()
    {
        gameplayActive = false;
        pauseActive = false;
        Time.timeScale = 1f;
        targetSpawner?.StopGame();
        roundSummaryUI?.HideSummary();
        leaderboardUI?.HideAll();
        leaderboardUI?.SetHomeButtonVisible(true);
        pauseMenuUI?.HidePause();
        if (playButton != null) playButton.SetActive(true);
        if (exitButton != null) exitButton.SetActive(true);
        if (optionButton != null) optionButton.SetActive(false);
        if (difficultyPanel != null) difficultyPanel.SetActive(false);
        scoreManager?.ResetScore();
        gameTimer?.ResetTimerDisplay();
        SetGameplayUIVisible(false);
        mouseLook?.SetCursorLockedState(false);
    }

    // Bouton Exit
    public void OnExitClicked()
    {
        ResetToMain();
    }

    public void ShowSummary(int targetsKilled, int totalShots, int missedTargets, float roundDuration)
    {
        GameDifficulty finishedDifficulty = targetSpawner != null ? targetSpawner.CurrentDifficulty : GameDifficulty.None;
        int finalScore = scoreManager != null ? scoreManager.CurrentScore : targetsKilled;

        gameplayActive = false;
        pauseActive = false;
        Time.timeScale = 1f;
        targetSpawner?.StopGame();
        leaderboardUI?.HideAll();
        pauseMenuUI?.HidePause();
        SetGameplayUIVisible(false);
        mouseLook?.SetCursorLockedState(false);
        roundSummaryUI?.ShowSummary(targetsKilled, totalShots, missedTargets, roundDuration);
        leaderboardUI?.TryPromptForScore(finishedDifficulty, finalScore);
    }

    public void OnSummaryExitClicked()
    {
        ResetToMain();
    }

    public void OnPauseResumeClicked()
    {
        ResumeGame();
    }

    public void OnPauseExitClicked()
    {
        ResetToMain();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void SetMainMenuButtonsVisible(bool visible)
    {
        if (playButton != null) playButton.SetActive(visible);
        if (exitButton != null) exitButton.SetActive(visible);

        if (optionButton != null)
            optionButton.SetActive(false);

        if (difficultyPanel != null)
            difficultyPanel.SetActive(false);
    }

    void SetGameplayUIVisible(bool visible)
    {
        if (gameplayUI == null) return;

        foreach (GameObject uiElement in gameplayUI)
        {
            if (uiElement != null)
                uiElement.SetActive(visible);
        }
    }

    bool HasAnyMenuReference()
    {
        return playButton != null
            || exitButton != null
            || optionButton != null
            || difficultyPanel != null;
    }

    void PauseGame()
    {
        if (pauseActive) return;
        if (gameTimer != null && !gameTimer.IsRoundRunning) return;

        pauseActive = true;
        Time.timeScale = 0f;
        gameTimer?.PauseTimer();
        mouseLook?.SetCursorLockedState(false);
        pauseMenuUI?.ShowPause();
    }

    void ResumeGame()
    {
        if (!pauseActive) return;

        pauseActive = false;
        Time.timeScale = 1f;
        gameTimer?.ResumeTimer();
        pauseMenuUI?.HidePause();
        mouseLook?.SetCursorLockedState(true);
    }
}

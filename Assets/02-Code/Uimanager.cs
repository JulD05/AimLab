using UnityEngine;

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
    [SerializeField] private GameObject[] gameplayUI;

    // Etat initial : Play + Exit, pas d'Option, pas de difficulté
    void Start()
    {
        ResetToMain();
    }

    // Play : cache Play, montre Option
    public void OnPlayClicked()
    {
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

        scoreManager?.ResetScore();
        SetGameplayUIVisible(true);
        mouseLook?.SetCursorLockedState(true);
    }

    // Exit : revient à l'état initial
    public void ResetToMain()
    {
        targetSpawner?.StopGame();
        if (playButton != null) playButton.SetActive(true);
        if (exitButton != null) exitButton.SetActive(true);
        if (optionButton != null) optionButton.SetActive(false);
        if (difficultyPanel != null) difficultyPanel.SetActive(false);
        scoreManager?.ResetScore();
        SetGameplayUIVisible(false);
        mouseLook?.SetCursorLockedState(false);
    }

    // Bouton Exit
    public void OnExitClicked()
    {
        ResetToMain();
    }

    public void QuitGame()
    {
        Application.Quit();
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
}

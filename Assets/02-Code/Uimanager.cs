using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("Buttons")]
    public GameObject playButton;
    public GameObject exitButton;
    public GameObject optionButton;

    [Header("Panels")]
    public GameObject difficultyPanel;

    // Play : cache Play, montre Option
    public void OnPlayClicked()
    {
        if (playButton != null) playButton.SetActive(false);
        if (optionButton != null) optionButton.SetActive(true);
    }

    // Exit : état initial (Play + Exit), cache Option + Difficulty
    public void OnExitClicked()
    {
        if (playButton != null) playButton.SetActive(true);
        if (exitButton != null) exitButton.SetActive(true);
        if (optionButton != null) optionButton.SetActive(false);
        if (difficultyPanel != null) difficultyPanel.SetActive(false);
    }

    // Option : toggle difficulté
    public void OnOptionClicked()
    {
        if (difficultyPanel == null) return;
        difficultyPanel.SetActive(!difficultyPanel.activeSelf);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
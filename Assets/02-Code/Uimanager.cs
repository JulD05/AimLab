using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject exitButton;
    [SerializeField] private GameObject optionButton;

    [Header("Panels")]
    [SerializeField] private GameObject difficultyPanel;

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
    }

    // Exit : revient à l'état initial
    public void ResetToMain()
    {
        if (playButton != null) playButton.SetActive(true);
        if (exitButton != null) exitButton.SetActive(true);
        if (optionButton != null) optionButton.SetActive(false);
        if (difficultyPanel != null) difficultyPanel.SetActive(false);
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
}
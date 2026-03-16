using TMPro;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private float easyDuration = 10f;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private TargetSpawner targetSpawner;
    [SerializeField] private RaycastShooter raycastShooter;

    private float remainingTime;
    private float currentRoundDuration;
    private bool isRunning;
    private bool isPaused;

    public bool IsRoundRunning => isRunning;
    public bool IsPaused => isPaused;

    void Awake()
    {
        if (raycastShooter == null && Camera.main != null)
            raycastShooter = Camera.main.GetComponent<RaycastShooter>();

        ResetTimerDisplay();
    }

    void Update()
    {
        if (isPaused) return;
        if (!isRunning) return;

        remainingTime -= Time.deltaTime;
        if (remainingTime <= 0f)
        {
            remainingTime = 0f;
            isRunning = false;
            UpdateTimerDisplay();
            targetSpawner?.StopGame();
            uiManager?.ShowSummary(
                raycastShooter != null ? raycastShooter.SuccessfulShots : 0,
                raycastShooter != null ? raycastShooter.TotalShots : 0,
                targetSpawner != null ? targetSpawner.MissedTargets : 0,
                currentRoundDuration
            );
            return;
        }

        UpdateTimerDisplay();
    }

    public void StartEasyRound()
    {
        raycastShooter?.ResetRoundStats();
        isPaused = false;

        if (timerText != null)
            timerText.gameObject.SetActive(true);

        currentRoundDuration = easyDuration;
        remainingTime = currentRoundDuration;
        isRunning = true;
        UpdateTimerDisplay();
    }

    public void ResetTimerDisplay()
    {
        isRunning = false;
        isPaused = false;
        remainingTime = easyDuration;
        currentRoundDuration = 0f;
        UpdateTimerDisplay();

        if (timerText != null)
            timerText.gameObject.SetActive(false);
    }

    void UpdateTimerDisplay()
    {
        if (timerText == null) return;
        timerText.text = "Temps: " + Mathf.CeilToInt(remainingTime);
    }

    public void PauseTimer()
    {
        if (!isRunning) return;
        isPaused = true;
    }

    public void ResumeTimer()
    {
        if (!isRunning) return;
        isPaused = false;
    }
}

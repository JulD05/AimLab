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
    private bool isRunning;

    void Awake()
    {
        if (raycastShooter == null && Camera.main != null)
            raycastShooter = Camera.main.GetComponent<RaycastShooter>();

        ResetTimerDisplay();
    }

    void Update()
    {
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
                easyDuration
            );
            return;
        }

        UpdateTimerDisplay();
    }

    public void StartEasyRound()
    {
        raycastShooter?.ResetRoundStats();

        if (timerText != null)
            timerText.gameObject.SetActive(true);

        remainingTime = easyDuration;
        isRunning = true;
        UpdateTimerDisplay();
    }

    public void ResetTimerDisplay()
    {
        isRunning = false;
        remainingTime = easyDuration;
        UpdateTimerDisplay();

        if (timerText != null)
            timerText.gameObject.SetActive(false);
    }

    void UpdateTimerDisplay()
    {
        if (timerText == null) return;
        timerText.text = "Temps: " + Mathf.CeilToInt(remainingTime);
    }
}

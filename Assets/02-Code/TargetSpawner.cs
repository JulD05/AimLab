using UnityEngine;

public class TargetSpawner : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] private GameObject targetPrefab;

    [Header("Alignement UI")]
    [SerializeField] private RectTransform referenceButton;
    [SerializeField] private Camera uiCamera;               

    [Header("Caméra 3D")]
    [SerializeField] private Camera worldCamera;
    [SerializeField] private float distance = 5f;

    [Header("Spawn")]
    [Range(0f, 0.4f)]
    [SerializeField] private float marginX = 0.1f;
    [SerializeField] private float mediumTargetLifetime = 2f;

    private GameObject current;
    private bool gameStarted = false;
    private bool timedTargetMode;
    private float currentTargetLifetime;
    private float remainingTargetLifetime;
    private int missedTargets;

    public int MissedTargets => missedTargets;

    void Awake()
    {
        if (worldCamera == null) worldCamera = Camera.main;
    }

    void Update()
    {
        if (!gameStarted || !timedTargetMode || current == null) return;

        remainingTargetLifetime -= Time.deltaTime;
        if (remainingTargetLifetime > 0f) return;

        missedTargets++;
        Destroy(current);
        current = null;
        Spawn();
    }

   
    public void StartGame()
    {
        StartEasyMode();
    }

    public void StartEasyMode()
    {
        gameStarted = true;
        timedTargetMode = false;
        currentTargetLifetime = 0f;
        remainingTargetLifetime = 0f;
        missedTargets = 0;
        Respawn();
    }

    public void StartMediumMode()
    {
        gameStarted = true;
        timedTargetMode = true;
        currentTargetLifetime = mediumTargetLifetime;
        remainingTargetLifetime = currentTargetLifetime;
        missedTargets = 0;
        Respawn();
    }

  
    public void StopGame()
    {
        gameStarted = false;
        timedTargetMode = false;
        currentTargetLifetime = 0f;
        remainingTargetLifetime = 0f;

        if (current != null)
        {
            Destroy(current);
            current = null;
        }
    }

    public void Spawn()
    {
        if (!gameStarted) return;     
        if (current != null) return;

        if (targetPrefab == null || referenceButton == null || worldCamera == null)
        {
            Debug.LogError("TargetSpawner: assigne targetPrefab + referenceButton + worldCamera");
            return;
        }

        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(uiCamera, referenceButton.position);
        float vy = Mathf.Clamp01(screenPos.y / Screen.height);

        float vx = Random.Range(marginX, 1f - marginX);

        Vector3 worldPos = worldCamera.ViewportToWorldPoint(new Vector3(vx, vy, distance));
        current = Instantiate(targetPrefab, worldPos, Quaternion.identity);

        if (timedTargetMode)
            remainingTargetLifetime = currentTargetLifetime;
    }

    public void Respawn()
    {
        if (!gameStarted) return;

        if (current != null) Destroy(current);
        current = null;
        Spawn();
    }
}

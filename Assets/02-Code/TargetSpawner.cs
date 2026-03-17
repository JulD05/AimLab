using UnityEngine;

public class TargetSpawner : MonoBehaviour
{
    [SerializeField] private AudioSource radioSource;
    [Header("Audio")]
    [SerializeField] private AudioSource menuMusic;
    [Header("Prefab")]
    [SerializeField] private GameObject targetPrefab;

    [Header("Zone de Spawn")]
    [SerializeField] private Vector3 spawnCenter = new Vector3(0f, 2.2f, 4f);
    [SerializeField] private Vector2 spawnSize = new Vector2(12f, 3f);
    [SerializeField] private int maxSpawnAttempts = 10;

    [Header("Collision")]
    [SerializeField] private LayerMask obstructionMask;
    [SerializeField] private float sphereRadius = 0.5f;

    [Header("Mode Moyen")]
    [SerializeField] private float mediumTargetLifetime = 2f;
    [SerializeField] private Vector2 mediumDepthOffsetRange = new Vector2(-4f, 2f);
    [SerializeField] private Vector2 mediumScaleRange = new Vector2(0.55f, 1f);

    [Header("Mode Difficile")]
    [SerializeField] private float hardMoveSpeed = 6f;

    private GameObject current;
    private bool gameStarted;
    private bool timedTargetMode;
    private bool movingTargetMode;
    private float remainingTargetLifetime;
    private float currentMoveTargetX;
    private int missedTargets;
    private GameDifficulty currentDifficulty = GameDifficulty.None;

    public int MissedTargets => missedTargets;
    public GameDifficulty CurrentDifficulty => currentDifficulty;

    void Update()
    {
        if (!gameStarted) return;

        if (movingTargetMode && current != null)
            MoveCurrentTarget();

        if (!timedTargetMode || current == null) return;

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
        currentDifficulty = GameDifficulty.Easy;
        BeginGame(false, false);
    }

    public void StartMediumMode()
    {
        currentDifficulty = GameDifficulty.Medium;
        BeginGame(true, false);
    }

    public void StartDifficultMode()
    {
        currentDifficulty = GameDifficulty.Difficult;
        BeginGame(true, true);
    }

    void BeginGame(bool enableTimedTargets, bool enableMovingTargets)
    {
        if (menuMusic != null) 
        {
            menuMusic.Stop();
        }
        if (radioSource != null)
        {
            radioSource.Play();
        }
        missedTargets = 0;
        gameStarted = true;
        timedTargetMode = enableTimedTargets;
        movingTargetMode = enableMovingTargets;
        remainingTargetLifetime = 0f;
        currentMoveTargetX = 0f;
        Respawn();
    }

    public void StopGame()
    {
        if (menuMusic != null) 
        {
            menuMusic.Play(); // La musique revient au menu !
        }
        gameStarted = false;
        timedTargetMode = false;
        movingTargetMode = false;
        remainingTargetLifetime = 0f;
        currentMoveTargetX = 0f;
        currentDifficulty = GameDifficulty.None;

        if (current != null)
        {
            Destroy(current);
            current = null;
        }
    }

    public void ResetStats()
    {
        missedTargets = 0;
    }

    public void Spawn()
    {
        if (!gameStarted || current != null || targetPrefab == null) return;

        int attempts = Mathf.Max(1, maxSpawnAttempts);
        for (int i = 0; i < attempts; i++)
        {
            float targetScale = GetRandomTargetScale();
            Vector3 spawnPosition = GetRandomSpawnPosition(targetScale);

            if (obstructionMask.value != 0 && Physics.CheckSphere(spawnPosition, sphereRadius * targetScale, obstructionMask))
                continue;

            current = Instantiate(targetPrefab, spawnPosition, Quaternion.identity);
            current.transform.localScale = Vector3.one * targetScale;

            if (timedTargetMode)
                remainingTargetLifetime = mediumTargetLifetime;

            return;
        }

        Debug.LogWarning("TargetSpawner: no valid spawn position found in the area.");
    }

    Vector3 GetRandomSpawnPosition(float targetScale)
    {
        float minX = spawnCenter.x - spawnSize.x * 0.5f + sphereRadius * targetScale;
        float maxX = spawnCenter.x + spawnSize.x * 0.5f - sphereRadius * targetScale;
        float randomX = Random.Range(minX, maxX);
        float randomY = Random.Range(-spawnSize.y * 0.5f, spawnSize.y * 0.5f);
        float randomZ = 0f;

        if (timedTargetMode)
            randomZ = Random.Range(mediumDepthOffsetRange.x, mediumDepthOffsetRange.y);

        if (movingTargetMode)
        {
            bool leftToRight = Random.value < 0.5f;
            randomX = leftToRight ? minX : maxX;
            currentMoveTargetX = leftToRight ? maxX : minX;
        }
        else
        {
            currentMoveTargetX = randomX;
        }

        return spawnCenter + new Vector3(randomX - spawnCenter.x, randomY, randomZ);
    }

    float GetRandomTargetScale()
    {
        if (!timedTargetMode) return 1f;
        return Random.Range(mediumScaleRange.x, mediumScaleRange.y);
    }

    void MoveCurrentTarget()
    {
        Vector3 currentPosition = current.transform.position;
        currentPosition.x = Mathf.MoveTowards(currentPosition.x, currentMoveTargetX, hardMoveSpeed * Time.deltaTime);
        current.transform.position = currentPosition;
    }

    public void Respawn()
    {
        if (!gameStarted) return;

        if (current != null)
            Destroy(current);

        current = null;
        Spawn();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(spawnCenter, new Vector3(spawnSize.x, spawnSize.y, 0.2f));

        Gizmos.color = Color.yellow;
        Vector3 mediumCenter = spawnCenter + new Vector3(0f, 0f, (mediumDepthOffsetRange.x + mediumDepthOffsetRange.y) * 0.5f);
        float mediumDepth = Mathf.Abs(mediumDepthOffsetRange.y - mediumDepthOffsetRange.x);
        Gizmos.DrawWireCube(mediumCenter, new Vector3(spawnSize.x, spawnSize.y, mediumDepth));
    }
}

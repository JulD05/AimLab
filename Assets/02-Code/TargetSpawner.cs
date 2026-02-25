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

    private GameObject current;
    private bool gameStarted = false; 

    void Awake()
    {
        if (worldCamera == null) worldCamera = Camera.main;
    }

    void Start()
    {
        // plus de Spawn() ici
        // Spawn uniquement quand une difficulté est choisie
    }

   
    public void StartGame()
    {
        gameStarted = true;
        Respawn(); 
    }

  
    public void StopGame()
    {
        gameStarted = false;

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
    }

    public void Respawn()
    {
        if (!gameStarted) return;

        if (current != null) Destroy(current);
        current = null;
        Spawn();
    }
}
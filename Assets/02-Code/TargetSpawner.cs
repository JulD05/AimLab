using UnityEngine;

public class TargetSpawner : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] private GameObject targetPrefab;

    [Header("Alignement UI")]
    [SerializeField] private RectTransform referenceButton; // ex: Play ou Options
    [SerializeField] private Camera uiCamera;               // souvent null si Canvas = Screen Space Overlay

    [Header("Caméra 3D")]
    [SerializeField] private Camera worldCamera;
    [SerializeField] private float distance = 5f;

    [Header("Spawn")]
    [Range(0f, 0.4f)]
    [SerializeField] private float marginX = 0.1f; // évite les bords

    private GameObject current;

    void Awake()
    {
        if (worldCamera == null) worldCamera = Camera.main;
    }

    void Start()
    {
        Spawn();
    }

    public void Spawn()
    {
        if (current != null) return;

        if (targetPrefab == null || referenceButton == null || worldCamera == null)
        {
            Debug.LogError("TargetSpawner: assigne targetPrefab + referenceButton + worldCamera");
            return;
        }

        // Récupère la hauteur écran du bouton
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(uiCamera, referenceButton.position);
        float vy = Mathf.Clamp01(screenPos.y / Screen.height);

        // X random, Y fixé au niveau du bouton
        float vx = Random.Range(marginX, 1f - marginX);

        Vector3 worldPos = worldCamera.ViewportToWorldPoint(new Vector3(vx, vy, distance));
        current = Instantiate(targetPrefab, worldPos, Quaternion.identity);
    }

    public void Respawn()
    {
        if (current != null) Destroy(current);
        current = null;
        Spawn();
    }
}
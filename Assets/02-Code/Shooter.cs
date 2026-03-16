using UnityEngine;
using UnityEngine.InputSystem;

public class Shooter : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private TargetSpawner spawner;
    [SerializeField] private ScoreManager scoreManager;

    [Header("VFX / SFX")]
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private AudioClip explosionSound;
    [Range(0f, 1f)] [SerializeField] private float explosionVolume = 0.8f;

    [Header("Hit only this layer")]
    [SerializeField] private LayerMask targetLayer; 

    void Awake()
    {
        if (cam == null) cam = Camera.main;
    }

    void Update()
    {
        if (Mouse.current == null) return;
        if (!Mouse.current.leftButton.wasPressedThisFrame) return;

    
       Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, targetLayer))
        {
            if (hit.collider.GetComponent<Target>() == null) return;

            if (explosionPrefab != null)
            {
                var fx = Instantiate(explosionPrefab, hit.point, Quaternion.identity);
                Destroy(fx, 1f);
            }

            if (explosionSound != null)
                AudioSource.PlayClipAtPoint(explosionSound, hit.point, explosionVolume);

            if (scoreManager != null) scoreManager.AddPoint();

            Destroy(hit.collider.gameObject);
            if (spawner != null) spawner.Respawn();
        }
    }
}
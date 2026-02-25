using UnityEngine;

public class Shooter : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private TargetSpawner spawner;
    [SerializeField] private ScoreManager scoreManager;

    [Header("VFX / SFX")]
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private AudioClip explosionSound;
    [Range(0f, 1f)]
    [SerializeField] private float explosionVolume = 0.8f;

    void Awake()
    {
        if (cam == null) cam = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.GetComponent<Target>() != null)
                {
                    // VFX
                    if (explosionPrefab != null)
                    {
                        GameObject explosion = Instantiate(explosionPrefab, hit.point, Quaternion.identity);
                        Destroy(explosion, 1f);
                    }

                    // SFX
                    if (explosionSound != null)
                    {
                        AudioSource.PlayClipAtPoint(explosionSound, hit.point, explosionVolume);
                    }

                    // Score
                    if (scoreManager != null)
                        scoreManager.AddPoint();

                    Destroy(hit.collider.gameObject);
                    spawner.Respawn();
                }
            }
        }
    }
}
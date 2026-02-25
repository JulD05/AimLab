using UnityEngine;

public class Shooter : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private TargetSpawner spawner;
    [SerializeField] private ScoreManager scoreManager;

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
                    // ✅ On utilise le ScoreManager
                    if (scoreManager != null)
                        scoreManager.AddPoint();

                    Destroy(hit.collider.gameObject);
                    spawner.Respawn();
                }
            }
        }
    }
}
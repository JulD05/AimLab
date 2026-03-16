// using UnityEngine;
// using UnityEngine.InputSystem;

// public class RaycastShooter : MonoBehaviour
// {
//     [Header("Raycast")]
//     public float range = 100f;
//     public LayerMask environmentMask;

//     [Header("Impact")]
//     public GameObject impactPrefab;
//     public float impactLifetime = 1f;

//     Camera cam;

//     void Awake()
//     {
//         cam = GetComponent<Camera>();
//     }

//     void Update()
//     {
//         if (Mouse.current.leftButton.wasPressedThisFrame)
//         {
//             Shoot();
//         }
//     }

//     void Shoot()
//     {
//         Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));

//         if (Physics.Raycast(ray, out RaycastHit hit, range, environmentMask))
//         {
//             SpawnImpact(hit);
//         }
//     }

//     void SpawnImpact(RaycastHit hit)
//     {
//         GameObject impact = Instantiate(
//             impactPrefab,
//             hit.point,
//             Quaternion.LookRotation(hit.normal)
//         );

//         Destroy(impact, impactLifetime);
//     }
// }

using UnityEngine;
using UnityEngine.InputSystem;

public class RaycastShooter : MonoBehaviour
{
    public float range = 100f;
    public LayerMask environmentMask;

    public GameObject impactPrefab;
    public float impactLifetime = 1f;

    public AudioSource shootAudio;
    public TargetSpawner targetSpawner;
    public ScoreManager scoreManager;

    Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        if (Mouse.current == null) return;
        if (Cursor.lockState != CursorLockMode.Locked) return;
        if (!Mouse.current.leftButton.wasPressedThisFrame) return;

        Shoot();
    }

    void Shoot()
    {
        if (shootAudio != null)
            shootAudio.Play();

        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));

        if (!Physics.Raycast(ray, out RaycastHit hit, range)) return;

        Target target = hit.collider.GetComponentInParent<Target>();
        if (target != null)
        {
            scoreManager?.AddPoint();
            target.Hit();
            targetSpawner?.Respawn();
            return;
        }

        if ((environmentMask.value & (1 << hit.collider.gameObject.layer)) == 0) return;
        if (impactPrefab == null) return;

        GameObject impact = Instantiate(
            impactPrefab,
            hit.point,
            Quaternion.LookRotation(hit.normal)
        );

        Destroy(impact, impactLifetime);
    }
}

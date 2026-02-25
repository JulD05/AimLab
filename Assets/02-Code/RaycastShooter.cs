using UnityEngine;
using UnityEngine.InputSystem;

public class RaycastShooter : MonoBehaviour
{
    [Header("Raycast")]
    public float range = 100f;
    public LayerMask environmentMask;

    [Header("Impact")]
    public GameObject impactPrefab;
    public float impactLifetime = 1f;

    Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Shoot();
        }
    }

    void Shoot()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));

        if (Physics.Raycast(ray, out RaycastHit hit, range, environmentMask))
        {
            SpawnImpact(hit);
        }
    }

    void SpawnImpact(RaycastHit hit)
    {
        GameObject impact = Instantiate(
            impactPrefab,
            hit.point,
            Quaternion.LookRotation(hit.normal)
        );

        Destroy(impact, impactLifetime);
    }
}
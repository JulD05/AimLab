using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponRecoil : MonoBehaviour
{
    [SerializeField] private Transform weaponRoot;
    [SerializeField] private Vector3 recoilOffset = new Vector3(0f, 0f, -1f);
    [SerializeField] private float recoilBackDuration = 0.04f;
    [SerializeField] private float recoilReturnDuration = 0.08f;

    private readonly string[] recoilPartNames =
    {
        "slid_low",
        "led_low",
        "behind scoop_low",
        "front scoop_low",
    };

    private readonly List<Transform> recoilParts = new List<Transform>();
    private readonly List<Vector3> startLocalPositions = new List<Vector3>();
    private Coroutine recoilRoutine;
    private bool warnedMissingParts;

    void Awake()
    {
        CacheParts();
    }

    public void PlayRecoil()
    {
        if (recoilParts.Count == 0)
            CacheParts();

        if (recoilParts.Count == 0)
            return;

        if (recoilRoutine != null)
            StopCoroutine(recoilRoutine);

        recoilRoutine = StartCoroutine(PlayRecoilRoutine());
    }

    void CacheParts()
    {
        recoilParts.Clear();
        startLocalPositions.Clear();

        Transform searchRoot = weaponRoot != null ? weaponRoot : transform;
        HashSet<string> foundNames = new HashSet<string>();

        foreach (string partName in recoilPartNames)
        {
            Transform part = FindDeepChild(searchRoot, partName);
            if (part == null) continue;

            recoilParts.Add(part);
            startLocalPositions.Add(part.localPosition);
            foundNames.Add(partName);
        }

        if (warnedMissingParts || foundNames.Count == recoilPartNames.Length)
            return;

        warnedMissingParts = true;
        Debug.LogWarning("Recul de l'arme: certains éléments n'ont pas été trouvés.");
    }

    IEnumerator PlayRecoilRoutine()
    {
        yield return AnimateOffset(Vector3.zero, recoilOffset, recoilBackDuration);
        yield return AnimateOffset(recoilOffset, Vector3.zero, recoilReturnDuration);
        ApplyOffset(Vector3.zero);
        recoilRoutine = null;
    }

    IEnumerator AnimateOffset(Vector3 fromOffset, Vector3 toOffset, float duration)
    {
        float safeDuration = Mathf.Max(0.001f, duration);
        float elapsed = 0f;

        while (elapsed < safeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / safeDuration);
            ApplyOffset(Vector3.Lerp(fromOffset, toOffset, t));
            yield return null;
        }
    }

    void ApplyOffset(Vector3 offset)
    {
        for (int i = 0; i < recoilParts.Count; i++)
        {
            if (recoilParts[i] == null) continue;
            recoilParts[i].localPosition = startLocalPositions[i] + offset;
        }
    }

    static Transform FindDeepChild(Transform parent, string childName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == childName)
                return child;

            Transform nestedChild = FindDeepChild(child, childName);
            if (nestedChild != null)
                return nestedChild;
        }

        return null;
    }
}

using UnityEngine;

public class Target : MonoBehaviour
{
    public System.Action OnDestroyed;

    public void Hit()
    {
        OnDestroyed?.Invoke();
        Destroy(gameObject);
    }
}
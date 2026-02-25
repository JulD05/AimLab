using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonScale : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] float hoverScale = 1.08f;
    [SerializeField] float pressScale = 0.95f;
    [SerializeField] float speed = 12f;

    Vector3 baseScale;
    Vector3 targetScale;

    void Awake()
    {
        baseScale = transform.localScale;
        targetScale = baseScale;
    }

    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.unscaledDeltaTime * speed);
    }

    public void OnPointerEnter(PointerEventData eventData) => targetScale = baseScale * hoverScale;
    public void OnPointerExit(PointerEventData eventData)  => targetScale = baseScale;
    public void OnPointerDown(PointerEventData eventData)  => targetScale = baseScale * pressScale;
    public void OnPointerUp(PointerEventData eventData)    => targetScale = baseScale * hoverScale;
}
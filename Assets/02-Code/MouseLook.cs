using UnityEngine;
using UnityEngine.InputSystem;

public class MouseLook : MonoBehaviour
{
    [Header("Sensitivity")]
    public float mouseSensitivity = 0.1f;

    [Header("References")]
    public Transform playerBody;

    [Header("Cursor")]
    [SerializeField] private bool startUnlocked = false;

    private float xRotation = 0f;
    private bool isLocked = true;

    void Start()
    {
        RecenterPlayerPivotOnCamera();
        xRotation = NormalizeAngle(transform.localEulerAngles.x);
        SetCursorLocked(!startUnlocked);
    }

    void Update()
    {
        // Toggle curseur avec ESC
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            SetCursorLocked(!isLocked);
        }

        // Si curseur pas lock => on ne tourne pas la caméra (tu peux cliquer l'UI)
        if (!isLocked) return;
        if (Mouse.current == null) return;

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        float mouseX = mouseDelta.x * mouseSensitivity;
        float mouseY = mouseDelta.y * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }

    public void SetCursorLockedState(bool locked)
    {
        SetCursorLocked(locked);
    }

    void RecenterPlayerPivotOnCamera()
    {
        if (playerBody == null) return;
        if (transform.parent != playerBody) return;

        Vector3 cameraWorldPosition = transform.position;
        playerBody.position = cameraWorldPosition;
        transform.localPosition = Vector3.zero;
    }

    float NormalizeAngle(float angle)
    {
        if (angle > 180f) angle -= 360f;
        return angle;
    }

    private void SetCursorLocked(bool locked)
    {
        isLocked = locked;
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }
}

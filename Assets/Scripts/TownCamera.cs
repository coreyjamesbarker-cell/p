using UnityEngine;

// Steep top-down camera. Pans with WASD/arrows or screen-edge, zooms with the scroll wheel,
// and keeps its focus point inside the town.
[RequireComponent(typeof(Camera))]
public class TownCamera : MonoBehaviour
{
    [Header("Angle")]
    [SerializeField] float pitch = 60f;
    [SerializeField] float yaw = 0f;

    [Header("Pan")]
    [SerializeField] float panSpeed = 14f;
    [SerializeField] bool edgeScroll = true;
    [SerializeField] float edgeSize = 12f;          // pixels from the screen edge

    [Header("Zoom (distance from focus point)")]
    [SerializeField] float distance = 30f;
    [SerializeField] float minDistance = 10f;
    [SerializeField] float maxDistance = 40f;
    [SerializeField] float zoomStep = 3f;
    [SerializeField] float zoomSmoothing = 10f;

    [Header("Bounds (focus point, world XZ)")]
    [SerializeField] Vector2 boundsMin = new Vector2(-16f, -16f);
    [SerializeField] Vector2 boundsMax = new Vector2(16f, 16f);
    [SerializeField] Vector3 startFocus = new Vector3(0f, 0f, -2f);

    Vector3 focus;
    float targetDistance;

    void Start()
    {
        focus = startFocus;
        targetDistance = distance;
        ApplyTransform();
    }

    void Update()
    {
        float dt = Time.unscaledDeltaTime;

        // Keyboard + edge scroll pan
        Vector2 move = GameInput.MoveAxis;
        if (edgeScroll && Application.isFocused)
        {
            Vector2 m = GameInput.MousePosition;
            bool inside = m.x >= 0 && m.y >= 0 && m.x <= Screen.width && m.y <= Screen.height;
            if (inside)
            {
                if (m.x < edgeSize) move.x -= 1f;
                else if (m.x > Screen.width - edgeSize) move.x += 1f;
                if (m.y < edgeSize) move.y -= 1f;
                else if (m.y > Screen.height - edgeSize) move.y += 1f;
            }
        }
        move = Vector2.ClampMagnitude(move, 1f);

        // Pan relative to camera yaw; pan a bit faster when zoomed out.
        Quaternion yawRot = Quaternion.Euler(0f, yaw, 0f);
        Vector3 worldMove = yawRot * new Vector3(move.x, 0f, move.y);
        float zoomFactor = Mathf.Lerp(0.6f, 1.4f, Mathf.InverseLerp(minDistance, maxDistance, distance));
        focus += worldMove * panSpeed * zoomFactor * dt;
        focus.x = Mathf.Clamp(focus.x, boundsMin.x, boundsMax.x);
        focus.z = Mathf.Clamp(focus.z, boundsMin.y, boundsMax.y);

        // Zoom
        float scroll = GameInput.ScrollDelta;
        if (Mathf.Abs(scroll) > 0.01f)
            targetDistance = Mathf.Clamp(targetDistance - scroll * zoomStep, minDistance, maxDistance);
        distance = Mathf.Lerp(distance, targetDistance, 1f - Mathf.Exp(-zoomSmoothing * dt));

        ApplyTransform();
    }

    void ApplyTransform()
    {
        Quaternion rot = Quaternion.Euler(pitch, yaw, 0f);
        transform.rotation = rot;
        transform.position = focus - rot * Vector3.forward * distance;
    }

    void OnValidate()
    {
        minDistance = Mathf.Max(1f, minDistance);
        maxDistance = Mathf.Max(minDistance, maxDistance);
        distance = Mathf.Clamp(distance, minDistance, maxDistance);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3 c = new Vector3((boundsMin.x + boundsMax.x) * 0.5f, 0f, (boundsMin.y + boundsMax.y) * 0.5f);
        Vector3 s = new Vector3(boundsMax.x - boundsMin.x, 0.1f, boundsMax.y - boundsMin.y);
        Gizmos.DrawWireCube(c, s);
    }
}

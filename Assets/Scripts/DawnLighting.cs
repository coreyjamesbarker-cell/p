using UnityEngine;

// Soft blue pre-dawn light that warms to golden sunrise as Progress goes 0 -> 1.
// GameManager will drive Progress from the dawn timer; until then, drag the slider in Play mode.
public class DawnLighting : MonoBehaviour
{
    [Range(0f, 1f)] public float progress = 0f;

    [SerializeField] Light sun;
    [SerializeField] Camera cam;

    [Header("Pre-dawn (progress 0)")]
    [SerializeField] Color startSunColour = new Color(0.72f, 0.78f, 1f);
    [SerializeField] float startSunIntensity = 0.85f;
    [SerializeField] float startSunPitch = 55f;
    [SerializeField] Color startAmbient = new Color(0.42f, 0.47f, 0.62f);
    [SerializeField] Color startSky = new Color(0.32f, 0.38f, 0.58f);

    [Header("Sunrise (progress 1)")]
    [SerializeField] Color endSunColour = new Color(1f, 0.78f, 0.5f);
    [SerializeField] float endSunIntensity = 1.25f;
    [SerializeField] float endSunPitch = 30f;
    [SerializeField] Color endAmbient = new Color(0.66f, 0.54f, 0.42f);
    [SerializeField] Color endSky = new Color(1f, 0.7f, 0.48f);

    [SerializeField] float sunYaw = -35f;

    void Start()
    {
        if (sun == null) sun = GetComponent<Light>();
        if (cam == null) cam = Camera.main;
        Apply();
    }

    void Update() => Apply();

    public void Apply()
    {
        float t = Mathf.SmoothStep(0f, 1f, progress);

        if (sun != null)
        {
            sun.color = Color.Lerp(startSunColour, endSunColour, t);
            sun.intensity = Mathf.Lerp(startSunIntensity, endSunIntensity, t);
            sun.transform.rotation = Quaternion.Euler(Mathf.Lerp(startSunPitch, endSunPitch, t), sunYaw, 0f);
        }

        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = Color.Lerp(startAmbient, endAmbient, t);

        if (cam != null)
        {
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = Color.Lerp(startSky, endSky, t);
        }
    }
}

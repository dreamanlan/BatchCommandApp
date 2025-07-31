using UnityEngine;
using UnityEngine.UI;

public class AutoFitSceneViewer : MonoBehaviour
{
    public Camera captureCamera;
    public RenderTexture renderTexture;

    public int rtWidth = 256;
    public int rtHeight = 256;
    public float refreshInterval = 1f;

    private RawImage rawImage;
    private float lastRefreshTime;

    public void Setup(int width, int height, float interval)
    {
        if (rtWidth != width || rtHeight != height) {
            rtWidth = width;
            rtHeight = height;

            if (null != renderTexture) {
                renderTexture.Release();
                renderTexture = new RenderTexture(rtWidth, rtHeight, 16, RenderTextureFormat.ARGB32);
                renderTexture.Create();
            }
            if (null != captureCamera) {
                captureCamera.targetTexture = renderTexture;
            }
            if (null != rawImage) {
                rawImage.texture = renderTexture;

                RectTransform rt = rawImage.rectTransform;
                rt.anchorMin = new Vector2(0, 1);
                rt.anchorMax = new Vector2(0, 1);
                rt.pivot = new Vector2(0, 1);
                rt.sizeDelta = new Vector2(rtWidth, rtHeight);
                rt.anchoredPosition = new Vector2(10, -10);
            }
        }
        refreshInterval = interval;
    }
    public void Refresh()
    {
        Bounds sceneBounds = CalculateSceneBounds();
        PositionCameraToFitBounds(captureCamera, sceneBounds);
    }

    void Start()
    {
        if (captureCamera == null) {
            GameObject camGO = gameObject;
            captureCamera = camGO.AddComponent<Camera>();
        }

        renderTexture = new RenderTexture(rtWidth, rtHeight, 16, RenderTextureFormat.ARGB32);
        renderTexture.Create();

        captureCamera.targetTexture = renderTexture;

        Bounds sceneBounds = CalculateSceneBounds();
        PositionCameraToFitBounds(captureCamera, sceneBounds);

        SetupUI();
        lastRefreshTime = Time.time;
    }

    void Update()
    {
        float curTime = Time.time;
        if (curTime - lastRefreshTime > refreshInterval) {
            Refresh();
            lastRefreshTime = curTime;
        }
    }

    Bounds CalculateSceneBounds()
    {
        Renderer[] renderers = FindObjectsOfType<Renderer>();
        if (renderers.Length == 0)
            return new Bounds(Vector3.zero, Vector3.one);

        Bounds bounds = renderers[0].bounds;
        foreach (var rend in renderers) {
            bounds.Encapsulate(rend.bounds);
        }
        return bounds;
    }

    void PositionCameraToFitBounds(Camera cam, Bounds bounds)
    {
        Vector3 center = bounds.center;
        Vector3 size = bounds.size;

        float maxSize = Mathf.Max(size.x, size.y, size.z);

        float fov = cam.fieldOfView;
        float aspect = cam.aspect;

        float distance = maxSize / Mathf.Tan(fov * 0.5f * Mathf.Deg2Rad);

        Vector3 direction = new Vector3(0, -1, 1);
        cam.transform.position = center - direction * distance;

        cam.transform.LookAt(center);

        cam.nearClipPlane = distance * 0.1f;
        cam.farClipPlane = distance * 3f;
    }

    void SetupUI()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null) {
            GameObject canvasGO = new GameObject("SceneViewCanvas");
            canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();
        }

        GameObject rawImageGO = new GameObject("SceneCaptureRawImage");
        rawImageGO.transform.SetParent(canvas.transform, false);

        rawImage = rawImageGO.AddComponent<RawImage>();
        rawImage.texture = renderTexture;

        RectTransform rt = rawImage.rectTransform;
        rt.anchorMin = new Vector2(0, 1);
        rt.anchorMax = new Vector2(0, 1);
        rt.pivot = new Vector2(0, 1);
        rt.sizeDelta = new Vector2(rtWidth, rtHeight);
        rt.anchoredPosition = new Vector2(10, -10);
    }

    void OnDestroy()
    {
        if (renderTexture != null) {
            renderTexture.Release();
            Destroy(renderTexture);
        }
        if (captureCamera != null) {
            Destroy(captureCamera.gameObject);
        }
        if (rawImage != null) {
            Destroy(rawImage.gameObject);
        }
    }
}
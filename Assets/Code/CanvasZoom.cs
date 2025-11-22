using UnityEngine;

public class CanvasZoom : MonoBehaviour
{
    public RectTransform uiRoot;
    public float zoomSpeed = 0.5f;
    public float minScale = 1f;
    public float maxScale = 2f;

    public bool isActive = false;

    void Update()
    {
        // Kalau tidak aktif, jangan lakukan apa-apa
        if (!isActive)
            return;

        Vector3 scale = uiRoot.localScale;

        // Zoom In
        scale += Vector3.one * zoomSpeed * Time.deltaTime;

        // Clamp agar tidak lebih dari maxScale
        scale.x = Mathf.Clamp(scale.x, minScale, maxScale);
        scale.y = Mathf.Clamp(scale.y, minScale, maxScale);
        uiRoot.localScale = scale;

        // Jika sudah mencapai maxScale, matikan otomatis (opsional)
        if (scale.x >= maxScale)
        {
            isActive = false;
        }
    }

    // Fungsi untuk mereset scale ke nilai minimum
    public void ResetScale()
    {
        uiRoot.localScale = new Vector3(minScale, minScale, minScale);
        isActive = false;
    }
}

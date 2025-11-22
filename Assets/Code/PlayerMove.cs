using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] Animator _transisi_fade;

    [Header("Pengaturan Zoom")]
    [SerializeField] private float zoomSpeed = 0.5f;
    [SerializeField] private float minScale = 1f;
    [SerializeField] private float maxScale = 2f;

    // List GameObject diisi dari Inspector
    [SerializeField] private List<GameObject> objectsToManage;

    // Dictionary index -> GameObject
    private Dictionary<int, GameObject> objectDictionary = new Dictionary<int, GameObject>();

    private GameObject previouslyActiveObject = null;

    // State Input
    private enum PlayerInputState
    {
        WaitingForInput,
        Processing
    }

    private PlayerInputState currentState;

    void Start()
    {
        currentState = PlayerInputState.WaitingForInput;
        StartCoroutine(PlayerInputCoroutine());

        // Masukkan semua ke dictionary & nonaktifkan
        for (int i = 0; i < objectsToManage.Count; i++)
        {
            if (objectsToManage[i] != null)
            {
                objectDictionary.Add(i, objectsToManage[i]);
                objectsToManage[i].SetActive(false);
            }
        }
    }

    IEnumerator PlayerInputCoroutine()
    {
        while (true)
        {
            if (currentState == PlayerInputState.WaitingForInput)
            {
                if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    StartCoroutine(HandleInputAndTransition());
                }
                else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
                {
                    StartCoroutine(HandleInputAndTransition());
                }
            }
            yield return null;
        }
    }

    IEnumerator HandleInputAndTransition()
    {
        // 1. Ganti state agar tidak terima input lain
        currentState = PlayerInputState.Processing;

        // 2. Mulai zoom-in pada object yang aktif saat ini DAN transisi fade-in secara bersamaan
        if (previouslyActiveObject != null)
        {
            StartCoroutine(ZoomInCoroutine(previouslyActiveObject.GetComponent<RectTransform>()));
        }

        // 4. Mulai transisi fade-in (layar menghitam)
        _transisi_fade.SetTrigger("In");

        // 5. Tunggu transisi fade-in selesai (misalnya 0.5 detik)
        yield return new WaitForSeconds(2f);

        // 6. Aktifkan object baru dan reset scalenya ke posisi awal
        GameObject newActiveObject = ActivateAndResetRandomObject();

        // 7. Mulai transisi fade-out (layar kembali normal)
        _transisi_fade.SetTrigger("Out");

        // 8. Tunggu transisi fade-out selesai
        yield return new WaitForSeconds(0.5f);

        // 9. Reset semua object LAINNYA ke skala semula
        ResetAllInactiveObjects(newActiveObject);

        // 10. Kembali ke state tunggu input, siap untuk gerakan berikutnya
        currentState = PlayerInputState.WaitingForInput;
    }

    GameObject ActivateAndResetRandomObject()
    {
        if (objectDictionary.Count == 0)
            return null;

        // Nonaktifkan object lama SEKARANG, tepat sebelum memilih yang baru
        if (previouslyActiveObject != null)
        {
            previouslyActiveObject.SetActive(false);
        }

        int randomIndex;

        // Pilih acak, tapi jangan sama dengan sebelumnya
        do
        {
            randomIndex = Random.Range(0, objectDictionary.Count);
        } while (objectDictionary.Count > 1 && objectDictionary[randomIndex] == previouslyActiveObject);

        previouslyActiveObject = objectDictionary[randomIndex];
        previouslyActiveObject.SetActive(true);

        // Reset scale object yang baru aktif secara langsung
        RectTransform uiRoot = previouslyActiveObject.GetComponent<RectTransform>();
        if (uiRoot != null)
        {
            uiRoot.localScale = new Vector3(minScale, minScale, minScale);
        }

        return previouslyActiveObject;
    }

    void ResetAllInactiveObjects(GameObject activeObject)
    {
        // Iterasi melalui semua nilai (GameObject) dalam dictionary
        foreach (GameObject obj in objectDictionary.Values)
        {
            // Jika objek ini BUKAN objek yang sedang aktif
            if (obj != activeObject)
            {
                RectTransform uiRoot = obj.GetComponent<RectTransform>();
                if (uiRoot != null)
                {
                    uiRoot.localScale = new Vector3(minScale, minScale, minScale);
                }
            }
        }
    }

    IEnumerator ZoomInCoroutine(RectTransform uiRoot)
    {
        if (uiRoot == null) yield break;

        float currentScale = uiRoot.localScale.x;
        while (currentScale < maxScale)
        {
            currentScale += zoomSpeed * Time.deltaTime;
            float clampedScale = Mathf.Clamp(currentScale, minScale, maxScale);
            uiRoot.localScale = new Vector3(clampedScale, clampedScale, clampedScale);
            yield return null;
        }
        // Pastikan skala berakhir tepat di maxScale
        uiRoot.localScale = new Vector3(maxScale, maxScale, maxScale);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ManagerGame : MonoBehaviour
{
    [SerializeField] AudioSource bgm;

    [SerializeField] Animator _transisi_fade;

    [SerializeField] float timeDisplayOn;

    [Header("Pengaturan Zoom")]
    [SerializeField] private float zoomSpeed = 0.5f;
    [SerializeField] private float minScale = 1f;
    [SerializeField] private float maxScale = 2f;

    [Header("Start")]
    [SerializeField] private Animator returnAnimatorAfterJumpscare;

    [Header("List Animator (UI/Scene Objects)")]
    [SerializeField] private List<Animator> objectsToManage;

    [Header("Jumpscare")]
    [SerializeField] private GameObject jumpscareObject;

    private Animator previouslyActiveAnimator = null;
    private Coroutine _jumpscareCoroutine = null;

    private int _indexRandomJumpscare = 0;
    private int _randomIndex = 0;

    private enum PlayerInputState
    {
        WaitingForInput,
        Processing
    }

    private PlayerInputState currentState;

    private bool canPress = true;
    public void SetCanPress(bool value)
    {
        canPress = value;
    }

    void Start()
    {
        // Hasilkan angka acak dari 0 sampai jumlah total objek - 1
        if (objectsToManage != null && objectsToManage.Count > 0)
        {
            _indexRandomJumpscare = Random.Range(1, objectsToManage.Count);
        }

        currentState = PlayerInputState.WaitingForInput;
        StartCoroutine(PlayerInputCoroutine());

        // Set semua jadi tidak tampil (alpha 0)
        foreach (Animator anim in objectsToManage)
        {
            if (anim == null) continue;

            CanvasGroup cg = anim.GetComponent<CanvasGroup>();
            if (cg == null)
                cg = anim.gameObject.AddComponent<CanvasGroup>();

            cg.alpha = 0f;
            cg.interactable = false;
            cg.blocksRaycasts = false;
        }
    }

    void Update()
    {
        if (_indexRandomJumpscare == _randomIndex)
        {
            // Mulai coroutine jumpscare dan langsung reset index agar tidak ter-trigger berulang kali
            _jumpscareCoroutine = StartCoroutine(JumpscareSequence());

            // Acak ulang _indexRandomJumpscare ke nilai baru yang berbeda dari yang sekarang
            if (objectsToManage.Count > 1)
            {
                int newJumpscareIndex;
                do
                {
                    newJumpscareIndex = Random.Range(0, objectsToManage.Count);
                } while (newJumpscareIndex == _randomIndex); // Pastikan tidak sama dengan indeks saat ini

                _indexRandomJumpscare = newJumpscareIndex;
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    //Fungsi untuk player input
    IEnumerator PlayerInputCoroutine()
    {
        while (true)
        {
            if (currentState == PlayerInputState.WaitingForInput && canPress)
            {
                // INPUT KIRI
                if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    SFXManager.Instance.PlaySFX(0);

                    canPress = false;                 // Kunci input
                    StartCoroutine(UnlockInput());    // Buka lagi setelah 2 detik

                    StartCoroutine(HandleInputAndTransition(2f));

                    if (_indexRandomJumpscare == _randomIndex)
                    {
                        _indexRandomJumpscare = Random.Range(0, objectsToManage.Count);
                    }
                }

                // INPUT KANAN
                else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
                {
                    SFXManager.Instance.PlaySFX(0);

                    canPress = false;                 // Kunci input
                    StartCoroutine(UnlockInput());    // Buka lagi setelah 2 detik

                    StartCoroutine(HandleInputAndTransition(1f));

                    if (_indexRandomJumpscare == _randomIndex)
                    {
                        _indexRandomJumpscare = Random.Range(0, objectsToManage.Count);
                    }
                }
            }

            yield return null;
        }
    }

    //Fungsi untuk handleinput
    IEnumerator HandleInputAndTransition(float animatorValue)
    {
        currentState = PlayerInputState.Processing;

        // Hentikan jumpscare jika sedang berjalan
        if (_jumpscareCoroutine != null)
        {
            StopCoroutine(_jumpscareCoroutine);
            _jumpscareCoroutine = null;
        }

        // Matikan jumpscare jika sedang aktif saat transisi dimulai
        jumpscareObject.SetActive(false);

        // Zoom current
        if (previouslyActiveAnimator != null)
        {
            RectTransform rt = previouslyActiveAnimator.GetComponent<RectTransform>();
            if (rt != null)
                StartCoroutine(ZoomInCoroutine(rt));
        }

        _transisi_fade.SetTrigger("In");

        // Terapkan value kepada animasi lama
        if (previouslyActiveAnimator != null)
            previouslyActiveAnimator.SetFloat("value", animatorValue);

        yield return new WaitForSeconds(timeDisplayOn);

        // Aktifkan object baru
        Animator newActiveAnimator = ActivateRandomObject();

        // Setelah object baru ditampilkan, dapatkan indexnya dan simpan ke _randomIndex
        if (newActiveAnimator != null)
        {
            _randomIndex = objectsToManage.IndexOf(newActiveAnimator);
        }

        _transisi_fade.SetTrigger("Out");
        yield return new WaitForSeconds(0.5f);

        ResetAllInactiveObjects(newActiveAnimator);
        currentState = PlayerInputState.WaitingForInput;
    }

    //Fungsi untuk Aktifkan kembali animasi kembali
    Animator ActivateRandomObject()
    {
        List<Animator> candidates = new List<Animator>(objectsToManage);
        candidates.RemoveAll(a => a == null);

        if (candidates.Count == 0)
            return null;

        if (previouslyActiveAnimator != null && candidates.Count > 1)
            candidates.Remove(previouslyActiveAnimator);

        int lastChosenIndex = -1; // di atas, sebagai field

        // Pada saat memilih:
        int index;

        do
        {
            index = Random.Range(0, candidates.Count);
        }
        while (index == lastChosenIndex);

        lastChosenIndex = index;
        Animator chosen = candidates[index];

        // Set parameter untuk animator baru
        chosen.SetFloat("value", 0);

        // Reset scale UI ke awal
        RectTransform rt = chosen.GetComponent<RectTransform>();
        if (rt != null)
            rt.localScale = new Vector3(minScale, minScale, minScale);

        // Munculkan dengan CanvasGroup
        CanvasGroup cg = chosen.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = chosen.gameObject.AddComponent<CanvasGroup>();

        cg.alpha = 1f;
        cg.interactable = true;
        cg.blocksRaycasts = true;

        previouslyActiveAnimator = chosen;
        return chosen;
    }

    void ActivateSpecifiedAnimator(Animator target)
    {
        if (target == null)
            return;

        // Reset animator sebelumnya
        if (previouslyActiveAnimator != null)
        {
            CanvasGroup oldCg = previouslyActiveAnimator.GetComponent<CanvasGroup>();
            if (oldCg == null)
                oldCg = previouslyActiveAnimator.gameObject.AddComponent<CanvasGroup>();

            oldCg.alpha = 0f;
            oldCg.interactable = false;
            oldCg.blocksRaycasts = false;
        }

        _transisi_fade.gameObject.SetActive(true);

        // Aktifkan animator tujuan
        target.SetFloat("value", 0);

        RectTransform rt = target.GetComponent<RectTransform>();
        if (rt != null)
            rt.localScale = new Vector3(minScale, minScale, minScale);

        CanvasGroup cg = target.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = target.gameObject.AddComponent<CanvasGroup>();

        cg.alpha = 1f;
        cg.interactable = true;
        cg.blocksRaycasts = true;

        // Simpan sebagai aktif sekarang
        previouslyActiveAnimator = target;

        // Update _randomIndex juga kalau kamu butuh
        _randomIndex = 99;
    }

    //Fungsi Reset animasi dan object
    void ResetAllInactiveObjects(Animator activeAnimator)
    {
        foreach (Animator anim in objectsToManage)
        {
            if (anim == null || anim == activeAnimator)
                continue;

            // Reset value agar nanti kalau muncul lagi mulai dari 1
            anim.SetFloat("value", 0);

            RectTransform rt = anim.GetComponent<RectTransform>();
            if (rt != null)
                rt.localScale = new Vector3(minScale, minScale, minScale);

            // Sembunyikan pake CanvasGroup, bukan disable GameObject
            CanvasGroup cg = anim.GetComponent<CanvasGroup>();
            if (cg == null)
                cg = anim.gameObject.AddComponent<CanvasGroup>();

            cg.alpha = 0f;
            cg.interactable = false;
            cg.blocksRaycasts = false;
        }
    }

    //Fungsi ZoomIn Camera
    IEnumerator ZoomInCoroutine(RectTransform uiRoot)
    {
        if (uiRoot == null)
            yield break;

        float currentScale = uiRoot.localScale.x;

        while (currentScale < maxScale)
        {
            currentScale += zoomSpeed * Time.deltaTime;
            float clamped = Mathf.Clamp(currentScale, minScale, maxScale);
            uiRoot.localScale = new Vector3(clamped, clamped, clamped);
            yield return null;
        }

        uiRoot.localScale = new Vector3(maxScale, maxScale, maxScale);
    }

    //Fungsi untuk Jumpscare dan reset randomnya
    IEnumerator JumpscareSequence()
    {
        // 2. Setelah 2 detik, jalankan logika jumpscare
        if (previouslyActiveAnimator != null)
        {
            previouslyActiveAnimator.SetFloat("value", 3f);
        }

        // 1. Tunggu selama 1.5 detik
        yield return new WaitForSeconds(3f);

        bgm.Stop();

        jumpscareObject.SetActive(true);

        // Tunggu 3 detik sebelum dihilangkan lagi
        yield return new WaitForSeconds(3f);

        jumpscareObject.SetActive(false);

        _transisi_fade.gameObject.SetActive(false);
        // Setelah jumpscare, langsung ke animator tujuan
        ActivateSpecifiedAnimator(returnAnimatorAfterJumpscare);

        // Reset coroutine
        _jumpscareCoroutine = null;

        bgm.Play();
        
        canPress = true;
    }

    IEnumerator UnlockInput()
    {
        yield return new WaitForSeconds(2f);
        canPress = true;
    }

    public void Reset()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

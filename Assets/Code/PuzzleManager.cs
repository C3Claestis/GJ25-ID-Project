using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleManager : MonoBehaviour
{
    private int digit1, digit2, digit3;
    private int index1, index2, index3;

    ManagerGame manager;    

    [Header("BGM")]
    [SerializeField] AudioSource bgm;
    [SerializeField] AudioClip finishing;

    [Header("Anim Gembok")]
    [SerializeField] Animator doorLockAnim;
    [SerializeField] Animator gembokAnim;

    [Header("Angka Gembok")]
    [SerializeField] Text digit1Txt;
    [SerializeField] Text digit2Txt;
    [SerializeField] Text digit3Txt;

    [Header("Button Gembok")]
    [SerializeField] Button button1;
    [SerializeField] Button button2;
    [SerializeField] Button button3;

    [SerializeField] GameObject finisher;

    void Start()
    {
        GeneratePuzzle();
        manager = GetComponent<ManagerGame>();        
    }

    private bool isUnlocked = false;

    void Update()
    {
        if (!isUnlocked) // hanya cek selama belum terbuka
        {
            if (digit1Txt.text == digit1.ToString() &&
                digit2Txt.text == digit2.ToString() &&
                digit3Txt.text == digit3.ToString())
            {
                StartCoroutine(OpenGembok());
            }
        }
    }

    IEnumerator OpenGembok()
    {
        isUnlocked = true;  // tandai agar tidak dipanggil lagi

        button1.enabled = false;
        button2.enabled = false;
        button3.enabled = false;

        gembokAnim.SetBool("IsOpen", isUnlocked);

        yield return new WaitForSeconds(2);

        
        manager.SetCanPress(false);

        // Nonaktifkan parent dari gembokAnim
        if (gembokAnim.transform.parent != null)
        {
            gembokAnim.transform.parent.gameObject.SetActive(false);
        }

        doorLockAnim.SetBool("IsExit", true);

        yield return new WaitForSeconds(3);

        finisher.SetActive(true);

        bgm.clip = finishing;
        bgm.Play();
    }

    public void GeneratePuzzle()
    {
        digit1 = Random.Range(0, 10);
        digit2 = Random.Range(0, 10);
        digit3 = Random.Range(0, 10);

        List<int> pool = Enumerable.Range(0, 21).ToList();

        index1 = pool.PopRandom();
        index2 = pool.PopRandom();
        index3 = pool.PopRandom();

        Debug.Log($"Digit {digit1} → index {index1}");
        Debug.Log($"Digit {digit2} → index {index2}");
        Debug.Log($"Digit {digit3} → index {index3}");
    }

    // Fungsi siap dipanggil dari Button
    public void CheckButton(int buttonIndex, TextMeshProUGUI label)
    {
        if (buttonIndex == index1)
            label.text = digit1.ToString();
        else if (buttonIndex == index2)
            label.text = digit2.ToString();
        else if (buttonIndex == index3)
            label.text = digit3.ToString();
        else
            label.text = "";
    }

    #region Fungsi untuk menonaktifkan objektif    
    public void DisableObjektif(GameObject gameObject)
    {
        StartCoroutine(DisableObjektifCoroutine(gameObject));
    }

    private IEnumerator DisableObjektifCoroutine(GameObject gameObject)
    {
        Animator animator = gameObject.GetComponentInChildren<Animator>();
        if (animator != null)
        {
            animator.SetFloat("value", 0);
        }
        else
        {
            Animator anim_ = gameObject.GetComponent<Animator>();
            anim_.SetFloat("value", 0);
        }

        yield return new WaitForSeconds(0.25f);
        gameObject.SetActive(false);
    }
    #endregion
}

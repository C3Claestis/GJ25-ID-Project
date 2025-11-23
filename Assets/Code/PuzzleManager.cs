using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    private int digit1, digit2, digit3;
    private int index1, index2, index3;

    void Start()
    {
        GeneratePuzzle();
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

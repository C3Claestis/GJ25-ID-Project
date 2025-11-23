using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisableObjektif(GameObject gameObject)
    {
        // Memulai Coroutine untuk menangani jeda waktu
        StartCoroutine(DisableObjektifCoroutine(gameObject));
    }

    private IEnumerator DisableObjektifCoroutine(GameObject gameObject)
    {
        // Mengambil komponen Animator terlebih dahulu
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

        // Berikan jeda waktu 1 detik
        yield return new WaitForSeconds(0.25f);

        // Nonaktifkan GameObject setelah animasi selesai (atau setelah jeda)
        gameObject.SetActive(false);
    }
}

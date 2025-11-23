using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleObjektif : MonoBehaviour
{
    private Animator _objek_anim;

    // Start is called before the first frame update
    void Start()
    {
        _objek_anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClick(int index)
    {
        if (_objek_anim != null)
        {
            _objek_anim.SetFloat("value", index);
        }
    }
}

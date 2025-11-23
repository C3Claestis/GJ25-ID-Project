using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleObjektif : MonoBehaviour
{
    private Animator _objek_anim;
    private Text _numberLockTxt;

    private bool _isLockOnCooldown = false;
    private int _currentLockNumber = 0;

    [SerializeField] bool isLocked = false;

    // Start is called before the first frame update
    void Start()
    {
        _objek_anim = GetComponent<Animator>();

        if (isLocked)
        {
            _numberLockTxt = GetComponentInChildren<Text>();

            if (_numberLockTxt != null)
                _numberLockTxt.text = _currentLockNumber.ToString();
        }
    }

    public void OnClick(int index)
    {
        if (_objek_anim != null)
        {
            _objek_anim.SetFloat("value", index);
        }
    }

    public void OnClickLock()
    {
        if (_isLockOnCooldown)
        {
            return;
        }

        _objek_anim.SetTrigger("Suffle");

        _currentLockNumber++;
        if (_currentLockNumber > 9)
        {
            _currentLockNumber = 0;
        }

        if (_numberLockTxt != null)
            _numberLockTxt.text = _currentLockNumber.ToString();

        StartCoroutine(LockCooldown());
    }

    private IEnumerator LockCooldown()
    {
        _isLockOnCooldown = true;
        yield return new WaitForSeconds(1f);
        _isLockOnCooldown = false;
    }
}

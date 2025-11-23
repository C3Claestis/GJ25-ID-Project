using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Button))]
public class PuzzleButton : MonoBehaviour
{
    [Header("Index button ini (0-20 dsb)")]
    public int buttonIndex;

    [Header("Text tempat hasil digit ditampilkan")]
    public TextMeshProUGUI targetLabel;

    private PuzzleManager manager;
    private Button _button;

    [Header("Mode Loker")]
    [SerializeField] private bool isLoker = false;
    [SerializeField] private TextMeshProUGUI[] textLoker;

    void Start()
    {
        manager = FindObjectOfType<PuzzleManager>();
        _button = GetComponent<Button>();

        // Pasang listener supaya selalu memanggil OnButtonPressed ketika ditekan
        _button.onClick.AddListener(OnButtonPressed);
    }

    void OnDestroy()
    {
        // Bersihkan listener untuk keamanan
        if (_button != null)
            _button.onClick.RemoveListener(OnButtonPressed);
    }

    private void OnButtonPressed()
    {
        // Jika bukan mode Loker → langsung cek
        if (!isLoker)
        {
            manager.CheckButton(buttonIndex, targetLabel);
            return;
        }

        // Kalau mode Loker → pakai coroutine
        StartCoroutine(HandleLokerPress());
    }

    private IEnumerator HandleLokerPress()
    {
        // 1. Kosongkan semua teks di loker
        if (textLoker != null && textLoker.Length > 0)
        {
            foreach (var txt in textLoker)
            {
                if (txt != null)
                    txt.text = "";
            }
        }

        // 2. Tunggu 0.5 detik
        yield return new WaitForSeconds(0.5f);

        // 3. Isi kembali digit di slot yang cocok
        manager.CheckButton(buttonIndex, targetLabel);
    }
}

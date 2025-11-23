using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class PuzzleButton : MonoBehaviour
{
    public int buttonIndex;
    public TextMeshProUGUI targetLabel;

    private PuzzleManager manager;
    private Button _button;

    void Start()
    {
        manager = FindObjectOfType<PuzzleManager>();
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnButtonPressed);
    }

    void OnDestroy()
    {
        _button.onClick.RemoveListener(OnButtonPressed);
    }

    private void OnButtonPressed()
    {
        manager.CheckButton(buttonIndex, targetLabel);
    }
}

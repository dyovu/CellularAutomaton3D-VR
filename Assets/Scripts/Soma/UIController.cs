using UnityEngine;
using UnityEngine.UIElements;
using UniRx;

public class UIController : MonoBehaviour
{
    private VisualElement root;
    private VisualElement debugUI;
    private Label debugLabel;

    void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        debugUI = root.Q<VisualElement>("debug-ui");
        debugLabel = debugUI.Q<Label>("currentbeat-label");

        // BeatClockを探して購読
        var beatClock = FindFirstObjectByType<BeatClock>();
        if (beatClock != null)
        {
            beatClock.OnBeat
                .Subscribe(beat =>
                {
                    debugLabel.text = $"{beat}";
                })
                .AddTo(this);
        }
        else
        {
            Debug.LogError("BeatClock not found in scene.");
        }
    }
}

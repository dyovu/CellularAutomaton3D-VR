using UnityEngine;
using UniRx;
using System;
using System.Linq;
using System.Collections.Generic;
public class Glider : MonoBehaviour
{
    [SerializeField] private List<int> triggerBeats = new List<int> { 4, 8, 12 };
    [SerializeField] private Color flashColor = Color.white;
    [SerializeField] private float flashDuration = 0.2f;

    private BeatClock clock;
    private Renderer rend;
    private Color originalColor;

    private void Start()
    {
        clock = FindFirstObjectByType<BeatClock>();
        if (clock != null)
        {
            Initialize(clock);
        }
        else
        {
            Debug.LogError("BeatClock not found in the scene. Please ensure it is present.");
        }

        rend = GetComponent<Renderer>();
        if (rend != null)
        {
            originalColor = rend.material.color;
        }    
    }
    public void Initialize(BeatClock clock)
    {
        clock.OnBeat
            .Where(beat => triggerBeats.Contains(beat))
            .Subscribe(_ => ReactToBeat())
            .AddTo(this);
    }

    private void ReactToBeat()
    {
        // Implement the reaction to the beat here
        Debug.Log($"Glider_A reacting to beat {triggerBeats}");

         // 光る
        // if (rend != null)
        // {
        //     rend.material.color = flashColor;

        //     Observable.Timer(TimeSpan.FromSeconds(flashDuration))
        //         .Subscribe(_ => rend.material.color = originalColor)
        //         .AddTo(this);
        // }
    }
}

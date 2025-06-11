using UnityEngine;
using UniRx;
using System;
using System.Linq;
using System.Collections.Generic;
public class Glider : MonoBehaviour
{
    [SerializeField] private List<int> triggerBeats = new List<int> { 4, 8, 12 };

    private BeatClock clock;

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
    }
}

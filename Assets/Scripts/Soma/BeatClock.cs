using UnityEngine;
using UniRx;
using System;
using UnityEngine.UI;


public class BeatClock : MonoBehaviour
{
    private readonly Subject<int> beatSubject = new Subject<int>();
    public IObservable<int> OnBeat => beatSubject;

    private int currentBeat = 0;
    [SerializeField] private Text beatText;
    [SerializeField] private float bpm = 120f;
    void Start()
    {
        float interval = 60f / bpm / 4f;
        Observable.Interval(TimeSpan.FromSeconds(interval))
            .Subscribe(_ =>
            {
                currentBeat = (currentBeat % 16) + 1;
                beatSubject.OnNext(currentBeat);
                Debug.Log($"Beat: {currentBeat}");

                if (beatText != null)
                {
                    beatText.text = $"Beat: {currentBeat}";
                }
            })
            .AddTo(this);
    }

    void Update()
    {
        
    }
}

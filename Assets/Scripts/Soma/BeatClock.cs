using UnityEngine;
using UniRx;
using System;


public class BeatClock : MonoBehaviour
{
    private readonly Subject<int> beatSubject = new Subject<int>();
    public IObservable<int> OnBeat => beatSubject;

    private int currentBeat = 0;
    [SerializeField] private float bpm = 120f;
    void Start()
    {
        float interval = 60f / bpm / 4f;
        Observable.Interval(TimeSpan.FromSeconds(interval))
            .Subscribe(_ =>
            {
                beatSubject.OnNext(currentBeat);
                Debug.Log($"Beat: {currentBeat}");
                currentBeat = (currentBeat + 1) % 16;
            })
            .AddTo(this);
    }

    void Update()
    {
        
    }
}

using UnityEngine;
using UniRx;
using System;
using System.Linq;
using System.Collections.Generic;

public class BeatAudioTrigger : MonoBehaviour
{
    [SerializeField] private List<int> triggerBeats; // キックのタイミング

    // 現在のbeatを受け取った数字で割った剰余が0なら反応するようにする
    [SerializeField] private int beatDivisor; // 割る数
    [SerializeField] private int remain; // 追加のトリガービート

    [SerializeField] private AudioClip audioClip;
    [SerializeField] private float volume = 1.0f;
    [SerializeField] private float pitch = 1.0f;
    [SerializeField] private bool randomizePitch = false;
    [SerializeField] private float pitchVariation = 0.1f; // ピッチのランダム幅

    private BeatClock clock;
    private AudioSource _audioSource;

    private void Start()
    {
        // AudioSourceの取得・作成
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
        }

        // BeatClockの取得
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
            .Where(beat => triggerBeats.Contains(beat) || beat % beatDivisor == remain)
            .Subscribe(beat => ReactToBeat(beat))
            .AddTo(this);
    }

    private void ReactToBeat(int beat)
    {
        Debug.Log($"BeatAudioTrigger reacting to beat {beat}");
        PlayAudio();
    }

    private void PlayAudio()
    {
        if (_audioSource != null && audioClip != null)
        {
            _audioSource.Stop();
            // ピッチの設定
            if (randomizePitch)
            {
                _audioSource.pitch = pitch + UnityEngine.Random.Range(-pitchVariation, pitchVariation);
            }
            else
            {
                _audioSource.pitch = pitch;
            }

            // 音量設定
            _audioSource.volume = volume;

            // 音声再生
            _audioSource.clip = audioClip;
            _audioSource.Play();
        }
        else
        {
            Debug.LogWarning("AudioSource or AudioClip is missing!");
        }
    }

    // 実行時にトリガービートを変更するメソッド
    public void SetTriggerBeats(List<int> newTriggerBeats)
    {
        triggerBeats = newTriggerBeats;
    }

    // 特定のビートを追加
    public void AddTriggerBeat(int beat)
    {
        if (!triggerBeats.Contains(beat))
        {
            triggerBeats.Add(beat);
        }
    }

    // 特定のビートを削除
    public void RemoveTriggerBeat(int beat)
    {
        triggerBeats.Remove(beat);
    }
}
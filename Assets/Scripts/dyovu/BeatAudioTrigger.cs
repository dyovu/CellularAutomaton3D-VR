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
    private IDisposable beatSubscription; // 購読を管理
    private bool isListening = false; // 現在の購読状態

    private void Start()
    {
        // AudioSourceの取得・作成
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
        }

        // BeatClockの取得（ただし自動開始はしない）
        clock = FindFirstObjectByType<BeatClock>();
        if (clock == null)
        {
            Debug.LogError("BeatClock not found in the scene. Please ensure it is present.");
        }
    }

    // ビート購読を開始
    public void StartListening()
    {
        if (clock == null || isListening) return;

        beatSubscription = clock.OnBeat
            .Where(beat => triggerBeats.Contains(beat) || beat % beatDivisor == remain)
            .Subscribe(beat => ReactToBeat(beat))
            .AddTo(this);
        
        isListening = true;
        Debug.Log("BeatAudioTrigger started listening");
    }

    // ビート購読を停止
    public void StopListening()
    {
        if (!isListening) return;

        beatSubscription?.Dispose();
        beatSubscription = null;
        isListening = false;
        Debug.Log("BeatAudioTrigger stopped listening");
    }

    // 購読状態を取得
    public bool IsListening => isListening;

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


}
using UnityEngine;
using UniRx;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Soma
{
    public class Glider : MonoBehaviour
    {
        [SerializeField] private List<int> triggerBeats = new List<int> { 4, 8, 12 };
        [SerializeField] private Color emissionColor = Color.white;
        [SerializeField] private float emissionMaxIntensity = 2f;
        [SerializeField] private float fadeDuration = 0.5f;
        [SerializeField] private AudioClip se;

        private BeatClock clock;
        private Material _material;
        private AudioSource _audioSource;

        private void Start()
        {
            _material = GetComponent<Renderer>().material;
            clock = FindFirstObjectByType<BeatClock>();
            if (clock != null)
            {
                Initialize(clock);
            }
            else
            {
                Debug.LogError("BeatClock not found in the scene. Please ensure it is present.");
            }
            _audioSource = GetComponent<AudioSource>();
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

            PlaySE();

            // 光らせた後フェードアウト
            _material.EnableKeyword("_EMISSION");
            float time = 0f;

            Observable.EveryUpdate()
            .TakeWhile(_ => time < fadeDuration)
            .Subscribe(_ =>
            {
                time += Time.deltaTime;
                float intensity = Mathf.Lerp(emissionMaxIntensity, 0f, time / fadeDuration);
                _material.SetColor("_EmissionColor", emissionColor * intensity);
            },
            () =>
            {
                _material.SetColor("_EmissionColor", Color.black);
                _material.DisableKeyword("_EMISSION");
            })
            .AddTo(this);
        }

        private void PlaySE()
        {
            // サウンドを再生
            if (_audioSource != null)
            {
                _audioSource.PlayOneShot(se);
            }
        }



        private void OnDestroy()
        {
            if (_material != null)
            {
                Destroy(_material);
            }
        }
    }
}

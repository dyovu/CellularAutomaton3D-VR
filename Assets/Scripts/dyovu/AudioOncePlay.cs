using UnityEngine;

public class AudioOncePlay : MonoBehaviour
{
    [SerializeField] private AudioClip audioClip;
    [SerializeField] private float volume = 1.0f;
    [SerializeField] private float pitch = 1.0f;
    

    private AudioSource _audioSource;

    private void Start()
    {
        // AudioSourceの取得・作成
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void Play()
    {
        if (_audioSource != null && audioClip != null)
        {
            // 元の設定を保存
            float originalPitch = _audioSource.pitch;
            float originalVolume = _audioSource.volume;

            // 設定を適用
            _audioSource.pitch = pitch;
            _audioSource.volume = volume;

            // 一回再生
            _audioSource.PlayOneShot(audioClip);

            // 元の設定を復元
            _audioSource.pitch = originalPitch;
            _audioSource.volume = originalVolume;
        }
        else
        {
            Debug.LogWarning("Audio clip or AudioSource is not set.");
        }
    }
}
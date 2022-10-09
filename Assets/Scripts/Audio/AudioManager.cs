using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class AudioManager
{
    private static Dictionary<ClipId, AudioClip> _clips;
    private static AudioSource _audioSource;
    private static AudioData _audioData;

    public static void Init(AudioData audioData)
    {
        _audioData = audioData;
        InitAudioDictionary();
        InitAudioSource();
    }

    private static void InitAudioSource()
    {
        var gameObject = new GameObject();
        gameObject.name = "AudioSource";
        _audioSource = gameObject.AddComponent<AudioSource>();
        _audioSource.volume = 0.5f;
        _audioSource.loop = false;
        _audioSource.playOnAwake = false;
    }

    private static void InitAudioDictionary()
    {
        _clips = new Dictionary<ClipId, AudioClip>();
        foreach (var clipData in _audioData.audioObjects)
            _clips[clipData.clipId] = clipData.clip;
    }

    public static void PlaySfx(ClipId clipId)
    {
        if (_clips.TryGetValue(clipId, out var clip))
            _audioSource.PlayOneShot(clip);
    }

    public static void PlayPositionalSfx(ClipId clipId,Vector3 position)
    {
        if (_clips.TryGetValue(clipId, out var clip))
        {
            _audioSource.transform.position = position;
            _audioSource.clip = clip;
            _audioSource.Play(); 
        }
    }
}

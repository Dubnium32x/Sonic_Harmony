﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public static class SFX {
    static Dictionary<string, AudioClip> audioClipCache = new Dictionary<string, AudioClip>();

    static AudioMixer _mixer;

    public static AudioClip Get(string path) {
        if (!audioClipCache.ContainsKey(path))
            audioClipCache[path] = Resources.Load<AudioClip>(path);

        return audioClipCache[path];
    }

    public static void PlayOneShot(AudioSource audioSource, string constantName, float volume = 1F) {
        if (string.IsNullOrEmpty(constantName)) return;
        var path = Constants.Get(constantName);

        if (_mixer == null)
            _mixer = Resources.Load<AudioMixer>("Main");

        _mixer.GetFloat("SFX Volume", out var _mixerVolume);
        _mixerVolume = 1F - (_mixerVolume / -40F);

        audioSource.PlayOneShot(
            Get(path),
            volume * _mixerVolume
        );
    }

    public static void Play(AudioSource audioSource, string constantName, float pitch = 1) {
        if (string.IsNullOrEmpty(constantName)) return;
        var path = Constants.Get(constantName);
        
        audioSource.clip = Get(path);
        audioSource.pitch = pitch;
        audioSource.Play();
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour {
    static MusicManager _current;

    AudioSource audioSourceIntro;
    AudioSource audioSourceLoop;
    float fadeSpeed = 0.333F;
    float fadeVolume = 1F;
    AudioMixer mixer;
    AudioMixerGroup mixerGroup;
    List<MusicStackEntry> musicStack = new List<MusicStackEntry>();

    MusicStackEntry musicStackEntryPrev;

    public float tempo = 1F;

    public static MusicManager current { get {
        if (_current == null)
            _current = GameObject.FindObjectOfType<MusicManager>();
        return _current;
    }}

    public MusicStackEntry musicStackEntryCurrent { get {
        MusicStackEntry musicStackEntryMax = null;
        var priorityMax = int.MinValue;
        foreach (var entry in musicStack.Where(entry => entry.priority >= priorityMax))
        {
            priorityMax = entry.priority;
            musicStackEntryMax = entry;
        }
        return musicStackEntryMax;
    }}

    public void FadeIn() {
        fadeVolume = -40;
        fadeSpeed = 0.333F;
    }

    public void FadeOut() {
        fadeVolume = 0;
        fadeSpeed = -0.333F;
    }

    // Start is called before the first frame update
    void Start() {
        mixer = Resources.Load<AudioMixer>("Main");
        mixerGroup = mixer.FindMatchingGroups("Music")[0];

        audioSourceIntro = gameObject.AddComponent<AudioSource>();
        audioSourceLoop = gameObject.AddComponent<AudioSource>();
        audioSourceLoop.loop = true;
    }

    public void Add(MusicStackEntry musicStackEntry) {
        musicStack.Add(musicStackEntry);
    }

    public void Remove(MusicStackEntry musicStackEntry) {
        musicStack.Remove(musicStackEntry);
    }

    public void Play(MusicStackEntry musicStackEntry) {
        Clear();
        musicStack.Add(musicStackEntry);
        fadeSpeed = 0;
        fadeVolume = 0;
    }

    // Update is called once per frame
    void Update() {
        var entry = musicStackEntryCurrent;
        var entryPrev = musicStackEntryPrev;

        var entryDone = (
            (audioSourceIntro.clip != null) &&
            (audioSourceLoop.clip == null) &&
            !audioSourceIntro.isPlaying
        );

        if (entryDone) {
            if (entry.fadeInAfter) FadeIn();
            musicStack.Remove(entry);
            audioSourceIntro.Stop();
            audioSourceLoop.Stop();
            audioSourceIntro.clip = null;
            audioSourceLoop.clip = null;
            return;
        }

        mixer.SetFloat("Music Pitch", tempo);
        mixer.SetFloat("Music Pitch Shift", 1F / tempo);
        mixer.SetFloat("Music Volume", fadeVolume);
        fadeVolume = Mathf.Max(
            -40,
            Mathf.Min(
                0,
                fadeVolume + fadeSpeed
            )
        );
    
        if (entry != null) {
            mixer.SetFloat("SFX Volume", entry.disableSfx ? -40 : 0);
        }

        if (entry == entryPrev) return;
        if (entry == null) return;
        musicStackEntryPrev = entry;

        entry.Init();

        audioSourceIntro.Stop();
        audioSourceLoop.Stop();
        audioSourceIntro.clip = null;
        audioSourceLoop.clip = null;

        audioSourceIntro.outputAudioMixerGroup = mixerGroup;
        audioSourceLoop.outputAudioMixerGroup = mixerGroup;

        var dspTime = AudioSettings.dspTime;
        if (entry.introClip != null) {
            audioSourceIntro.clip = entry.introClip;
            audioSourceIntro.PlayScheduled(dspTime + 0.1);
        }

        if (entry.loopClip == null) return;
        audioSourceLoop.clip = entry.loopClip;
        if (entry.introClip != null) {
            var clipDuration = (double)audioSourceIntro.clip.samples / audioSourceIntro.clip.frequency;
            audioSourceLoop.PlayScheduled(dspTime + 0.1 + clipDuration);
        } else
        {
            audioSourceLoop.Play();
        }
    }

    public void Clear() {
        for (var i = musicStack.Count - 1; i >= 0; i--) {
            if (musicStack[i].ignoreClear) continue;
            musicStack.RemoveAt(i);
        }
    }

    public class MusicStackEntry {
        bool _initDone = false;
        public bool disableSfx = false;
        public bool fadeInAfter = false;

        public bool ignoreClear = false;
        // public string mixerGroup = "Music";

        // public float timeCurrent;
        public AudioClip introClip;
        public string introPath;
        public AudioClip loopClip;
        public string loopPath;
        public int priority = 0;
        public bool resumeAfter = false;

        public void Init() {
            if (_initDone) return;
            
            if ((introClip == null) && (introPath != null))
                introClip = Resources.Load<AudioClip>(introPath);

            if ((loopClip == null) && (loopPath != null))
                loopClip = Resources.Load<AudioClip>(loopPath);
            
            _initDone = true;
        }
    }
}

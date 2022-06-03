using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour {

    /* AudioManager class; controls all audio in the game */

    public Sound[] sounds;

    private void Awake() {
        foreach (Sound S in sounds) {
            S.name = S.clip.name;
            S.source = gameObject.AddComponent<AudioSource>();
            S.source.clip = S.clip;
            S.source.loop = S.loop;
        }
    }

    public void Play(string name) {
        Sound S =  Array.Find(sounds, Sound => Sound.name == name);
        S.source.Play();
    }

    public void Stop(string name) {
        Sound S = Array.Find(sounds, Sound => Sound.name == name);
        S.source.Stop();
    }

    public Sound Get(string name) {
        return Array.Find(sounds, Sound => Sound.name == name);
    }

    public void Pause(string name) {
        Sound S = Array.Find(sounds, Sound => Sound.name == name);
        S.source.Pause();
    }

    public void UnPause(string name) {
        Sound S = Array.Find(sounds, Sound => Sound.name == name);
        S.source.UnPause();
    }

    public void StopAll() {
        foreach(Sound S in sounds) {
            S.source.Stop();
        }
    }

    public void PauseAll() {
        foreach(Sound S in sounds) {
            S.source.Pause();
        }
    }

    public void UnpauseAll() {
        foreach(Sound S in sounds) {
            S.source.UnPause();
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

static public class SoundData {
    static Dictionary<string, AudioClip> soundData = new Dictionary<string, AudioClip>();
    static Dictionary<string, AudioClip> bgmData = new Dictionary<string, AudioClip>();

    static public AudioClip GetSound(string path) {
        if (soundData.ContainsKey(path)) {
            return soundData[path];
        } else {
            AudioClip ac = Resources.Load<AudioClip>("Sound/" + path);
            if (ac) {
                soundData.Add(path, ac);
                return ac;
            } else {
                return null;
            }
        }
    }

    static public AudioClip GetBGM(string path) {
        if (bgmData.ContainsKey(path)) {
            return bgmData[path];
        } else {
            AudioClip ac = Resources.Load<AudioClip>("BGM/" + path);
            if (ac) {
                bgmData.Add(path, ac);
                return ac;
            } else {
                return null;
            }
        }
    }
}

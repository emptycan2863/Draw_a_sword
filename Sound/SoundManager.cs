using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    static GameObject thisObject = null;
    public GameObject pSoundObject = null;
    static GameObject soundObject = null;
    static List<AudioSource> soundPlayList = new List<AudioSource>();

    static AudioSource bgmMainChanel = null;
    static string BGMName = "";

    void Awake() {
        thisObject = this.gameObject;
        soundObject = pSoundObject;

        DontDestroyOnLoad(thisObject);
    }

    static public void PlaySound(string name, float volum = 1, float pitch = 1) {
        AudioClip ac = SoundData.GetSound(name);
        if (ac == null) return;
        GameObject soundGO = Instantiate(soundObject, thisObject.transform);
        AudioSource audioSource = soundGO.GetComponent<AudioSource>();
        audioSource.clip = ac;
        audioSource.volume = volum;
        audioSource.pitch = pitch;
        audioSource.Play();
        soundPlayList.Add(audioSource);
    }

    static public void PlayBGM(string name, float volum = 1, float pitch = 1) {
        AudioSource audioSource;
        if (BGMName == name) {
            audioSource = bgmMainChanel;
        } else {
            if (bgmMainChanel != null) StopBGM();
            AudioClip ac = SoundData.GetBGM(name);
            if (ac == null) return;
            GameObject soundGO = Instantiate(soundObject, thisObject.transform);
            audioSource = soundGO.GetComponent<AudioSource>();
            audioSource.clip = ac;
            BGMName = name;
            audioSource.Play();
        }

        audioSource.volume = volum;
        audioSource.pitch = pitch;
        audioSource.loop = true;
        bgmMainChanel = audioSource;
    }

    static public void StopBGM() {
        if (bgmMainChanel != null) GameObject.Destroy(bgmMainChanel.gameObject);
        bgmMainChanel = null;
        BGMName = "";
    }

    static public void SoundUpdate() {
        if (soundPlayList.Count == 0) return;
        for (int i = soundPlayList.Count - 1; i >= 0; --i) {
            if (!soundPlayList[i].isPlaying) {
                GameObject.Destroy(soundPlayList[i].gameObject);
                soundPlayList.RemoveAt(i);
            }
        }
    }
}

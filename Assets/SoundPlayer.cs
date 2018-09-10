using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour {

    AudioSource _oneSource;
    AudioSource _loopSource;
    [SerializeField]
    AudioClip _fall;
    [SerializeField]
    AudioClip _jump;
    [SerializeField]
    AudioClip _land;
    [SerializeField]
    AudioClip _grind;

    [SerializeField]
    AudioClip _trash;
    [SerializeField]
    AudioClip _boost;

    // Use this for initialization
    void Start () {
        _oneSource = gameObject.AddComponent<AudioSource>();
        _loopSource = gameObject.AddComponent<AudioSource>();
        _oneSource.priority = 0;
        _loopSource.priority = 10;
        _loopSource.loop = true;
        _loopSource.clip = _grind;
    }

    public void fall()
    {
        play(_fall);
    }

    public void jump()
    {
        play(_jump);
    }

    public void trash()
    {
        play(_trash);
    }

    public void boost()
    {
        play(_boost);
    }

    public void land()
    {
        play(_land,0.3f);
    }

    public void startGrind()
    {
        if (!_loopSource.isPlaying)
            _loopSource.Play();
    }

    public void stopGrind()
    {
        if (_loopSource.isPlaying)
            _loopSource.Stop();
    }

    void play(AudioClip c, float vol = 1.0f)
    {
        _oneSource.PlayOneShot(c, vol);
    }
}

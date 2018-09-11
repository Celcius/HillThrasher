using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class musiccontroller : MonoBehaviour {

    bool playing = true;
    AudioSource _source;

    [SerializeField]
    GameObject bg;
    float startVol;
	// Use this for initialization
	void Start () {
        _source = GetComponent<AudioSource>();
        startVol = _source.volume;
    }
	
	// Update is called once per frame
	void Update () {


    }

    public void swapVolume()
    {
        playing = !playing;

        _source.volume = playing ? startVol : 0;


    }

    public void swapBG()
    {
        bg.SetActive(!bg.activeInHierarchy);
    }
}

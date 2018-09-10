using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualPlayer : MonoBehaviour {

    MyPlayerController _controller;
    [SerializeField]
    Sprite _normalSkater;

    [SerializeField]
    Sprite _hunkeredSkater;

    [SerializeField]
    Sprite _jumpingSkater;

    [SerializeField]
    Sprite _rotatingSkater;

    [SerializeField]
    ParticleSystem[] _railEffects;


    [SerializeField]
    ParticleSystem _boostEffect;
    SpriteRenderer _renderer;

    public bool isLocal;

    bool visualGrinding = false;
    bool visualBoosting = false;

    // Use this for initialization
    void Start () {
        _controller = GetComponent<MyPlayerController>();
        _renderer = GetComponentInChildren<SpriteRenderer>();
		
        foreach(ParticleSystem system in _railEffects)
        {
            system.Stop();
        }

        _boostEffect.Stop();

    }
	
	// Update is called once per frame
	void Update () {

        if (!isLocal)
            _renderer.color = new Color(1.0f, 0, 0, _renderer.color.a);

        Sprite s = _normalSkater;
        if (_controller._grounded)
        {
            if (_controller._hunkeredDown)
                s = _hunkeredSkater;
        }
        else
        {
            s = _jumpingSkater;
        }

        _renderer.sprite = s;

        bool isGrinding = _controller.grinding();
        if(visualGrinding != isGrinding)
        { 
            foreach (ParticleSystem system in _railEffects)
            {
                if (isGrinding)
                    system.Play();
                else
                    system.Stop();
            }
            visualGrinding = isGrinding;
        }

        bool isBoosting = _controller.isBoosting();
        if(visualBoosting != isBoosting)
        {
            if (isBoosting)
                _boostEffect.Play();
            else
                _boostEffect.Stop();
            visualBoosting = isBoosting;
        }
    }
     
}

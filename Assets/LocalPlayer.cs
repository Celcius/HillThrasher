using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalPlayer : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GetComponent<VisualPlayer>().isLocal = true;
        GetComponent<MyPlayerController>().setLocal();

        followPlayer cam = Camera.main.GetComponent<followPlayer>();
        cam._player = transform;
        transform.position = cam._mainMap._startPos.position;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}

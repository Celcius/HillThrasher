using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class networkPlayer : NetworkBehaviour
{

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void OnStartLocalPlayer()
    {
        GetComponent<VisualPlayer>().isLocal = true;
        GetComponent<MyPlayerController>().isLocalPlayer = true;
        
        followPlayer cam = Camera.main.GetComponent<followPlayer>();
        cam._player = transform;
        transform.position = cam._mainMap._startPos.position;
    }

}

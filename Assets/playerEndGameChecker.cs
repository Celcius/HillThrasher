using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerEndGameChecker : MonoBehaviour {

    float finalPosX = 0;
    bool finished = false;
    float finishTime = 0;
    public bool readyToRestart;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (finalPosX == 0)
            finalPosX = Camera.main.GetComponent<followPlayer>()._mainMap._finishLine.position.x;

        bool isFinishInstant = transform.position.x >= finalPosX && !finished;

        if(isFinishInstant)
        {
            finishTime = 2.0f;
            finished = true;
        }

        if(finished)
        {
            if (finishTime > 0)
            { 
                finishTime -= Time.deltaTime;
                if(finishTime <= 0)
                {
                    readyToRestart = true;
                }
            }

            if(readyToRestart && isInputRelease())
                Application.LoadLevel(Application.loadedLevel);
        }
    }

    bool isInputRelease()
    {
        return (Input.GetKeyUp(KeyCode.Space))
            || (Input.touchCount > 0
            && (Input.GetTouch(0).phase == TouchPhase.Ended
            || Input.GetTouch(0).phase == TouchPhase.Canceled));
    }
}

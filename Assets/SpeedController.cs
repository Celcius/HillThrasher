using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedController : MonoBehaviour {

    public MyPlayerController _player;
    [SerializeField]
    float minNormalRot = -45.0f;
    [SerializeField]
    float maxNormalRot = -7;

    [SerializeField]
    float minSpeedingRot = 2.5f;
    [SerializeField]
    float maxSpeedingRot = 35;

    [SerializeField]
    float minGrindingRot = 42;
    [SerializeField]
    float maxGrindingRot = 53;

    

    // Use this for initialization
    void Start () {

        transform.up = Quaternion.Euler(0, 0, -minNormalRot + 45) * new Vector3(0.5f, 0.5f);
    }
	
	// Update is called once per frame
	void Update () {
        if (_player == null)
            return;

        float speed = _player.getCurrentSpeedVisual();

        if (speed < 0)
            return;

        if(speed == 0)
            transform.up = Quaternion.Euler(0, 0, -minNormalRot + 45) * new Vector3(0.5f, 0.5f);

        float min, max = 0;
    
        if(speed > 2.0f)
        {
            min = minGrindingRot;
            max = maxGrindingRot;
            speed -= 2.0f;
        }
        else if(speed > 1.0f)
        {
            min = minSpeedingRot;
            max = maxSpeedingRot;
            speed -= 1.0f;
        }
        else
        {
            min = minNormalRot;
            max = maxNormalRot;
        }

        speed = Mathf.Clamp(speed, 0.0f, 1.0f);
        
        float rot = -((max - min) * speed + min) + 45;
        //Debug.Log("SPEED " + speed + " " + rot);
        
        transform.up += Quaternion.Euler(0, 0, rot) * new Vector3(0.5f, 0.5f) * Time.deltaTime*5.0f;

    }


}

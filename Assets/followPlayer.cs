using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class followPlayer : MonoBehaviour {

    [SerializeField]
    public MapController _mainMap;

    [SerializeField]
    followPlayerSimple _simple;
    [SerializeField]
    public Transform _player;
    MyPlayerController _controller;
    float endY = 0;

    [SerializeField]
    DrawMinimap _minimap;
    [SerializeField]
    Vector2 cameraOffset = new Vector2(0, 0);
    [SerializeField]
    public SpeedController _speedController;
	// Use this for initialization
	void Start () {
		
	}
    float lockedAngle = 0;

	// Update is called once per frame
	void Update () {
        if (_player != null)
        {
            if (_controller == null)
                _controller = _player.GetComponent<MyPlayerController>();
            Vector3 playerPos = _player.position + new Vector3(cameraOffset.x, cameraOffset.y,0);
            transform.position = new Vector3(Mathf.Clamp(playerPos.x, maxLeftPos(), maxRightPos()), endY == 0 ? playerPos.y : endY, transform.position.z);
            if(transform.position.x == maxRightPos() && endY == 0)
            {
                endY = transform.position.y;
            }
            _simple.transform.position = new Vector3(playerPos.x-cameraOffset.x, playerPos.y-cameraOffset.y, _simple.transform.position.z);
            
            if (_controller.achievedRotation && lockedAngle == 0)
            {
                lockedAngle = Mathf.Clamp(_controller._rotation + 30, -360, 360);
            }
            else if (!_controller.achievedRotation)
                lockedAngle = 0;
            float angle = lockedAngle != 0? lockedAngle : Mathf.Clamp(_controller._rotation + _controller._visualBoostStart, -360, 360);
            _simple.setText(_controller._rotation == 0 ? "" : _controller.achievedRotation ? "Boost!" : (angle.ToString("F0") + "º"), _controller.achievedRotation ? Color.yellow : Color.white);

            _minimap.setMap(_mainMap, _player);
        }

        _minimap.UpdateMap(_mainMap.mapPoint(_player.transform.position.x > maxRightPos() ? _mainMap._finishLine.position : _player.transform.position));
    }

    float maxRightPos()
    {
        return _mainMap._finishLine.position.x - Camera.main.orthographicSize*1.5f;
    }

    float maxLeftPos()
    {
        return _mainMap._startPos.position.x + Camera.main.orthographicSize * 1.5f;
    }
}

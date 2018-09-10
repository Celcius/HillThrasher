using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MyPlayerController : MonoBehaviour
{

    [SerializeField]
    float _jumpForce = 10;


    [SerializeField]
    float _badCollisionAngle = 50;

    [SerializeField]
    float _airBaseRotRight = 30;
    [SerializeField]
    float _airRightRotIncrement;
    [SerializeField]
    float _airMaxRightRot;
    float _airCurrentRotRight;

    [SerializeField]
    float _airBaseLeftRot = 10.0f;

    public bool isLocalPlayer;

    SoundPlayer _sp;

    float _airCurrentLeftRot = 15.0f;

    [SerializeField]
    float _airMaxLeftRot = 15.0f;

    [SerializeField]
    float _airLeftRotIncrement = 1.0f;
    
    [SerializeField]
    float _minVelocity = 1.0f;

    float _obstacleTime = -1.0f;
    [SerializeField]
    float maxObstaclePenaltyTime = 1.0f;

    float _rotationBoostTime = -1.0f;
    [SerializeField]
    float maxRotationBoostSpeed = 20;
    [SerializeField]
    float maxRotationBoostTime = 3.0f;
    public bool achievedRotation = false;

    [SerializeField]
    float _maxVelocity = 5.0f;

    [SerializeField]
    float _gravityAccel = 2.0f;

    Vector2 _velocity;
    bool _rotating = false;
    float jumpRotate = -1.0f;
    
    public bool _grounded = false;

    bool _blinking = false;
    float _blinkTimer = 0.0f;

    [SerializeField]
    float _blinkTime = 1.0f;
    [SerializeField]
    float _blinkIncrement = 600;
    float _currentBlink = 0;

    [SerializeField]
    Sprite _playerSprite;

    [SerializeField]
    float _climbDecelerationRation = 2.0f;

    [SerializeField]
    MapController _currentMap;

    MapController _mainMap;
    SpriteRenderer _spriteRender;

    public bool _hunkeredDown = false;
    [SerializeField]
    float _hunkeredDownMaxSpeed;
    [SerializeField]
    float _hunkeredDownMinSpeed;

    [SerializeField]
    float _grindSpeed;

    public float _rotation = 0;

    [SerializeField]
    public float _visualBoostStart = 75;

    Rigidbody2D _body;
    // Use this for initialization
    void Start () {
        _blinking = false;
        _airCurrentLeftRot = _airBaseLeftRot;
        _airCurrentRotRight = _airBaseRotRight;
        _body = GetComponent<Rigidbody2D>();
        _spriteRender = GetComponentInChildren<SpriteRenderer>();
        _sp = GetComponent<SoundPlayer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(_blinking)
        {
            updateBlink();
            return;
        }

        checkJump();

        updateRotation();
        updateVelocity();

        if (!grinding())
            _sp.stopGrind();
    }
    void checkJump()
    {
        if (!isLocalPlayer)
        {   
            return;
        }

        if (_grounded || grinding())
        {
            if(!_hunkeredDown && isInputDown())
            {
                _hunkeredDown = true;
                
            }
            //else
            if(/*_hunkeredDown && *//*isInputRelease()  isInputDown()|| (grinding() && isInputDown())*/isInputDown())
            {
                jump();
            }
        } else {
            bool rot = (isInputDown() || isInputHold());
            if(rot != _rotating)
            {

                if(_rotating)
                    _airCurrentLeftRot = _airBaseLeftRot;
                else 
                    _airCurrentRotRight = _airBaseRotRight;
            }

            _rotating = rot;
        }
    }

    void jump(float mod = 1.0f)
    {
        Vector3 diagonal = transform.up + transform.right;
        diagonal = new Vector3(diagonal.x * 0.4f, diagonal.y * 0.6f);
        _velocity = new Vector2(diagonal.x, diagonal.y).normalized * (_jumpForce + _velocity.magnitude * 0.8f* mod);
        _grounded = false;
        _hunkeredDown = false;
        _currentMap = null;

        _sp.jump();
    }

    void weakJump()
    {
        if(isInputDown() || isInputRelease())
        {
            jump(1.1f);
            return;
        }
        Vector3 diagonal = Vector3.up + Vector3.right;
        _velocity = new Vector2(diagonal.x, diagonal.y).normalized * (_jumpForce/2.0f+_velocity.magnitude);
        _grounded = false;
        _hunkeredDown = false;
        _currentMap = null;

        _sp.jump();
    }

    bool isInputRelease()
    {
        return (Input.GetKeyUp(KeyCode.Space))
            || (Input.touchCount > 0 
            && (Input.GetTouch(0).phase == TouchPhase.Ended
            || Input.GetTouch(0).phase == TouchPhase.Canceled));
    }

    bool isInputDown()
    {
        return (Input.GetKeyDown(KeyCode.Space))
            || (Input.touchCount > 0
            && (Input.GetTouch(0).phase == TouchPhase.Began));
    }

    bool isInputHold()
    {
        return Input.GetKey(KeyCode.Space)
              || (Input.touchCount > 0
            && (Input.GetTouch(0).phase == TouchPhase.Moved
            || Input.GetTouch(0).phase == TouchPhase.Stationary));
    }

    void updateRotation() {


        if (!_grounded)
        {

            float angle = -_airCurrentRotRight;
            if (_rotating)
            {
                if (jumpRotate >= 0.0f)
                {
                    jumpRotate -= Time.deltaTime;

                    if (jumpRotate <= 0.0f)
                    {
                        jumpRotate = -1.0f;
                        _rotating = false;
                    }
                }
                angle = _airCurrentLeftRot;
                _airCurrentLeftRot = Mathf.Min(_airCurrentLeftRot + _airLeftRotIncrement * Time.deltaTime, _airMaxLeftRot);
                
                    // Here
                _rotation += angle * Time.deltaTime;
                bool startRot = achievedRotation;
                achievedRotation = achievedRotation || Mathf.Abs(_rotation) + _visualBoostStart > 360;
                if (achievedRotation && startRot != achievedRotation)
                    _sp.boost();
                transform.Rotate(Vector3.forward * angle * Time.deltaTime);
                //transform.up += Quaternion.Euler(0,0, angle) * transform.up * mult * Time.deltaTime;
            }
            else
            {
                if(_mainMap == null)
                    _mainMap = Camera.main.GetComponent<followPlayer>()._mainMap;
                Vector2 goal2d = _mainMap.getInterpolatedForward(transform.position.x).normalized;
                Vector3 goal = new Vector3(goal2d.x, goal2d.y, transform.right.z);
                Vector3 dir = goal - transform.right;
            //    Debug.Log(dir.magnitude);
                if (dir.magnitude > 0.01f)
                {
                   transform.right += dir * Time.deltaTime;
                }
                else
                    transform.right = goal;
                // _airCurrentRotRight = Mathf.Min(_airCurrentRotRight + _airRightRotIncrement * Time.deltaTime, _airMaxRightRot);

            }



        }
        else if (_currentMap != null) { 
           adjustRotationToMap(false);
        }
    }

    void adjustRotationToMap(bool instant)
    {
        float instantMod = instant ? 20 :  10;
        Vector2 goal = _currentMap.getInterpolatedForward(transform.position.x).normalized;
        Vector3 goal3 = new Vector3(goal.x, goal.y, transform.right.z);
        transform.right +=
            (goal3
            - transform.right) * Time.deltaTime * instantMod;
    }
    void updateVelocity() {

        if(!_grounded)
        {
            _velocity = new Vector2(_velocity.x, _velocity.y - _gravityAccel * Time.deltaTime);
        }
        else
        {
            Vector2 dir = _currentMap.getInterpolatedForward(transform.position.x).normalized;
           
            float angle = Vector3.Angle(Vector3.right, transform.right) * (_currentMap.getRightY(transform.position.x) < transform.position.y ? -1 : 1);

            if(_rotationBoostTime > 0)
            { 
                _rotationBoostTime -= Time.deltaTime;
                if (_rotationBoostTime <= 0)
                    _rotationBoostTime = 0;
            }

            if(_obstacleTime > 0)
            {
                _obstacleTime -= Time.deltaTime;
                if (_obstacleTime <= 0)
                    _obstacleTime = 0;
            }

            float maxVelocity = _obstacleTime > 0 ? _minVelocity :
                            _rotationBoostTime > 0 ? maxRotationBoostSpeed :
                            grinding() ? _grindSpeed 
                           : _hunkeredDown ? _hunkeredDownMaxSpeed : _maxVelocity;

            float minVelocity = _obstacleTime > 0 ? 0 :
                            _rotationBoostTime > 0 ? maxRotationBoostSpeed :
                            grinding() ? _grindSpeed 
                           : _hunkeredDown ? _hunkeredDownMinSpeed : _minVelocity;

            float accel =(_minVelocity - maxVelocity) / 180 * angle;
            if (accel < 0)
                accel *= _climbDecelerationRation;
          
            _velocity = dir * Mathf.Clamp(_velocity.magnitude+accel * Time.deltaTime, _minVelocity, maxVelocity);
          //  Debug.Log("Min " + _minVelocity + "Max " + maxVelocity + " " + _obstacleTime);
            // Debug.Log(angle + "   " + accel + "  " + _velocity.magnitude);
        }
        
        Vector3 pos = new Vector3(transform.position.x + _velocity.x * Time.deltaTime,
            transform.position.y + _velocity.y * Time.deltaTime,
            transform.position.z);

        if (_grounded)
        { 
            pos.y = _currentMap.getYForX(pos.x);

            if (_currentMap.leftMap(pos.x))
            {
                _grounded = false;
                _currentMap = null;
            }   
        }
        else
        {
            MapController map = Camera.main.GetComponent<followPlayer>()._mainMap;
            if (transform.position.y <= map.getYForX(transform.position.x) -1.0f)
            {
                linkToMap(map);
                pos = transform.position;
            }
        }

        transform.position = pos;
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if(!_grounded && other.gameObject.tag == "Terrain")
        {
             MapController map = other.gameObject.GetComponent<MapController>();
            if(!map.leftMap(transform.position.x))
            {
                bool isMain = map == Camera.main.GetComponent<followPlayer>()._mainMap;

                if (!map.badCollision(transform.right, transform.position.x, _badCollisionAngle))
                { 
                    _grounded = true;
                    if(achievedRotation)
                    {
                        _rotationBoostTime = maxRotationBoostTime;
                    }
                    achievedRotation = false;
                    _rotation = 0;
                    
                    _currentMap = map;
                    adjustRotationToMap(true);

                    if (isMain)
                        _sp.land();
                    else
                        _sp.startGrind();
                }
                else if (isMain)
                {
                    _rotationBoostTime = 0;
                    blink();
                }
            }
        }
        else if(other.gameObject.tag == "Obstacle")
        {
            _obstacleTime = maxObstaclePenaltyTime;
            _rotationBoostTime = 0;
            obstacle o = other.gameObject.GetComponent<obstacle>();
            _velocity = _velocity.normalized * 0;
            _sp.trash();
            o.destroy();
        }
        else if(other.gameObject.tag == "ramp")
        {
            if(_grounded && _currentMap == _mainMap)
            {
                weakJump();
            }
        }
    }

    void updateBlink()
    {
        _blinkTimer -= Time.deltaTime;

        if(_blinkTimer <= 0)
        {
            _blinking = false;
            _spriteRender.color = Color.white;
        }
        else
        {
            if(_spriteRender.color.a <= 0)
            {
                _currentBlink = _blinkIncrement;
            }
            else if(_spriteRender.color.a >= 1.0f)
            {
                _currentBlink = -_blinkIncrement;
            }

            _spriteRender.color += new Color(0, 0, 0, _currentBlink * Time.deltaTime);
        }
    }
    
   void blink()
    {
        _currentMap = Camera.main.GetComponent<followPlayer>()._mainMap;
        linkToMap(_currentMap);
        _velocity = _velocity.normalized * _minVelocity;

        _currentBlink = -_blinkIncrement;
        _blinkTimer = _blinkTime;
        _blinking = true;

        _sp.fall();
    }

    void linkToMap(MapController controller)
    {
        _currentMap = controller;
        _grounded = true;
        achievedRotation = false;
        _rotation = 0;
        transform.position = new Vector3(transform.position.x, _currentMap.getYForX(transform.position.x), transform.position.z);
        Vector2 goal = _currentMap.getInterpolatedForward(transform.position.x).normalized;
        Vector3 goal3 = new Vector3(goal.x, goal.y, transform.right.z);
        transform.right += (goal3 - transform.right);
    }

    public bool isBoosting()
    {
        return _rotationBoostTime > 0;
    }
    public bool grinding()
    {
        return _grounded && _currentMap != null && _currentMap != Camera.main.GetComponent<followPlayer>()._mainMap;
    }

    public void setLocal()
    {
        isLocalPlayer = true;
        Camera.main.GetComponent<followPlayer>()._speedController._player = this;
    }

    public float getCurrentSpeedVisual()
    {
        if (!_grounded && _blinkTimer <= 0.0f)
            return -1.0f;

        if (_blinkTimer > 0.0f)
            return 0.0f;

        if (grinding() || _rotationBoostTime > 0)
        {
            return 2.0f + _velocity.magnitude / _grindSpeed;
        }
           
        
        if (_hunkeredDown)
            return 1.0f + (_velocity.magnitude - _hunkeredDownMinSpeed) / (_hunkeredDownMaxSpeed - _hunkeredDownMinSpeed);
        else
            return (_velocity.magnitude - _minVelocity) / (_maxVelocity - _minVelocity);
    }
}



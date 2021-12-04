using RootMotion.FinalIK;
using UnityEngine;
using System;
using System.IO;
using SimpleJSON;

public class PlayerController : Bolt.EntityBehaviour<ISniperPlayerState>
{
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float sprintSpeed = 20f;
    [SerializeField] private float jumpForce = 4f;
    [SerializeField] private float airControl = 0.5f;
    [SerializeField] private float lookSensitivity = 3f;
    [SerializeField] private float aimSensitivity = 1f;
    [SerializeField] private float maxAngle;
    
    [HideInInspector] public float defaultPlayerFOV;
    public float velocityMag;

    private PlayerMotor motor;
    private AimIK aimIK;
    private Animation camAnimator;
    private WeaponManager3 wManager;
    private float mSensitivity;
    private float movementSpeed;

    private float boneV;

    private bool bCanRun;
    private bool bCanJump;
    private bool m_invertMouse;

    private string configFilePath;
    private JSONObject configJSON;

    public WeaponBase activeWeapon
    {
        get { return wManager.activeWeapon; }
    }

    private void Awake()
    {
        motor = GetComponent<PlayerMotor>();
        aimIK = GetComponentInChildren<AimIK>();
    }

    private void Start()
    {
        configFilePath = Application.persistentDataPath + "/Settings.json";

        if (File.Exists(configFilePath))
        {
            configJSON = new JSONObject();

            string jSONString = File.ReadAllText(configFilePath);
            configJSON = JSON.Parse(jSONString) as JSONObject;
        }

        lookSensitivity = configJSON["Mouse_General_Sensitivity"];
        aimSensitivity = configJSON["Mouse_Aim_Sensitivity"];
        m_invertMouse = configJSON["Mouse_Invert"];

        #region PlayerPrefs Region
        /*if (PlayerPrefs.HasKey("MouseGeneralSensitivity"))
            lookSensitivity = PlayerPrefs.GetFloat("MouseGeneralSensitivity");

        if (PlayerPrefs.HasKey("MouseAimingSensitivity"))
            aimSensitivity = PlayerPrefs.GetFloat("MouseAimingSensitivity");

        if (PlayerPrefs.HasKey("MouseInvertStatus"))
        {
            string m_invStatus = PlayerPrefs.GetString("MouseInvertStatus").ToLower();
            m_invertMouse = Convert.ToBoolean(m_invStatus);
        }*/
        #endregion

        if (entity.isOwner)
        {
            camAnimator = CameraController.instance.GetComponent<Animation>();
            defaultPlayerFOV = CameraController.instance.myCamera.fieldOfView;
        }

        wManager = GetComponent<WeaponManager3>();
    }

    public override void Attached()
    {
        state.SetTransforms(state.Transform, transform);
        state.SetAnimator(GetComponentInChildren<Animator>());

        state.AddCallback("playerIKWeight", OnSprint);

        state.OnFiring += OnFire;
    }

    //This is like Late Update
    public override void SimulateOwner()
    {
        if (PauseMenu.isOn)
            return;

        if (state.IsDead)
            return;

        //Movement region
        #region
        float _xMov = Input.GetAxis("Horizontal");
        float _zMov = Input.GetAxis("Vertical");

        if (state.IsRunning)
        {
            _xMov *= 0.3f;
        }

        //Animation
        Vector2 input = new Vector2(_xMov, _zMov).normalized;

        state.Horizontal = input.x;
        state.Vertical = input.y;

        Vector3 _moveHorizontal = transform.right * _xMov;
        Vector3 _moveVertical = transform.forward * _zMov;

        Vector3 _velocity = Vector3.zero;

        //Final Movement
        if (motor.CheckGrounded())
        {
            movementSpeed = state.IsRunning ? sprintSpeed : moveSpeed;

            _velocity = (_moveHorizontal + _moveVertical) * movementSpeed;
            velocityMag = _velocity.magnitude;

            //Running (Player can only run forward, not backward and not aiming)
            if (Input.GetKey(KeyCode.LeftShift) && _zMov > 0.2f && (bCanRun && !activeWeapon.ironSightFlag))
            {
                state.IsRunning = true;

                if(entity.isOwner)
                    state.playerIKWeight = Mathf.SmoothDamp(state.playerIKWeight, 0f, ref boneV, 0.1f);
            }
            else
            {
                state.IsRunning = false;

                if (entity.isOwner)
                    state.playerIKWeight = Mathf.SmoothDamp(state.playerIKWeight, 1f, ref boneV, 0.3f);
            }

            //Crouching
            if (Input.GetKey(KeyCode.C))
            {
                _velocity /= 2f;
                state.IsCrouching = true;
                bCanRun = false;
                //bCanJump = false;

                //Center.y = -0.385277
                //Height = 1.228116
                //Cam.y = -0.62
            }
            else
            {
                state.IsCrouching = false;
                bCanRun = true;
                //bCanJump = true;
            }

            //Apply movement
            motor.Move(_velocity);
        }

        if (activeWeapon.WeaponIsFiring())
            bCanRun = false;

        #endregion

        //Jump region
        #region
        Vector3 _jumpForce = Vector3.zero;

        if(Input.GetButtonDown("Jump") && (motor.CheckGrounded() && bCanJump))
        {
            state.IsJumping();
            _jumpForce = new Vector3(0, jumpForce, 0);

            if (entity.isOwner)
            {
                camAnimator.Play(CameraController.instance.camJumpAnimClip);
                camAnimator[CameraController.instance.camJumpAnimClip].speed = 5f;
            }
        }

        motor.Jump(_jumpForce);
        #endregion

        //Rotation region
        #region
        var _yRot = Input.GetAxis("Mouse X");
        var _xRot = Input.GetAxis("Mouse Y");

        if( (activeWeapon != null) && activeWeapon.isActiveAndEnabled)
            mSensitivity = activeWeapon.ironSightFlag ? aimSensitivity : lookSensitivity;

        var _rotation = new Vector3(0f, _yRot, 0f) * mSensitivity;

        motor.Rotate(_rotation);

        var _cameraRotationX = 0f;

        if(m_invertMouse)
            _cameraRotationX = -_xRot * mSensitivity;
        else
            _cameraRotationX = _xRot * mSensitivity;

        motor.RotateCamera(_cameraRotationX);
        #endregion
    }

    #region
    /*void Update()
    {
        if (state.IsRunning)
        {
            for (int i = 0; i < aimIK.solver.bones.Length; i++)
            {
                state.playerBones[i] = Mathf.SmoothDamp(aimIK.solver.bones[i].weight, 0f, ref boneV, 0.1f);
                aimIK.solver.bones[i].weight = state.playerBones[i];
                //aimIK.solver.bones[i].weight = Mathf.SmoothDamp(aimIK.solver.bones[i].weight, )
            }
        }
        else
        {
            for (int i = 0; i < aimIK.solver.bones.Length; i++)
            {
                state.playerBones[i] = Mathf.SmoothDamp(aimIK.solver.bones[i].weight, 1f, ref boneV, 0.4f);
                aimIK.solver.bones[i].weight = state.playerBones[i];
            }
        }
    }*/
    #endregion

    void OnFire()
    {
        if (activeWeapon != null && activeWeapon.isActiveAndEnabled)
            activeWeapon.StartCoroutine(activeWeapon.MuzzleFlashTimerTP(0.02f));

        activeWeapon.TPFire();
    }

    void OnSprint()
    {
        if (entity.isOwner)
        {
            if (state.playerIKWeight < 0.001f)
                state.playerIKWeight = 0f;

            if (state.playerIKWeight > 0.999f)
                state.playerIKWeight = 1f;
        }

        aimIK.solver.SetIKPositionWeight(state.playerIKWeight);
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("I just Landed with magnitude: "+collision.relativeVelocity.magnitude);
        //Debug.Log("I just Landed with force");

        if(!state.IsDead)
            Landed(collision.relativeVelocity.magnitude);
    }

    private void OnCollisionStay(Collision collision)
    {
        foreach (ContactPoint cp in collision.contacts)
        {
            if (Vector3.Angle(cp.normal, Vector3.up) < maxAngle)
                bCanJump = true;
            else
                bCanJump = false;
        }
    }

    void Landed(float magnitude)
    {
        if (entity.isOwner)
        {
            if(magnitude > 4f)
            {
                camAnimator.Play(CameraController.instance.camJumpAnimClip);
                camAnimator[CameraController.instance.camJumpAnimClip].speed = 5f;
            }

            if (magnitude > 8f)
            {
                camAnimator.Play(CameraController.instance.camHardLandAnimClip);
                camAnimator[CameraController.instance.camHardLandAnimClip].speed = 3f;
            }
        }
    }
}

#region
/*public class PlayerController : Bolt.EntityBehaviour<ISniperPlayerState>
{
    float _xMov;
    float _zMov;

    PlayerMotor _motor;

    private void Awake()
    {
        _motor = GetComponent<PlayerMotor>();
    }

    public override void Attached()
    {
        state.SetTransforms(state.Transform, transform);
    }

    private void Update()
    {
        PollKeys();
    }

    void PollKeys()
    {
        _xMov = Input.GetAxis("Horizontal");
        _zMov = Input.GetAxis("Vertical");
    }

    public override void SimulateController()
    {
        PollKeys();

        ISniperPlayerCommandInput input = SniperPlayerCommand.Create();

        input.Horizontal = _xMov;
        input.Vertical = _zMov;

        entity.QueueInput(input);
    }

    public override void ExecuteCommand(Command command, bool resetState)
    {
        SniperPlayerCommand cmd = (SniperPlayerCommand)command;

        if (resetState)
            _motor.SetState(cmd.Result.Position);
        else
        {
            PlayerMotor.State motorState = _motor.Move(cmd.Input.Horizontal, cmd.Input.Vertical);

            cmd.Result.Position = motorState.position;
        }
    }
}*/
#endregion
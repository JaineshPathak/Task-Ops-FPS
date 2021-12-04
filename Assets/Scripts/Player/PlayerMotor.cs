using System.Collections;
using System.Collections.Generic;
using Bolt;
using UnityEngine;

[RequireComponent (typeof(Rigidbody))]
public class PlayerMotor : Bolt.EntityBehaviour<ISniperPlayerState>
{
    private float cameraRotationX = 0f;
    private float currentCameraRotationX = 0f;

    [SerializeField] private float distanceToGround = 0.2f;
    [SerializeField] private float cameraRotationLimit = 85f;

    private Vector3 velocity = Vector3.zero;
    private Vector3 rotation = Vector3.zero;
    private Vector3 jumpForce = Vector3.zero;

    private float crouchCollisionHeight = 1.228116f;
    private Vector3 crouchYCollisionHeight = new Vector3(0, -0.385277f, 0);
    private Vector3 defaultCamPos;
    private Vector3 crouchCamPos;

    private float defCollisionHeight;
    private Vector3 defCollisionCenter;

    private float collHV;
    private Vector3 collVectorV;
    private Vector3 camVectorV;

    private Rigidbody _rb;
    private Camera cam;
    private Animation camAnimator;
    private CapsuleCollider capColl;

    public float curCamRotX
    {
        get { return currentCameraRotationX; }
        set { currentCameraRotationX = value; }
    }

    public Vector3 rigidBodyVelocity
    {
        get { return _rb.velocity; }
    }

    /*public override void Attached()
    {
        capColl = GetComponent<CapsuleCollider>();

        if (capColl != null)
        {
            if (entity.isOwner)
            {
                state.DefaultCrouchHeight = capColl.height;
                state.NewCrouchHeight = state.DefaultCrouchHeight / 2f;
            }
        }
    }*/

    void Start()
    {
        _rb = GetComponent<Rigidbody>();

        if (entity.isOwner)
        {
            cam = CameraController.instance.myCamera;
            defaultCamPos = CameraController.instance.myCamera.transform.localPosition;
            crouchCamPos = new Vector3(defaultCamPos.x, -0.62f, defaultCamPos.z);
            //LookAtTransform = cam.transform.Find("LookAtTransform");
        }

        capColl = GetComponent<CapsuleCollider>();
        if (capColl != null)
        {
            defCollisionHeight = capColl.height;
            defCollisionCenter = capColl.center;
        }
        //cam = GetComponentInChildren<Camera>();
    }

    public Vector3 Velocity
    {
        get { return _rb.velocity; }
    }

    public void Move(Vector3 _velocity)
    {
        velocity = _velocity;
    }

    public void Rotate(Vector3 _rotation)
    {
        rotation = _rotation;
    }

    public void RotateCamera(float _cameraRotationX)
    {
        cameraRotationX = _cameraRotationX;
    }

    public void Jump(Vector3 _jumpForce)
    {
        jumpForce = _jumpForce;
    }

    private void FixedUpdate()
    {
        PerformMovement();
        PerformRotation();

        if (!CheckGrounded())
            CheckGroundAngle();
    }

    void PerformMovement()
    {
        if (PauseMenu.isOn)
            return;

        if (state.IsDead)
            return;

        if (velocity != Vector3.zero)
            _rb.MovePosition(_rb.position + velocity * BoltNetwork.FrameDeltaTime);

        if (state.IsCrouching)
        {
            capColl.height = Mathf.SmoothDamp(capColl.height, crouchCollisionHeight, ref collHV, 0.2f);
            capColl.center = Vector3.SmoothDamp(capColl.center, crouchYCollisionHeight, ref collVectorV, 0.2f);

            if (entity.isOwner)
                CameraController.instance.myCamera.transform.localPosition = Vector3.SmoothDamp(CameraController.instance.myCamera.transform.localPosition, crouchCamPos, ref camVectorV, 0.1f);
        }
        else
        {
            capColl.height = Mathf.SmoothDamp(capColl.height, defCollisionHeight, ref collHV, 0.2f);
            capColl.center = Vector3.SmoothDamp(capColl.center, defCollisionCenter, ref collVectorV, 0.2f);

            if (entity.isOwner)
                CameraController.instance.myCamera.transform.localPosition = Vector3.SmoothDamp(CameraController.instance.myCamera.transform.localPosition, defaultCamPos, ref camVectorV, 0.1f);
        }

        if (jumpForce != Vector3.zero)
            _rb.velocity = jumpForce;

        //rb.AddForce(jumpForce /* * Time.fixedDeltaTime*/, ForceMode.Impulse);
    }

    void PerformRotation()
    {
        if (PauseMenu.isOn)
            return;

        if (state.IsDead)
            return;

        _rb.MoveRotation(_rb.rotation * Quaternion.Euler(rotation));
        //transform.localRotation = transform.localRotation * Quaternion.Euler(rotation);
        if (cam != null)
        {
            //cam.transform.Rotate(-cameraRotation);

            //New Camera Rotation code
            currentCameraRotationX -= cameraRotationX;
            currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);

            cam.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);


            /*RaycastHit hit;
            if(Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, Mathf.Infinity))
            {
                Debug.Log(hit.point);
                state.camPos = hit.point;
            }*/

            state.camPos = cam.transform.Find("LookAtTarget").position;

            //state.camPitch = currentCameraRotationX;
        }
    }

    private void CheckGroundAngle()
    {
        RaycastHit hitInfo;
        if (Physics.SphereCast(transform.position, capColl.radius, Vector3.down, out hitInfo, ((capColl.height / 2f) - capColl.radius) + distanceToGround, Physics.AllLayers, QueryTriggerInteraction.Ignore))
        {
            if (Mathf.Abs(Vector3.Angle(hitInfo.normal, Vector3.up)) < 85f)
            {
                _rb.velocity = Vector3.ProjectOnPlane(_rb.velocity, hitInfo.normal);
            }
        }
    }

    /*public void Crouch()
    {
        capColl.height = Mathf.Lerp(capColl.height, state.NewCrouchHeight, 10f * BoltNetwork.frameDeltaTime);
    }

    public void UnCrouch()
    {
        capColl.height = Mathf.Lerp(state.NewCrouchHeight, state.DefaultCrouchHeight, 10f * BoltNetwork.frameDeltaTime);
    }*/

    public bool CheckGrounded()
    {
        RaycastHit hit;
        //return Physics.Raycast(transform.position, Vector3.down, distanceToGround);
        return Physics.SphereCast(transform.position, capColl.radius, Vector3.down, out hit, ((capColl.height / 2f) - capColl.radius) + distanceToGround, Physics.AllLayers, QueryTriggerInteraction.Ignore);
    }
}

#region
/*public class PlayerMotor : MonoBehaviour
{
    public struct State
    {
        public Vector3 position;
    }

    [SerializeField] float movingSpeed = 10f;

    State _state;
    Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _state = new State();
        _state.position = transform.localPosition;
    }

    public void SetState(Vector3 position)
    {
        _state.position = position;

        transform.localPosition = _state.position;
    }

    void Move(Vector3 velocity)
    {
        _rb.MovePosition(_rb.position + velocity * BoltNetwork.frameDeltaTime);

        _state.position = transform.localPosition;
    }

    public State Move(float Horizontal, float Vertical)
    {
        Vector3 moveHorizontal = transform.right * Horizontal;
        Vector3 moveVertical = transform.forward * Vertical;

        _state.position = (moveHorizontal + moveVertical) * movingSpeed;

        Move(_state.position);

        //DetectTunneling();

        _state.position = transform.localPosition;

        return _state;
    }
}*/
#endregion
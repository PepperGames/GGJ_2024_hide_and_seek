using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

public abstract class ThirdPersonControllerNew : MonoBehaviourPun
{
    private float mouseX;
    private float mouseY;
    public float rotationSpeed = 5f;
    public float pitchRangeTop = 80f;
    public float pitchRangeBot = -80f;
    protected float pitch = 0f;
    public CameraMode _cameraMode = CameraMode.Hard;
    [SerializeField] protected GameObject _mainCamera;
    [SerializeField] protected Transform _startCameraPosition;

    [SerializeField] protected Rigidbody rigidBody;

    public float movementSpeed = 5f;

    public float jumpForce = 5;
    [SerializeField] protected bool canDoubleJump;
    protected bool isGrounded = false;

    public float groundedRadius = 0.28f;
    public float groundedOffset = 0.8f;
    public LayerMask groundLayers;

    [SerializeField] private bool _canRotateCamera = true;
    [SerializeField] private bool _canRotateCharacter = true;
    [SerializeField] private bool _canMove = true;

    public Transform target;
    public float distance = 5.0f;
    public float xSpeed = 120.0f;
    public float ySpeed = 120.0f;

    public float yMinLimit = -20f;
    public float yMaxLimit = 80f;

    public float distanceMin = .5f;
    public float distanceMax = 15f;

    public Vector3 _offset;

    public UnityEvent OnIdle;
    public UnityEvent OnMove;
    public UnityEvent OnJump;
    public UnityEvent OnDoubleJump;
    public UnityEvent OnNotGrounded;
    public UnityEvent OnLanding;

    private void Start()
    {
        StartCoroutine(LandingCheck());
        Cursor.lockState = CursorLockMode.Locked;
        if (!photonView.IsMine)
        {
            enabled = false;
        }
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            GroundedCheck();
            RotateCamera();
            RotateCharacter();
            Move();
            Jump();

            if (!isGrounded)
            {
                OnNotGrounded?.Invoke();
            }
        }
    }

    private void RotateCamera()
    {
        if (!_canRotateCamera)
            return;

        if (_cameraMode == CameraMode.Hard)
        {
            RotateCameraWithCharacter();
        }
        else
        {
            RotateCameraAroundCharacter();
        }
    }

    public void RotateCameraWithCharacter()
    {
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");

        pitch -= mouseY * rotationSpeed;
        pitch = Mathf.Clamp(pitch, pitchRangeBot, pitchRangeTop);

        _mainCamera.transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    public void RotateCameraAroundCharacter()
    {
        distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel"), distanceMin, distanceMax);

        mouseX += Input.GetAxis("Mouse X") * xSpeed * distance * 0.02f;
        mouseY -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

        mouseY = Mathf.Clamp(mouseY, yMinLimit, yMaxLimit);

        Quaternion rotation = Quaternion.Euler(mouseY, mouseX, 0);

        RaycastHit hit;
        if (Physics.Linecast(target.position, transform.position, out hit, 7/*дальность вроде*/, QueryTriggerInteraction.Collide))
        {
            distance -= hit.distance;
        }
        Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
        Vector3 position = rotation * negDistance + target.position + _offset;

        _mainCamera.transform.rotation = rotation;
        _mainCamera.transform.position = position;
    }

    private void RotateCharacter()
    {
        if (!_canRotateCharacter)
            return;

        transform.Rotate(Vector3.up * mouseX * rotationSpeed);
    }

    private void Move()
    {
        if (!_canMove)
        {
            OnIdle?.Invoke();
            return;
        }

        Vector3 playerMovementInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
        Vector3 moveVector = transform.TransformDirection(playerMovementInput) * movementSpeed;
        if (moveVector != Vector3.zero)
        {
            OnMove?.Invoke();
        }
        else
        {
            OnIdle?.Invoke();
        }

        rigidBody.velocity = new Vector3(moveVector.x, rigidBody.velocity.y, moveVector.z);
    }

    private void Jump()
    {
        if (Input.GetKeyDown("space"))
        {
            if (isGrounded)
            {
                rigidBody.velocity = Vector3.up * jumpForce;
                canDoubleJump = true;
                OnJump?.Invoke();
            }
            else if (canDoubleJump)
            {
                rigidBody.velocity = Vector3.up * jumpForce;
                canDoubleJump = false;
                OnJump?.Invoke();
                //OnDoubleJump?.Invoke();
            }
        }
    }

    private void GroundedCheck()
    {
        // set sphere position, with offset
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - groundedOffset,
            transform.position.z);

        isGrounded = Physics.CheckSphere(spherePosition, groundedRadius, groundLayers,
            QueryTriggerInteraction.Ignore);

    }

    private IEnumerator LandingCheck()
    {
        while (true)
        {
            bool prevIsGrounded = isGrounded;
            yield return new WaitForSeconds(ConstantsHolder.LANDING_CHECK_TIME);
            if (!prevIsGrounded && isGrounded)
            {
                OnLanding?.Invoke();
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        if (isGrounded) Gizmos.color = transparentGreen;
        else Gizmos.color = transparentRed;

        // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
        Gizmos.DrawSphere(
            new Vector3(transform.position.x, transform.position.y - groundedOffset, transform.position.z),
            groundedRadius);
    }

    public void EnableRotateCamera()
    {
        _canRotateCamera = true;
    }
    public void DisableRotateCamera()
    {
        _canRotateCamera = false;
    }

    public void EnableRotateCharacter()
    {
        _canRotateCharacter = true;
    }
    public void DisableRotateCharacter()
    {
        _canRotateCharacter = false;
    }

    public void EnableMove()
    {
        _canMove = true;
    }
    public void DisableMove()
    {
        _canMove = false;
    }

    public void ChangeCameraMode(CameraMode cameraMode)
    {
        switch (cameraMode)
        {
            case CameraMode.Hard:
                Vector3 directionToCamera = transform.position - _mainCamera.transform.position;
                directionToCamera.y = 0;
                Quaternion rotation = Quaternion.LookRotation(directionToCamera);
                transform.rotation = Quaternion.Euler(0, rotation.eulerAngles.y, 0);
                _mainCamera.transform.position = _startCameraPosition.position;
                break;

            case CameraMode.FreelyRotating:
                break;
            default:
                break;
        }
        _cameraMode = cameraMode;
    }
}

public enum CameraMode
{
    Hard,
    FreelyRotating
}
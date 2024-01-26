using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

public abstract class ThirdPersonControllerNew : MonoBehaviourPun
{
    private bool _canRotateCamera = true;
    private bool _canRotateCharacter = true;
    private bool _canMove = true;

    [SerializeField] protected Rigidbody rigidBody;

    [Header("Movement")]
    public float movementSpeed = 5f;

    [Header("Jump")]
    public float jumpForce = 5;
    [SerializeField] protected bool canDoubleJump;

    [Header("Grounded")]
    public float groundedRadius = 0.28f;
    public float groundedOffset = 0.8f;
    protected bool isGrounded = false;
    public LayerMask groundLayers;

    [Header("Camera")]
    [SerializeField] private float mouseX;
    [SerializeField] private float mouseY;
    public float distance = 5.0f;
    public Transform target;

    [Header("Camera Hard")]
    [SerializeField] private float rotationSpeed = 5f;

    [SerializeField] private float pitchRangeTop = 80f;
    [SerializeField] private float pitchRangeBot = -80f;
    protected float pitch = 0f;

    public CameraMode _cameraMode = CameraMode.Hard;

    [SerializeField] protected GameObject _mainCamera;
    [SerializeField] protected Transform _startCameraPosition;

    [Header("Camera FreelyRotating")]
    [SerializeField] private float xSpeed = 120.0f;
    [SerializeField] private float ySpeed = 120.0f;

    [SerializeField] private float yMaxLimit = 80f;
    [SerializeField] private float yMinLimit = -20f;

    [SerializeField] private float distanceMin = .5f;
    [SerializeField] private float distanceMax = 15f;

    [SerializeField] private Vector3 _offset;

    [Header("UnityEvents")]
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
        distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel"), distanceMin, distanceMax);

        Quaternion rotation = Quaternion.Euler(pitch, 0f, 0f);
        _mainCamera.transform.localRotation = rotation;

        Vector3 direction = _startCameraPosition.transform.position - transform.position;
        direction = direction.normalized;
        Vector3 newPosition = transform.position + direction * distance;

        _mainCamera.transform.position = newPosition;
    }

    public void RotateCameraAroundCharacter()
    {
        distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel"), distanceMin, distanceMax);

        mouseX += Input.GetAxis("Mouse X") * xSpeed * distance * 0.02f;
        mouseY -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

        mouseY = Mathf.Clamp(mouseY, yMinLimit, yMaxLimit);

        Quaternion rotation = Quaternion.Euler(mouseY, mouseX, 0);

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
                Vector3 directionToCharacter = transform.position - _mainCamera.transform.position;
                directionToCharacter.y = 0;
                Quaternion characterRotation = Quaternion.LookRotation(directionToCharacter);
                transform.rotation = Quaternion.Euler(0, characterRotation.eulerAngles.y, 0);
                _mainCamera.transform.position = _startCameraPosition.position;
                break;

            case CameraMode.FreelyRotating:
                mouseX = transform.eulerAngles.y;
                mouseY = _mainCamera.transform.eulerAngles.x;
                if (mouseY < 0)
                {
                    mouseY = -mouseY;
                }
                mouseY = Mathf.Clamp(mouseY, yMinLimit, yMaxLimit);
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
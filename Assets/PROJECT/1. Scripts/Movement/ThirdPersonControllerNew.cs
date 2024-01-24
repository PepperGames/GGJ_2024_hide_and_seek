using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

public abstract class ThirdPersonControllerNew : MonoBehaviourPun
{
    public float rotationSpeed = 5f;
    public float pitchRangeTop = 80f;
    public float pitchRangeBot = -80f;
    protected float pitch = 0f;
    [SerializeField] protected GameObject _mainCamera;

    [SerializeField] protected Rigidbody rigidBody;

    public float movementSpeed = 5f;

    public float jumpForce = 5;
    [SerializeField] protected bool canDoubleJump;
    protected bool isGrounded = false;

    public float groundedRadius = 0.28f;
    public float groundedOffset = 0.8f;
    public LayerMask groundLayers;

    public UnityEvent OnMove;
    public UnityEvent OnJump;
    public UnityEvent OnDoubleJump;
    public UnityEvent OnLanding;

    private void Start()
    {
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
            Rotate();
            Move();
            Jump();
        }
    }

    private void Rotate()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        transform.Rotate(Vector3.up * mouseX * rotationSpeed);

        pitch -= mouseY * rotationSpeed;
        pitch = Mathf.Clamp(pitch, pitchRangeBot, pitchRangeTop);

        _mainCamera.transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    private void Move()
    {
        Vector3 playerMovementInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
        Vector3 moveVector = transform.TransformDirection(playerMovementInput) * movementSpeed;
        if (moveVector != Vector3.zero)
        {
            OnMove?.Invoke();
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
                OnDoubleJump?.Invoke();
            }
        }
    }

    private void GroundedCheck()
    {
        // set sphere position, with offset
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - groundedOffset,
            transform.position.z);

        bool prevIsGrounded = isGrounded;

        isGrounded = Physics.CheckSphere(spherePosition, groundedRadius, groundLayers,
            QueryTriggerInteraction.Ignore);
        if (prevIsGrounded == false && isGrounded == true)
        {
            OnLanding?.Invoke();
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
}
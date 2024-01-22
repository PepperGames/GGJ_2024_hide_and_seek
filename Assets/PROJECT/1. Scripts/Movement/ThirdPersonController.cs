using Photon.Pun;
using StarterAssets;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM 
using UnityEngine.InputSystem;
#endif

/* 
 * Note: animations are called via the controller for both the character and capsule using animator null checks
 * Примечание: анимация вызывается через контроллер как для персонажа, 
 * так и для капсулы с использованием нулевых проверок аниматора.
*/

[RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM
[RequireComponent(typeof(PlayerInput))]
#endif
public class ThirdPersonController : MonoBehaviourPun
{
    [Header("Player")]
    [Tooltip("Move speed of the character in m/s")]
    public float MoveSpeed = 2.0f;

    [Header("AudioClips")]
    public AudioClip LandingAudioClip;
    public AudioClip[] FootstepAudioClips;
    [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

    [Header("Jump")]
    [Space(10)]
    [Tooltip("The height the player can jump")]
    public float JumpHeight = 1.2f;

    private int jumpCount = 0; // Счетчик прыжков
    public int maxJumpCount = 2; // Максимальное количество прыжков
    public float JumpPower1 = 2f;
    public float JumpPower2 = 2f;

    [Space(10)]
    [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
    public float JumpTimeout = 0.50f;// 0.50f;

    [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
    public float FallTimeout = 0.15f;

    [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
    public float Gravity = -15.0f;

    [Header("Grounded")]
    [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
    public bool Grounded = true;

    [Tooltip("Useful for rough ground")]
    public float GroundedOffset = -0.14f;

    [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
    public float GroundedRadius = 0.28f;

    [Tooltip("What layers the character uses as ground")]
    public LayerMask GroundLayers;

    private float _verticalVelocity;
    private float _terminalVelocity = 53.0f;

    // timeout deltatime
    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;

    public float rotationSpeed = 5f;
    public float pitchRangeTop = 80f;
    public float pitchRangeBot = -80f;
    private float pitch = 0f;

#if ENABLE_INPUT_SYSTEM
    private PlayerInput _playerInput;
#endif
    [SerializeField] private Animator _animator;
    private CharacterController _controller;
    private StarterAssetsInputs _input;
    [SerializeField] private GameObject _mainCamera;

    private bool _hasAnimator;

    private bool IsCurrentDeviceMouse
    {
        get
        {
#if ENABLE_INPUT_SYSTEM
            return _playerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
        }
    }

    private void Start()
    {
        _hasAnimator = TryGetComponent(out _animator);
        _controller = GetComponent<CharacterController>();
        _input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM
        _playerInput = GetComponent<PlayerInput>();
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif

        // reset our timeouts on start
        _jumpTimeoutDelta = JumpTimeout;
        _fallTimeoutDelta = FallTimeout;
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            _hasAnimator = TryGetComponent(out _animator);

            JumpAndGravity();
            GroundedCheck();
            Move();
        }
    }

    private void GroundedCheck()
    {
        // set sphere position, with offset
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
            transform.position.z);
        Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
            QueryTriggerInteraction.Ignore);
    }

    private void Move()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // Вращение персонажа вокруг оси Y
        transform.Rotate(Vector3.up * mouseX * rotationSpeed);

        // Вращение камеры вокруг оси X с ограничением по углам
        pitch -= mouseY * rotationSpeed;
        pitch = Mathf.Clamp(pitch, pitchRangeBot, pitchRangeTop);

        // Применение вращения камеры
        _mainCamera.transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);

        float verticalInput = Input.GetAxisRaw("Vertical");
        float horizontalInput = Input.GetAxisRaw("Horizontal");

        Vector3 moveDirection = new Vector3(horizontalInput, 0f, verticalInput).normalized;

        if (moveDirection.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;
            Vector3 moveAngle = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            _controller.Move(moveAngle.normalized * Time.deltaTime * MoveSpeed);
        }
        _controller.Move(new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
    }

    private void JumpAndGravity()
    {
        if (Grounded)
        {
            jumpCount = 0;
            //Debug.Log("JumpCount1 " + jumpCount);
            _fallTimeoutDelta = FallTimeout;

            if (_verticalVelocity < 0.0f)
            {
                _verticalVelocity = -JumpPower1;
            }

            if (_input.jump && _jumpTimeoutDelta <= 0.0f && jumpCount == 0)
            {
                _verticalVelocity = Mathf.Sqrt(JumpHeight * -JumpPower1 * Gravity);
                jumpCount++;
                //Debug.Log("JumpCount2 " + jumpCount);
            }

            if (_jumpTimeoutDelta >= 0.0f)
            {
                _jumpTimeoutDelta -= Time.deltaTime;
            }

            if (jumpCount == 0)
            {
                _input.jump = false;
                _input.doubleJump = false;
            }
        }
        else
        {
            if (jumpCount < maxJumpCount)
            {
                _jumpTimeoutDelta = JumpTimeout;
                if (_input.doubleJump)
                {
                    //Debug.Log("doubleJump");
                    //Debug.Log("_verticalVelocity " + _verticalVelocity);
                    jumpCount++;
                   // Debug.Log("JumpCount3 " + jumpCount);

                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -JumpPower2 * Gravity);
                    _fallTimeoutDelta += FallTimeout;
                }
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }

               //Debug.Log("JumpCount4 " + jumpCount);
            }
        }

        if (_verticalVelocity < _terminalVelocity)
        {
            _verticalVelocity += Gravity * Time.deltaTime;
        }
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    private void OnDrawGizmosSelected()
    {
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        if (Grounded) Gizmos.color = transparentGreen;
        else Gizmos.color = transparentRed;

        // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
        Gizmos.DrawSphere(
            new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
            GroundedRadius);
    }

    private void OnFootstep(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            if (FootstepAudioClips.Length > 0)
            {
                var index = Random.Range(0, FootstepAudioClips.Length);
                AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center), FootstepAudioVolume);
            }
        }
    }

    private void OnLand(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), FootstepAudioVolume);
        }
    }
}
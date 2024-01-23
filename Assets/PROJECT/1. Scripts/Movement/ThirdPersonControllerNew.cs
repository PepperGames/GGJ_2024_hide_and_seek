using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonControllerNew : MonoBehaviour
{
    public float moveSpeed = 5f;

    public float rotationSpeed = 5f;
    public float pitchRangeTop = 80f;
    public float pitchRangeBot = -80f;
    private float pitch = 0f;

    public float jumpForce = 10f;
    public int maxJumps = 2;

    private bool isGrounded;
    private int jumpsRemaining;
    private float currentVelocityY;

    private float Gravity = -15.0f;
    private bool Grounded = true;
    private float GroundedOffset = -0.14f;
    private float GroundedRadius = 0.28f;
    public LayerMask GroundLayers;
    private float _verticalVelocity;
    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;
    private float _terminalVelocity = 53.0f;
    private int jumpCount = 0; // Счетчик прыжков
    public int maxJumpCount = 2; // Максимальное количество прыжков
    public float JumpPower1 = 2f;
    public float JumpPower2 = 2f;
    public float FallTimeout = 0.15f;
    public float JumpHeight = 1.2f;
    public float JumpTimeout = 0.50f;// 0.50f;

    [SerializeField] private Rigidbody rb;
    [SerializeField] private GameObject _mainCamera;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // Убедитесь, что Rigidbody установлен как kinematic
        rb.isKinematic = true;
        jumpsRemaining = maxJumps;
    }

    private void FixedUpdate()
    {
        Rotate();
        GroundedCheck();
        Move();
        // Проверяем, нажата ли клавиша прыжка и есть ли еще оставшиеся прыжки
        if (Input.GetButtonDown("Jump") && jumpsRemaining > 0)
        {
            Jump();
        }
    }

    private void Rotate()
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
    }

    private void Move()
    {
        // Получаем ввод от пользователя
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Вычисляем вектор направления
        Vector3 movement = new Vector3(horizontal, 0f, vertical) * moveSpeed * Time.deltaTime;

        // Применяем движение к Rigidbody
        rb.MovePosition(rb.position + transform.TransformDirection(movement));

        transform.Translate(Vector3.up * currentVelocityY * Time.deltaTime, Space.World);
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

    private void GroundedCheck()
    {
        // set sphere position, with offset
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
            transform.position.z);
        Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
            QueryTriggerInteraction.Ignore);
    }

    void OnCollisionEnter(Collision collision)
    {
        // При соприкосновении с землей сбрасываем количество оставшихся прыжков
        if (collision.gameObject.CompareTag("Ground"))
        {
            jumpsRemaining = maxJumps;
        }
    }
}

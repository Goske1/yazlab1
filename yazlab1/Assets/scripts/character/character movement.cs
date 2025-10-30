using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class SimpleTransformMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] public float sprintMultiplier = 2f;
    [SerializeField] public float moveSpeed = 6f;
    
    [Header("Jump Settings")]
    [SerializeField] public bool canJump = true;    
    [SerializeField] public int jumpcounter = 0;
    [SerializeField] public float mass = 3f;
    [SerializeField] public int jumpForce = 30;
    [SerializeField] public int groundCheckDistance = 1;

    [Header("Dash Settings")]
    [SerializeField] public float dashMultipler = 20f;
    [SerializeField] public float dashDuration = 0.25f;
    [SerializeField] public float dashCooldown = 1f;
    private float dashTimeLeft;
    private float lastDashTime = -Mathf.Infinity;
    private bool isDashing = false;
    private Vector3 dashDirection; // Dash yönü için

    [Header("Physics Settings")]
    [SerializeField] public float gravityMultiplier = 2.5f; // Hem çıkışta hem inişte uygulanacak ek gravity çarpanı

    public Vector3 moveDirection;
    public Rigidbody playerRb;

    public void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        if (playerRb == null) return;

        playerRb.freezeRotation = true;
        playerRb.constraints = RigidbodyConstraints.FreezeRotation;
        playerRb.mass = mass;

        Physics.gravity = new Vector3(0, -9.81f, 0);
    }

    public void Update()
    {
        HandleMovementInput();
        HandleJumpInput();
        HandleDashInput(); // Dash inputu ekledik
    }

    public void FixedUpdate()
    {
        MovePlayer();
        ApplyExtraGravity();
    }

    public void HandleMovementInput()
    {
        moveDirection = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) moveDirection += Vector3.forward;
        if (Input.GetKey(KeyCode.S)) moveDirection += Vector3.back;
        if (Input.GetKey(KeyCode.A)) moveDirection += Vector3.left;
        if (Input.GetKey(KeyCode.D)) moveDirection += Vector3.right;

        moveDirection.Normalize();

        if (Input.GetKey(KeyCode.LeftShift))
        {
            moveDirection *= sprintMultiplier;
        }
    }

    public void HandleJumpInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && (IsGrounded() || jumpcounter < 2))
        {
            Jump();
        }
        if (IsGrounded())
        {
            jumpcounter = 0;
            Invoke(nameof(ResetJump), 0.1f);
        }
    }

    public void Jump()
    {
        if (!canJump) return;
        playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        jumpcounter++;
        if (jumpcounter == 1)
        {
            canJump = false;
        }
    }

    public void ResetJump()
    {
        canJump = true;
    }

    public bool IsGrounded()
    {
        float radius = 0.1f;
        Vector3 rayStart = transform.position + Vector3.up * 0.1f;
        return Physics.SphereCast(rayStart, radius, Vector3.down, out RaycastHit hit, groundCheckDistance + 0.1f);
    }

    public void HandleDashInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl) && Time.time >= lastDashTime + dashCooldown && !isDashing)
        {
            StartDash();
        }

        if (isDashing)
        {
            dashTimeLeft -= Time.deltaTime;
            if (dashTimeLeft <= 0)
            {
                EndDash();
            }
        }
    }

    private void StartDash()
    {
        isDashing = true;
        dashTimeLeft = dashDuration;
        lastDashTime = Time.time;

        // Dash yönü mevcut hareket yönü
        dashDirection = moveDirection;
        if (dashDirection == Vector3.zero)
            dashDirection = transform.forward; // input yoksa baktığı yön

        // Dash anında hız uygula
        playerRb.linearVelocity = dashDirection * dashMultipler;
    }

    private void EndDash()
    {
        isDashing = false;
        // Dash bittiğinde velocity'yi moveDirection ile normal hâle getir
        Vector3 velocity = moveDirection * moveSpeed;
        velocity.y = playerRb.linearVelocity.y; // gravity koru
        playerRb.linearVelocity = velocity;
    }

    public void MovePlayer()
    {
        if (!isDashing)
        {
            Vector3 velocity = moveDirection * moveSpeed;
            velocity.y = playerRb.linearVelocity.y;
            playerRb.linearVelocity = velocity;
        }
    }

    public void ApplyExtraGravity()
    {
        playerRb.linearVelocity += Vector3.up * Physics.gravity.y * (gravityMultiplier - 1) * Time.fixedDeltaTime;
    }
}

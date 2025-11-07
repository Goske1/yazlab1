using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;

public class SimpleTransformMovement : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] Animator CharacterMovement;

    [Header("Movement Settings")]
    [SerializeField] public float sprintMultiplier = 2f;
    [SerializeField] public float moveSpeed = 6f;
    [SerializeField] public float rotationSpeed = 15f;

    [Header("Jump Settings")]
    [SerializeField] public bool canJump = true;
    [SerializeField] public int jumpcounter = 0;
    [SerializeField] public float mass = 3f;
    [SerializeField] public int jumpForce = 70;
    [SerializeField] public int groundCheckDistance = 1;

    [SerializeField] public bool isGrounded;

    [Header("Dash Settings")]

    private CapsuleCollider capsuleCollider;
    [SerializeField] public float dashMultipler = 20f;
    [SerializeField] public float dashDuration = 0.4f;
    [SerializeField] public float dashCooldown = 1f;
    private Vector3 rawInputDirection;
    private float dashTimeLeft;
    private float lastDashTime = -Mathf.Infinity;
    private bool isDashing = false;
    private Vector3 dashDirection; 
    private float originalHeight;
    private Vector3 originalCenter;

    [Tooltip("Roll sırasında collider'ın yüksekliği")]
    public float rollHeight = 0.9f;
    
    [Tooltip("Roll sırasında collider'ın Y eksenindeki merkezi")]
    public float rollCenterY = 0.45f;
    
    [Tooltip("Roll'un toplam süresi (saniye)")]
    public float rollDuration = 0.6f;

    [Tooltip("Yere çökme ve kalkma geçişinin hızı")]
    public float transitionSpeed = 15f;


    [Header("Physics Settings")]
    [SerializeField] public float gravityMultiplier = 2.5f; 

    [Header("References")]
    [SerializeField] public Transform cameraTransform;

    public Vector3 moveDirection;
    public Rigidbody playerRb;

    public void Start()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
        if (capsuleCollider == null)
        {
            Debug.LogError("Karakterde CapsuleCollider bulunamadı! Roll mekaniği çalışmayacak.");
            return;
        }

        originalHeight = capsuleCollider.height;
        originalCenter = capsuleCollider.center;
  
        CharacterMovement = GetComponent<Animator>();
        playerRb = GetComponent<Rigidbody>();
        if (playerRb == null) return;

        playerRb.freezeRotation = true;
        playerRb.constraints = RigidbodyConstraints.FreezeRotation;
        playerRb.mass = mass;

        Physics.gravity = new Vector3(0, -9.81f, 0);

        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
            if (cameraTransform == null)
            {
                Debug.LogError("SimpleTransformMovement: Ana Kamera (Main Camera) bulunamadı. 'MainCamera' tag'ini kontrol edin veya Inspector'dan manuel olarak atayın.");
            }
        }
    }

    [Obsolete]
    public void Update()
    {
        HandleMovementInput();
        HandleRotation();
        HandleJumpInput();
        HandleDashInput(); 
        isGrounded = IsGrounded();
        CharacterMovement.SetBool("IsGrounded", isGrounded);

        float vInput = Input.GetAxisRaw("Vertical");
        float hInput = Input.GetAxisRaw("Horizontal");
        bool isSprinting = Input.GetKey(KeyCode.LeftShift);

        CharacterMovement.SetFloat("Vertical", vInput);
        CharacterMovement.SetFloat("Horizontal", hInput);
        CharacterMovement.SetBool("IsSprint", isSprinting);
    }

    public void FixedUpdate()
    {
        MovePlayer();
        ApplyExtraGravity();
    }

    public void HandleMovementInput()
    {
        if (cameraTransform == null) return;

        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        moveDirection = Vector3.zero;
        rawInputDirection = Vector3.zero; 

        if (Input.GetKey(KeyCode.W)) moveDirection += camForward;
        if (Input.GetKey(KeyCode.S)) moveDirection -= camForward;
        if (Input.GetKey(KeyCode.A)) moveDirection -= camRight;
        if (Input.GetKey(KeyCode.D)) moveDirection += camRight;

        moveDirection.Normalize();


        rawInputDirection = moveDirection; 


        if (Input.GetKey(KeyCode.LeftShift))
        {
            moveDirection *= sprintMultiplier;
        }
    }
    public void HandleRotation()
    {

        if (isDashing) return;


        if (cameraTransform == null) return;


        Vector3 camForward = cameraTransform.forward;

 
        camForward.y = 0;
        camForward.Normalize();


        if (camForward != Vector3.zero)
        {

            Quaternion targetRotation = Quaternion.LookRotation(camForward);


            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    public void HandleJumpInput()
    {
        if (isGrounded)
        {
            jumpcounter = 0;
            Invoke(nameof(ResetJump), 0.1f);
        }
        if (Input.GetKeyDown(KeyCode.Space) && (jumpcounter < 2))
        {
            Jump();
        }
    }

    public void Jump()
    {
        if (!canJump) return;
        playerRb.linearVelocity = new Vector3(playerRb.linearVelocity.x, 0f, playerRb.linearVelocity.z);
        playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        CharacterMovement.SetTrigger("JumpTrig");

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
            CharacterMovement.SetTrigger("DashTrig");
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


        if (capsuleCollider != null)
        {
            capsuleCollider.height = rollHeight;

            capsuleCollider.center = new Vector3(originalCenter.x, rollCenterY, originalCenter.z);
        }


        dashDirection = rawInputDirection;
        if (dashDirection == Vector3.zero)
        {
            dashDirection = transform.forward;
        }
        float currentYVelocity = playerRb.linearVelocity.y;
        Vector3 dashVelocity = dashDirection * dashMultipler;
        playerRb.linearVelocity = new Vector3(dashVelocity.x, currentYVelocity, dashVelocity.z);
    }

    private void EndDash()
    {
        isDashing = false;


        if (capsuleCollider != null)
        {
            capsuleCollider.height = originalHeight;
            capsuleCollider.center = originalCenter;
        }
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
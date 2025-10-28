using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class SimpleTransformMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private float groundCheckDistance = 1f; // Yere değme kontrolü için mesafe

    private Vector3 moveDirection;
    private bool canJump = true;
    private Rigidbody playerRb;

    private void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        if (playerRb == null) return;

        playerRb.freezeRotation = true;
        playerRb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    private void Update()
    {
        HandleMovementInput();
        HandleJumpInput();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void HandleMovementInput()
    {
        moveDirection = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) moveDirection += Vector3.forward;
        if (Input.GetKey(KeyCode.S)) moveDirection += Vector3.back;
        if (Input.GetKey(KeyCode.A)) moveDirection += Vector3.left;
        if (Input.GetKey(KeyCode.D)) moveDirection += Vector3.right;

        moveDirection.Normalize();
    }

    private void HandleJumpInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            Jump();
        }
    }

    private void Jump()
    {
        if (!canJump || !IsGrounded()) return;

        playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        canJump = false;
        Invoke(nameof(ResetJump), 0.1f);
    }

    private void ResetJump()
    {
        canJump = true;
    }

    private bool IsGrounded()
    {
        // Terrain için daha uzun mesafe ve daha geniş çap kullanıyoruz
        float radius = 0.5f;
        Vector3 rayStart = transform.position + Vector3.up * 0.5f;
        return Physics.SphereCast(rayStart, radius, Vector3.down, out RaycastHit hit, groundCheckDistance + 0.5f);
    }

    private void MovePlayer()
    {
        Vector3 velocity = moveDirection * moveSpeed;
        velocity.y = playerRb.linearVelocity.y; // Changed from linearVelocity to velocity
        playerRb.linearVelocity = velocity; // Changed from linearVelocity to velocity
    }
}
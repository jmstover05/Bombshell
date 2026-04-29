using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Camera / Movement Direction")]
    public Transform movementReference;

    [Header("Movement")]
    public float moveSpeed = 8.0f;
    public float airControlPercent = 0.6f;
    public float jumpHeight = 1.6f;
    public float gravity = -24.0f;

    [Header("Jump Forgiveness")]
    public float coyoteTime = 0.2f;
    public float jumpBufferTime = 0.12f;

    [Header("External Push / Gun Jump")]
    public float impulseDrag = 8.0f;

    private CharacterController controller;
    private Vector3 verticalVelocity;
    private Vector3 horizontalImpulseVelocity;

    private float coyoteTimer = 0.0f;
    private float jumpBufferTimer = 0.0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        bool isGrounded = controller.isGrounded;

        if (isGrounded)
        {
            coyoteTimer = coyoteTime;

            if (verticalVelocity.y < 0.0f)
            {
                verticalVelocity.y = -2.0f;
            }
        }
        else
        {
            coyoteTimer -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferTimer = jumpBufferTime;
        }
        else
        {
            jumpBufferTimer -= Time.deltaTime;
        }

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 forward;
        Vector3 right;

        if (movementReference != null)
        {
            forward = movementReference.forward;
            right = movementReference.right;

            forward.y = 0.0f;
            right.y = 0.0f;

            forward.Normalize();
            right.Normalize();
        }
        else
        {
            forward = transform.forward;
            right = transform.right;
        }

        Vector3 moveDirection = forward * moveZ + right * moveX;

        if (moveDirection.sqrMagnitude > 1.0f)
        {
            moveDirection.Normalize();
        }

        float moveMultiplier = isGrounded ? 1.0f : airControlPercent;
        Vector3 inputMove = moveDirection * moveSpeed * moveMultiplier;

        if (jumpBufferTimer > 0.0f && coyoteTimer > 0.0f)
        {
            verticalVelocity.y = Mathf.Sqrt(jumpHeight * -2.0f * gravity);

            jumpBufferTimer = 0.0f;
            coyoteTimer = 0.0f;
        }

        verticalVelocity.y += gravity * Time.deltaTime;

        Vector3 totalMove =
            inputMove +
            horizontalImpulseVelocity +
            new Vector3(0.0f, verticalVelocity.y, 0.0f);

        controller.Move(totalMove * Time.deltaTime);

        horizontalImpulseVelocity = Vector3.MoveTowards(
            horizontalImpulseVelocity,
            Vector3.zero,
            impulseDrag * Time.deltaTime
        );
    }

    public void ForceJump(float customJumpHeight = -1.0f)
    {
        float heightToUse = customJumpHeight > 0.0f ? customJumpHeight : jumpHeight;
        verticalVelocity.y = Mathf.Sqrt(heightToUse * -2.0f * gravity);
    }

    public void ApplyGunJump(Vector3 impulseDirection, float impulsePower, float minimumUpVelocity)
    {
        if (impulseDirection.sqrMagnitude <= 0.001f)
        {
            return;
        }

        Vector3 impulse = impulseDirection.normalized * impulsePower;

        Vector3 horizontalImpulse = new Vector3(impulse.x, 0.0f, impulse.z);
        horizontalImpulseVelocity += horizontalImpulse;

        float upwardVelocity = Mathf.Max(impulse.y, minimumUpVelocity);
        verticalVelocity.y = Mathf.Max(verticalVelocity.y, upwardVelocity);
    }

    public void TeleportTo(Vector3 worldPosition)
    {
        controller.enabled = false;
        transform.position = worldPosition;
        controller.enabled = true;

        verticalVelocity = Vector3.zero;
        horizontalImpulseVelocity = Vector3.zero;
        coyoteTimer = 0.0f;
        jumpBufferTimer = 0.0f;
    }
}
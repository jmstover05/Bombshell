using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Optional")]
    public Transform movementReference;

    [Header("Movement")]
    public float moveSpeed = 8.0f;
    public float airControlPercent = 0.6f;
    public float jumpHeight = 1.6f;
    public float gravity = -24.0f;

    private CharacterController controller;
    private Vector3 verticalVelocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        bool isGrounded = controller.isGrounded;

        if (isGrounded && verticalVelocity.y < 0.0f)
        {
            verticalVelocity.y = -2.0f;
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
        controller.Move(moveDirection * moveSpeed * moveMultiplier * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            verticalVelocity.y = Mathf.Sqrt(jumpHeight * -2.0f * gravity);
        }

        verticalVelocity.y += gravity * Time.deltaTime;
        controller.Move(verticalVelocity * Time.deltaTime);
    }

    public void TeleportTo(Vector3 worldPosition)
    {
        controller.enabled = false;
        transform.position = worldPosition;
        controller.enabled = true;
        verticalVelocity = Vector3.zero;
    }
}
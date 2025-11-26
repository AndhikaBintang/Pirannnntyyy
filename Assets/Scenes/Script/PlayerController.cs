using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Ground & Movement")]
    [SerializeField] LayerMask groundLayer;
    [SerializeField] private float runSpeed = 10f;
    [SerializeField] private Transform[] groundChecks;
    [SerializeField] private Transform[] wallChecks;

    [Header("Jump & Hover")]
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float gravity = -50f;
    [SerializeField] private int maxJumps = 2;
    [SerializeField] private float hoverGravity = -5f;
    [SerializeField] private float hoverDuration = 1.2f;

    [Header("Death & Respawn")]
    [SerializeField] private float fallDeathY = -10f;
    [SerializeField] private Transform respawnPoint;

    private CharacterController characterController;

    // --- Controller MPU6050 ---
    [SerializeField] private MPU6050Controller controller;

    private Vector3 velocity;
    private Animator animator;
    private bool isGrounded;
    private int jumpCount = 0;

    private bool isHovering = false;
    private float hoverTimer = 0f;

    // ==== Tambahan Sensor ====
    [Header("Sensor Movement Settings")]
    public float sensorSensitivity = 0.02f;
    public bool invertPitch = false;
    public bool invertRoll = false;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        if (respawnPoint == null)
        {
            GameObject go = new GameObject("RespawnPoint");
            go.transform.position = transform.position;
            respawnPoint = go.transform;
        }
    }

    void Update()
    {
        // ==== Death Check ====
        if (transform.position.y < fallDeathY)
        {
            DieAndRespawn();
            return;
        }

        // ================================
        //       INPUT KEYBOARD
        // ================================
        float horizontalInput = Input.GetAxisRaw("Horizontal"); // A/D atau ??
        float verticalInput = Input.GetAxisRaw("Vertical");     // W/S atau ??

        // ================================
        //       INPUT SENSOR MPU6050
        // ================================
        float pitch = controller != null ? controller.Pitch : 0f; // anggukan
        float roll = controller != null ? controller.Roll : 0f; // miring kiri/kanan

        // Pembalikan jika perlu
        if (invertPitch) pitch *= -1f;
        if (invertRoll) roll *= -1f;

        // Gabungkan sensor + keyboard
        horizontalInput += roll * sensorSensitivity;
        verticalInput += pitch * sensorSensitivity;

        // ================================
        //       UPDATE FACING PLAYER
        // ================================
        Vector3 facingDir = new Vector3(horizontalInput, 0, verticalInput);
        if (facingDir.sqrMagnitude > 0.01f)
        {
            transform.forward = facingDir.normalized;
        }

        // ================================
        //          GROUND CHECK
        // ================================
        isGrounded = false;
        foreach (var groundCheck in groundChecks)
        {
            if (Physics.CheckSphere(groundCheck.position, 0.1f, groundLayer, QueryTriggerInteraction.Ignore))
            {
                isGrounded = true;
                break;
            }
        }

        // ================================
        //          WALL CHECK
        // ================================
        bool blocked = false;
        foreach (var wallCheck in wallChecks)
        {
            if (Physics.CheckSphere(wallCheck.position, 0.1f, groundLayer, QueryTriggerInteraction.Ignore))
            {
                blocked = true;
                break;
            }
        }

        // ================================
        //     RESET JUMP & HOVER
        // ================================
        if (isGrounded)
        {
            jumpCount = 0;
            isHovering = false;
            hoverTimer = 0f;
            if (velocity.y < 0) velocity.y = 0;
        }

        // ================================
        //              LOMPAT
        // ================================
        if (Input.GetButtonDown("Jump"))
        {
            if (jumpCount < maxJumps)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
                jumpCount++;
                if (jumpCount >= maxJumps)
                {
                    hoverTimer = hoverDuration;
                }
            }
        }

        // ================================
        //              HOVER
        // ================================
        if (jumpCount >= maxJumps && !isGrounded && Input.GetButton("Jump") && hoverTimer > 0f)
        {
            isHovering = true;
            velocity.y = Mathf.Max(velocity.y, hoverGravity * Time.deltaTime);
            hoverTimer -= Time.deltaTime;
        }
        else
        {
            isHovering = false;
            velocity.y += gravity * Time.deltaTime;
        }

        // ================================
        //              GERAK
        // ================================
        Vector3 move = new Vector3(horizontalInput * runSpeed, 0, verticalInput * runSpeed);
        if (blocked) move = Vector3.zero;

        characterController.Move((move + velocity) * Time.deltaTime);

        // ================================
        //             ANIMATOR
        // ================================
        if (animator != null)
        {
            animator.SetFloat("speed", new Vector2(horizontalInput, verticalInput).magnitude);
            animator.SetBool("isGrounded", isGrounded);
            animator.SetFloat("VerticalSpeed", velocity.y);
        }
    }

    public void DieAndRespawn()
    {
        Debug.Log("Player jatuh & respawn!");

        velocity = Vector3.zero;
        characterController.enabled = false;
        transform.position = respawnPoint.position;
        characterController.enabled = true;

        jumpCount = 0;
        isHovering = false;
        hoverTimer = 0f;
    }

    public void SetCheckpoint(Vector3 newPosition)
    {
        respawnPoint.position = newPosition;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] LayerMask groundLayer;
    [SerializeField] private float runSpeed = 10f;
    [SerializeField] private Transform[] groundChecks;
    [SerializeField] private Transform[] wallChecks;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float gravity = -50f;
    [SerializeField] private int maxJumps = 2;           // double jump
    [SerializeField] private float hoverGravity = -5f;   // gravitasi ringan saat hover
    [SerializeField] private float hoverDuration = 1.2f; // durasi maksimal hover (detik)

    private CharacterController characterController;
    private Vector3 velocity;
    private bool isGrounded;
    private float horizontalInput;
    private int jumpCount = 0;

    private bool isHovering = false;
    private float hoverTimer = 0f;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        horizontalInput = 1; // auto jalan kanan
        transform.forward = Vector3.right; // biar gak miring 45°

        // ==== Ground Check ====
        isGrounded = false;
        foreach (var groundCheck in groundChecks)
        {
            if (Physics.CheckSphere(groundCheck.position, 0.1f, groundLayer, QueryTriggerInteraction.Ignore))
            {
                isGrounded = true;
                break;
            }
        }

        // ==== Wall Check ====
        bool blocked = false;
        foreach (var wallCheck in wallChecks)
        {
            if (Physics.CheckSphere(wallCheck.position, 0.1f, groundLayer, QueryTriggerInteraction.Ignore))
            {
                blocked = true;
                break;
            }
        }

        // ==== Reset Jump & Hover saat di tanah ====
        if (isGrounded)
        {
            jumpCount = 0;
            isHovering = false;
            hoverTimer = 0f;
            if (velocity.y < 0) velocity.y = 0;
        }

        // ==== Lompat (maksimal 2 kali) ====
        if (Input.GetButtonDown("Jump"))
        {
            if (jumpCount < maxJumps)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
                jumpCount++;
                if (jumpCount >= maxJumps) 
                {
                    // siap hover setelah lompat terakhir
                    hoverTimer = hoverDuration;
                }
            }
        }

        // ==== Hovering (hanya setelah double jump & dengan timer) ====
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

        // ==== Movement ====
        Vector3 move = blocked ? Vector3.zero : new Vector3(horizontalInput * runSpeed, 0, 0);
        characterController.Move((move + velocity) * Time.deltaTime);
    }
}

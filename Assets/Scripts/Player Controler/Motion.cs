using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Motion : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float spintModifier;
    [SerializeField] private float jumpForce;
    [SerializeField] private Camera normalCam;
    [SerializeField] private Transform groundDetector;
    [SerializeField] private LayerMask ground;

    private Rigidbody rb;

    private float baseFOV;
    private float spintFOVModifier = 1.25f;

    private void Start()
    {
        baseFOV = normalCam.fieldOfView;
        Camera.main.enabled = false;
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        //axles
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        //controls
        bool sprint = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        bool jump = Input.GetKeyDown(KeyCode.Space);

        //states
        bool isGrounded = Physics.Raycast(groundDetector.position, Vector3.down, 0.1f, ground);
        bool isJumping = jump && isGrounded;
        bool isSprinting = sprint && moveY > 0 && !isJumping && isGrounded;

        //jumping
        if (isJumping)
        {
            rb.AddForce(Vector3.up * jumpForce);
        }

        //movement
        Vector3 direction = new(moveX, 0, moveY);
        direction.Normalize();
        float adjustedSpeed = speed;
        if (isSprinting) adjustedSpeed *= spintModifier;


        Vector3 targetVelocity = adjustedSpeed * Time.deltaTime * transform.TransformDirection(direction);
        targetVelocity.y = rb.velocity.y;
        rb.velocity = targetVelocity;

        //field of view
        if (isSprinting)
        {
            normalCam.fieldOfView = Mathf.Lerp(normalCam.fieldOfView, baseFOV * spintFOVModifier, Time.deltaTime * 8f);
        }
        else
        {
            normalCam.fieldOfView = Mathf.Lerp(normalCam.fieldOfView, baseFOV, Time.deltaTime * 4f);
        }
    }
}

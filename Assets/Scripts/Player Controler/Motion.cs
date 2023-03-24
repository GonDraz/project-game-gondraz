using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using Photon;
using Photon.Pun;

public class Motion : MonoBehaviourPunCallbacks
{
    #region Variables

    [SerializeField] private float speed;
    [SerializeField] private float spintModifier;
    [SerializeField] private float jumpForce;
    [SerializeField] private Camera normalCam;
    [SerializeField] private GameObject cameraParent;
    [SerializeField] private Transform weaponParent;
    [SerializeField] private Transform groundDetector;
    [SerializeField] private LayerMask ground;

    private Rigidbody rb;

    private Vector3 targetWeaponBobposition;
    private Vector3 weaponparentOrigin;

    private float movementCounter;
    private float idleCounter;

    private float baseFOV;
    private float spintFOVModifier = 1.25f;

    #endregion

    #region MonoBehaviour Callbacks
    private void Start()
    {

        cameraParent.SetActive(photonView.IsMine);

        baseFOV = normalCam.fieldOfView;

        if (Camera.main)
        {
            Camera.main.enabled = false;
        }
        rb = GetComponent<Rigidbody>();
        weaponparentOrigin = weaponParent.localPosition;
    }


    private void Update()
    {
        if (!photonView.IsMine) return;

        //axles
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        //controls
        bool jump = Input.GetKeyDown(KeyCode.Space);

        //states
        bool isGrounded = Physics.Raycast(groundDetector.position, Vector3.down, 0.1f, ground);
        bool isJumping = jump && isGrounded;

        //jumping
        if (isJumping)
        {
            rb.AddForce(Vector3.up * jumpForce);
        }

        //Head Bob
        if (moveX == 0 && moveY == 0)
        {
            HeadBod(idleCounter, 0.025f, 0.025f);
            idleCounter += Time.deltaTime;
            weaponParent.localPosition = Vector3.Lerp(weaponParent.localPosition, targetWeaponBobposition, Time.deltaTime * 2f);
        }
        else
        {
            HeadBod(movementCounter, 0.035f, 0.035f);
            movementCounter += Time.deltaTime;
            weaponParent.localPosition = Vector3.Lerp(weaponParent.localPosition, targetWeaponBobposition, Time.deltaTime * 6f);
        }
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
    #endregion

    #region Private Methor

    private void HeadBod(float z, float xIntensity, float yIntensity)
    {
        targetWeaponBobposition = weaponparentOrigin + new Vector3(Mathf.Cos(z * 2) * xIntensity, Mathf.Sin(z * 2) * yIntensity, 0);
    }
    #endregion
}

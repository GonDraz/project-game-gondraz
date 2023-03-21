using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Look : MonoBehaviour
{
    public static bool cursorLocked = true;


    [SerializeField] private Transform player;
    [SerializeField] private Transform cams;

    [SerializeField] private float xSensitivity;
    [SerializeField] private float ySensitivity;
    [SerializeField] private float maxAngle;

    private Quaternion camCenter;

    private void Start()
    {
        camCenter = cams.localRotation;
    }
    private void Update()
    {
        SetY();
        SetX();

        UpdateCursorLock();
    }
    private void SetY()
    {
        float inputY = Input.GetAxis("Mouse Y") * ySensitivity * Time.deltaTime;
        Quaternion abj = Quaternion.AngleAxis(inputY, -Vector3.right);
        Quaternion delta = cams.localRotation * abj;
        if (Quaternion.Angle(camCenter, delta) < maxAngle)
        {
            cams.localRotation = delta;
        }
    }
    private void SetX()
    {
        float inputX = Input.GetAxis("Mouse X") * xSensitivity * Time.deltaTime;
        Quaternion abj = Quaternion.AngleAxis(inputX, -Vector3.down);
        Quaternion delta = player.localRotation * abj;

        player.localRotation = delta;
    }

    private void UpdateCursorLock()
    {
        if (cursorLocked)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = false;
            if (Input.GetKeyDown(KeyCode.Escape)) { cursorLocked = false; }
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            if (Input.GetKeyDown(KeyCode.Escape)) { cursorLocked = true; }
        }
    }

}

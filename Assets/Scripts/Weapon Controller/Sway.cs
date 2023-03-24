using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sway : MonoBehaviour
{

    #region Variables

    [SerializeField] private float intensity;
    [SerializeField] private float smooth;

     private Quaternion originRotation;

    #endregion

    #region MonoBehaviour Callbacks

    void Start()
    {
        originRotation = transform.localRotation;
    }

    void Update()
    {
        UpdateSway();
    }
    #endregion


    #region Private Methods


    // Update is called once per frame
    void UpdateSway()
    {
        //Controls
        float moveX = Input.GetAxis("Mouse X");
        float moveY = Input.GetAxis("Mouse Y");

        //calculate tarfer rotation
        Quaternion xAbj = Quaternion.AngleAxis(intensity * moveX, Vector3.up);
        Quaternion yAbj = Quaternion.AngleAxis(intensity * moveY, Vector3.left);
        Quaternion targetRotation = originRotation * xAbj * yAbj;

        transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, Time.deltaTime * smooth);
    }
    #endregion
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Motion : MonoBehaviour
{
    [SerializeField] private float speed;

    private Rigidbody rb;


    private void Start()
    {
        Camera.main.enabled = false;
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        Vector3 direction = new(moveX, 0, moveY);
        direction.Normalize();

        rb.velocity = speed * Time.deltaTime * transform.TransformDirection(direction);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float walkSpeed = 100f;

    Vector3 dir;
    Rigidbody rb;


    // TODO: Implement running, jumping and crouching

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // FIXME: Less tedious way of translating input vector to be relative to local orientation?
        Vector3 horizontalVec = transform.right * horizontalInput;
        Vector3 verticalVec = transform.forward * verticalInput;
        rb.velocity = Vector3.ClampMagnitude((horizontalVec + verticalVec) * walkSpeed * Time.deltaTime, walkSpeed * Time.deltaTime);
    }
}

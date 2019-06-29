using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour
{
    [SerializeField]
    Camera cam;

    Vector3 velocity = Vector3.zero;
    Vector3 rotation = Vector3.zero;
    Vector3 cameraRotation = Vector3.zero;
    Rigidbody rb;
    
    CursorLockMode locked = CursorLockMode.Locked;
    CursorLockMode free = CursorLockMode.None;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = locked;
        cam = transform.Find("Player Camera").gameObject.GetComponent<Camera>();
    }

    void FixedUpdate()
    {
        PerformMovement();
        PerformRotation();
    }


    // public request methods //

    public void Move (Vector3 newVelocity)
    {
        velocity = newVelocity;
    }

    public void Rotate(Vector3 newRotation)
    {
        rotation = newRotation;
    }

    public void RotateCamera(Vector3 newCameraRotation)
    {
        cameraRotation = newCameraRotation;
    }

    public void ModifyCursor()
    {
        Cursor.lockState = (Cursor.lockState == locked) ? free : locked;
    }

    void PerformMovement()
    {
        if (velocity != Vector3.zero)
        {
            rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
        }
    }

    void PerformRotation()
    {
        rb.MoveRotation(rb.rotation * Quaternion.Euler(rotation));
        if (cam != null)
        {
            cam.transform.Rotate(-cameraRotation);
        }
    }
}

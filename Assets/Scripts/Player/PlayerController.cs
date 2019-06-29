using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool lockMovement = false;
    public bool lockRotation = false;
    public float moveSpeed = 50f;
    public float lookSensitivity = 3f;

    bool onMenu = false;
    PlayerMotor motor;

    void Start()
    {
        motor = GetComponent<PlayerMotor>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // TODO: Make this not ugly
            onMenu = !onMenu;
            motor.ModifyCursor();
            ToggleMovement(!onMenu);
            ToggleRotation(!onMenu);
        }

        // TODO: This might be redundant, move when possible
        if (onMenu) return;
        // calculate movement velocity
        if (!lockMovement)
        {
            float xMove = Input.GetAxis("Horizontal");
            float zMove = Input.GetAxis("Vertical");
            float yMove = Input.GetAxis("Jump");

            Vector3 moveHorizontal = transform.right * xMove;
            Vector3 moveVertical = transform.forward * zMove;
            Vector3 moveJump = transform.up * yMove;

            Vector3 velocity = Vector3.ClampMagnitude((moveHorizontal + moveVertical + moveJump) * moveSpeed, moveSpeed);

            motor.Move(velocity);
        }


        if (!lockRotation)
        {
            // calculate player rotation as a 3d vector
            float yRot = Input.GetAxisRaw("Mouse X");
            Vector3 rotation = new Vector3(0f, yRot, 0f) * lookSensitivity;

            motor.Rotate(rotation);

            HandleCamera();
        }
    }

    // handle camera rotation when necessary
    void HandleCamera()
    {
        float xRot = Input.GetAxisRaw("Mouse Y");
        Vector3 cameraRotation = new Vector3(xRot, 0f, 0f) * lookSensitivity;

        motor.RotateCamera(cameraRotation);
    }

    // stops all rotations & translations of both the camera and the player
    public void ToggleMovement(bool canMove)
    {
        if (!canMove)
        {
            motor.Move(Vector3.zero);
        }
        lockMovement = !canMove;
    }

    public void ToggleRotation(bool canRotate)
    {
        if (!canRotate)
        {
            motor.Rotate(Vector3.zero);
            motor.RotateCamera(Vector3.zero);
        }
        lockRotation = !canRotate;
    }
}

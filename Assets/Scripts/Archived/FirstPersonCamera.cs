using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// controls a primitive first person camera which follows the player
public class FirstPersonCamera : MonoBehaviour
{
    public float sensitivity = 100f;
    public bool controlPlayerYaw = true;

    CursorLockMode locked;
    CursorLockMode free;
    public Transform player;
    Vector3 offset;

    void Start()
    {
        // player = GameObject.Find("Player").transform;
        offset = transform.position - player.position;
        locked = CursorLockMode.Locked;
        free = CursorLockMode.None;
        Cursor.lockState = locked;
    }

    void Update()
    {
        transform.position = player.position + offset;
        HandleCursor();
        if (Cursor.lockState == free) return;
        HandleOrientation();
    }

    // let player toggle cursor lock with escape
    void HandleCursor()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = (Cursor.lockState == locked) ? free : locked;
        }
    }

    // rotate the camera based on mouse movement
    void HandleOrientation()
    {
        // rotate around y axis
        player.forward = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime, Vector3.up) * player.forward;

        // rotate around local x axis
        Vector3 rotatedAroundX = Quaternion.AngleAxis(-Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime, transform.right) * transform.forward;

        // rotation does not exceed the bounds of (-90, 90) around the local x axis
        Vector3 flatTransformForward = Vector3.ProjectOnPlane(transform.forward, Vector3.up);
        if (Vector3.Dot(flatTransformForward, rotatedAroundX) > 0f)
        {
            transform.forward = rotatedAroundX;
        }
    }

    void LateUpdate()
    {
        // TODO: make camera rotation independent from player rotation
        // move camera
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalCamera : MonoBehaviour
{

    public Transform playerCamera;
    public Transform portalLookingThrough;
    public Transform portalProjectingOn;
    // Update is called once per frame
    void Update()
    {
        Vector3 playerOffsetFromPortal = playerCamera.position - portalProjectingOn.position;
        transform.position = portalLookingThrough.position + playerOffsetFromPortal;

        float angularDifferenceBetweenPortalRotations = Quaternion.Angle(portalLookingThrough.rotation, portalProjectingOn.rotation);

        Quaternion portalRotationalDifference = Quaternion.AngleAxis(angularDifferenceBetweenPortalRotations, Vector3.up);
        Vector3 newCameraDirection = portalRotationalDifference * playerCamera.forward;
        transform.rotation = Quaternion.LookRotation(newCameraDirection, Vector3.up);

    }
}

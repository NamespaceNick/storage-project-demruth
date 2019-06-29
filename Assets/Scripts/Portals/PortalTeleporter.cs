using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalTeleporter : MonoBehaviour
{
    public Transform reciever; // RenderPlane of target portal

    bool playerIsOverlapping = false;
    Transform player;

    void Start()
    {
        player = GameObject.Find("Player").transform;
        if (player == null) Debug.LogError("PortalTeleporter could not find object named 'Player' in scene!");
    }


    void Update()
    {
        if (playerIsOverlapping)
        {
            Vector3 portalToPlayer = player.position - transform.position;
            float dotProduct = Vector3.Dot(transform.up, portalToPlayer);

            if (dotProduct < 0f)
            {

                float rotationDiff = -Quaternion.Angle(transform.rotation, reciever.rotation);
                rotationDiff += 180;
                player.Rotate(Vector3.up, rotationDiff);

                Vector3 positionOffset = Quaternion.Euler(0f, rotationDiff, 0f) * portalToPlayer;
                player.position = reciever.position + positionOffset;
                playerIsOverlapping = false;
            }
        }
    }

    void OnTriggerEnter (Collider other) 
    {
        if (other.tag == "Player")
        {
            playerIsOverlapping = true;
        }
    }
    void OnTriggerExit (Collider other) 
    {
        if (other.tag == "Player")
        {
            playerIsOverlapping = false;
        }
    }
}

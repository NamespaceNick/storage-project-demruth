using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RealmPlayer : MonoBehaviour
{
    public float interactRange = 1.25f;
    public float interactDelta = 20f;
    public float maxInteractRange = 4f;
    public float minInteractRange = 1.25f;
    public Text realmText;

    bool isDead = false;
    Transform cam;
    Transform selectCube;
    Camera playerCamera, previewCamera;
    PlayerController playerController;

    private bool leftMouseDown;
    private bool rightMouseDown;
    private Vector3 dragStartPos;

    void Start()
    {
        playerCamera = Camera.main;
        selectCube = transform.Find("Selection Cube");
        selectCube.gameObject.SetActive(false);
        realmText.enabled = false;
        playerController = GetComponent<PlayerController>();
    }

    void Update()
    {
        // TODO: Create HandleInput() function
        if (Input.GetKeyDown(KeyCode.Tab) && RealmManager.instance.DEVELOPMENT_WorldEditor)
        {
            RealmManager.instance.CycleEditingRealmType();
        }

        AdjustInteractRange();

        PositionSelectionCube();

        DetectBlockSwitchRequest();

        HandleRealmPreviews();

        DetectDeath();
    }

   // adjusts the distance between the selection cube and the player camera
    void AdjustInteractRange()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0f) // forward
        {
            interactRange = Mathf.Min(interactRange + (interactDelta * Time.deltaTime), maxInteractRange);
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f) // backward
        {
            interactRange = Mathf.Max(interactRange - (interactDelta * Time.deltaTime), minInteractRange);
        }
    }

    // handles selection cube activation/deactivation, places selection cube at crosshair coordinates
    void PositionSelectionCube()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            selectCube.gameObject.SetActive(!selectCube.gameObject.activeSelf);
        }

        if (!selectCube.gameObject.activeSelf) return;
        Vector3 lookingPoint = playerCamera.transform.position + (playerCamera.transform.forward * interactRange);
        if (Input.GetKey(KeyCode.F))
        {
            if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit))
            {
                lookingPoint = hit.transform.position + hit.normal;
            }
        }
        lookingPoint = Utility.RoundedVector(lookingPoint);

        selectCube.position = lookingPoint;
        selectCube.rotation = Quaternion.identity;
    }

    // handles switching blocks in regular mode and
    // placing and destroying block in god mode
    void DetectBlockSwitchRequest()
    {
        if(!selectCube.gameObject.activeSelf) return;

        // started left mouse drag
        if(Input.GetMouseButtonDown(0) && !rightMouseDown)
        {
            leftMouseDown = true;
            dragStartPos = selectCube.position;
        }
        // started right mouse drag
        else if(Input.GetMouseButtonDown(1) && !leftMouseDown)
        {
            rightMouseDown = true;
            dragStartPos = selectCube.position;
        }
        // finished left mouse drag
        else if(Input.GetMouseButtonUp(0) && leftMouseDown)
        {
            leftMouseDown = false;
            Vector3 dragEndPos = selectCube.position;
            RealmManager.instance.SwapRealmBlocks(/*createBlock*/ true, dragStartPos, dragEndPos, RealmManager.instance.realmViewing);
        }
        // finished right mouse drag
        else if(Input.GetMouseButtonUp(1) && rightMouseDown)
        {
            rightMouseDown = false;
            Vector3 dragEndPos = selectCube.position;
            RealmManager.instance.SwapRealmBlocks(/*createBlock*/ false, dragStartPos, dragEndPos, RealmManager.instance.realmViewing);
        }
    }


    // will switch the player's camera to show previews of each realm, and accept a realm shift request
    void HandleRealmPreviews()
    {
        // They want to look at the realm previews, switch camera and disable controls
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            StartCoroutine(RealmPreview());
        }
    }

    // determines whether the player has fallen out of bounds and requires a level reset
    void DetectDeath()
    {
        if ((transform.position.y < RealmManager.instance.worldFloor) && !isDead)
        {
            isDead = true;
            RealmManager.instance.RequestLevelReset();
        }
    }


    // handles the realm preview UI and interactions
    IEnumerator RealmPreview()
    {
        if (!Input.GetKeyDown(KeyCode.LeftShift))
        {
            Debug.LogError("RealmPreview called without having pressed leftshift");
            yield break;
        }
        playerController.ToggleMovement(false);
        realmText.enabled = true;
        playerCamera.enabled = false;
        previewCamera = RealmManager.instance.realmCameras[RealmManager.instance.realmViewing];
        previewCamera.enabled = true;

        // realm preview is being shown
        while (Input.GetKey(KeyCode.LeftShift))
        {
            yield return null;
            bool cycledLeft = Input.GetKeyDown(KeyCode.A);

            // cycle between realm previews
            if (cycledLeft || Input.GetKeyDown(KeyCode.D))
            {
                Debug.Log("Player is cycling: " + (cycledLeft ? "left" : "right"));
                previewCamera = RealmManager.instance.CycleRealmPreview(cycledLeft);
            }

            // the player wants to realm shift to the target realm preview
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Vector3 cameraToPlayer = transform.position - playerCamera.transform.position;

                // FIXME: Add checks to make sure it is possible to shift to that realm
                RealmManager.instance.RealmShift();
                transform.position = RealmManager.instance.realmCameras[RealmManager.instance.realmViewing].transform.position + cameraToPlayer;
            }
        }

        // LeftShift released, toggle off realm preview & re-enable controls
        realmText.enabled = false;
        playerController.ToggleMovement(true);
        previewCamera.enabled = false;
        playerCamera.enabled = true;

        yield break;
    }

}

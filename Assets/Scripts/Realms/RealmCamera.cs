using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealmCamera : MonoBehaviour
{
    public int realmID;

    [SerializeField]
    bool isSetup = false;
    Transform playerCam;
    Transform realm;

    void Start()
    {
        playerCam = Camera.main.transform;
    }

    void Update()
    {
        if (!isSetup) return;
        // preserve camera offset from realm origin
        transform.position = realm.position + 
            (playerCam.position - RealmManager.instance.realms[RealmManager.instance.currentRealm].position);

        // preserve camera rotation
        transform.rotation = Quaternion.LookRotation(playerCam.forward, playerCam.up);
    }

    public void SetupRealm(int myRealm)
    {
        realmID = myRealm;
        realm = RealmManager.instance.realms[realmID];
        isSetup = true;
    }
}

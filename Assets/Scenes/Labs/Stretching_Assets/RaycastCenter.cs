using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastCenter : MonoBehaviour
{
    public GameObject to_center;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit_info;
        if (Physics.Raycast(transform.position, transform.forward, out hit_info)) {
            to_center.transform.position = hit_info.point;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script for testing random things
public class TestScript : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        Debug.Log("Instance ID: " + gameObject.GetInstanceID());
    }
}

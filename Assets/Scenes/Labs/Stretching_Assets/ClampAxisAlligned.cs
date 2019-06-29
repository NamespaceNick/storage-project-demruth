using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClampAxisAlligned : MonoBehaviour
{
    public GameObject root;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 angle = root.transform.eulerAngles;
        angle.y = Mathf.Round(angle.y / 90) * 90;
        transform.eulerAngles = angle;
    }
}

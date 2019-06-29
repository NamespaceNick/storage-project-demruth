using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// contains scene information that allows RealmManager to re-acquire publicly assigned variables
// after scene re-loads
public class SceneStorage : MonoBehaviour
{
    public float worldFloor = -50f;
    public List<Transform> realms;
    public TextAsset worldEditLog;
    public GameObject defaultTile;
}

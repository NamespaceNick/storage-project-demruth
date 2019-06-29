using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldEditor : MonoBehaviour
{
    public static string filePath = Application.persistentDataPath + "/worldEditLog";

    static string delim = " ";
    static string objectDelimiter = ",";


    public static void BlockCreated(int realm, Realm blockType, Vector3 position)
    {
        // StreamWriter writer = new StreamWriter(filePath, true);
        Debug.Log("C" + delim + realm + delim + blockType.ToString() + delim + position.x + delim
            + position.y + delim + position.z + objectDelimiter);
    }

    // TODO: Transfer function from RealmManager to here
    public static void BlockDeleted(int realm, Realm blockType, Vector3 position)
    {
        Debug.LogWarning("Block Deleted called, but not yet implemented.");
    }

    // TODO: Transfer function from RealmManager to here
    public static void CreateBlock(int realm, Realm blockType, Vector3 position)
    {

    }
}

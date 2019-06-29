using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class WorldTools : ScriptableWizard
{
/*
    TextAsset worldEditFile;
    GameObject defaultTile;
    public string tilePaths = "Assets/Scenes/Labs/RealmShift_Assets/";
    public string worldEditLogPath = "Assets/Scenes/Labs/RealmShift_Assets/worldEditLog.txt";

    // TODO: Make an editor script that runs this whenever the editor stops playing something
    [MenuItem("World Editor/Import World Edits")]
    static void CreateWizard()
    {
        if (IsGameActive()) return;
        ScriptableWizard.DisplayWizard<WorldTools>("World Edit Importer", "Import world edits");
    }

    // reads from the text file containing the log of all edits made while in
    // world edit mode and implements those changes
    void OnWizardCreate()
    {
        if (IsGameActive()) return;
        AssetDatabase.ImportAsset(worldEditLogPath);
        defaultTile = AssetDatabase.LoadAssetAtPath<GameObject>(tilePaths + "Default Realm Tile.prefab");
        worldEditFile = AssetDatabase.LoadAssetAtPath<TextAsset>(worldEditLogPath);

        // tokenize each modification log entry
        foreach (string tileModification in worldEditFile.text.Split(','))
        {
            // check if end of file
            if (tileModification == "" )
            {
                Debug.Log("Reached the end of the file with string: " + tileModification);
                continue;
            }
            // tokenize the tile modification log entry
            string[] blockEdit = tileModification.Split(' ');

            // check token size invariant
            // token order[ {C(reate)/D(estroy)} , {realm int} , {block type} , {x}, {y}, {z} ]
            if (blockEdit.Length != 6)
            {
                Debug.LogError("ERROR: tile change string did not have 6 tokens: " + tileModification);
                continue;
            }

            // store tokens
            string action = blockEdit[0];
            int realmNum = int.Parse(blockEdit[1]);
            string typeString = blockEdit[2];
            Vector3 expectedPosition = new Vector3(
                float.Parse(blockEdit[3]), float.Parse(blockEdit[4]), float.Parse(blockEdit[5]));

            if (action == "C")
            {
                // acquire tile type
                GameObject tilePrefab;
                switch (typeString)
                {
                    case "Default":
                        tilePrefab = defaultTile;
                        break;
                    default:
                        Debug.LogError("Incorrect tile type (" + typeString + ") given for tile at: " + expectedPosition);
                        continue;
                }

                // acquire target realm
                string parentObjectStr = "Realm " + realmNum + "/Tiles/" + typeString;
                GameObject parentObject = GameObject.Find(parentObjectStr);
                if (parentObject == null)
                {
                    Debug.LogError("Could not find parentObject '" + parentObjectStr + "' for block at: " +
                        expectedPosition);
                    continue;
                }

                CreateBlock(tilePrefab, typeString, expectedPosition, parentObject);

            }
            else if (action == "D")
            {
                // TODO: Implement block destroy functions
                Debug.Log("Delete called, but not yet implemented");
                continue;
            }
        }
        ClearEditLog();
        AssetDatabase.ImportAsset(worldEditLogPath);
        // TODO: Clear out the file
    }

    // clears all text from the world edit log
    void ClearEditLog()
    {
        // create streamwriter that will overwrite the document
        StreamWriter w = new StreamWriter(worldEditLogPath, false);
        w.Close();
        Debug.Log("World edit log cleared");
    }

    // instantiates an instance of the given prefab with the given name at objPos as a child of parent
    // parent should be Realm {}/Tiles/{type}
    static void CreateBlock(GameObject prefab, string objName, Vector3 objPos, GameObject parent)
    {
        GameObject newBlock = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        newBlock.transform.position = objPos;
        newBlock.transform.rotation = Quaternion.identity;
        newBlock.transform.SetParent(parent.transform);
        newBlock.name = objName;
        Debug.Log("Creating block at " + objPos);
    }

    // deletes block located at position
    static void DeleteBlock(Vector3 position)
    {
        // TODO: Implement this function
    }

    // ensures game is not running or paused when importer is called
    static bool IsGameActive()
    {
        Debug.LogError("Cannot Import world edits while editor is in play mode or paused mode");
        return EditorApplication.isPlaying;
    }
    */
}

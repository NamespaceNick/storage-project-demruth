using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// ensure class initializer is called whenever scripts recompile
[InitializeOnLoad]
public static class ApplyChanges
{
    static TextAsset worldEditFile;
    static Dictionary<string, GameObject> tileTable;
    static string tilePaths = "Assets/Scenes/Labs/RealmShift_Assets/Tiles/";
    static string worldEditLogPath = "Assets/Scenes/Labs/RealmShift_Assets/worldEditLog.txt";
    static Dictionary<Vector3, GameObject> blockMap = new Dictionary<Vector3, GameObject>();

    static ApplyChanges()
    {
        EditorApplication.playModeStateChanged += SaveWorldEdits;
    }

    static void SaveWorldEdits(PlayModeStateChange state)
    {
        AcquireWorldMap();
        if (state == PlayModeStateChange.EnteredEditMode)
        {
            Debug.Log("Saving World Edits...");
            AssetDatabase.ImportAsset(worldEditLogPath);
            // FIXME: Rename default realm tile for standardized algorithm for initializing all tiles
            tileTable = new Dictionary<string, GameObject>();
            worldEditFile = AssetDatabase.LoadAssetAtPath<TextAsset>(worldEditLogPath);
            FillTileTable();

            // tokenize each modification log entry
            foreach (string tileModification in worldEditFile.text.Split(','))
            {
                // check if end of file
                if (tileModification == "")
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
                int realmNumOrObjID = int.Parse(blockEdit[1]);
                string typeString = blockEdit[2];
                Vector3 expectedPosition = new Vector3(
                    float.Parse(blockEdit[3]), float.Parse(blockEdit[4]), float.Parse(blockEdit[5]));

                if (action == "C")
                {
                    // acquire tile type
                    GameObject tilePrefab = tileTable[typeString];
                    // acquire target realm
                    string parentObjectStr = "Realm " + realmNumOrObjID + "/Tiles/";
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
                    DeleteBlock(realmNumOrObjID, expectedPosition);
                    continue;
                }
            }
            ClearEditLog();
            AssetDatabase.ImportAsset(worldEditLogPath);
        }
        Debug.Log("Finished saving world edits.");
    }

    // instantiates an instance of the given prefab with the given name at objPos as a child of parent
    // parent should be Realm {}/Tiles/{type}
    static void CreateBlock(GameObject prefab, string objName, Vector3 objPos, GameObject parent)
    {
        if (blockMap.ContainsKey(objPos)) return;
        GameObject newBlock = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        newBlock.transform.position = objPos;
        newBlock.transform.rotation = Quaternion.identity;
        newBlock.transform.SetParent(parent.transform);
        newBlock.name = objName;
        blockMap[objPos] = newBlock;
        Debug.Log("Creating block at " + objPos);
    }

    // deletes block located at position
    // TODO: Implement an undo function (after prototype)
    static void DeleteBlock(int objID, Vector3 objPos)
    {
        if (!blockMap.ContainsKey(objPos)) return;
        GameObject toDestroy = blockMap[objPos];
        Debug.Log("Destroying object with ID " + objID + " at location " + toDestroy.transform.position);
        blockMap.Remove(objPos);
        GameObject.DestroyImmediate(toDestroy);
    }


    // initializes hash map with all blocks that existed at the time of AcquireWorldMap() being called
    static void AcquireWorldMap()
    {
        blockMap.Clear();
        GameObject[] allTiles = GameObject.FindGameObjectsWithTag("RealmTile");
        Debug.Log("Located " + allTiles.Length + " tiles to add to WorldMap");
        foreach (GameObject tile in allTiles)
        {
            // TODO: Remove the RoundedVector() after enforcing integer positions
            blockMap.Add(Utility.RoundedVector(tile.transform.position), tile);
        }
    }

    // clears all text from the world edit log
    static void ClearEditLog()
    {
        // create streamwriter that will overwrite the document
        StreamWriter w = new StreamWriter(worldEditLogPath, false);
        w.Close();
        Debug.Log("World edit log cleared");
    }


    // instantiates the hash map with the different tiles
    // TODO: refactor to not be hardcoded
    static void FillTileTable()
    {
        tileTable.Add("Default", AssetDatabase.LoadAssetAtPath<GameObject>((tilePaths + "Default Realm Tile.prefab")));
        tileTable.Add("Water", AssetDatabase.LoadAssetAtPath<GameObject>((tilePaths + "Water Tile.prefab")));
        tileTable.Add("Earth", AssetDatabase.LoadAssetAtPath<GameObject>((tilePaths + "Earth Tile.prefab")));
        tileTable.Add("Fire", AssetDatabase.LoadAssetAtPath<GameObject>((tilePaths + "Fire Tile.prefab")));
    }
}

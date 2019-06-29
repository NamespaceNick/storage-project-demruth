using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;

public class RealmManager : MonoBehaviour
{
    public static RealmManager instance;

    public bool DEVELOPMENT_WorldEditor = false;
    public bool DEVELOPMENT_GodMode = false;
    public float shrinkForward = 1f;
    public float worldFloor = -100f;
    public int currentRealm = 0;
    public int realmViewing = 0;
    [HideInInspector]
    public List<Transform> realms;
    [HideInInspector]
    public List<Camera> realmCameras;
    public GameObject realmCameraPrefab;
    public Text realmTypeText;
    public TextAsset worldEditLog;
    public GameObject defaultTilePrefab; // TODO: Remove when refactoring
    public GameObject[] tilePrefabs; // TODO: Move to WorldEditor when refactoring

    private int tilePrefabIdx = 0; // TODO: Remove this after refactoring
    private bool isTransitioning = false;
    private int tileLayerMask = 0;
    private Dictionary<Realm, GameObject> tileTable = new Dictionary<Realm, GameObject>();
    private Realm activeRealmEditType = Realm.Default;

    void Awake()
    {
        // singleton
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

        HandleDebugModes();

        // instantiate map that holds tilePrefabs
        foreach (GameObject t in tilePrefabs)
        {
            tileTable.Add(t.GetComponent<RealmTile>().realm, t);
        }

        // determine layer mask for tiles
        tileLayerMask = 1 << LayerMask.NameToLayer("RealmTile");

        SceneManager.sceneLoaded += OnLevelLoaded;
    }

    void OnLevelLoaded(Scene scene, LoadSceneMode mode)
    {
        ClearAndUpdateSceneReferences();
    }

    // switches the selected block in the current realm with the sister-block in the target realm
    // returns false if there was not a block that could be switched at the desired position in one of the realms
    // TODO: Separate this into a different function for world edit calls
    public void SwapRealmBlocks(bool createBlock, Vector3 startPos, Vector3 endPos, int targetRealm)
    {

        foreach (Vector3 blockPos in new Utility.BlockRegion(startPos, endPos))
        {
            // TODO: Move this to the WorldMap script (after prototype)
            if (DEVELOPMENT_WorldEditor)
            {
                // TODO: implement detection for the block type when we create more
                if (createBlock)
                {
                    if (WorldMap.instance.Contains(blockPos))
                    {
                        Debug.LogError("World Map already contains block at position: " + blockPos);
                        return;
                    }
                    Debug.Log("Create block request");
                    string parentStr = "Realm " + currentRealm + "/Tiles/";
                    GameObject parentObj = GameObject.Find(parentStr);
                    if (parentObj != null) CreateBlock(tileTable[activeRealmEditType], blockPos, parentObj);
                }
                else // delete the block
                {
                    if (!WorldMap.instance.Contains(blockPos))
                    {
                        Debug.LogError("World Map doesn't contain block at position: " + blockPos);
                    }
                    Debug.Log("Destroy block request");
                    RemoveBlock(blockPos);
                }
            }
            else
            {
                // acquire global position of the new block
                Vector3 blockOtherDimensionPos = blockPos + (realms[targetRealm].position - realms[currentRealm].position);

                // get reference to blocks by a raycast
                GameObject block = GetRealmBlock(blockPos);
                GameObject blockOtherDimension = GetRealmBlock(blockOtherDimensionPos);

                // switch block locations
                if (block != null) block.transform.position = blockOtherDimensionPos;
                if (blockOtherDimension != null) blockOtherDimension.transform.position = blockPos;
            }
        }
    }

    // Shoots a small raycast to collide with and get a reference to a desired realm block
    GameObject GetRealmBlock(Vector3 globalPosition)
    {
        RaycastHit hit;
        Debug.DrawRay(globalPosition, Vector3.forward, Color.red);
        Debug.Log("Ray start point: " + globalPosition);
        if (Physics.Raycast(globalPosition + Vector3.forward, -Vector3.forward, out hit, shrinkForward, tileLayerMask))
        {
            Debug.Log("Raycast hit: " + hit.transform.gameObject.name);
            return hit.transform.gameObject;
        }
        return null;
    }

    int mod(int x, int m) {
        return (x%m + m)%m;
    }

    // updates which realm is being previewed, takes care of previewCamera handoff
    public Camera CycleRealmPreview(bool cycleLeft)
    {
        // disable current preview camera
        realmCameras[realmViewing].enabled = false;
        realmViewing = mod(realmViewing + (cycleLeft ? -1 : 1), realms.Count);

        // enable and return new preview camera
        realmCameras[realmViewing].enabled = true;
        return realmCameras[realmViewing];
    }

    public void RequestLevelReset()
    {
        if (isTransitioning) return;
        isTransitioning = true;
        StartCoroutine(LevelReset());
    }

    // called by player that wants to realm shift, ensures proper setting of variable
    public int RealmShift()
    {
        currentRealm = realmViewing;
        return currentRealm;
    }

    IEnumerator LevelReset()
    {
        Debug.Log("Resetting Level");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // records world edits in the worldEditLog asset, so that all world edits can be imported once the scene is done
    // TODO: Move this to a WorldEditor class
    // FIXME: Horrible hack, setting currentRealmString to object instance if it is a delete (after prototype)
    void RecordWorldEdit(bool wasCreated, Vector3 position, Realm realmType)
    {
        if (worldEditLog == null)
        {
            Debug.LogError("world edit log not found");
            return;
        }
        // include the object instance ID if it was a deletion
        string currentRealmString = wasCreated ? currentRealm.ToString() : WorldMap.instance[position].GetInstanceID().ToString();
        string createOrDestroy = wasCreated ? "C" : "D";
        string worldEditString = createOrDestroy + " " + currentRealmString + " " + realmType.ToString() + " " + Mathf.Round(position.x) + " " + Mathf.Round(position.y) + " " + Mathf.Round(position.z) + ",";

        // append to the world edit log
        Debug.Log("Log Entry: " + worldEditString);

        StreamWriter writer = new StreamWriter(AssetDatabase.GetAssetPath(worldEditLog), true);
        // log template - [{C(reate)/D(estroy)} {realm int} {tileType} {x} {y} {z},]
        writer.Write(worldEditString);
        writer.Flush();
        writer.Close();
    }

    // clears scene-specific values and re-acquires scene-specific references
    void ClearAndUpdateSceneReferences()
    {
        // reset persistent, scene-specific values
        currentRealm = realmViewing = 0;
        isTransitioning = false;
        realmCameras.Clear();
        tilePrefabIdx = 0;

        // acquire necessary scene references()
        SceneStorage sceneStorage = GameObject.Find("Scene Storage").GetComponent<SceneStorage>();
        if (sceneStorage)
        {
            realms = sceneStorage.realms;
            worldFloor = sceneStorage.worldFloor;
            worldEditLog = sceneStorage.worldEditLog;
        }

        // setup world edit mode
        if (DEVELOPMENT_WorldEditor)
        {
            // acquire log to record world edits
            worldEditLog = sceneStorage.worldEditLog;

            // instantiate text for which type of block will be placed
            GameObject checkTextExists = GameObject.Find("Realm Type Text");
            if (checkTextExists)
            {
                realmTypeText = checkTextExists.GetComponent<Text>();
            }
            else
            {
                Debug.LogError("RealmManager could not acquire Realm Type Text object");
            }
            realmTypeText.enabled = DEVELOPMENT_WorldEditor;
            SetRealmTypeText(Realm.Default);
        }

        CreateRealmCameras();
    }

    // creates a preview camera for each dimension
    void CreateRealmCameras()
    {
        for (int i = 0; i < realms.Count; ++i)
        {
            GameObject realmCam = Instantiate(realmCameraPrefab, realms[i].position, Quaternion.identity);
            RealmCamera rc = realmCam.GetComponent<RealmCamera>();
            rc.SetupRealm(i);
            realmCam.GetComponent<Camera>().enabled = false;
            realmCameras.Add(realmCam.GetComponent<Camera>());
        }
    }

    // FIXME: This function belongs in WorldMap (after prototype)
    // for world editing. creates an instance of prefab at the desired location and with the identified parent
    void CreateBlock(GameObject prefab, Vector3 pos, GameObject parent)
    {
        // create block that player wants
        GameObject createdBlock = (GameObject) PrefabUtility.InstantiatePrefab(tileTable[activeRealmEditType]);
        createdBlock.transform.position = Utility.RoundedVector(pos);
        createdBlock.transform.rotation = Quaternion.identity;
        createdBlock.transform.SetParent(parent.transform);
        Debug.Log("Created block at (" + pos.x + ", " + pos.y + ", " + pos.z + ")");
        WorldMap.instance.AddBlock(createdBlock);
        RecordWorldEdit(true, createdBlock.transform.position, activeRealmEditType);
    }

    // disables block at specified position, removes it from worldMap, records delete in world edit log
    // FIXME: remove need for realmType (after prototype)
    void RemoveBlock(Vector3 blockPos)
    {
        if (!WorldMap.instance.Contains(blockPos)) return;

        // realm type doesn't matter because it's a delete
        RecordWorldEdit(false, blockPos, Realm.Default);
        WorldMap.instance.DeleteBlock(blockPos);
    }

    public void CycleEditingRealmType()
    {
        Debug.Assert(DEVELOPMENT_WorldEditor);
        tilePrefabIdx = (tilePrefabIdx + 1) % tilePrefabs.Length;
        activeRealmEditType = tilePrefabs[tilePrefabIdx].GetComponent<RealmTile>().realm;
        SetRealmTypeText(activeRealmEditType);
    }


    void SetRealmTypeText(Realm realmType)
    {
        if (!DEVELOPMENT_WorldEditor) return;
        realmTypeText.text = realmType.ToString();
        realmTypeText.color = tileTable[realmType].GetComponent<RealmTile>().realmColor;
    }

    // determines if the scene should be in god mode or in world edit mode
    // if so, carries out actions to prepare and begin those modes
    void HandleDebugModes()
    {
        DEVELOPMENT_GodMode |= DEVELOPMENT_WorldEditor;
        if (DEVELOPMENT_GodMode)
        {
            GameObject.Find("Realm Player").GetComponent<Rigidbody>().useGravity = false;
            GameObject.Find("Realm Player").GetComponent<Collider>().enabled = false;
        }

    }
}

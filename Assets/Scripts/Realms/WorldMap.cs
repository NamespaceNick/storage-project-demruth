using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldMap : MonoBehaviour
{
    public static WorldMap instance;
    static Dictionary<Vector3, GameObject> worldMap = new Dictionary<Vector3, GameObject>();
    static bool mapInitialized = false;

    // singleton
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
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        float startTime = Time.realtimeSinceStartup;
        // TODO: Create editor script to ensure all blocks are rounded positions
        mapInitialized = false;
        worldMap.Clear();
        GameObject[] allTiles = GameObject.FindGameObjectsWithTag("RealmTile");
        Debug.Log("Located " + allTiles.Length + " tiles to add to WorldMap");
        foreach (GameObject tile in allTiles)
        {
            // TODO: Remove the RoundedVector() after enforcing integer positions
            worldMap.Add(Utility.RoundedVector(tile.transform.position), tile);
        }
        float totalTime = Time.realtimeSinceStartup - startTime;
        Debug.Log("Added all tiles to WorldMap in " + totalTime + " seconds");
        mapInitialized = true;
    }

    // determines if a block already exists at blockPos
    public bool Contains(Vector3 blockPos)
    {
        if (!mapInitialized)
        {
            // TODO: don't just make it return true (after prototype)
            Debug.LogError("Contains() called before WorldMap was initialized");
            return true;
        }
        return worldMap.ContainsKey(Utility.RoundedVector(blockPos));
    }

    // public accessor for the worldMap
    // FIXME: Probably remove this (after prototype)
    public GameObject this[Vector3 idx]
    {
        get
        {
            Vector3 roundIdx = Utility.RoundedVector(idx);
            return worldMap.ContainsKey(roundIdx) ? worldMap[roundIdx] : null;
        }

    }

    // removes a block from the worldMap
    public void DeleteBlock(Vector3 position)
    {
        Vector3 roundedPos = Utility.RoundedVector(position);
        if (!mapInitialized)
        {
            Debug.LogError("DeleteBlock() called before WorldMap was initialized");
            return;
        }
        if (worldMap.ContainsKey(roundedPos))
        {
            worldMap[roundedPos].SetActive(false);
            worldMap.Remove(roundedPos);
        }

        // TODO: record block removal in persistent file (after prototype) (currently in RealmManager)
    }

    // adds a block to the worldMap if it doesn't already exist
    // if block already exists, return false
    // TODO: Make the worldMap place the blocks (after prototype)
    public bool AddBlock(/*Vector3 posToPlace, */GameObject newObject)
    {
        if (!mapInitialized)
        {
            Debug.LogError("AddBlock() called before WorldMap was initialized");
            return false;
        }
        // if (worldMap.ContainsKey(posToPlace)) return false;
        worldMap.Add(Utility.RoundedVector(newObject.transform.position), newObject);
        return true;

        // TODO: record block placement in persistent file (after prototype)
    }

    public bool IsInitialized()
    {
        return mapInitialized;
    }

    // called when two tiles are switched, so their placements in worldMap can be adjusted
    public void SwitchTiles(Vector3 firstTile, Vector3 secondTile)
    {
        if (!mapInitialized)
        {
            Debug.LogError("SwitchTiles() called before WorldMap was initialized");
            return;
        }
        // TODO: This function
    }
}

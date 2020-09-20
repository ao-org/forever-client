using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldManager : MonoBehaviour
{
    // Flag for local testing (true = testing engabled)
    [SerializeField] private bool mLocalDebugMode;

    // Spawn scene for testing mode
    [SerializeField] private int mTestingMapID;

    // Currently loaded map scenes
    private IntStringDictionary mCurrentlyLoadedMapScenes = new IntStringDictionary();

    // Active map reference
    private Map mActiveMap = null;

    // Async loading helpers ****
    private bool mWaitingNextFrameToLoadSyncMap = false;
    private string mLastSyncMapName;
    private int mFrameSkipCount = 0;

    private bool mWaitingForMapReposition = false;
    //***************************

    #region singleton
    private static WorldManager _instance;
    public static WorldManager Instance { get { return _instance; } }
    #endregion

    #region unity loop
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else
        {
            _instance = this;
        }
    }

    private void Start()
    {
        // Test mode (local)
        if (mLocalDebugMode)
        {
            // Retrieve the local character
            CharacterInfo character = GameObject.FindObjectOfType<CharacterInfo>();

            // Teleport to testing level
            character.TeleportToMap(mTestingMapID, new Vector2(50, 50), CardinalDirection.SOUTH);

            // Manage spawn scene management
            LoadScenesAfterSpawn(mTestingMapID);
        }
    }

    private void Update()
    {
        // Async map loading
        if (mWaitingNextFrameToLoadSyncMap)
        {
            if (mFrameSkipCount > 0)
            {
                mFrameSkipCount--;
            }
            else
            {
                // Set the new active scene
                SceneManager.SetActiveScene(SceneManager.GetSceneByName(mLastSyncMapName));

                // Retrieve active maps exits (only cardinal directions)
                mActiveMap = GameObject.FindObjectOfType<Map>();
                Vector2 currentSceneCoordinates = new Vector2(mActiveMap.transform.position.x, mActiveMap.transform.position.y);

                // Load adjacents maps (async)
                foreach (KeyValuePair<CardinalDirection, int> adjacentMap in mActiveMap.mAdjacentMaps)
                {
                    string sceneName = MapScenesManager.GetNameFor(adjacentMap.Value);
                    if (!WorldManager._instance.mCurrentlyLoadedMapScenes.ContainsKey(adjacentMap.Value))
                    {
                        // Load scene
                        WorldManager.LoadMapAsync(sceneName);
                        WorldManager._instance.mCurrentlyLoadedMapScenes.Add(adjacentMap.Value, sceneName);
                    }
                }

                mLastSyncMapName = "";
                mWaitingNextFrameToLoadSyncMap = false;
                mWaitingForMapReposition = true;
                mFrameSkipCount = 2;
            }
        }

        // Async map repositions
        else if (mWaitingForMapReposition)
        {
            if (mFrameSkipCount > 0)
            {
                mFrameSkipCount--;
            }
            else
            {
                StartCoroutine("RelocateAdjacentMaps");
                mWaitingForMapReposition = false;
            }
        }
    }
    #endregion

    private IEnumerator RelocateAdjacentMaps()
    {
        // Fetch all loaded maps
        foreach (KeyValuePair<CardinalDirection, int> adjacentMap in mActiveMap.mAdjacentMaps)
        {
            // Get the map GO
            GameObject aMap = null;
            while (aMap == null)
            {
                aMap = GameObject.Find("Map_" + adjacentMap.Value);
                yield return new WaitForSeconds(.1f);
            }

            // Get the map component
            Map mapComponent = aMap.GetComponent<Map>();         
            
            // Reposition the map
            Vector2 adjacentPosition = GetOriginForAdjacentMap(adjacentMap.Key, mActiveMap.transform.position);
            aMap.transform.position = new Vector3(adjacentPosition.x, adjacentPosition.y, aMap.transform.position.z);
        }
    }

    private void LoadScenesAfterSpawn(int activeMapID)
    {
        // Retrieve map scene name
        string mapName = MapScenesManager.GetNameFor(activeMapID);

        // Load scene and wait
        SceneManager.LoadScene(mapName, LoadSceneMode.Additive);
        WorldManager._instance.mCurrentlyLoadedMapScenes.Add(activeMapID, mapName);

        mLastSyncMapName = mapName;
        mWaitingNextFrameToLoadSyncMap = true;
        mFrameSkipCount = 2;
    }

    public static void LoadMapAsync(string sceneName)
    {
        AsyncOperation loadMap = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
    }

    public Vector2 GetPositionInMap(int mapID, Vector2 worldPos)
    {
        // TODO return converted pos relative to the map
        // Could be like...
        // If I'm in map 34, I check where that map is in X Y relative to the world grid.
        // So if map 34 is in [3,4], I know the offset is 200x200.
        // 200 * 3 = 600 in X
        // 200 * 4 = 800 in Y
        // Those are the absolute coordinates.
        // The inverse operation should return pos relative to map, instead of absolute pos.
        return Vector2.up;
    }

    public Vector2 GetOriginForAdjacentMap(CardinalDirection direction, Vector2 activeMapOrigin)
    {
        Vector2 adjacentOrigin = Vector2.zero;
        int x = 0;
        int y = 0;

        switch (direction)
        {
            case CardinalDirection.NORTH :
                                            y = (int) activeMapOrigin.y + Map.MAP_SIZE.y;
                                            break;
            case CardinalDirection.NORTHEAST:
                                            x = (int)activeMapOrigin.x + Map.MAP_SIZE.x;
                                            y = (int)activeMapOrigin.y + Map.MAP_SIZE.y;
                                            break;
            case CardinalDirection.EAST:
                                            x = (int)activeMapOrigin.x + Map.MAP_SIZE.x;
                                            break;
            case CardinalDirection.SOUTHEAST:
                                            x = (int)activeMapOrigin.x + Map.MAP_SIZE.x;
                                            y = (int)activeMapOrigin.y - Map.MAP_SIZE.y;
                                            break;
            case CardinalDirection.SOUTH:
                                            y = (int)activeMapOrigin.y - Map.MAP_SIZE.y;
                                            break;
            case CardinalDirection.SOUTHWEST:
                                            x = (int)activeMapOrigin.x - Map.MAP_SIZE.x;
                                            y = (int)activeMapOrigin.y - Map.MAP_SIZE.y;
                                            break;
            case CardinalDirection.WEST:
                                            x = (int)activeMapOrigin.x - Map.MAP_SIZE.x;
                                            break;
            case CardinalDirection.NORTHWEST:
                                            x = (int)activeMapOrigin.x - Map.MAP_SIZE.x;
                                            y = (int)activeMapOrigin.y + Map.MAP_SIZE.y;
                                            break;
        }

        adjacentOrigin = new Vector2(x, y);
        return adjacentOrigin;
    }

    public static void ProcessMapChange(int destinationMapID, CharacterInfo character)
    {
        //UnityEngine.Debug.Log(" PROCESSING MAP CHANGE TO " + destinationMapID);

        // If the new map is already loaded...
        if (WorldManager._instance.mCurrentlyLoadedMapScenes.ContainsKey(destinationMapID))
        {
            // Get the active map GO and component
            Map newActiveMap = GameObject.Find("Map_" + destinationMapID).GetComponent<Map>();

            // Unload not-adjacent map scenes
            foreach (int loadedMapID in WorldManager._instance.mCurrentlyLoadedMapScenes.Keys)
            {
                // If the loaded map scene is not adjacent of the new active map...
                if (!newActiveMap.mAdjacentMaps.ContainsValue(loadedMapID))
                {
                    // Unload the map scene
                    SceneManager.UnloadSceneAsync(WorldManager._instance.mCurrentlyLoadedMapScenes[loadedMapID]);
                }
            }
        }

        // If the new map is not loaded yet...
        else
        {
            // Retrieve map scene name
            string mapName = MapScenesManager.GetNameFor(destinationMapID);

            // Load scene and wait
            SceneManager.LoadScene(mapName, LoadSceneMode.Additive);
            WorldManager._instance.mCurrentlyLoadedMapScenes.Add(destinationMapID, mapName);
        }

        // Clear adjacents
        WorldManager._instance.mCurrentlyLoadedMapScenes.Clear();
        WorldManager._instance.mLastSyncMapName = MapScenesManager.GetNameFor(destinationMapID);
        WorldManager._instance.mWaitingNextFrameToLoadSyncMap = true;
        WorldManager._instance.mFrameSkipCount = 2;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldManager : MonoBehaviour
{
    #region singleton
    private static WorldManager _instance;
    public static WorldManager Instance { get { return _instance; } }

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
    #endregion

    public void LoadMap(int mapID)
    {
        AsyncOperation loadMap = SceneManager.LoadSceneAsync(mapID.ToString(), LoadSceneMode.Additive);

        // TODO add handler for when loadMap finished loading
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
}

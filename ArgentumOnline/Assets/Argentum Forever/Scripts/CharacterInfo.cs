using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInfo : MonoBehaviour
{
    [SerializeField] public int mCurrentMapID;
    [SerializeField] public string mCharactername = "Unnamed Character";

    #region unity loop
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(transform.parent);
    }
    #endregion

    public void TeleportToMap(int mapID, Vector2 position, CardinalDirection direction)
    {
        mCurrentMapID = mapID;
        transform.position = new Vector3(position.x, position.y, transform.position.z);
        //TODO forzar direccion
        //TODO conciliar con el server
    }

    public void EnteredMap(int mapID)
    {
        mCurrentMapID = mapID;
        //TODO conciliar con el server
    }

    public Vector2 GetPositionInCurrentMap()
    {
        // Real position
        Vector2 realPosition = new Vector2(transform.position.x, transform.position.y);

        //TODO realizar traduccion de las coordenadas del mapa actual a coordenadas del mundo

        // Return the real position
        return realPosition;
    }
}

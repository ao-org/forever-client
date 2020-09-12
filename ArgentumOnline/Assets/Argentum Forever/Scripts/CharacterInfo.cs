using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInfo : MonoBehaviour
{
    [SerializeField] private int mCurrentMapID;

    #region unity loop
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    #endregion

    public void EnteredMap(int mapID, Vector2 position, CardinalDirection direction)
    {
        mCurrentMapID = mapID;
        //TODO actualizar posicion del personaje
        //TODO conciliar con el server
    }

    public Vector2 GetPositionInCurrentMap()
    {
        // Real position
        Vector2 realPosition = Vector2.zero;

        //TODO realizar traduccion de las coordenadas del mapa actual a coordenadas del mundo

        // Return the real position
        return realPosition;
    }
}

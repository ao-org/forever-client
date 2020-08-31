using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    // Basic information
    [SerializeField] private int mID;
    [SerializeField] private string mMapName;
    [SerializeField] private static Vector2Int MAP_SIZE = new Vector2Int(190, 130);

    // Connected maps
    private Dictionary<CardinalDirection, int> mConnectedMaps;

    // Dungeons (offmap connections)
    //TODO cambiar string por script "Portal"
    private Dictionary<string, int> mOffmapConnections;

    // Custom music
    [SerializeField] private AudioClip[] mMusicTracks;

    // Custom SFXs
    [SerializeField] private AudioClip[] mAmbienceSFXs;

    private void Awake()
    {
        //TODO cargar automaticamente las offmapconnections
    }

}

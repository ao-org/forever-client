using System;
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
    private Dictionary<Portal, int> mConnectedMaps;

    // Dungeons (offmap connections) <Portal prefab, Destination Map ID>
    private Dictionary<Portal, int> mOffmapConnections;

    // Custom music
    [SerializeField] private AudioClip[] mMusicTracks;

    // Custom SFXs
    [SerializeField] private AudioClip[] mAmbienceSFXs;

    private void Awake()
    {
        // Load off-map connections
        LoadAllMapExits();
    }

    private void LoadAllMapExits()
    {
        // Portals dictionary
        mOffmapConnections = new Dictionary<Portal, int>();

        // Collect all "Portal" childs
        foreach (Transform childPortals in transform.Find("Portals"))
        {
            Portal portal = childPortals.GetComponent<Portal>();
            if (portal != null && !portal.mIsEdge)
            {
                mOffmapConnections.Add(portal, portal.mDestinationMapID);
            }
        }

        // Edge exits dictionary
        mConnectedMaps = new Dictionary<Portal, int>();

        // Collect all "Portal" childs
        foreach (Transform childPortals in transform.Find("Edges"))
        {
            Portal portal = childPortals.GetComponent<Portal>();
            if (portal != null && portal.mIsEdge)
            {
                mConnectedMaps.Add(portal, portal.mDestinationMapID);
            }
        }
    }
}

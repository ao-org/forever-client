﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    // Basic information
    [SerializeField] public int mID;
    [SerializeField] public string mMapName;
    [SerializeField] public static Vector2Int MAP_SIZE = new Vector2Int(200, 200);

    // Adjacent maps (including non-walkable diagonals)
    [SerializeField] public DirectionIntDicionary mAdjacentMaps = new DirectionIntDicionary();

    // Dungeons (offmap connections) <Portal prefab, Destination Map ID>
    private Dictionary<Portal, int> mOffmapConnections;

    // Custom music
    [SerializeField] private AudioClip[] mMusicTracks;

    // Custom SFXs
    [SerializeField] private AudioClip[] mAmbienceSFXs;

    private void Awake()
    {
        // Load off-map connections
        LoadAllMapExitsReferences();

        // Change GO name to include the map ID
        gameObject.name += "_" + mID;
    }

    private void LoadAllMapExitsReferences()
    {
        // Portals dictionary
        mOffmapConnections = new Dictionary<Portal, int>();

        // Fetch all "Portal" childs
        foreach (Transform childPortals in transform.Find("Portals"))
        {
            Portal portal = childPortals.GetComponent<Portal>();
            if (portal != null && !portal.mIsEdge)
            {
                mOffmapConnections.Add(portal, portal.mDestinationMapID);
            }
        }

        // Set all "Edge portals" destinations
        foreach (Transform childPortals in transform.Find("Edges"))
        {
            Portal portal = childPortals.GetComponent<Portal>();
            if (portal != null && portal.mIsEdge)
            {
                if (mAdjacentMaps.ContainsKey(portal.mEdgeOrientation))
                {
                    portal.mDestinationMapID = mAdjacentMaps[portal.mEdgeOrientation];
                }
            }
        }
    }
}
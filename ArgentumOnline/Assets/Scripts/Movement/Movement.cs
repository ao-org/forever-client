/*
    Copyright 2020, Pablo Ignacio Marquez Tello aka Morgolock, All rights reserved.
    Argentum Online Clasico
    noland.studios@gmail.com
*/

using System;
﻿using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Threading;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Movement : MonoBehaviour
{
   public enum Direction
   {
        South = 0,
        North = 1,
        West = 2,
        East = 3,
        SouthEast = 4,
        SouthWest =5,
        NorthWest = 6,
        NorthEast = 7
    }
    public Direction dir;
    public Rigidbody2D mBody;
    public Tilemap mWaterTilemap;
    public Tilemap mNavegable0;
    public Tilemap mNavegable1;
    public Tilemap mNavegable2;
    public Tilemap mNavegable3;
    public Tilemap mTilemapLevel1;
    public Animator mAnimator;
    public Direction GetDirection() { return dir; }//fg
    public virtual void Awake(){
        if (WarpingDestination.warping){
            UnityEngine.Debug.Log("Warp X:" + WarpingDestination.teleport_x + " Y:" + WarpingDestination.teleport_y);
            Vector3 newpos = transform.position;
            newpos.x = WarpingDestination.teleport_x;
            newpos.y = WarpingDestination.teleport_y;
            this.transform.position = newpos;
            WarpingDestination.warping = false;
            this.dir = WarpingDestination.direction;
        }
        else {
            dir = Direction.South;
            gameObject.GetComponent<Animator>().Play("StandSur");
        }
    }
    // Start is called before the first frame update
    public virtual void Start(){
        mAnimator = gameObject.GetComponent<Animator>();
        mBody = GetComponent<Rigidbody2D>();
        mWaterTilemap = GameObject.Find("Tilemap_base").GetComponent<Tilemap>();
        mNavegable0 = GameObject.Find("Navegable0").GetComponent<Tilemap>();
        mNavegable1 = GameObject.Find("Navegable1").GetComponent<Tilemap>();
        mNavegable2 = GameObject.Find("Navegable2").GetComponent<Tilemap>();
        mNavegable3 = GameObject.Find("Navegable3").GetComponent<Tilemap>();
        mTilemapLevel1 = GameObject.Find("TilemapNivel1").GetComponent<Tilemap>();
    }
    public bool IsThereSomething(Vector3 pos){
        Vector3Int cellPosition = mTilemapLevel1.WorldToCell(pos);
        return mTilemapLevel1.HasTile(cellPosition);
    }
    public bool IsThereWater(Vector3 pos){
        if (mWaterTilemap != null)
        {
            Vector3Int cellPosition = mWaterTilemap.WorldToCell(pos);
            Vector3Int cellNavegable0 = mWaterTilemap.WorldToCell(pos);
            Vector3Int cellNavegable1 = mWaterTilemap.WorldToCell(pos);
            Vector3Int cellNavegable2 = mWaterTilemap.WorldToCell(pos);
            Vector3Int cellNavegable3 = mWaterTilemap.WorldToCell(pos);
            return mNavegable0.HasTile(cellNavegable0) || mNavegable1.HasTile(cellNavegable1) || mNavegable2.HasTile(cellNavegable2) || mNavegable3.HasTile(cellNavegable3);
        }
        return false;
    }
    public bool IsThereWaterForBarco(Vector3 pos)
    {
        if (mWaterTilemap != null)
        {
            Vector3Int cellPosition = mWaterTilemap.WorldToCell(pos);
            Vector3Int cellNavegable0 = mWaterTilemap.WorldToCell(pos);
            Vector3Int cellNavegable1 = mWaterTilemap.WorldToCell(pos);
            Vector3Int cellNavegable2 = mWaterTilemap.WorldToCell(pos);
            Vector3Int cellNavegable3 = mWaterTilemap.WorldToCell(pos);
            return mNavegable2.HasTile(cellNavegable2) || mNavegable3.HasTile(cellNavegable3);
        }
        return false;
    }

    // Update is called once per frame
    void Update(){
        

    }
}

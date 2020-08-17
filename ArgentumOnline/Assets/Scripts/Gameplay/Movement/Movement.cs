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
    public enum Action
    {
         Walk = 0,
         Run = 1,
         Attack = 2
    }

    protected Tuple<Action,Direction> mCurrentAction;

    protected  static Dictionary< string,Tuple<Action,Direction>> AnimNameToAction =
            new Dictionary<string,Tuple<Action,Direction>>
    {
                { "RunSur", new Tuple<Action,Direction>(Action.Run,Direction.South) },
                { "RunNorte", new Tuple<Action,Direction>(Action.Run,Direction.North) },
                { "RunEste", new Tuple<Action,Direction>(Action.Run,Direction.East) },
                { "RunOeste", new Tuple<Action,Direction>(Action.Run,Direction.West) },

                { "RunNoreste", new Tuple<Action,Direction>(Action.Run,Direction.NorthEast) },
                { "RunNoroeste" ,new Tuple<Action,Direction>(Action.Run,Direction.NorthWest) },
                { "RunSureste",new Tuple<Action,Direction>(Action.Run,Direction.SouthEast) },
                { "RunSuroeste" , new Tuple<Action,Direction>(Action.Run,Direction.SouthWest)},

                { "WalkSur", new Tuple<Action,Direction>(Action.Walk,Direction.South) },
                { "WalkNorte", new Tuple<Action,Direction>(Action.Walk,Direction.North) },
                { "WalkEste", new Tuple<Action,Direction>(Action.Walk,Direction.East) },
                { "WalkOeste", new Tuple<Action,Direction>(Action.Walk,Direction.West) },

                { "WalkNoreste", new Tuple<Action,Direction>(Action.Walk,Direction.NorthEast) },
                { "WalkNoroeste", new Tuple<Action,Direction>(Action.Walk,Direction.NorthWest) },
                { "WalkSureste", new Tuple<Action,Direction>(Action.Walk,Direction.SouthEast) },
                { "WalkSuroeste", new Tuple<Action,Direction>(Action.Walk,Direction.SouthWest) },

    };

    protected static Dictionary<Tuple<Action,Direction>, string> AnimationNames =
            new Dictionary<Tuple<Action,Direction>,string>
    {
        { new Tuple<Action,Direction>(Action.Run,Direction.South), "RunSur" },
        { new Tuple<Action,Direction>(Action.Run,Direction.North), "RunNorte" },
        { new Tuple<Action,Direction>(Action.Run,Direction.East), "RunEste" },
        { new Tuple<Action,Direction>(Action.Run,Direction.West), "RunOeste" },
		{ new Tuple<Action,Direction>(Action.Run,Direction.NorthWest), "RunNoroeste" },
        { new Tuple<Action,Direction>(Action.Run,Direction.NorthEast), "RunNoreste" },
        { new Tuple<Action,Direction>(Action.Run,Direction.SouthEast), "RunSureste" },
        { new Tuple<Action,Direction>(Action.Run,Direction.SouthWest), "RunSuroeste" },

        { new Tuple<Action,Direction>(Action.Walk,Direction.South), "WalkSur" },
        { new Tuple<Action,Direction>(Action.Walk,Direction.North), "WalkNorte" },
        { new Tuple<Action,Direction>(Action.Walk,Direction.East), "WalkEste" },
        { new Tuple<Action,Direction>(Action.Walk,Direction.West), "WalkOeste" },
        { new Tuple<Action,Direction>(Action.Walk,Direction.NorthWest), "WalkNoroeste" },
        { new Tuple<Action,Direction>(Action.Walk,Direction.NorthEast), "WalkNoreste" },
        { new Tuple<Action,Direction>(Action.Walk,Direction.SouthEast), "WalkSureste" },
        { new Tuple<Action,Direction>(Action.Walk,Direction.SouthWest), "WalkSuroeste" },

	};

    protected bool IsAnimationPlaying(string anim)
    {
        return
            mAnimator.GetCurrentAnimatorStateInfo(0).IsName(anim + "Sur") ||
            mAnimator.GetCurrentAnimatorStateInfo(0).IsName(anim + "Norte") ||
            mAnimator.GetCurrentAnimatorStateInfo(0).IsName(anim + "Oeste") ||
            mAnimator.GetCurrentAnimatorStateInfo(0).IsName(anim + "Este") ||
            mAnimator.GetCurrentAnimatorStateInfo(0).IsName(anim + "Noroeste") ||
            mAnimator.GetCurrentAnimatorStateInfo(0).IsName(anim + "Noreste") ||
            mAnimator.GetCurrentAnimatorStateInfo(0).IsName(anim + "Sureste") ||
            mAnimator.GetCurrentAnimatorStateInfo(0).IsName(anim + "Suroeste") ;
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

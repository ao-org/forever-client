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
    protected Color mSkinColor;
    private Direction mDir;
    public Rigidbody2D mBody;
    public Tilemap mWaterTilemap;
    public Tilemap mNavegable0;
    public Tilemap mNavegable1;
    public Tilemap mNavegable2;
    public Tilemap mNavegable3;
    public Tilemap mTilemapLevel1;
    protected Animator mAnimator;

    public Direction GetDirection() { return mDir; }
    public void SetDirection(Direction d) { mDir = d; }


    protected Direction GetDirectionFromDelta(Vector2 delta){
        if (delta.x == 0f && delta.y > 0f)
                return Direction.North;
        else if (delta.x > 0f && delta.y > 0f)
                return Direction.NorthEast;
        else if (delta.x > 0f && delta.y == 0f)
                return Direction.East;
        else if (delta.x > 0f && delta.y < 0f)
                return Direction.SouthEast;
        else if (delta.x == 0f && delta.y < 0f)
                return Direction.South;
        else if (delta.x < 0f && delta.y < 0f)
                return Direction.SouthWest;
        else if (delta.x < 0f && delta.y == 0f)
                return Direction.West;
        else if (delta.x < 0f && delta.y > 0f)
                return Direction.NorthWest;
        else {
                UnityEngine.Debug.Assert(false, "Bad delta");
                return Direction.South;
        }
    }

    protected void PlayAnimation(string anim)
    {
        switch (mDir)
        {
            case Direction.South:
                mAnimator.Play(anim + "Sur"); break;
            case Direction.North:
                mAnimator.Play(anim + "Norte"); break;
            case Direction.West:
                mAnimator.Play(anim + "Oeste"); break;
            case Direction.East:
                mAnimator.Play(anim + "Este"); break;
            case Direction.SouthWest:
                mAnimator.Play(anim + "Suroeste"); break;
            case Direction.NorthWest:
                mAnimator.Play(anim + "Noroeste"); break;
            case Direction.NorthEast:
                mAnimator.Play(anim + "Noreste"); break;
            case Direction.SouthEast:
                mAnimator.Play(anim + "Sureste"); break;
            default:
                UnityEngine.Debug.Assert(false, "PlayAnimation-Bad direction"); break;
        }

    }

    public bool IsAnimationLastFrame()
    {
        return (mAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1);
    }

    public void ChangeColorSkin(string color)
    {
        Color newColor = new Color(1, 1, 1); ;
        switch (color)
        {
            case "1":
                newColor = new Color(0.141f, 0.141f, 0.141f);
                break;
            case "2":
                newColor = new Color(0.2f, 0.180f, 0.180f);
                break;
            case "3":
                newColor = new Color(0.258f, 0.258f, 0.258f);
                break;
            case "4":
                newColor = new Color(0.356f, 0.356f, 0.356f);
                break;
            case "5":
                newColor = new Color(0.462f, 0.298f, 0.207f);
                break;
            case "6":
                newColor = new Color(0.490f, 0.392f, 0.266f);
                break;
            case "7":
                newColor = new Color(0.603f, 0.423f, 0.380f);
                break;
            case "8":
                newColor = new Color(0.690f, 0.568f, 0.568f);
                break;
            case "9":
                newColor = new Color(0.8f, 0.752f, 0.752f);
                break;
            case "10":
                newColor = new Color(1, 1, 1);
                break;
        }

        mSkinColor = newColor;
        if (mSpriteRenderer!=null){
            mSpriteRenderer = GetComponent<SpriteRenderer>();
            mSpriteRenderer.color = mSkinColor;
        }
    }

    protected SpriteRenderer mSpriteRenderer;

    public void Awake()
    {
        SetDirection(Direction.South);
    }

    void LateUpdate()
    {
        if (mSpriteRenderer!=null && mSpriteRenderer.isVisible)
            mSpriteRenderer.sortingOrder = (int)Camera.main.WorldToScreenPoint(transform.position).y * -1;
    }

    // Start is called before the first frame update
    public virtual void Start(){
        mSpriteRenderer = GetComponent<SpriteRenderer>();
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


}

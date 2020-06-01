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
    private Direction dir = Movement.Direction.South;
    private float WalkSpeed = 8.0f;
    private Rigidbody2D mBody;
    private Tilemap mWaterTilemap;
    private Tilemap mTilemapLevel1;
    private Animator mAnimator;
    public Direction GetDirection() { return dir; }
    void Awake(){
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
            gameObject.GetComponent<Animator>().Play("WalkSur");
        }
    }
    // Start is called before the first frame update
    void Start(){
        mAnimator = gameObject.GetComponent<Animator>();
        mBody = GetComponent<Rigidbody2D>();
        mWaterTilemap = GameObject.Find("Tilemap_base").GetComponent<Tilemap>();
        mTilemapLevel1 = GameObject.Find("TilemapNivel1").GetComponent<Tilemap>();
    }
    private bool IsThereSomething(Vector3 pos){
        Vector3Int cellPosition = mTilemapLevel1.WorldToCell(pos);
        return mTilemapLevel1.HasTile(cellPosition);
    }
    private bool IsThereWater(Vector3 pos){
        Vector3Int cellPosition = mWaterTilemap.WorldToCell(pos);
        //UnityEngine.Debug.Log("Pos " + cellPosition);
        return mWaterTilemap.HasTile(cellPosition);
    }
    private bool TryToMove(Vector3 pos){
        if(IsThereWater(pos))
        {
            // nothing to do
            return false;
        }
        else
        {
            mBody.MovePosition(pos);
            return true;
        }
    }
    // Update is called once per frame
    void Update(){
      bool RightArrowPressed  = Input.GetKey(KeyCode.RightArrow);
      bool LeftArrowPressed   = Input.GetKey(KeyCode.LeftArrow);
      bool UpArrowPressed     = Input.GetKey(KeyCode.UpArrow);
      bool DownArrowPressed   = Input.GetKey(KeyCode.DownArrow);
      bool Moving             = RightArrowPressed || LeftArrowPressed || UpArrowPressed    || DownArrowPressed;

      // NorthEast
      if (RightArrowPressed && UpArrowPressed && ! DownArrowPressed && !LeftArrowPressed) {
              dir = Direction.NorthEast;
              mAnimator.Play("WalkNoreste");
              Vector3 newpos = transform.position + Vector3.right * WalkSpeed * Time.deltaTime + Vector3.up * WalkSpeed * Time.deltaTime;
              TryToMove(newpos);
      }
      else // North
      if (!RightArrowPressed && UpArrowPressed && ! DownArrowPressed && !LeftArrowPressed) {
              dir = Direction.North;
              mAnimator.Play("WalkNorte");
              TryToMove(transform.position+Vector3.up * WalkSpeed * Time.deltaTime);
      }
      else // South
      if (!RightArrowPressed && !UpArrowPressed && DownArrowPressed && !LeftArrowPressed) {
              dir = Direction.South;
              mAnimator.Play("WalkSur");
              Vector3 newpos = transform.position + Vector3.down * WalkSpeed * Time.deltaTime;
              TryToMove(newpos);
      }
      else // SouthEast
      if (RightArrowPressed && DownArrowPressed && ! UpArrowPressed && !LeftArrowPressed) {
              dir = Direction.SouthEast;
              mAnimator.Play("WalkSureste");
              TryToMove(transform.position + Vector3.right * WalkSpeed * Time.deltaTime + Vector3.down * WalkSpeed * Time.deltaTime);
      }
      else
      if (RightArrowPressed && !DownArrowPressed && !UpArrowPressed && !LeftArrowPressed) {
              dir = Direction.East;
              mAnimator.Play("WalkEste");
              TryToMove(transform.position + Vector3.right * WalkSpeed * Time.deltaTime);
      }
      else
      if (LeftArrowPressed && !UpArrowPressed && !DownArrowPressed && !RightArrowPressed) {
              dir = Direction.West;
              mAnimator.Play("WalkOeste");
              TryToMove(transform.position +  Vector3.left* WalkSpeed * Time.deltaTime);
      }
      else
      if (LeftArrowPressed && UpArrowPressed && !DownArrowPressed && !RightArrowPressed) {
              dir = Direction.NorthWest;
              mAnimator.Play("WalkNoroeste");
              TryToMove(transform.position + Vector3.left* WalkSpeed * Time.deltaTime + Vector3.up * WalkSpeed * Time.deltaTime );
      }
      else
      if (LeftArrowPressed && !UpArrowPressed && DownArrowPressed && !RightArrowPressed) {
              dir = Direction.SouthWest;
              mAnimator.Play("WalkSuroeste");
              TryToMove(transform.position + Vector3.left* WalkSpeed * Time.deltaTime + Vector3.down * WalkSpeed * Time.deltaTime );
      }
      if(!Moving) {
            switch(dir)
            {
                case Direction.South:
                    mAnimator.Play("StandSur");break;
                case Direction.North:
                    mAnimator.Play("StandNorte");break;
                case Direction.West:
                    mAnimator.Play("StandOeste");break;
                case Direction.East:
                    mAnimator.Play("StandEste");break;
                case Direction.SouthWest:
                    mAnimator.Play("StandSuroeste");break;
                case Direction.NorthWest:
                    mAnimator.Play("StandNoroeste");break;
                case Direction.NorthEast:
                    mAnimator.Play("StandNoreste");break;
                case Direction.SouthEast:
                    mAnimator.Play("StandSureste");break;
                default:
                    UnityEngine.Debug.Assert(false, "Bad direction"); break;
            }
      }
        if (Input.GetKey(KeyCode.A))
        {
            switch (dir)
            {
                case Direction.South:
                    mAnimator.Play("DeadSur"); break;
                case Direction.North:
                    mAnimator.Play("DeadNorte"); break;
                case Direction.West:
                    mAnimator.Play("DeadOeste"); break;
                case Direction.East:
                    mAnimator.Play("DeadEste"); break;
                case Direction.SouthWest:
                    mAnimator.Play("DeadSuroeste"); break;
                case Direction.NorthWest:
                    mAnimator.Play("DeadNoroeste"); break;
                case Direction.NorthEast:
                    mAnimator.Play("DeadNoreste"); break;
                case Direction.SouthEast:
                    mAnimator.Play("DeadSureste"); break;
                default:
                    UnityEngine.Debug.Assert(false, "Bad direction"); break;
            }
        }
    }
}

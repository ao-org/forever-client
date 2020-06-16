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
    private float WalkSpeed = 6.0f; //Velocidad caminar
    private float runDelta = 2.2f; // delta Velocidad correr. se multiplica por la velocidad de caminar
    private float walkDiagDelta = 0.7f; //Delta de velocidad de las diagonales. (No modificar 0.7 default
    private float WalkRunSpeed;
    //private bool running = false;
    //private bool isDead = false;
    private Rigidbody2D mBody;
    private Joystick mJoystick;
    private JoystickButton mJoystickButton;
    private Tilemap mWaterTilemap; 
    private Tilemap mNavegable1;
    private Tilemap mNavegable2;
    private Tilemap mNavegable3;
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
            gameObject.GetComponent<Animator>().Play("StandSur");
        }
    }
    // Start is called before the first frame update
    void Start(){
        mJoystick = GameObject.Find("Joystick").GetComponent<Joystick>();
        mJoystickButton = GameObject.Find("Botones").GetComponent<JoystickButton>();
        mJoystick.Start();
        mJoystickButton.Start();
        mAnimator = gameObject.GetComponent<Animator>();
        mBody = GetComponent<Rigidbody2D>();
        WalkRunSpeed = WalkSpeed;
        mWaterTilemap = GameObject.Find("Tilemap_base").GetComponent<Tilemap>(); //comentar ccz
        mNavegable1 = GameObject.Find("Navegable1").GetComponent<Tilemap>();
        mNavegable2 = GameObject.Find("Navegable2").GetComponent<Tilemap>();
        mNavegable3 = GameObject.Find("Navegable3").GetComponent<Tilemap>();
        mTilemapLevel1 = GameObject.Find("TilemapNivel1").GetComponent<Tilemap>();
    }
    private bool IsThereSomething(Vector3 pos){
        Vector3Int cellPosition = mTilemapLevel1.WorldToCell(pos);
        return mTilemapLevel1.HasTile(cellPosition);
    }
    private bool IsThereWater(Vector3 pos){
        Vector3Int cellPosition = mWaterTilemap.WorldToCell(pos); //comentar ccz
        Vector3Int cellNavegable1 = mWaterTilemap.WorldToCell(pos);
        Vector3Int cellNavegable2 = mWaterTilemap.WorldToCell(pos);
        Vector3Int cellNavegable3 = mWaterTilemap.WorldToCell(pos);
        //UnityEngine.Debug.Log("Pos " + cellPosition);
        return mNavegable1.HasTile(cellNavegable1) || mNavegable2.HasTile(cellNavegable2) || mNavegable3.HasTile(cellNavegable3);
        //return mWaterTilemap.HasTile(cellPosition); //comentar ccz
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
        
        if (mJoystickButton.mDead)
        {
            if (mJoystickButton.mAlive)
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
                mJoystickButton.mAlive = false;

            }
            else
            {
                mAnimator.Play("StandSur");
                mJoystickButton.mDead = false;
                mJoystickButton.mRun = false;
                WalkRunSpeed = WalkSpeed;
                mJoystickButton.mAlive = true;
            }
            mJoystickButton.mDead = false;
        }
        if (!mJoystickButton.mAlive)
            return;


        if (mAnimator.GetCurrentAnimatorStateInfo(0).IsName("AttackSur") || mAnimator.GetCurrentAnimatorStateInfo(0).IsName("AttackNorte") ||
            mAnimator.GetCurrentAnimatorStateInfo(0).IsName("AttackOeste") || mAnimator.GetCurrentAnimatorStateInfo(0).IsName("AttackEste") ||
            mAnimator.GetCurrentAnimatorStateInfo(0).IsName("AttackNoroeste") || mAnimator.GetCurrentAnimatorStateInfo(0).IsName("AttackNoreste") ||
            mAnimator.GetCurrentAnimatorStateInfo(0).IsName("AttackSureste") || mAnimator.GetCurrentAnimatorStateInfo(0).IsName("AttackSuroeste"))
        {
            return;
        }
        
        if (mJoystickButton.mAttack)
        {
            switch (dir)
            {
                case Direction.South:
                    mAnimator.Play("AttackSur"); break;
                case Direction.North:
                    mAnimator.Play("AttackNorte"); break;
                case Direction.West:
                    mAnimator.Play("AttackOeste"); break;
                case Direction.East:
                    mAnimator.Play("AttackEste"); break;
                case Direction.SouthWest:
                    mAnimator.Play("AttackSuroeste"); break;
                case Direction.NorthWest:
                    mAnimator.Play("AttackNoroeste"); break;
                case Direction.NorthEast:
                    mAnimator.Play("AttackNoreste"); break;
                case Direction.SouthEast:
                    mAnimator.Play("AttackSureste"); break;
                default:
                    UnityEngine.Debug.Assert(false, "Bad direction"); break;
            }
            mJoystickButton.mAttack = false;
            return;
        }

        bool RightArrowPressed = false;
        bool LeftArrowPressed = false;
        bool UpArrowPressed = false;
        bool DownArrowPressed = false;

        Vector3 direction = Vector3.forward * mJoystick.Vertical + Vector3.right * mJoystick.Horizontal;

        if (direction.x > 0.5) RightArrowPressed = true;
        if (direction.x < -0.5) LeftArrowPressed = true;
        if (direction.z > 0.5) UpArrowPressed = true;
        if (direction.z < -0.5) DownArrowPressed = true;

        bool Moving = RightArrowPressed || LeftArrowPressed || UpArrowPressed || DownArrowPressed;

        if (!mJoystickButton.mRun)
        {
            WalkRunSpeed = WalkSpeed;
        }
        else
        {
            WalkRunSpeed = WalkSpeed * runDelta;
        }

        // NorthEast
        if (RightArrowPressed && UpArrowPressed && ! DownArrowPressed && !LeftArrowPressed) {
              dir = Direction.NorthEast;
              if (mJoystickButton.mRun)
                mAnimator.Play("RunNoreste");
              else
                mAnimator.Play("WalkNoreste");
              Vector3 newpos = transform.position + Vector3.right * WalkRunSpeed * Time.deltaTime * walkDiagDelta + Vector3.up * WalkRunSpeed * Time.deltaTime * walkDiagDelta;
              TryToMove(newpos);
      }
      else // North
      if (!RightArrowPressed && UpArrowPressed && ! DownArrowPressed && !LeftArrowPressed) {
              dir = Direction.North;
            if (mJoystickButton.mRun)
                mAnimator.Play("RunNorte");
            else
                mAnimator.Play("WalkNorte");
              TryToMove(transform.position+Vector3.up * WalkRunSpeed * Time.deltaTime);
      }
      else // South
      if (!RightArrowPressed && !UpArrowPressed && DownArrowPressed && !LeftArrowPressed) {
              dir = Direction.South;
            if (mJoystickButton.mRun)
                mAnimator.Play("RunSur");
            else
                mAnimator.Play("WalkSur");
              Vector3 newpos = transform.position + Vector3.down * WalkRunSpeed * Time.deltaTime;
              TryToMove(newpos);
      }
      else // SouthEast
      if (RightArrowPressed && DownArrowPressed && ! UpArrowPressed && !LeftArrowPressed) {
              dir = Direction.SouthEast;
            if(mJoystickButton.mRun)
                mAnimator.Play("RunSureste");
            else
                mAnimator.Play("WalkSureste");
              TryToMove(transform.position + Vector3.right * WalkRunSpeed * Time.deltaTime * walkDiagDelta + Vector3.down * WalkRunSpeed * Time.deltaTime * walkDiagDelta);
      }
      else
      if (RightArrowPressed && !DownArrowPressed && !UpArrowPressed && !LeftArrowPressed) {
              dir = Direction.East;
            if (mJoystickButton.mRun)
                mAnimator.Play("RunEste");
            else
                mAnimator.Play("WalkEste");
              TryToMove(transform.position + Vector3.right * WalkRunSpeed * Time.deltaTime);
      }
      else
      if (LeftArrowPressed && !UpArrowPressed && !DownArrowPressed && !RightArrowPressed) {
              dir = Direction.West;
            if (mJoystickButton.mRun)
                mAnimator.Play("RunOeste");
            else
                mAnimator.Play("WalkOeste");
              TryToMove(transform.position +  Vector3.left* WalkRunSpeed * Time.deltaTime);
      }
      else
      if (LeftArrowPressed && UpArrowPressed && !DownArrowPressed && !RightArrowPressed) {
              dir = Direction.NorthWest;
            if (mJoystickButton.mRun)
                mAnimator.Play("RunNoroeste");
            else
                mAnimator.Play("WalkNoroeste");
              TryToMove(transform.position + Vector3.left* WalkRunSpeed * Time.deltaTime * walkDiagDelta + Vector3.up * WalkRunSpeed * Time.deltaTime * walkDiagDelta);
      }
      else
      if (LeftArrowPressed && !UpArrowPressed && DownArrowPressed && !RightArrowPressed) {
              dir = Direction.SouthWest;
            if (mJoystickButton.mRun)
                mAnimator.Play("RunSuroeste");
            else
                mAnimator.Play("WalkSuroeste");
              TryToMove(transform.position + Vector3.left* WalkRunSpeed * Time.deltaTime * walkDiagDelta + Vector3.down * WalkRunSpeed * Time.deltaTime * walkDiagDelta);
      }
        
        if (!Moving) {
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
        

    }
}

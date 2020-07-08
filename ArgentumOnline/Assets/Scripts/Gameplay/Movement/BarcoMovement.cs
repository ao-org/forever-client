/*
    Copyright 2020, Pablo Ignacio Marquez Tello aka Morgolock, All rights reserved.
    Argentum Online Clasico
    noland.studios@gmail.com
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Threading;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BarcoMovement : Movement
{

    public float WalkSpeed = 6.0f; //Velocidad normal
    private float runDelta = 2.2f; // delta Velocidad correr. se multiplica por la velocidad de caminar
    private float walkDiagDelta = 0.7f; //Delta de velocidad de las diagonales. (No modificar 0.7 default)
    private float WalkRunSpeed;
    private bool running = false;
    private bool isDead = false;
    public bool isActive = false;
    public GameObject player = null;

    public override void Awake()
    {
        base.Awake();
    }
    // Start is called before the first frame update
    public override void Start()
    {
        dir = Direction.South;
        WalkRunSpeed = WalkSpeed;
        base.Start();
    }

    private bool TryToMove(Vector3 pos)
    {
        if (IsThereWaterForBarco(pos))
        {
            mBody.MovePosition(pos);
            return true;
        }
        else
        {
            // nothing to do
            return false;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (!isActive)
            return;
        if (Input.GetKeyDown(KeyCode.B))
        {
            isActive = false;
            player.SetActive(true);
            GameObject mainCamera = GameObject.FindGameObjectsWithTag("MainCamera")[0];
            Vector3 cameraPos = new Vector3(player.transform.position.x, player.transform.position.y, -1);
            mainCamera.transform.position = cameraPos;
            mainCamera.transform.SetParent(player.transform);
            return;
        }
        if (isDead)
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                mAnimator.Play("StandSur");
                isDead = false;
                running = false;
            }
            else
                return;
        }

        if (mAnimator.GetCurrentAnimatorStateInfo(0).IsName("AttackSur") || mAnimator.GetCurrentAnimatorStateInfo(0).IsName("AttackNorte") ||
            mAnimator.GetCurrentAnimatorStateInfo(0).IsName("AttackOeste") || mAnimator.GetCurrentAnimatorStateInfo(0).IsName("AttackEste") ||
            mAnimator.GetCurrentAnimatorStateInfo(0).IsName("AttackNoroeste") || mAnimator.GetCurrentAnimatorStateInfo(0).IsName("AttackNoreste") ||
            mAnimator.GetCurrentAnimatorStateInfo(0).IsName("AttackSureste") || mAnimator.GetCurrentAnimatorStateInfo(0).IsName("AttackSuroeste"))
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            switch (this.dir)
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
            isDead = true;
            return;

        }
        if (Input.GetKeyDown(KeyCode.A))
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
            return;
        }

        bool RightArrowPressed = Input.GetKey(KeyCode.RightArrow);
        bool LeftArrowPressed = Input.GetKey(KeyCode.LeftArrow);
        bool UpArrowPressed = Input.GetKey(KeyCode.UpArrow);
        bool DownArrowPressed = Input.GetKey(KeyCode.DownArrow);
        bool Moving = RightArrowPressed || LeftArrowPressed || UpArrowPressed || DownArrowPressed;




        if (Input.GetKeyDown(KeyCode.R))
        {
            if (running)
            {
                running = false;
                WalkRunSpeed = WalkSpeed;
            }
            else
            {
                running = true;
                WalkRunSpeed = WalkSpeed * runDelta;
            }
        }

        // NorthEast
        if (RightArrowPressed && UpArrowPressed && !DownArrowPressed && !LeftArrowPressed)
        {
            dir = Direction.NorthEast;
            if (running)
                mAnimator.Play("RunNoreste");
            else
                mAnimator.Play("WalkNoreste");
            Vector3 newpos = transform.position + Vector3.right * WalkRunSpeed * Time.deltaTime * walkDiagDelta + Vector3.up * WalkRunSpeed * Time.deltaTime * walkDiagDelta;
            TryToMove(newpos);
        }
        else // North
      if (!RightArrowPressed && UpArrowPressed && !DownArrowPressed && !LeftArrowPressed)
        {
            dir = Direction.North;
            if (running)
                mAnimator.Play("RunNorte");
            else
                mAnimator.Play("WalkNorte");
            TryToMove(transform.position + Vector3.up * WalkRunSpeed * Time.deltaTime);
        }
        else // South
      if (!RightArrowPressed && !UpArrowPressed && DownArrowPressed && !LeftArrowPressed)
        {
            dir = Direction.South;
            if (running)
                mAnimator.Play("RunSur");
            else
                mAnimator.Play("WalkSur");
            Vector3 newpos = transform.position + Vector3.down * WalkRunSpeed * Time.deltaTime;
            TryToMove(newpos);
        }
        else // SouthEast
      if (RightArrowPressed && DownArrowPressed && !UpArrowPressed && !LeftArrowPressed)
        {
            dir = Direction.SouthEast;
            if (running)
                mAnimator.Play("RunSureste");
            else
                mAnimator.Play("WalkSureste");
            TryToMove(transform.position + Vector3.right * WalkRunSpeed * Time.deltaTime * walkDiagDelta + Vector3.down * WalkRunSpeed * Time.deltaTime * walkDiagDelta);
        }
        else
      if (RightArrowPressed && !DownArrowPressed && !UpArrowPressed && !LeftArrowPressed)
        {
            dir = Direction.East;
            if (running)
                mAnimator.Play("RunEste");
            else
                mAnimator.Play("WalkEste");
            TryToMove(transform.position + Vector3.right * WalkRunSpeed * Time.deltaTime);
        }
        else
      if (LeftArrowPressed && !UpArrowPressed && !DownArrowPressed && !RightArrowPressed)
        {
            dir = Direction.West;
            if (running)
                mAnimator.Play("RunOeste");
            else
                mAnimator.Play("WalkOeste");
            TryToMove(transform.position + Vector3.left * WalkRunSpeed * Time.deltaTime);
        }
        else
      if (LeftArrowPressed && UpArrowPressed && !DownArrowPressed && !RightArrowPressed)
        {
            dir = Direction.NorthWest;
            if (running)
                mAnimator.Play("RunNoroeste");
            else
                mAnimator.Play("WalkNoroeste");
            TryToMove(transform.position + Vector3.left * WalkRunSpeed * Time.deltaTime * walkDiagDelta + Vector3.up * WalkRunSpeed * Time.deltaTime * walkDiagDelta);
        }
        else
      if (LeftArrowPressed && !UpArrowPressed && DownArrowPressed && !RightArrowPressed)
        {
            dir = Direction.SouthWest;
            if (running)
                mAnimator.Play("RunSuroeste");
            else
                mAnimator.Play("WalkSuroeste");
            TryToMove(transform.position + Vector3.left * WalkRunSpeed * Time.deltaTime * walkDiagDelta + Vector3.down * WalkRunSpeed * Time.deltaTime * walkDiagDelta);
        }

        if (!Moving)
        {
            switch (dir)
            {
                case Direction.South:
                    mAnimator.Play("StandSur"); break;
                case Direction.North:
                    mAnimator.Play("StandNorte"); break;
                case Direction.West:
                    mAnimator.Play("StandOeste"); break;
                case Direction.East:
                    mAnimator.Play("StandEste"); break;
                case Direction.SouthWest:
                    mAnimator.Play("StandSuroeste"); break;
                case Direction.NorthWest:
                    mAnimator.Play("StandNoroeste"); break;
                case Direction.NorthEast:
                    mAnimator.Play("StandNoreste"); break;
                case Direction.SouthEast:
                    mAnimator.Play("StandSureste"); break;
                default:
                    UnityEngine.Debug.Assert(false, "Bad direction"); break;
            }
        }


    }
}


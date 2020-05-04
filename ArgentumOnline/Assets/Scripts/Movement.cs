/*
    Copyright 2020, Pablo Ignacio Marquez Tello aka Morgolock, All rights reserved.
    Argentum Online Clasico
    noland.studios@gmail.com
*/

﻿using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Threading;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Movement : MonoBehaviour
{
   enum Direction
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

    // Start is called before the first frame update
    void Start()
    {
        mBody = GetComponent<Rigidbody2D>();
        mWaterTilemap = GameObject.Find("Tilemap_base").GetComponent<Tilemap>();
        mTilemapLevel1 = GameObject.Find("TilemapNivel1").GetComponent<Tilemap>();
        dir = Direction.South;
        gameObject.GetComponent<Animator>().Play("FantasmaCaminaSur");
    }
    private bool IsThereSomething(Vector3 pos)
    {
        Vector3Int cellPosition = mTilemapLevel1.WorldToCell(pos);
        return mTilemapLevel1.HasTile(cellPosition);
    }

    private bool IsThereWater(Vector3 pos)
    {
        Vector3Int cellPosition = mWaterTilemap.WorldToCell(pos);
        UnityEngine.Debug.Log("Pos " + cellPosition);
        return mWaterTilemap.HasTile(cellPosition);
    }

    private bool TryToMove(Vector3 pos)
    {
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
    void Update()
    {
      bool RightArrowPressed  = Input.GetKey(KeyCode.RightArrow);
      bool LeftArrowPressed   = Input.GetKey(KeyCode.LeftArrow);
      bool UpArrowPressed     = Input.GetKey(KeyCode.UpArrow);
      bool DownArrowPressed   = Input.GetKey(KeyCode.DownArrow);
      bool Moving             = RightArrowPressed || LeftArrowPressed || UpArrowPressed    || DownArrowPressed;

      // NorthEast
      if (RightArrowPressed && UpArrowPressed && ! DownArrowPressed && !LeftArrowPressed) {
              dir = Direction.NorthEast;
              gameObject.GetComponent<Animator>().Play("FantasmaCaminaNoreste");
              Vector3 newpos = transform.position + Vector3.right * WalkSpeed * Time.deltaTime + Vector3.up * WalkSpeed * Time.deltaTime;
              TryToMove(newpos);
      }
      else // North
      if (!RightArrowPressed && UpArrowPressed && ! DownArrowPressed && !LeftArrowPressed) {
              dir = Direction.North;
              gameObject.GetComponent<Animator>().Play("FantasmaCaminaNorte");
              TryToMove(transform.position+Vector3.up * WalkSpeed * Time.deltaTime);
      }
      else // South
      if (!RightArrowPressed && !UpArrowPressed && DownArrowPressed && !LeftArrowPressed) {
              dir = Direction.South;
              gameObject.GetComponent<Animator>().Play("FantasmaCaminaSur");
              Vector3 newpos = transform.position + Vector3.down * WalkSpeed * Time.deltaTime;
              TryToMove(newpos);
      }
      else // SouthEast
      if (RightArrowPressed && DownArrowPressed && ! UpArrowPressed && !LeftArrowPressed) {
              dir = Direction.SouthEast;
              gameObject.GetComponent<Animator>().Play("FantasmaCaminaSureste");
              TryToMove(transform.position + Vector3.right * WalkSpeed * Time.deltaTime + Vector3.down * WalkSpeed * Time.deltaTime);
      }
      else
      if (RightArrowPressed && !DownArrowPressed && !UpArrowPressed && !LeftArrowPressed) {
              dir = Direction.East;
              gameObject.GetComponent<Animator>().Play("FantasmaCaminaEste");
              TryToMove(transform.position + Vector3.right * WalkSpeed * Time.deltaTime);
      }
      else
      if (LeftArrowPressed && !UpArrowPressed && !DownArrowPressed && !RightArrowPressed) {
              dir = Direction.West;
              gameObject.GetComponent<Animator>().Play("FantasmaCaminaOeste");
              TryToMove(transform.position +  Vector3.left* WalkSpeed * Time.deltaTime);
      }
      else
      if (LeftArrowPressed && UpArrowPressed && !DownArrowPressed && !RightArrowPressed) {
              dir = Direction.NorthWest;
              gameObject.GetComponent<Animator>().Play("FantasmaCaminaNoroeste");
              TryToMove(transform.position + Vector3.left* WalkSpeed * Time.deltaTime + Vector3.up * WalkSpeed * Time.deltaTime );
      }
      else
      if (LeftArrowPressed && !UpArrowPressed && DownArrowPressed && !RightArrowPressed) {
              dir = Direction.SouthWest;
              gameObject.GetComponent<Animator>().Play("FantasmaCaminaSuroeste");
              TryToMove(transform.position + Vector3.left* WalkSpeed * Time.deltaTime + Vector3.down * WalkSpeed * Time.deltaTime );
      }
      if(!Moving) {
              if( dir == Direction.South )
                  gameObject.GetComponent<Animator>().Play("FantasmaCaminaSur");
              else if (dir == Direction.North)
                  gameObject.GetComponent<Animator>().Play("FantasmaCaminaNorte");
              else
                  gameObject.GetComponent<Animator>().Play("FantasmaCaminaSur");
      }
    }
}

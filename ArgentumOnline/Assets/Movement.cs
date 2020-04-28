/*
    Copyright 2020, Pablo Ignacio Marquez Tello aka Morgolock, All rights reserved.
    Argentum Online Clasico
    noland.studios@gmail.com
*/

﻿using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

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
    private float WalkSpeed = 4.0f;
    private Rigidbody2D body;
    //private Vector3 SouthVector = Vector3(0.0f,1.0f,0.0f);

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();

        dir = Direction.South;
        gameObject.GetComponent<Animator>().Play("FantasmaCaminaSur");
    }

    // Update is called once per frame
    void Update()
    {
      bool RightArrowPressed  = Input.GetKey(KeyCode.RightArrow);
      bool LeftArrowPressed   = Input.GetKey(KeyCode.LeftArrow);
      bool UpArrowPressed     = Input.GetKey(KeyCode.UpArrow);
      bool DownArrowPressed   = Input.GetKey(KeyCode.DownArrow);
      bool Moving          =
          RightArrowPressed || LeftArrowPressed ||
          UpArrowPressed    || DownArrowPressed;




      if (RightArrowPressed && UpArrowPressed && ! DownArrowPressed && !LeftArrowPressed) {
              dir = Direction.NorthEast;
              gameObject.GetComponent<Animator>().Play("FantasmaCaminaNoreste");
            //transform.position += Vector3.right * WalkSpeed * Time.deltaTime;
            //transform.position += Vector3.up * WalkSpeed * Time.deltaTime;
            body.MovePosition(transform.position + Vector3.right * WalkSpeed * Time.deltaTime + Vector3.up * WalkSpeed * Time.deltaTime);
      }
      else
      if (!RightArrowPressed && UpArrowPressed && ! DownArrowPressed && !LeftArrowPressed) {
              dir = Direction.North;
              gameObject.GetComponent<Animator>().Play("FantasmaCaminaNorte");
            //transform.position += Vector3.up * WalkSpeed * Time.deltaTime;
            body.MovePosition(transform.position+Vector3.up * WalkSpeed * Time.deltaTime);
      }
      else
      if (!RightArrowPressed && !UpArrowPressed && DownArrowPressed && !LeftArrowPressed) {
              dir = Direction.South;
              gameObject.GetComponent<Animator>().Play("FantasmaCaminaSur");
            //transform.position += Vector3.down * WalkSpeed * Time.deltaTime;
            body.MovePosition(transform.position + Vector3.down * WalkSpeed * Time.deltaTime);


      }
      else
      if (RightArrowPressed && DownArrowPressed && ! UpArrowPressed && !LeftArrowPressed) {
              dir = Direction.SouthEast;
              gameObject.GetComponent<Animator>().Play("FantasmaCaminaSureste");
              transform.position += Vector3.right * WalkSpeed * Time.deltaTime;
              transform.position += Vector3.down * WalkSpeed * Time.deltaTime;
      }
      else
      if (RightArrowPressed && !DownArrowPressed && !UpArrowPressed && !LeftArrowPressed) {
              dir = Direction.East;
              gameObject.GetComponent<Animator>().Play("FantasmaCaminaEste");
              transform.position += Vector3.right * WalkSpeed * Time.deltaTime;
      }
      else
      if (LeftArrowPressed && !UpArrowPressed && !DownArrowPressed && !RightArrowPressed) {
              dir = Direction.West;
              gameObject.GetComponent<Animator>().Play("FantasmaCaminaOeste");
              transform.position += Vector3.left* WalkSpeed * Time.deltaTime;
      }
      else
      if (LeftArrowPressed && UpArrowPressed && !DownArrowPressed && !RightArrowPressed) {
              dir = Direction.NorthWest;
              gameObject.GetComponent<Animator>().Play("FantasmaCaminaNoroeste");
              transform.position += Vector3.left* WalkSpeed * Time.deltaTime;
              transform.position += Vector3.up * WalkSpeed * Time.deltaTime;
      }
      else
      if (LeftArrowPressed && !UpArrowPressed && DownArrowPressed && !RightArrowPressed) {
              dir = Direction.SouthWest;
              gameObject.GetComponent<Animator>().Play("FantasmaCaminaSuroeste");
              transform.position += Vector3.left* WalkSpeed * Time.deltaTime;
              transform.position += Vector3.down * WalkSpeed * Time.deltaTime;
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

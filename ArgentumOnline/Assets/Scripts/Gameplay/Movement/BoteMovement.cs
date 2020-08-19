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

public class BoteMovement : Movement
{

    public float WalkSpeed = 6.0f; //Velocidad normal
    private float runDelta = 2.2f; // delta Velocidad correr. se multiplica por la velocidad de caminar
    private float walkDiagDelta = 0.7f; //Delta de velocidad de las diagonales. (No modificar 0.7 default)
    private float WalkRunSpeed;
    private bool running = false;
    private bool isDead = false;


    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
    }

    private bool TryToMove(Vector3 pos)
    {
        if (IsThereWater(pos))
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


    }
}

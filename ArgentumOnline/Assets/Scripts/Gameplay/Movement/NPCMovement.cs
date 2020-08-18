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
using UnityEngine.UI;
using TMPro;

public class NPCMovement : Movement
{
    public float WalkSpeed = 6.0f; //Velocidad normal
    private float runDelta = 2.2f; // delta Velocidad correr. se multiplica por la velocidad de caminar
    private float walkDiagDelta = 0.7f; //Delta de velocidad de las diagonales. (No modificar 0.7 default)
    private float WalkRunSpeed = 6.0f;
    private bool running = false;
    private bool isDead = false;
    private int life = 100;
    private float health;
    private Vector3 scaleHuman;
    private Vector3 teleportingPos;
    private bool takeDamage = false;
    private float damageValue = 0f;
    public Slider healthSlider;
    private TextMeshProUGUI textName;


    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
    }
    private bool TryToMove(Vector3 pos)
    {
        if (IsThereWater(pos))
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
    public void TakeDamage(float damage)
    {
        takeDamage = true;
        damageValue += damage;
        UnityEngine.Debug.Log("Damage: " + damageValue);
        return;
    }
    public void QuitDamage(float damage)
    {
        UnityEngine.Debug.Log("DamageBeforeExit: " + damageValue);
        damageValue -= damage;
        UnityEngine.Debug.Log("DamageAfterExit: " + damageValue);
        if (damageValue <= 0f)
        {
            takeDamage = false;
            damageValue = 0;
        }
        return;

    }
    // Update is called once per frame
    void Update()
    {


    }

    

}

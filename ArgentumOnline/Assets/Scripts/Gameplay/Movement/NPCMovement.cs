﻿/*
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
        if (takeDamage)
        {
            UnityEngine.Debug.Log("Damage: " + damageValue);
            health -= damageValue;
            healthSlider.value = health;
            if (health <= 0)
            {
                damageValue = 0;
                PlayAnimation("Dead");
                isDead = true;
            }
        }
        if (isDead)
        {
            if (!IsAnimationLastFrame())
                return;
            else
            {
                if (Input.GetKeyDown(KeyCode.L))
                {
                    PlayAnimation("Stand");
                    isDead = false;
                    running = false;
                    WalkRunSpeed = WalkSpeed;
                    health = life;
                    healthSlider.gameObject.SetActive(true);
                    healthSlider.value = life;
                    return;
                }
                return;
            }
        }

        if (IsAnimationPlaying("Attack"))
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            PlayAnimation("Dead");
            healthSlider.value = 0;
            isDead = true;
            return;
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            PlayAnimation("Attack");
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
            PlayAnimation("Stand");
        }


    }

    private bool IsAnimationPlaying(string anim)
    {
        if (mAnimator.GetCurrentAnimatorStateInfo(0).IsName(anim + "Sur") || mAnimator.GetCurrentAnimatorStateInfo(0).IsName(anim + "Norte") ||
                mAnimator.GetCurrentAnimatorStateInfo(0).IsName(anim + "Oeste") || mAnimator.GetCurrentAnimatorStateInfo(0).IsName(anim + "Este") ||
                mAnimator.GetCurrentAnimatorStateInfo(0).IsName(anim + "Noroeste") || mAnimator.GetCurrentAnimatorStateInfo(0).IsName(anim + "Noreste") ||
                mAnimator.GetCurrentAnimatorStateInfo(0).IsName(anim + "Sureste") || mAnimator.GetCurrentAnimatorStateInfo(0).IsName(anim + "Suroeste"))
        {
            return true;
        }
        return false;
    }

    private void PlayAnimation(string anim)
    {
        switch (dir)
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
    private bool IsAnimationLastFrame()
    {
        return (mAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1);
    }

}

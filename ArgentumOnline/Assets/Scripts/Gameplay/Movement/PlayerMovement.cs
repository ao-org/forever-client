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

public class PlayerMovement : Movement
{
    public float WalkSpeed = 6.0f; //Velocidad normal
    private float runDelta = 2.2f; // delta Velocidad correr. se multiplica por la velocidad de caminar
    private float walkDiagDelta = 0.7f; //Delta de velocidad de las diagonales. (No modificar 0.7 default)
    private float WalkRunSpeed = 6.0f;
    private bool running = false;
    private Vector3 scaleHuman;
    private Vector3 teleportingPos;
    private WorldClient mWorldClient;
    private GameObject mCollidingChar;
    public void Awake()
    {
        mWorldClient = GameObject.Find("WorldClient").GetComponent<WorldClient>();
        UnityEngine.Debug.Assert(mWorldClient != null);
    }
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        scaleHuman = this.transform.localScale;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
            if (collision.collider.tag == "Human")
            {
                UnityEngine.Debug.Log("touch Player enter "+ collision.collider.name +" ****************************");
                mCollidingChar = collision.collider.gameObject;

            }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
            if (collision.collider.tag == "Human")
            {
                UnityEngine.Debug.Log("touch Player exit****************************");
            }
            mCollidingChar = null;
    }

    void OnAttack(){
        if(mCollidingChar != null){
            UnityEngine.Debug.Log("OnAttack victim:" + mCollidingChar.name);
            mWorldClient.OnPlayerMeleeAttacked(mCollidingChar.name);
        }
        else {
            mWorldClient.OnPlayerMeleeAttacked("Z");
        }
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
            mWorldClient.OnPlayerMoved(pos);
            mBody.MovePosition(pos);
            return true;
        }
    }

    public void SetTeleportingPos(Vector3 newPos )
    {
        teleportingPos = newPos;
    }

    public Vector3 GetTeleportingPos()
    {
        return teleportingPos;
    }

    void Update(){
        /*
        if (Input.GetKeyDown(KeyCode.L))
        {
            if (IsPhantom)
            {
                if (mAnimatorController != null) {
                            UnityEngine.Debug.Log("Player Update: " + mAnimatorController.name);
                }
                else {
                    UnityEngine.Debug.Log("Player Update: " + mAnimatorController.name);
                }

                //mAnimator.runtimeAnimatorController = mAnimatorController;

                PlayAnimation("Stand");
                isDead = false;
                running = false;
                WalkRunSpeed = WalkSpeed;
                health = life;
                IsPhantom = false;
                healthSlider.gameObject.SetActive(true);
                manaSlider.gameObject.SetActive(true);
                textToHead.gameObject.SetActive(true);
                textName.transform.localScale = new Vector3(1.6f, 1.6f, 1);
                this.transform.localScale =scaleHuman;
                healthSlider.value = life;
                return;
            }
        }
        */
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (running)
            {
                running = false;
            }
            else
            {
                running = true;
            }
        }
    }

    // Update is called once per frame
    float mTimeElapsedFixedUpdate = 0.0f;

    void FixedUpdate()
    {
        base.FixedUpdate();

        mTimeElapsedFixedUpdate +=  Time.fixedDeltaTime;
        if( mTimeElapsedFixedUpdate >= 0.05f ){
            mTimeElapsedFixedUpdate= 0.0f;
        }
        else{
            return ;
        }


        if (IsAnimationPlaying("Attack"))
        {
            return;
        }
        else if (Input.GetButton("Fire1"))
        {
            PlayAnimation("Attack");
            OnAttack();
            return;
        }

        Vector2 input_delta = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical"));

        string anim_name = "Stand";
        if (input_delta.x != 0f || input_delta.y != 0f)
        {
            if (!running)
            {
                WalkRunSpeed = WalkSpeed;
                anim_name = "Walk";
            }
            else
            {
                WalkRunSpeed = WalkSpeed * runDelta;
                anim_name = "Run";
            }
        }

        if (input_delta.x != 0f && input_delta.y != 0f) {
                //Diagonal treatment, CRISTOBAL please add comments
                input_delta *= walkDiagDelta;
        }

        if (input_delta.x != 0f || input_delta.y != 0f) {
                SetDirection(GetDirectionFromDelta(input_delta));
                var newpos = mBody.position + input_delta * WalkRunSpeed * Time.deltaTime;
                PlayAnimation(anim_name);
                TryToMove(newpos);
        }
        else {
                PlayAnimation(anim_name);
        }

    }




}

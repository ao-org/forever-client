/*
		Argentum Forever - Copyright 2020, Pablo Ignacio Marquez Tello aka Morgolock, All rights reserved.
		gulfas@gmail.com
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

public class CharacterMovement : Movement
{
    public float WalkSpeed = 6.0f; //Velocidad normal
    private float runDelta = 2.2f; // delta Velocidad correr. se multiplica por la velocidad de caminar
    private float walkDiagDelta = 0.7f; //Delta de velocidad de las diagonales. (No modificar 0.7 default)
    private float WalkRunSpeed;
    private bool running = false;
    private bool isDead = false;
    private int life = 100;
    private float health;
    private bool takeDamage = false;
    public bool IsPhantom;
    private float damageValue = 0f;
    public Slider healthSlider;
    public Slider manaSlider;
    private RuntimeAnimatorController mPhantomAnimatorController;
    private RuntimeAnimatorController mAnimatorController;


    private Queue<Tuple<short,float,float>> mActionQueue = new Queue<Tuple<short,float,float>>();


    public void PushMovement(Tuple<short,float,float> newpos){
        mActionQueue.Enqueue(newpos);
    }

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        WalkRunSpeed = WalkSpeed;
        mBody.isKinematic = true;
        if (IsPhantom)
        {
            mAnimator.runtimeAnimatorController = mPhantomAnimatorController;
            healthSlider.gameObject.SetActive(false);
            manaSlider.gameObject.SetActive(false);
        }
        else
            mAnimatorController = mAnimator.runtimeAnimatorController;
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
        if (!IsPhantom)
        {
            takeDamage = true;
            damageValue += damage;
            UnityEngine.Debug.Log("Damage: " + damageValue);
        }
        return;
    }
    public void QuitDamage(float damage)
    {
        if (!IsPhantom)
        {
            UnityEngine.Debug.Log("DamageBeforeExit: " + damageValue);
            damageValue -= damage;
            UnityEngine.Debug.Log("DamageAfterExit: " + damageValue);
            if (damageValue <= 0f)
            {
                takeDamage = false;
                damageValue = 0;
            }
        }
        return;

    }

    float mTimeElapsedFixedUpdate = 0.0f;

    void FixedUpdate()
    {

        mTimeElapsedFixedUpdate +=  Time.fixedDeltaTime;
        if( mTimeElapsedFixedUpdate >= 0.05f ){
            mTimeElapsedFixedUpdate= 0.0f;
        }
        else{
            return ;
        }


        { // Reset the force, we do not want the physics engine to move the Player
            mBody.velocity = Vector2.zero;
            mBody.angularVelocity = 0f;
        }

        if (takeDamage && !IsPhantom)
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
        if (isDead && !IsPhantom)
        {
            if (!IsAnimationLastFrame())
                return;
            else
            {
                mAnimator.runtimeAnimatorController = mPhantomAnimatorController;
                PlayAnimation("Stand");
                isDead = false;
                IsPhantom = true;
                running = false;
                WalkRunSpeed = WalkSpeed;
                healthSlider.gameObject.SetActive(false);
                manaSlider.gameObject.SetActive(false);
                return;
            }

        }

        if (mActionQueue.Count > 0){
            Tuple<short,float,float> e = mActionQueue.Dequeue();
            if(e.Item1==ProtoBase.ProtocolNumbers["CHARACTER_MOVED"])
            {
                var old_pos = transform.position;
                var new_pos = new Vector3(e.Item2,e.Item3,old_pos.z);
                var delta = new_pos - old_pos;
                bool RightArrowPressed  = delta.x>0.0f;
                bool LeftArrowPressed   = delta.x<0.0f;
                bool UpArrowPressed     = delta.y<0.0f;
                bool DownArrowPressed   = delta.y>0.0f;
                bool Moving = RightArrowPressed || LeftArrowPressed || UpArrowPressed || DownArrowPressed;
                // NorthEast
                if (RightArrowPressed && UpArrowPressed && !DownArrowPressed && !LeftArrowPressed)
                {
                    dir = Direction.NorthEast;
                    if (running && mCurrentAction!= AnimNameToAction["RunNoreste"] ){
                        mAnimator.Play("RunNoreste");
                        mCurrentAction = AnimNameToAction["RunNoreste"];
                    }
                    else if(mCurrentAction!= AnimNameToAction["WalkNoreste"]){
                        mAnimator.Play("WalkNoreste");
                        mCurrentAction = AnimNameToAction["WalkNoreste"];
                    }
                    TryToMove(new_pos);
                }
                else // North
                if (!RightArrowPressed && UpArrowPressed && !DownArrowPressed && !LeftArrowPressed)
                {
                    dir = Direction.North;
                    if (running && mCurrentAction!= AnimNameToAction["RunNorte"] ){
                        mAnimator.Play("RunNorte");
                        mCurrentAction = AnimNameToAction["RunNorte"];
                    }
                    else if(mCurrentAction!= AnimNameToAction["WalkNorte"]){
                        mAnimator.Play("WalkNorte");
                        mCurrentAction = AnimNameToAction["WalkNorte"];
                    }
                    TryToMove(new_pos);
                }
                else // South
                if (!RightArrowPressed && !UpArrowPressed && DownArrowPressed && !LeftArrowPressed)
                {
                    dir = Direction.South;
                    if (running && mCurrentAction!= AnimNameToAction["RunSur"] ){
                        mAnimator.Play("RunSur");
                        mCurrentAction = AnimNameToAction["RunSur"];
                    }
                    else if(mCurrentAction!= AnimNameToAction["WalkSur"]){
                        mAnimator.Play("WalkSur");
                        mCurrentAction = AnimNameToAction["WalkSur"];
                    }
                    TryToMove(new_pos);
                }
                else // SouthEast
              if (RightArrowPressed && DownArrowPressed && !UpArrowPressed && !LeftArrowPressed)
                {
                    dir = Direction.SouthEast;
                    if (running && mCurrentAction!= AnimNameToAction["RunSureste"] ){
                        mAnimator.Play("RunSureste");
                        mCurrentAction = AnimNameToAction["RunSureste"];
                    }
                    else if(mCurrentAction!= AnimNameToAction["WalkSureste"]){
                        mAnimator.Play("WalkSureste");
                        mCurrentAction = AnimNameToAction["WalkSureste"];
                    }
                    TryToMove(new_pos);
                }
                else
                // East
                if (RightArrowPressed && !DownArrowPressed && !UpArrowPressed && !LeftArrowPressed)
                {
                    dir = Direction.East;
                    if (running && mCurrentAction!= AnimNameToAction["RunEste"] ){
                        mAnimator.Play("RunEste");
                        mCurrentAction = AnimNameToAction["RunEste"];
                    }
                    else if(mCurrentAction!= AnimNameToAction["WalkEste"]){
                        mAnimator.Play("WalkEste");
                        mCurrentAction = AnimNameToAction["WalkEste"];
                    }
                    TryToMove(new_pos);
                }
                else
              if (LeftArrowPressed && !UpArrowPressed && !DownArrowPressed && !RightArrowPressed)
                {
                    dir = Direction.West;
                    if (running && mCurrentAction!= AnimNameToAction["RunOeste"] ){
                        mAnimator.Play("RunOeste");
                        mCurrentAction = AnimNameToAction["RunOeste"];
                    }
                    else if(mCurrentAction!= AnimNameToAction["WalkOeste"]){
                        mAnimator.Play("WalkOeste");
                        mCurrentAction = AnimNameToAction["WalkOeste"];
                    }
                    TryToMove(new_pos);
                }
                else
              if (LeftArrowPressed && UpArrowPressed && !DownArrowPressed && !RightArrowPressed)
                {
                    dir = Direction.NorthWest;
                    if (running && mCurrentAction!= AnimNameToAction["RunNoroeste"] ){
                        mAnimator.Play("RunNoroeste");
                        mCurrentAction = AnimNameToAction["RunNoroeste"];
                    }
                    else if(mCurrentAction!= AnimNameToAction["WalkNoroeste"]){
                        mAnimator.Play("WalkNoroeste");
                        mCurrentAction = AnimNameToAction["WalkNoroeste"];
                    }
                    TryToMove(new_pos);
                }
                else
              if (LeftArrowPressed && !UpArrowPressed && DownArrowPressed && !RightArrowPressed)
                {
                    dir = Direction.SouthWest;
                    if (running && mCurrentAction!= AnimNameToAction["RunSuroeste"] ){
                        mAnimator.Play("RunSuroeste");
                        mCurrentAction = AnimNameToAction["RunSuroeste"];
                    }
                    else if(mCurrentAction!= AnimNameToAction["WalkSuroeste"]){
                        mAnimator.Play("WalkSuroeste");
                        mCurrentAction = AnimNameToAction["WalkSuroeste"];
                    }
                    TryToMove(new_pos);
                }
            }
            else if(e.Item1==ProtoBase.ProtocolNumbers["CHARACTER_MELEE"])
            {
                PlayAnimation("Attack");
            }
            else if(e.Item1==ProtoBase.ProtocolNumbers["CHARACTER_NEWPOS"])
            {
                // We teleport to the current scene, only need to update the player position
                //UnityEngine.Debug.Log("Warping to the same scene");
                var old_pos = transform.position;
                var new_pos = new Vector3(e.Item2,e.Item3,old_pos.z);
                transform.position = new_pos;

            }


    }
    }


}

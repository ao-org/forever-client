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
                var old_pos = new Vector2(transform.position.x,transform.position.y);
                var new_pos = new Vector2(e.Item2,e.Item3);
                var delta = new_pos - old_pos;
                string anim_name = "Stand";
                if (delta.x != 0f || delta.y != 0f)
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

                if (delta.x != 0f && delta.y != 0f) delta *= walkDiagDelta;
                if (delta.x == 0f && delta.y > 0f) dir = Direction.North;
                if (delta.x > 0f && delta.y > 0f) dir = Direction.NorthEast;
                if (delta.x > 0f && delta.y == 0f) dir = Direction.East;
                if (delta.x > 0f && delta.y < 0f) dir = Direction.SouthEast;
                if (delta.x == 0f && delta.y < 0f) dir = Direction.South;
                if (delta.x < 0f && delta.y < 0f) dir = Direction.SouthWest;
                if (delta.x < 0f && delta.y == 0f) dir = Direction.West;
                if (delta.x < 0f && delta.y > 0f) dir = Direction.NorthWest;

                if (delta.x != 0f || delta.y != 0f) {
                        PlayAnimation(anim_name);
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

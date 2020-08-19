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
    private Queue<Tuple<short,float,float>> mActionQueue = new Queue<Tuple<short,float,float>>();
    private float mTimeElapsedFixedUpdate = 0.0f;

    public void PushMovement(Tuple<short,float,float> newpos){
        mActionQueue.Enqueue(newpos);
    }

    public override void Start()
    {
        base.Start();
        mBody.isKinematic = true;
    }

    public void Awake()
    {
        base.Awake();
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
            mBody.position = pos;
            return true;
        }
    }

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
                    bool running = Math.Abs(delta.x) > 0.250f || Math.Abs(delta.y) > 0.250f;
                    if (!running){
                        anim_name = "Walk";
                    }
                    else
                    {
                        anim_name = "Run";
                    }
                }

                if (delta.x != 0f || delta.y != 0f) {
                        SetDirection(GetDirectionFromDelta(delta));
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
                var old_pos = transform.position;
                var new_pos = new Vector3(e.Item2,e.Item3,old_pos.z);
                transform.position = new_pos;
            }
        }//
        else {
            if (!IsAnimationPlaying("Attack")){
                PlayAnimation("Stand");
            }
        }
    }
}

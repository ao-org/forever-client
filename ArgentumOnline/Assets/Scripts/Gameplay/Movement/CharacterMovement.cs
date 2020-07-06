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

    private Queue<Tuple<float,float>> mMovementsQueue = new Queue<Tuple<float,float>>();


    public void PushMovement(Tuple<float,float> newpos){
        mMovementsQueue.Enqueue(newpos);
    }
    public override void Awake()
    {
        //DontDestroyOnLoad(this.gameObject);
        base.Awake();
        health = life;
        healthSlider = GameObject.Find("SliderLife").GetComponent<Slider>();
        UnityEngine.Debug.Assert(healthSlider != null, "Cannot find Life Slider in Player");
        manaSlider = GameObject.Find("SliderMana").GetComponent<Slider>();
        UnityEngine.Debug.Assert(manaSlider != null, "Cannot find Mana Slider in Player");
        mPhantomAnimatorController = Resources.Load<RuntimeAnimatorController>("Phantom") as RuntimeAnimatorController;
        UnityEngine.Debug.Assert(mPhantomAnimatorController != null, "Cannot find Phantom Controller in Resources");
        dir = Direction.South;
    }
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        WalkRunSpeed = WalkSpeed;


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
    // Update is called once per frame
    void Update()
    {
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

        if (IsAnimationPlaying("Attack"))
        {
            return;
        }

        if (mMovementsQueue.Count > 0){
            Tuple<float,float> e = mMovementsQueue.Dequeue();
            var old_pos = transform.position;
            var new_pos = new Vector3(e.Item1,e.Item2,old_pos.z);
            var delta = new_pos-old_pos;
            UnityEngine.Debug.Log("delta : x=" + delta.x + " y="+ delta.y );



        bool RightArrowPressed  = delta.x>0.0f;
        bool LeftArrowPressed   = delta.x<0.0f;
        bool UpArrowPressed     = delta.y<0.0f;
        bool DownArrowPressed   = delta.y>0.0f;
        bool Moving = RightArrowPressed || LeftArrowPressed || UpArrowPressed || DownArrowPressed;



        // NorthEast
        if (RightArrowPressed && UpArrowPressed && !DownArrowPressed && !LeftArrowPressed)
        {
            dir = Direction.NorthEast;
            if (running)
                mAnimator.Play("RunNoreste");
            else
                mAnimator.Play("WalkNoreste");
            //Vector3 newpos = transform.position + Vector3.right * WalkRunSpeed * Time.deltaTime * walkDiagDelta + Vector3.up * WalkRunSpeed * Time.deltaTime * walkDiagDelta;
            TryToMove(new_pos);
        }
        else // North
      if (!RightArrowPressed && UpArrowPressed && !DownArrowPressed && !LeftArrowPressed)
        {
            dir = Direction.North;
            if (running)
                mAnimator.Play("RunNorte");
            else
                mAnimator.Play("WalkNorte");
            TryToMove(new_pos/*transform.position + Vector3.up * WalkRunSpeed * Time.deltaTime*/);
        }
        else // South
      if (!RightArrowPressed && !UpArrowPressed && DownArrowPressed && !LeftArrowPressed)
        {
            dir = Direction.South;
            if (running)
                mAnimator.Play("RunSur");
            else
                mAnimator.Play("WalkSur");
            //Vector3 newpos = transform.position + Vector3.down * WalkRunSpeed * Time.deltaTime;
            TryToMove(new_pos);
        }
        else // SouthEast
      if (RightArrowPressed && DownArrowPressed && !UpArrowPressed && !LeftArrowPressed)
        {
            dir = Direction.SouthEast;
            if (running)
                mAnimator.Play("RunSureste");
            else
                mAnimator.Play("WalkSureste");
            TryToMove(new_pos);
        }
        else
      if (RightArrowPressed && !DownArrowPressed && !UpArrowPressed && !LeftArrowPressed)
        {
            dir = Direction.East;
            if (running)
                mAnimator.Play("RunEste");
            else
                mAnimator.Play("WalkEste");
            TryToMove(new_pos);
        }
        else
      if (LeftArrowPressed && !UpArrowPressed && !DownArrowPressed && !RightArrowPressed)
        {
            dir = Direction.West;
            if (running)
                mAnimator.Play("RunOeste");
            else
                mAnimator.Play("WalkOeste");
            TryToMove(new_pos);
        }
        else
      if (LeftArrowPressed && UpArrowPressed && !DownArrowPressed && !RightArrowPressed)
        {
            dir = Direction.NorthWest;
            if (running)
                mAnimator.Play("RunNoroeste");
            else
                mAnimator.Play("WalkNoroeste");
            TryToMove(new_pos);
        }
        else
      if (LeftArrowPressed && !UpArrowPressed && DownArrowPressed && !RightArrowPressed)
        {
            dir = Direction.SouthWest;
            if (running)
                mAnimator.Play("RunSuroeste");
            else
                mAnimator.Play("WalkSuroeste");
            TryToMove(new_pos);
        }


        if (!Moving)
        {
            PlayAnimation("Stand");
        }
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
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

public class MotorPlayerMovement : Movement
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
    public bool IsPhantom;
    private float damageValue = 0f;
    public Slider healthSlider;
    public Slider manaSlider;
    private TextMeshProUGUI textToHead;
    private TextMeshProUGUI textName;
    private WorldClient mWorldClient;
    private SpriteRenderer spriteRenderer;
    private GameObject mCollidingChar;

    void LateUpdate()
    {
        if (spriteRenderer.isVisible)
            spriteRenderer.sortingOrder = (int)Camera.main.WorldToScreenPoint(transform.position).y * -1;
    }


    public void Awake()
    {
        health = life;
        healthSlider = GameObject.Find("SliderLife").GetComponent<Slider>();
        UnityEngine.Debug.Assert(healthSlider != null, "Cannot find Life Slider in Player");
        manaSlider = GameObject.Find("SliderMana").GetComponent<Slider>();
        UnityEngine.Debug.Assert(manaSlider != null, "Cannot find Mana Slider in Player");
        textToHead = GameObject.Find("TextToHead").GetComponent<TextMeshProUGUI>();
        UnityEngine.Debug.Assert(textToHead != null, "Cannot find Text To Head in Player");
        textName = GameObject.Find("TextName").GetComponent<TextMeshProUGUI>();
        UnityEngine.Debug.Assert(textName != null, "Cannot find Text Name in Player");
        SetDirection(Direction.South);
        spriteRenderer = GetComponent<SpriteRenderer>();
        DontDestroyOnLoad(this.gameObject);
    }

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
            //mWorldClient.OnPlayerMeleeAttacked(mCollidingChar.name);
        }
        else {
            //mWorldClient.OnPlayerMeleeAttacked("Z");
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
            //mWorldClient.OnPlayerMoved(pos);
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
    public void SetTeleportingPos(Vector3 newPos )
    {
        teleportingPos = newPos;
    }
    public Vector3 GetTeleportingPos()
    {
        return teleportingPos;
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

    void Update(){
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (!IsPhantom)
            {
                PlayAnimation("Dead");
                healthSlider.value = 0;
                isDead = true;
                return;
            }

        }

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
        mTimeElapsedFixedUpdate += Time.deltaTime;

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

        if (input_delta.x != 0f && input_delta.y != 0f) input_delta *= walkDiagDelta;
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

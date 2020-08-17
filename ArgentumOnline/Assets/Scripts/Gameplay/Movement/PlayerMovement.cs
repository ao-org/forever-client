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
    private RuntimeAnimatorController mPhantomAnimatorController;
    private RuntimeAnimatorController mAnimatorController;
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
        mPhantomAnimatorController = Resources.Load<RuntimeAnimatorController>("Phantom") as RuntimeAnimatorController;
        UnityEngine.Debug.Assert(mPhantomAnimatorController != null, "Cannot find Phantom Controller in Resources");
        mWorldClient = GameObject.Find("WorldClient").GetComponent<WorldClient>();
        UnityEngine.Debug.Assert(mWorldClient != null);
        dir = Direction.South;
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = mSkinColor;
    }
    private System.Diagnostics.Stopwatch mInputStopwatch;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        //WalkRunSpeed = WalkSpeed;
        mBody.AddForce(new Vector2(0,0));
        mBody.velocity = Vector3.zero;
        mBody.angularVelocity = 0;
        mBody.gravityScale = 0f;
        //mBody.isKinematic = true;
        //mBody.useFullKinematicContacts =true;
        mInputStopwatch = new System.Diagnostics.Stopwatch();
		mInputStopwatch.Start();

        if (IsPhantom)
        {
            mAnimator.runtimeAnimatorController = mPhantomAnimatorController;
            healthSlider.gameObject.SetActive(false);
            manaSlider.gameObject.SetActive(false);
            textToHead.gameObject.SetActive(false);
        }
        else
        {
            scaleHuman = this.transform.localScale;
            mAnimatorController = mAnimator.runtimeAnimatorController;
        }
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
                textToHead.gameObject.SetActive(false);
                textName.transform.localScale = new Vector3(2.4f,2.4f, 1);
                this.transform.localScale = new Vector3(1, 1, 1);

                return;
            }

        }


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

                mAnimator.runtimeAnimatorController = mAnimatorController;

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

        mBody.velocity = Vector2.zero;
        mBody.angularVelocity = 0f;

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
        if (input_delta.x == 0f && input_delta.y > 0f) dir = Direction.North;
        if (input_delta.x > 0f && input_delta.y > 0f) dir = Direction.NorthEast;
        if (input_delta.x > 0f && input_delta.y == 0f) dir = Direction.East;
        if (input_delta.x > 0f && input_delta.y < 0f) dir = Direction.SouthEast;
        if (input_delta.x == 0f && input_delta.y < 0f) dir = Direction.South;
        if (input_delta.x < 0f && input_delta.y < 0f) dir = Direction.SouthWest;
        if (input_delta.x < 0f && input_delta.y == 0f) dir = Direction.West;
        if (input_delta.x < 0f && input_delta.y > 0f) dir = Direction.NorthWest;

        if (input_delta.x != 0f || input_delta.y != 0f) {
                var newpos = mBody.position + input_delta * WalkRunSpeed * Time.deltaTime;
                PlayAnimation(anim_name);
                TryToMove(newpos);
        }
        else {
                PlayAnimation(anim_name);
        }

    }




}

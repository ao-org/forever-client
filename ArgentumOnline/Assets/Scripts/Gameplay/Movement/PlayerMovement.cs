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

    public override void Awake()
    {
        //DontDestroyOnLoad(this.gameObject);
        base.Awake();
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
        //spriteRenderer = GetComponent<SpriteRenderer>();
        //spriteRenderer.sortingOrder = (int)Camera.main.WorldToScreenPoint(spriteRenderer.bounds.min).y * -1;
        //mAnimatorController = mAnimator.runtimeAnimatorController;
        dir = Direction.South;
    }
    void LateUpdate()
    {

        //spriteRenderer.sortingOrder = (int)Camera.main.WorldToScreenPoint(spriteRenderer.bounds.min).y * -1;
        //spriteRenderer.sortingOrder = (int)transform.position.y * -1;
    }
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
            UnityEngine.Debug.Log("OnCollisionEnter2D****************************");
            if (collision.collider.tag == "Human")
            {
                UnityEngine.Debug.Log("touch Player enter "+ collision.collider.name +" ****************************");
                mCollidingChar = collision.collider.gameObject;

            }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
            UnityEngine.Debug.Log("OnCollisionExit2D****************************");
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
            mWorldClient.OnPlayerMeleeAttacked("NONE");
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
        return;
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
    // Update is called once per frame
    void Update()
    {
        mBody.velocity = Vector2.zero;
        mBody.angularVelocity = 0f;

/*
        if (Input.GetKeyDown(KeyCode.B))
        {

            GameObject barco = GameObject.FindGameObjectsWithTag("Barco")[0];
            UnityEngine.Debug.Assert(barco != null);
            BarcoMovement barcoScript = barco.GetComponent<BarcoMovement>();
            UnityEngine.Debug.Assert(barcoScript != null);
            barcoScript.isActive = true;
            //GameObject barcoCamera = barco.GetComponent<MainCamera>();
            GameObject mainCamera = GameObject.FindGameObjectsWithTag("MainCamera")[0];
            Vector3 cameraPos = new Vector3(barco.transform.position.x, barco.transform.position.y, -1);
            mainCamera.transform.position = cameraPos;
            mainCamera.transform.SetParent(barco.transform);
            GameObject player = GameObject.FindGameObjectsWithTag("Player")[0];
            barcoScript.player = player;
            player.SetActive(false);
            return;
        }
        */
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

        if (IsAnimationPlaying("Attack"))
        {
            return;
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
        if (Input.GetKeyDown(KeyCode.A))
        {
            PlayAnimation("Attack");
            OnAttack();
            return;
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            if (IsPhantom)
            {
                if (mAnimatorController != null) { UnityEngine.Debug.Log("Player Update: " + mAnimatorController.name); }
                //UnityEngine.Debug.Log("Player Update: " + mPhantomAnimatorController.name);
                else
                    UnityEngine.Debug.Log("Player Update: " + mAnimatorController.name);

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

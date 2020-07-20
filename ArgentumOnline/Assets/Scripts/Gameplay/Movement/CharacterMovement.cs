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

public class CharacterMovement : Movement
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
    //private WorldClient mWorldClient;
    private SpriteRenderer spriteRenderer;
    //Codigo prueba para abolir fisica entre rigidbody players
    public Vector3 position, velocity, forward, oldPos;
    private float angularVelocity;
    public Quaternion rotation;
    public bool isColliding;
    private int playersColliding = 0;
    private bool isTryingToMove = false;
    private Vector3 newpos;
    private Vector2 mMovement;
    private string mAnimation = "";
    private WorldClient mWorldClient;
    private Queue<Tuple<float, float>> mMovementsQueue = new Queue<Tuple<float, float>>();

    public void PushMovement(Tuple<float, float> newpos)
    {
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
        textToHead = GameObject.Find("TextToHead").GetComponent<TextMeshProUGUI>();
        UnityEngine.Debug.Assert(textToHead != null, "Cannot find Text To Head in Player");
        textName = GameObject.Find("TextName").GetComponent<TextMeshProUGUI>();
        UnityEngine.Debug.Assert(textName != null, "Cannot find Text Name in Player");
        mPhantomAnimatorController = Resources.Load<RuntimeAnimatorController>("Phantom") as RuntimeAnimatorController;
        UnityEngine.Debug.Assert(mPhantomAnimatorController != null, "Cannot find Phantom Controller in Resources");
        spriteRenderer = GetComponent<SpriteRenderer>();
        mWorldClient = GameObject.Find("WorldClient").GetComponent<WorldClient>();
        UnityEngine.Debug.Assert(mWorldClient != null);
        dir = Direction.South;
    }
    void LateUpdate()
    {

        if (spriteRenderer.isVisible)

            spriteRenderer.sortingOrder = (int)Camera.main.WorldToScreenPoint(transform.position).y * -1;

    }
    
    private bool IsFacingObject()
    {
        GameObject human = GameObject.FindGameObjectsWithTag("Human")[0];
        UnityEngine.Debug.Assert(human != null);
        GameObject player = GameObject.FindGameObjectsWithTag("Player")[0];
        UnityEngine.Debug.Assert(player != null);

        float angle = 70;
        if (Vector3.Angle(forward, human.transform.position - player.transform.position) < angle)
        {
            UnityEngine.Debug.Log("********LookingAt");
            return true;
        }
        UnityEngine.Debug.Log("********NOT LookingAt");
        return false;
    }
 
    
    void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.collider.tag == "Human")
        {
            UnityEngine.Debug.Log("touch Player enter****************************");
            isColliding = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.tag == "Human")
        {
            UnityEngine.Debug.Log("touch Player exit****************************");
            isColliding = false;
        }
    }
    
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        mBody.AddForce(new Vector2(0,0));
        mBody.velocity = Vector3.zero;
        mBody.angularVelocity = 0;
        mBody.gravityScale = 0f;
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
    private bool TryToMove(Vector3 pos)
    {
        if (IsThereWater(pos))
        {
            // nothing to do
            return false;
        }
        else
        {
            forward = new Vector3(pos.x - transform.position.x, pos.y - transform.position.y, 0);
            oldPos = transform.position;
            newpos = pos;
            //UnityEngine.Debug.Log("**ANTES***PosX: " + transform.position.x.ToString() + "- PosY: " + transform.position.y.ToString());
            isTryingToMove = true;
            //mBody.MovePosition(pos);
            mBody.MovePosition(mBody.position + mMovement * WalkRunSpeed * Time.fixedDeltaTime);
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
        if (isTryingToMove)
        {
            //UnityEngine.Debug.Log("**DESPUES***PosX: " + transform.position.x.ToString() + "- PosY: " + transform.position.y.ToString());
            isTryingToMove = false;
            if (!isColliding)
            {
                //UnityEngine.Debug.Log("Sent Move to Server");
                mWorldClient.OnPlayerMoved(newpos);
                isTryingToMove = false;
            }
        }
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
            return;

        if (Input.GetKey(KeyCode.LeftControl))
        {
            Attack();
            return;
        }
        
        if (Input.GetKeyDown(KeyCode.M))
            Dead();
        
        if (Input.GetKeyDown(KeyCode.L))
            Live();
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (running)
                running = false;
            else
                running = true;
        }
        if (mMovementsQueue.Count > 0)
        {
            Tuple<float, float> e = mMovementsQueue.Dequeue();
            var old_pos = transform.position;
            var new_pos = new Vector3(e.Item1, e.Item2, old_pos.z);
            mMovement = new_pos - old_pos;
        }

        mAnimation = "Stand";
        if (mMovement.x != 0f || mMovement.y != 0f)
        {
            if (!running)
            {
                WalkRunSpeed = WalkSpeed;
                mAnimation = "Walk";
            }
            else
            {
                WalkRunSpeed = WalkSpeed * runDelta;
                mAnimation = "Run";
            }
        }
        

        if (mMovement.x != 0f && mMovement.y != 0f) mMovement *= walkDiagDelta;

        if (mMovement.x == 0f && mMovement.y > 0f) dir = Direction.North;
        if (mMovement.x > 0f && mMovement.y > 0f) dir = Direction.NorthEast;
        if (mMovement.x > 0f && mMovement.y == 0f) dir = Direction.East;
        if (mMovement.x > 0f && mMovement.y < 0f) dir = Direction.SouthEast;
        if (mMovement.x == 0f && mMovement.y < 0f) dir = Direction.South;
        if (mMovement.x < 0f && mMovement.y < 0f) dir = Direction.SouthWest;
        if (mMovement.x < 0f && mMovement.y == 0f) dir = Direction.West;
        if (mMovement.x < 0f && mMovement.y > 0f) dir = Direction.NorthWest;
    }

    void FixedUpdate()
    {
        if (IsAnimationPlaying("Attack"))
            return;
        PlayAnimation(mAnimation);
        TryToMove(newpos);
        //IsFacingObject();
        //if (isTryingToMove)
        //mBody.MovePosition(pos);
    }

    public void Attack()
    {
        PlayAnimation("Attack");
    }

    public void Dead()
    {
        if (!IsPhantom)
        {
            PlayAnimation("Dead");
            healthSlider.value = 0;
            isDead = true;
            return;
        }
    }

    public void Live()
    {
        if (IsPhantom)
        {
            if (mAnimatorController != null) { UnityEngine.Debug.Log("Player Update: " + mAnimatorController.name); }
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
            this.transform.localScale = scaleHuman;
            healthSlider.value = life;
            return;
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
    /*private string GetAnimation(string action)
    {

    }*/
    private bool IsAnimationLastFrame()
    {
        return (mAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1);
    }

}

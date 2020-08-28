using System;
﻿using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(Rigidbody2D))]
public class EXPERIMENTAL_PlayerMovement : MonoBehaviour {

    [SerializeField] private Vector2 _movementDirection;
    [SerializeField] private float _baseMovementSpeed = 1.0f;

    private float _inputMovementSpeed;
    private float _finalMovementSpeed;

    private Rigidbody2D _rigidBody;
    [SerializeField] private Animator _animator;

    public Collider2D collider1;
    public Collider2D collider2;

    private WorldClient mWorldClient = null;
    private Vector3 mTeleportingPos = new Vector3();
    private bool mPC = false;
    private Color mSkinColor = new Color(1,1,1,1);
    private SpriteRenderer mSpriteRenderer = null;

    private static Dictionary<string,Color> ColorDict
        = new Dictionary<string, Color>
    {
        { "1" , new Color(0.141f, 0.141f, 0.141f,1) },
        { "2" , new Color(0.2f, 0.180f, 0.180f,1)   },
        { "3" , new Color(0.258f, 0.258f, 0.258f,1) },
        { "4" , new Color(0.356f, 0.356f, 0.356f,1) },
        { "5" , new Color(0.462f, 0.298f, 0.207f,1) },
        { "6" , new Color(0.490f, 0.392f, 0.266f,1) },
        { "7" , new Color(0.603f, 0.423f, 0.380f,1) },
        { "8" , new Color(0.690f, 0.568f, 0.568f,1) },
        { "9" , new Color(0.8f, 0.752f, 0.752f,1)},
        { "10", new Color(1, 1, 1,1) }
    };

    public Color GetColorFromString(string color){

        return ColorDict[color];
    }

    public void ChangeColorSkin(string color)
    {
        mSkinColor = GetColorFromString(color);
        if (mSpriteRenderer!=null){
            mSpriteRenderer.color = mSkinColor;
        }
    }

    public void SetTeleportingPos(Vector3 newPos)
    {
        mTeleportingPos = newPos;
    }

    public Vector3 GetTeleportingPos()
    {
        return mTeleportingPos;
    }

    private void Awake() {
        _rigidBody = GetComponent<Rigidbody2D>();
        mWorldClient = GameObject.Find("WorldClient").GetComponent<WorldClient>();
        UnityEngine.Debug.Assert(mWorldClient != null);
        mSpriteRenderer = GetComponent<SpriteRenderer>();
        mSpriteRenderer.color = mSkinColor;

    }

    private void Start() {
        Physics2D.IgnoreCollision(collider1, collider2, true);
    }

    private void Update() {
        if(mPC) {
            ProcessInputs();
        }
        else {
            //TODO
        }
        Animate();
    }
    public void SetColorSkin(string color)
    {

    }

    private Queue<Tuple<short,float,float>> mActionQueue = new Queue<Tuple<short,float,float>>();

    public void PushMovement(Tuple<short,float,float> newpos){
        mActionQueue.Enqueue(newpos);
    }

    public void SetPlayerCharater(bool pc) {
        mPC = pc;
    }
    private void FixedUpdate() {
        Move();
    }

    private void ProcessInputs() {
        _movementDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        _inputMovementSpeed = Mathf.Clamp(_movementDirection.magnitude, 0.0f, 1.0f);
        _movementDirection.Normalize();

        if (Input.GetKeyDown(KeyCode.R)) {
            if (_baseMovementSpeed < 2.0f) _baseMovementSpeed = 2.0f;
            else _baseMovementSpeed = 1.0f;
        }

        _finalMovementSpeed = _inputMovementSpeed * _baseMovementSpeed * 2.0f;
    }

    private void Move() {
        Vector2 newpos = new Vector2(_rigidBody.position.x,_rigidBody.position.y);
        if(mPC) {
            newpos = _rigidBody.position + _movementDirection * _finalMovementSpeed * Time.fixedDeltaTime;
            if(_finalMovementSpeed> 0f){
                    mWorldClient.OnPlayerMoved(newpos);
            }
        }
        else {
            if (mActionQueue.Count > 0){
                Tuple<short,float,float> e = mActionQueue.Dequeue();
                if(e.Item1==ProtoBase.ProtocolNumbers["CHARACTER_MOVED"])
                {
                    newpos = new Vector2(e.Item2,e.Item3);
                    var old_pos = new Vector2(transform.position.x,transform.position.y);
                    var delta = newpos - old_pos;
                    _movementDirection = delta;
                    _movementDirection.Normalize();
                    _inputMovementSpeed = Mathf.Clamp(_movementDirection.magnitude, 0.0f, 1.0f);
                    _finalMovementSpeed = _inputMovementSpeed * _baseMovementSpeed * 2.0f;

                }
                else if(e.Item1==ProtoBase.ProtocolNumbers["CHARACTER_MELEE"])
                {
                    //PlayAnimation("Attack");
                }
                else if(e.Item1==ProtoBase.ProtocolNumbers["CHARACTER_NEWPOS"])
                {
                    // We teleport to the current scene, only need to update the player position
                    var old_pos = transform.position;
                    var new_pos = new Vector3(e.Item2,e.Item3,old_pos.z);
                    transform.position = new_pos;
                }
            }//
        }
        _rigidBody.MovePosition(newpos);
    }

    private void Animate() {
        if (_movementDirection != Vector2.zero) {
            _animator.SetFloat("Horizontal", _movementDirection.x);
            _animator.SetFloat("Vertical", _movementDirection.y);
        }

        _animator.SetFloat("Speed", _finalMovementSpeed);
    }
}
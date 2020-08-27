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

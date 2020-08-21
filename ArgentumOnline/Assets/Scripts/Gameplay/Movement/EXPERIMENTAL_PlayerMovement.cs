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
    private bool mPC = false;

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
        var newpos = _rigidBody.position + _movementDirection * _finalMovementSpeed * Time.fixedDeltaTime;
        if(_finalMovementSpeed> 0f){
                mWorldClient.OnPlayerMoved(newpos);
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

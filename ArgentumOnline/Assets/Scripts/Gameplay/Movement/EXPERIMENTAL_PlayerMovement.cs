using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EXPERIMENTAL_PlayerMovement : MonoBehaviour {

    [SerializeField] private Vector2 _movementDirection;
    [SerializeField] private float _baseMovementSpeed = 1.0f;
    
    private float _inputMovementSpeed;
    private float _finalMovementSpeed;

    private Rigidbody2D _rigidBody;
    [SerializeField] private Animator _animator;

    void Awake() {
        _rigidBody = GetComponent<Rigidbody2D>();
    }

    void Update() {
        ProcessInputs();
        Animate();
    }

    void FixedUpdate() {
        Move();
    }

    void ProcessInputs() {
        _movementDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        _inputMovementSpeed = Mathf.Clamp(_movementDirection.magnitude, 0.0f, 1.0f);
        _movementDirection.Normalize();

        if (Input.GetKeyDown(KeyCode.R)) {
            if (_baseMovementSpeed < 2.0f) _baseMovementSpeed = 2.0f;
            else _baseMovementSpeed = 1.0f;
        }
    }

    void Move() {
        _finalMovementSpeed = _inputMovementSpeed * _baseMovementSpeed * 2.0f;
        _rigidBody.MovePosition(_rigidBody.position + _movementDirection * _finalMovementSpeed * Time.fixedDeltaTime);
    }

    void Animate() {
        if (_movementDirection != Vector2.zero) {
            _animator.SetFloat("Horizontal", _movementDirection.x);
            _animator.SetFloat("Vertical", _movementDirection.y);
        }
        
        _animator.SetFloat("Speed", _finalMovementSpeed);
    }
}

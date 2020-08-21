using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EXPERIMENTAL_CharacterMovement : MonoBehaviour {

    [SerializeField] private Vector2 _movementDirection;
    [SerializeField] private float _baseMovementSpeed = 1.0f;

    private float _inputMovementSpeed;
    private float _finalMovementSpeed;

    private Rigidbody2D _rigidBody;
    [SerializeField] private Animator _animator;

    public Collider2D collider1;
    public Collider2D collider2;

    private void Awake() {
        _rigidBody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        UnityEngine.Debug.Assert(_rigidBody != null);
        UnityEngine.Debug.Assert(_animator != null);
    }

    private void Start() {

        //Physics2D.IgnoreCollision(collider1, collider2, true);
    }

    private void Update() {
        Animate();
    }

    private void FixedUpdate() {
        Move();
    }


    private void Move() {
    }

    private void Animate() {
        if (_movementDirection != Vector2.zero) {
            _animator.SetFloat("Horizontal", _movementDirection.x);
            _animator.SetFloat("Vertical", _movementDirection.y);
        }

        _animator.SetFloat("Speed", _finalMovementSpeed);
    }
}

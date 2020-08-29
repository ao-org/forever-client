using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LocalPlayerMovement : MonoBehaviour {
    private float mInputMovementSpeed;
    private float mFinalMovementSpeed;
    private Vector2 mMovementDirection;

    private Rigidbody2D mRigidBody;

    [SerializeField] private float mBaseMovementSpeed = 1.0f;

    [SerializeField] private Animator mAnimator;

    [SerializeField] private Collider2D mCollider1;
    [SerializeField] private Collider2D mCollider2;

    private void Awake() {
        mRigidBody = GetComponent<Rigidbody2D>();
        Physics2D.IgnoreCollision(mCollider1, mCollider2, true);
    }

    private void Update() {
        ProcessInputs();
        Animate();
    }

    private void FixedUpdate() {
        Move();
    }

    private void ProcessInputs() {
        mMovementDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        mInputMovementSpeed = Mathf.Clamp(mMovementDirection.magnitude, 0.0f, 1.0f);
        mMovementDirection.Normalize();

        if (Input.GetKeyDown(KeyCode.R)) {
            if (mBaseMovementSpeed < 2.0f) mBaseMovementSpeed = 2.0f;
            else mBaseMovementSpeed = 1.0f;
        }

        mFinalMovementSpeed = mInputMovementSpeed * mBaseMovementSpeed * 2.0f;
    }

    private void Move() {
        mRigidBody.MovePosition(mRigidBody.position + mMovementDirection * mFinalMovementSpeed * Time.fixedDeltaTime);
    }

    private void Animate() {
        if (mMovementDirection != Vector2.zero) {
            mAnimator.SetFloat("Horizontal", mMovementDirection.x);
            mAnimator.SetFloat("Vertical", mMovementDirection.y);
        }

        mAnimator.SetFloat("Speed", mFinalMovementSpeed);
    }
}

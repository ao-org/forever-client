using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PruebaNPCMovement : Movement
{
    //Codigo prueba para abolir fisica entre rigidbody players
    public Vector3 position, velocity;
    private float angularVelocity;
    public Quaternion rotation;
    public bool isColliding;
    private int playersColliding = 0;
    public override void Awake()
    {
        //DontDestroyOnLoad(this.gameObject);
        base.Awake();
    }
        // Start is called before the first frame update
    void Start()
    {
        base.Start();
    }
    void FixedUpdate()
    {
        if (isColliding)
        {
            //mBody.constraints = RigidbodyConstraints2D.FreezeAll;
            //mBody.isKinematic = true;
            mBody.velocity = Vector3.zero;
            mBody.angularVelocity = 0f;
         
        }
        //mBody.AddForce(new Vector2(0, 0));
    }

    void LateUpdate()
    {
        //spriteRenderer.sortingOrder = (int)Camera.main.WorldToScreenPoint(spriteRenderer.bounds.min).y * -1;
        //spriteRenderer.sortingOrder = (int)transform.position.y * -1;
        /*if (isColliding)
        {
            mBody.position = position;
            //mBody.rotation = rotation;
            mBody.velocity = velocity;// Vector3.zero;
            mBody.angularVelocity = angularVelocity;// 0f;
        }*/
        /*if (isColliding)
        {
            GameObject player = GameObject.FindGameObjectsWithTag("Player")[0];
            //Debug.Assert(player != null);
            Rigidbody2D rigidBody = GetComponent<Rigidbody2D>();
            mBody.velocity = Vector3.zero;
            mBody.angularVelocity = 0f;
        }*/
    }

    void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.collider.tag == "Player")
        {
            UnityEngine.Debug.Log("touch Player enter****************************");
            isColliding = true;
        }
        /*if (collision.collider.tag == "Player")
        {
            UnityEngine.Debug.Log("touch Player enter****************************");
            playersColliding += 1;
            if (mBody.velocity.magnitude < 0.3) { }
                mBody.constraints = RigidbodyConstraints2D.FreezeAll;
        }*/
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.tag == "Player")
        {
            UnityEngine.Debug.Log("touch Player exit****************************");
            isColliding = false;
        }
        /*if (collision.collider.tag == "Player")
        {
            playersColliding -= 1;
            UnityEngine.Debug.Log("touch Human***************" + playersColliding + "*************");
            if (playersColliding == 0) { }
                mBody.constraints = RigidbodyConstraints2D.None;
        }*/
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}

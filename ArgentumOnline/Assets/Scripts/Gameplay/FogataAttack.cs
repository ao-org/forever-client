using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogataAttack : MonoBehaviour
{
    private float EnemyDamage = 0.1f;
    GameObject player;
    PlayerMovement playerLife;
    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        //playerLife = player.GetComponent<PlayerMovement>();
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            UnityEngine.Debug.Log("OnEnterCollider");
            playerLife.TakeDamage(EnemyDamage);
        }
    }
    void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            UnityEngine.Debug.Log("OnExitCollider");
            playerLife.QuitDamage(EnemyDamage);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogataAttack : MonoBehaviour
{
    public int EnemyDamage = 10;
    GameObject player;
    PlayerMovement playerLife;
    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerLife = player.GetComponent<PlayerMovement>();
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
        if(col.CompareTag("Player"))
        {
            playerLife.TakeDamage(EnemyDamage);
        }
    }
}

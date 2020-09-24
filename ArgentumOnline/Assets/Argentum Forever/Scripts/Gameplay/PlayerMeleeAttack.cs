using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMeleeAttack : NetworkBehaviour
{
    [SerializeField]
    CircleCollider2D attackZone;

    void Start()
    {
        Collider2D[] colliders = GetComponentsInChildren<Collider2D>();

        for (int i = 0; i < colliders.Length; i++)
        {
            Physics2D.IgnoreCollision(colliders[i], attackZone);
        }
    }

    [Command]
    public void DoAttack()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, attackZone.radius, Vector2.zero);
        for (int i = 0; i < hits.Length; i++)
        {
            if (transform == hits[i].transform) continue;
            Vector2 characterForward = GetComponent<PlayerMovement>().GetLastMovementDirection().normalized;
            float angle = Vector2.Angle(((Vector2)transform.position - hits[i].point).normalized, -characterForward);
            Debug.LogWarning("Angle: " + angle + " With: " + hits[i].transform.name);
            Debug.DrawLine(transform.position, hits[i].point, Color.red, 5f);
        }
    }
}

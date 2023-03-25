using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitBox : MonoBehaviour
{
    [SerializeField]
    private Player player;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
            player.OnDamaged(collision.transform.position, collision.gameObject.GetComponentInParent<Meteor>().atk);
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("HitBox"))
            player.OnDamaged(collision.transform.position, collision.gameObject.GetComponentInParent<Enemy>().atk);
        else if (collision.CompareTag("GrapBox"))
        {
            Enemy enemy = collision.GetComponentInParent<Enemy>();
            Vector2 hitDir = player.transform.position.x > enemy.transform.position.x ? Vector2.left : Vector2.right;
            StartCoroutine(player.Holding(enemy.atk, hitDir));
        }
    }
}
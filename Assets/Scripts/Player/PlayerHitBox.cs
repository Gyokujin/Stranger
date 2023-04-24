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
        {
            int damage = 0;

            if (collision.gameObject.GetComponent<Meteor>() != null)
                damage = collision.gameObject.GetComponentInParent<Meteor>().atk;
            else if (collision.gameObject.GetComponent<RosaryBullet>() != null)
                damage = collision.gameObject.GetComponentInParent<RosaryBullet>().atk;

            player.OnDamaged(collision.transform.position, damage);
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("HitBox"))
        {
            if (collision.gameObject.GetComponentInParent<Enemy>() != null)
                player.OnDamaged(collision.transform.position, collision.gameObject.GetComponentInParent<Enemy>().atk);
            else if (collision.gameObject.GetComponentInParent<Gaiten>() != null)
                player.OnDamaged(collision.transform.position, collision.gameObject.GetComponentInParent<Gaiten>().atk);
        }
            
        else if (collision.CompareTag("GrapBox"))
        {
            Enemy enemy = collision.GetComponentInParent<Enemy>();
            Vector2 hitDir = player.transform.position.x > enemy.transform.position.x ? Vector2.left : Vector2.right;
            StartCoroutine(player.Holding(enemy.atk, hitDir));
        }
    }
}
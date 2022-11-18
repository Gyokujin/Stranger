using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadZone : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player.instance.playerHP -= 10;

            if (Player.instance.playerHP > 0)
            {
                collision.attachedRigidbody.velocity = Vector2.zero;
                collision.gameObject.transform.position = GameManager.instance.startPoint;
            }
            else
            {
                GameManager.instance.GameOver();
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    [SerializeField]
    private int backgroundNum;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player player = collision.gameObject.GetComponent<Player>();
            
            if (player.onMove || player.onDash)
                gameObject.GetComponentInParent<Reposition>().ReAssign(backgroundNum, transform.position);
        }
    }
}
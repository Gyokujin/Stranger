using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            // #. Next Stage
            GameManager.instance.StageLoad(GameManager.instance.curScene + 1);
        }
    }
}
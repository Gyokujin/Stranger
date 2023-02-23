using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClearTrigger : MonoBehaviour
{
    public int stageNum;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            StartCoroutine(GameManager.instance.StageTransition(stageNum));
    }
}
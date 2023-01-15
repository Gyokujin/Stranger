using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage0_Event1 : MonoBehaviour
{
    [SerializeField]
    private GameObject flyingDemon;
    [SerializeField]
    private Vector2 targetPos;
    private bool onEvent;
    private float eventTime = 5;

    IEnumerator Stage0Event()
    {
        flyingDemon.SetActive(true);

        while (eventTime > 0)
        {
            eventTime -= Time.deltaTime;
            flyingDemon.transform.position = Vector2.MoveTowards(flyingDemon.transform.position, targetPos, 0.01f);
            
            yield return null;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !onEvent)
        {
            onEvent = true;
            StartCoroutine("Stage0Event");
        }
    }
}
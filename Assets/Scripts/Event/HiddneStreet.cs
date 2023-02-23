using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddneStreet : MonoBehaviour
{
    [SerializeField]
    private GameObject disembosomWall;
    [SerializeField]
    private GameObject[] hiddenEnemies;
    [SerializeField]
    private GameObject[] hiddenObjects;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            disembosomWall.SetActive(false);

            if (hiddenEnemies.Length > 0)
            {
                foreach (GameObject targetEnemies in hiddenEnemies)
                {
                    if (targetEnemies.name == transform.name)
                        return;

                    targetEnemies.SetActive(true);
                }
            }

            if (hiddenObjects.Length > 0)
            {
                foreach (GameObject targetObjects in hiddenObjects)
                {
                    if (targetObjects.name == transform.name)
                        return;

                    targetObjects.SetActive(true);
                }
            }
        }
    }
}
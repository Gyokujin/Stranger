using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddneStreet : MonoBehaviour
{
    [SerializeField]
    private GameObject disembosomWall;
    [SerializeField]
    private GameObject[] hiddneObjects;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            disembosomWall.SetActive(false);

            foreach (GameObject targetObjects in hiddneObjects)
            {
                if (targetObjects.name == transform.name)
                    return;

                targetObjects.SetActive(true);
            }
        }
        
    }
}
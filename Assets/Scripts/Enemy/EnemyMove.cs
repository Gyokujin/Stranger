using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    BoxCollider2D collider;

    void Awake()
    {
        collider = GetComponent<BoxCollider2D>();
    }

    void FixedUpdate()
    {
        
    }
}

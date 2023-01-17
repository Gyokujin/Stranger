using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteor : MonoBehaviour
{
    private Rigidbody2D rigid;

    void Start()
    {
        rigid.GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        //rigid.velocity = new Vector2(1, -1);
    }
}
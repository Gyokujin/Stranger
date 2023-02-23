using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteor : MonoBehaviour
{
    public float atk;
    [SerializeField]
    private GameObject hitBox;

    private Rigidbody2D rigid;
    private Animator animator;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("Platform") || collision.CompareTag("Wall"))
        {
            StartCoroutine("Explosion");
        }
    }

    IEnumerator Explosion()
    {
        yield return new WaitForSeconds(0.1f);
        hitBox.SetActive(true);
        rigid.simulated = false;
        animator.SetTrigger("Invocation");
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
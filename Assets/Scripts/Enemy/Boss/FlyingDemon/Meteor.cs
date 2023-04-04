using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteor : MonoBehaviour
{
    public int atk;
    [SerializeField]
    private GameObject hitBox;

    private Rigidbody2D rigid;
    private Animator animator;
    private CircleCollider2D collider;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        collider = GetComponent<CircleCollider2D>();
    }

    void Start()
    {
        Invoke("OnHitBox", 1f);
    }

    void OnHitBox()
    {
        collider.enabled = true;
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

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("Platform") || collision.CompareTag("Wall"))
        {
            StartCoroutine("Explosion");
        }
    }
}
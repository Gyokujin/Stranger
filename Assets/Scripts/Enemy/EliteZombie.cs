using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliteZombie : MonoBehaviour
{
    [SerializeField]
    private Vector2 sight;
    private bool onAttack;
    [SerializeField]
    private GameObject hitBox;

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rigid;
    private BoxCollider2D collider;
    private Animator animator;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        Check();
    }

    // #1. 플레이어 감지
    void Check()
    {
        if (!onAttack)
        {
            Debug.DrawRay(rigid.position, sight * 2f, Color.red);
            RaycastHit2D detectCheck = Physics2D.Raycast(rigid.position, sight, 2f, LayerMask.GetMask("Player"));

            if (detectCheck.collider != null)
            {
                StartCoroutine("Attack");
            }
        }
    }

    // #2. 공격 실행
    IEnumerator Attack()
    {
        Debug.Log("공격");
        onAttack = true;
        animator.SetTrigger("doAttack");
        rigid.AddForce(sight * 13f, ForceMode2D.Impulse);
        rigid.constraints = RigidbodyConstraints2D.None;
        collider.isTrigger = true;
        hitBox.SetActive(true);

        yield return new WaitForSeconds(0.1f);
        hitBox.SetActive(false);

        yield return new WaitForSeconds(0.9f);
        rigid.constraints = RigidbodyConstraints2D.FreezePositionX;
        collider.isTrigger = false;

        yield return new WaitForSeconds(1.0f);
        onAttack = false;
    }
}
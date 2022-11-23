using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : Enemy
{
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
        if (!onAttack && !onDamage && !onDie)
        {
            Debug.DrawRay(rigid.position, Vector2.left * 1.4f, Color.red); // 좌측 시야
            Debug.DrawRay(rigid.position, Vector2.right * 1.4f, Color.red); // 우측 시야
            RaycastHit2D leftCheck = Physics2D.Raycast(rigid.position, Vector2.left, 1.4f, LayerMask.GetMask("Player"));
            RaycastHit2D rightCheck = Physics2D.Raycast(rigid.position, Vector2.right, 1.4f, LayerMask.GetMask("Player"));

            if (leftCheck.collider != null) // 좌측 감지
            {
                gameObject.transform.localScale = new Vector3(1, 1, 1);
                StartCoroutine("Attack");
            }
            else if (rightCheck.collider != null) // 우측 감지
            {
                gameObject.transform.localScale = new Vector3(-1, 1, 1);
                StartCoroutine("Attack");
            }
        }
    }

    // #2. 공격 실행
    IEnumerator Attack()
    {
        onAttack = true;

        yield return new WaitForSeconds(0.6f);
        animator.SetTrigger("doAttack");
        hitBox.SetActive(true);
        
        yield return new WaitForSeconds(0.1f);
        hitBox.SetActive(false);

        yield return new WaitForSeconds(1.4f);
        onAttack = false;
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerHitBox"))
        {
            float damage = collision.gameObject.GetComponentInParent<Player>().playerATK;
            StartCoroutine("DamageProcess", damage);
        }
    }

    IEnumerator DamageProcess(float amount)
    {
        onDamage = true;
        hp -= amount;
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        if (hp <= 0)
        {
            onDie = true;
            StartCoroutine("DieProcess");
        }
        else
        {
            yield return new WaitForSeconds(0.1f);
            onDamage = false;

            yield return new WaitForSeconds(1);
            spriteRenderer.color = new Color(1, 1, 1, 1);
        }
    }

    IEnumerator DieProcess()
    {
        yield return new WaitForSeconds(1.0f);
        gameObject.SetActive(false);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliteZombie : MonoBehaviour
{
    private bool onAttack;
    private bool onChase;
    private Vector2 attackDir;
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
            Debug.DrawRay(rigid.position, Vector2.left * 3.5f, Color.red); // 좌측 시야
            Debug.DrawRay(rigid.position, Vector2.right * 3.5f, Color.red); // 우측 시야
            RaycastHit2D leftCheck = Physics2D.Raycast(rigid.position, Vector2.left, 3.5f, LayerMask.GetMask("Player"));
            RaycastHit2D rightCheck= Physics2D.Raycast(rigid.position, Vector2.right, 3.5f, LayerMask.GetMask("Player"));

            if (leftCheck.collider != null) // 좌측 감지
            {
                attackDir = Vector2.left;
                gameObject.transform.localScale = new Vector3(1, 1, 1);
                Debug.DrawRay(rigid.position + Vector2.left * 0.4f, Vector3.down, Color.green);
                RaycastHit2D platCheck = Physics2D.Raycast(rigid.position + Vector2.left * 0.4f, Vector3.down, 1, LayerMask.GetMask("Platform"));

                if (platCheck.collider == null)
                    StartCoroutine("Attack");
                else
                    StartCoroutine("Chase", Vector2.left);
            }
            else if (rightCheck.collider != null) // 우측 감지
            {
                attackDir = Vector2.right;
                gameObject.transform.localScale = new Vector3(-1, 1, 1);
                Debug.DrawRay(rigid.position + Vector2.right * 0.4f, Vector3.down, Color.green);
                RaycastHit2D platCheck = Physics2D.Raycast(rigid.position + Vector2.right * 0.4f, Vector3.down, 1, LayerMask.GetMask("Platform"));

                if (platCheck.collider == null)
                    StartCoroutine("Attack");
                else
                    StartCoroutine("Chase", Vector2.right);
            }
        }

        // #. 공격중 바닥 감지
        else if (onChase)
        {
            Debug.DrawRay(rigid.position + attackDir * 0.4f, Vector3.down, Color.green);
            RaycastHit2D platCheck = Physics2D.Raycast(rigid.position + attackDir * 0.4f, Vector3.down, 1, LayerMask.GetMask("Platform"));

            if (platCheck.collider == null)
            {
                StopCoroutine("Chase");
                rigid.velocity *= 0;
                onChase = false;
                onAttack = false;
                hitBox.SetActive(false);
                collider.isTrigger = false;
                rigid.constraints = RigidbodyConstraints2D.FreezeAll;
            }
        }
    }

    // #2. 근접 공격 실행
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

    // #3. 돌진 공격 실행
    IEnumerator Chase(Vector2 dir)
    {
        onAttack = true;
        onChase = true;

        yield return new WaitForSeconds(1.0f);
        Debug.Log("공격");        
        rigid.AddForce(dir * 10f, ForceMode2D.Impulse);
        rigid.constraints = RigidbodyConstraints2D.FreezePositionY;
        collider.isTrigger = true;

        yield return new WaitForSeconds(0.03f);
        hitBox.SetActive(true);

        yield return new WaitForSeconds(0.2f);
        animator.SetTrigger("doAttack");

        yield return new WaitForSeconds(0.1f);
        hitBox.SetActive(false);
        collider.isTrigger = false;
        rigid.constraints = RigidbodyConstraints2D.FreezeAll;

        yield return new WaitForSeconds(0.8f);
        onChase = false;

        yield return new WaitForSeconds(1.0f);
        onAttack = false;
    }
}
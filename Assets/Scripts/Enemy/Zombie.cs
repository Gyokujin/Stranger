using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : Enemy
{
    [Header("Action")]
    [SerializeField]
    private GameObject hitBox;
    [SerializeField]
    private GameObject backSight;

    [Header("Component")]
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
        base.Detect();

        if (!onAttack && onDetect && !onDie)
        {
            StartCoroutine("Attack");
        }
    }

    // #1. 공격 실행
    IEnumerator Attack()
    {
        onAttack = true;

        yield return new WaitForSeconds(atkTakeTime);        
        animator.SetTrigger("doAttack");
        hitBox.SetActive(true);

        yield return new WaitForSeconds(0.1f);
        hitBox.SetActive(false);

        yield return new WaitForSeconds(atkDelay);
        onAttack = false;
    }

    // #2. 피격 처리
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (onDie)
            return;

        if (collision.gameObject.CompareTag("PlayerHitBox"))
        {
            float damage = collision.gameObject.GetComponentInParent<Player>().playerATK;
            DamageProcess(damage);
        }
    }

    void DamageProcess(float damage)
    {
        hp -= damage;

        if (hp <= 0)
        {
            onDie = true;
            StartCoroutine("DieProcess");
        }
        else
        {
            BackCheck();
        }
    }

    // #3. 죽음 처리
    public IEnumerator DieProcess()
    {
        StopCoroutine("Attack");
        collider.isTrigger = true;
        spriteRenderer.color = new Color(1, 1, 1, 0.3f);

        yield return new WaitForSeconds(2.0f);
        gameObject.SetActive(false);
    }

    // #4. 후방 확인
    void BackCheck()
    {
        Debug.DrawRay(backSight.transform.position, Vector2.left * sightDir, Color.blue);
        RaycastHit2D backCheck = Physics2D.Raycast(backSight.transform.position, Vector2.left * sightDir, 1, LayerMask.GetMask("Player"));

        if (backCheck)
        {
            sightDir *= -1;
            transform.localScale = new Vector3(transform.localScale.x * -1, 1, 1);
        }
    }
}
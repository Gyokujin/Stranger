using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwampGuard : Enemy
{
    [Header("Action")]
    [SerializeField]
    private GameObject attackBox;
    [SerializeField]
    private float moveTimeMin;
    [SerializeField]
    private float moveTimeMax;
    [SerializeField]
    private Sprite standPose;
    RaycastHit2D platCheck;

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

    void Start()
    {
        StartCoroutine("Think");
    }

    void Update()
    {
        PlatCheck();
    }

    // #1. 대기 또는 이동 랜덤 패턴 실행
    IEnumerator Think()
    {
        if (!onDie)
        {
            yield return new WaitForSeconds(1f);
            int pattern = Random.Range(0, 3);

            switch (pattern)
            {
                case 0:
                    animator.enabled = false;
                    spriteRenderer.sprite = standPose;
                    StartCoroutine("Think");
                    break;
                case 1:
                    StartCoroutine("Move", -1);
                    break;
                case 2:
                    StartCoroutine("Move", 1);
                    break;
            }
        }
    }

    // #2. 이동 실행
    IEnumerator Move(int dir)
    {
        animator.enabled = true;
        float moveTime = Random.Range(moveTimeMin, moveTimeMax);
        sightDir = dir;
        Vector2 direction = dir == -1 ? Vector2.left : Vector2.right;
        transform.localScale = new Vector3(dir * (-0.6f), 0.6f, 1);
        attackBox.SetActive(true);

        while (moveTime > 0 && platCheck)
        {
            moveTime -= Time.deltaTime;
            rigid.velocity = direction * 0.5f;
            yield return null;
        }

        attackBox.SetActive(false);
        rigid.velocity = Vector2.zero;
        animator.enabled = false;
        spriteRenderer.sprite = standPose;
        StartCoroutine("Think");
    }

    // #3. 피격 처리
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (onDie)
            return;

        if (collision.gameObject.CompareTag("PlayerHitBox"))
        {
            int damage = collision.gameObject.GetComponentInParent<Player>().playerATK;
            StartCoroutine(DamageProcess(damage));
        }
    }

    IEnumerator DamageProcess(int damage)
    {
        onDamage = true;
        hp -= damage;

        if (hp <= 0)
        {
            onDie = true;
            StartCoroutine("DieProcess");
        }

        yield return new WaitForSeconds(2f);
        onDamage = false;
    }

    // #4. 죽음 처리
    public IEnumerator DieProcess()
    {
        StopCoroutine("Think");
        StopCoroutine("Move");
        attackBox.SetActive(false);
        rigid.velocity = Vector2.zero;
        animator.enabled = false;
        collider.enabled = false;
        spriteRenderer.color = new Color(1, 1, 1, 0.3f);

        yield return new WaitForSeconds(2.0f);
        gameObject.SetActive(false);
    }

    // #5. 바닥을 감지하여 떨어지는 것을 방지
    void PlatCheck()
    {
        Debug.DrawRay(rigid.position + Vector2.left * (-sightDir * 2f), Vector2.down * 0.4f, Color.blue);
        platCheck = Physics2D.Raycast(rigid.position + Vector2.left * (-sightDir * 2f), Vector3.down, 0.4f, LayerMask.GetMask("Platform"));
    }
}
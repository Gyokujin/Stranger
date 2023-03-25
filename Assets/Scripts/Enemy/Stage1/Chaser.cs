using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chaser : Enemy
{
    [Header("Action")]
    [SerializeField]
    private GameObject backSight;
    [SerializeField]
    private float patternDelay;
    [SerializeField]
    private float moveTime;
    RaycastHit2D platCheck;
    RaycastHit2D meleeCheck;

    [Header("Attack")]
    private bool onChase;
    [SerializeField]
    private GameObject attackBox;

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
        if (!onDie && !onChase && !onDamage)
        {
            base.Detect();

            if (!onChase && onDetect && !onAttack)
            {
                StopAllCoroutines();
                StartCoroutine("Chase");
            }
        }

        PlatCheck();
    }

    // #1. 추적을 제외한 랜덤 패턴 실행
    IEnumerator Think()
    {
        yield return new WaitForSeconds(1f);
        int pattern = Random.Range(0, 4); // 0, 1 : 대기, 2 : 왼쪽 이동, 3 : 오른쪽 이동

        if (pattern < 2)
            StartCoroutine("Think");
        else
        {
            Vector2 direction = pattern == 2 ? Vector2.left : Vector2.right;
            StartCoroutine("Patrol", direction);
        }
    }

    // #2. 감지시 추적 실행
    IEnumerator Chase()
    {
        onChase = true;
        animator.SetBool("onChase", true);
        float chaseTime = moveTime;

        while (!onDie && chaseTime > 0 && onDetect && platCheck)
        {
            Debug.DrawRay(rigid.position, Vector2.left * (-sightDir) * 3f, Color.yellow);
            meleeCheck = Physics2D.Raycast(rigid.position, Vector2.left * (-sightDir), 3f, LayerMask.GetMask("Player"));

            if (meleeCheck)
                yield return StartCoroutine("Attack"); // 감지중 플레이어와의 거리가 짧을시 공격 명령

            chaseTime -= Time.deltaTime;
            rigid.velocity = (transform.localScale.x == 1 ? Vector2.left : Vector2.right) * 2.2f;
            yield return null;
        }

        onChase = false;
        animator.SetBool("onChase", false);
        rigid.velocity = Vector2.zero;

        yield return new WaitForSeconds(patternDelay);
        StartCoroutine("Think");
    }

    // #3. 공격 실행
    IEnumerator Attack()
    {
        StopCoroutine("Chase");
        animator.SetTrigger("doAttack");
        rigid.AddForce(Vector2.left * (-sightDir) * 100, ForceMode2D.Impulse);
        attackBox.SetActive(true);

        yield return new WaitForSeconds(0.5f);
        animator.SetBool("onMove", false);
        animator.SetBool("onChase", false);
        attackBox.SetActive(false);
        rigid.velocity = Vector2.zero;

        yield return new WaitForSeconds(atkDelay);
        onChase = false;
        StartCoroutine("Think");
    }

    // #4. 탐지를 못할시에 주위 순찰
    IEnumerator Patrol(Vector2 dir)
    {
        animator.SetBool("onMove", true);

        if (dir == Vector2.left)
        {
            transform.localScale = new Vector3(1, 1, 1);
            sightDir = -1;
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1);
            sightDir = 1;
        }

        float moveT = moveTime;

        while (moveT > 0 && platCheck)
        {
            moveT -= Time.deltaTime;
            rigid.velocity = dir * 1f;

            yield return null;
        }

        animator.SetBool("onMove", false);
        rigid.velocity = Vector2.zero;

        yield return new WaitForSeconds(patternDelay);
        StartCoroutine("Think");
    }

    // #5. 피격 처리
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
        else
            BackCheck();

        yield return new WaitForSeconds(2f);
        onDamage = false;
    }

    // #6. 죽음 처리
    public IEnumerator DieProcess()
    {
        StopCoroutine("Patrol");
        StopCoroutine("Attack");
        StopCoroutine("Chase");
        attackBox.SetActive(false);
        rigid.velocity = Vector2.zero;
        animator.enabled = false;
        collider.isTrigger = true;
        spriteRenderer.color = new Color(1, 1, 1, 0.3f);

        yield return new WaitForSeconds(2.0f);
        gameObject.SetActive(false);
    }

    // #7. 바닥을 감지하여 떨어지는 것을 방지
    void PlatCheck()
    {
        Debug.DrawRay(rigid.position + Vector2.left * (-sightDir) * 1f, Vector2.down * 0.6f, Color.blue);
        platCheck = Physics2D.Raycast(rigid.position + Vector2.left * (-sightDir) * 1f, Vector3.down, 0.6f, LayerMask.GetMask("Platform"));
    }

    // #8. 후방 확인
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
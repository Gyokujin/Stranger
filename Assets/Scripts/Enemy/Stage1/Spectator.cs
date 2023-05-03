using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spectator : Enemy
{
    [Header("Action")]
    [SerializeField]
    private Transform backSight;
    private bool onChase;
    [SerializeField]
    private Sprite standPose;
    private int lifeCount = 1;

    [Header("Attack")]
    [SerializeField]
    private int damage;
    [SerializeField]
    private float grabDelay;
    [SerializeField]
    private Sprite grabPose;
    [SerializeField]
    private GameObject grabBox;
    [SerializeField]
    private GameObject spectatorArm;

    [Header("RayCheck")]
    private bool frontCheck;
    private RaycastHit2D meleeCheck;
    private RaycastHit2D backCheck;
    private RaycastHit2D platCheck;

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

    // #1. 이 몬스터는 플레이어 감지시 추적하는 패턴만 존재한다
    void Update()
    {
        if (onDie || onDamage)
            return;

        base.Detect();

        if (!onChase && !onAttack && platCheck)
        {
            if (frontCheck)
                StartCoroutine("Chase");
            else if (backCheck)
            {
                sightDir *= -1;
                transform.localScale = new Vector3(transform.localScale.x * -1, 1, 1);
                StartCoroutine("Chase");
            }
        }

        RayCheck();
    }

    // #2. 감지시 추적 실행
    IEnumerator Chase()
    {
        onChase = true;
        animator.enabled = true;

        while (!onDie && frontCheck && platCheck)
        {
            if (meleeCheck)
                yield return StartCoroutine("Grabbing");

            rigid.velocity = Vector2.left * (-sightDir) * 0.3f;
            yield return null;
        }

        onChase = false;
        rigid.velocity = Vector2.zero;
        animator.enabled = false;
        spriteRenderer.sprite = standPose;
    }

    // #3. 근접 거리에 있을시에 잡기 공격 시도
    IEnumerator Grabbing()
    {
        float delay = grabDelay;
        animator.enabled = false;

        while (meleeCheck && !onDie)
        {
            delay -= Time.deltaTime;
            yield return null;

            if (delay <= 0)
            {
                Transform target = Player.instance.transform;
                collider.isTrigger = true;
                rigid.gravityScale = 0;
                transform.position = new Vector2(target.position.x + sightDir * 0.05f, transform.position.y);
                spriteRenderer.sprite = grabPose;
                spectatorArm.SetActive(true);
                grabBox.SetActive(true);

                yield return new WaitForSeconds(0.1f);
                grabBox.SetActive(false);

                yield return new WaitForSeconds(2f);
                yield break;
            }
        }

        animator.enabled = true;
        spectatorArm.SetActive(false);
        collider.isTrigger = false;
        rigid.gravityScale = 1;
    }

    // #4. 피격 처리
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
            yield return StartCoroutine("DieProcess");

        yield return new WaitForSeconds(2f);
        onDamage = false;
    }

    // #5. 죽음 처리 및 부활
    public IEnumerator DieProcess()
    {
        StopCoroutine("Grabbing");
        onDie = true;
        onChase = false;
        onAttack = false;
        onDamage = false;
        rigid.velocity = Vector2.zero;
        rigid.gravityScale = 0;
        collider.isTrigger = true;
        spriteRenderer.color = new Color(1, 1, 1, 0.3f);

        yield return new WaitForSeconds(2.0f);

        if (lifeCount > 0)
        {
            lifeCount--;
            onDie = false;
            onDamage = false;
            rigid.gravityScale = 1;
            collider.isTrigger = false;
            animator.enabled = true;
            spriteRenderer.color = new Color(1, 1, 1, 1f);
        }
        else
            gameObject.SetActive(false);
    }

    void RayCheck()
    {
        // #. FrontCheck
        frontCheck = onDetect;

        // #. MeleeCheck
        Debug.DrawRay(transform.position, Vector2.left * (-sightDir) * 0.4f, Color.green);
        meleeCheck = Physics2D.Raycast(transform.position, Vector2.left * (-sightDir), 0.4f, LayerMask.GetMask("Player"));

        // #. BackCheck
        Debug.DrawRay(backSight.position, Vector2.left * sightDir * frontSight, Color.yellow);
        backCheck = Physics2D.Raycast(backSight.position, Vector2.left * sightDir, frontSight, LayerMask.GetMask("Player"));

        // #. PlatCheck
        Debug.DrawRay(rigid.position + Vector2.left * (-sightDir) * 0.2f, Vector2.down * 0.8f, Color.blue);
        platCheck = Physics2D.Raycast(rigid.position + Vector2.left * (-sightDir) * 0.2f, Vector2.down, 0.8f, LayerMask.GetMask("Platform"));
    }
}
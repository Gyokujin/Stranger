using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingDemon : Enemy
{
    [Header("Pattern")]
    [SerializeField]
    private float chaseSpeed;
    private int sightDir;
    [SerializeField]
    private float chaseTime = 2f;
    [SerializeField]
    private float patternDelayMin = 2f;
    [SerializeField]
    private float patternDelayMax = 4f;
    [SerializeField]
    private float chaseOffsetY = 2.2f;
    [SerializeField]
    private GameObject teleportEffect;
    [SerializeField]
    private float teleportDelay = 1f;
    private bool onPattern;
    private Transform target;
    [SerializeField]
    private GameObject hitBox;

    [Header("Meteor")]
    [SerializeField]
    private GameObject meteorObject;
    [SerializeField]
    private float meteorCast = 2.5f;
    [SerializeField]
    private int meteorMinNum = 4;
    [SerializeField]
    private int meteorMaxNum = 8;
    [SerializeField]
    private float meteorSpeedMin;
    [SerializeField]
    private float meteorSpeedMax;

    [Header("Component")]
    private SpriteRenderer sprite;
    private Rigidbody2D rigid;
    private Animator animator;

    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        sightDir = 1;
        target = Player.instance.transform;
        Invoke("Think", 5f);
    }

    void Think()
    {
        if (onPattern)
            return;

        float distanceX = target.transform.position.x - transform.position.x;
        float distanceY = target.transform.position.y + chaseOffsetY - transform.position.y;

        if (distanceX >= 0)
        {
            sightDir = 1;
            transform.localScale = new Vector3(1.3f, 1.3f, 1f);
        }
        else
        {
            sightDir = -1;
            transform.localScale = new Vector3(-1.3f, 1.3f, 1f);
        }

        if (Mathf.Abs(distanceX) > 5f || Mathf.Abs(distanceY) > 1f) // 거리가 멀때는 추적
            StartCoroutine("Chase", sightDir);
        else // 거리가 가까울때는 공격 시전
        {
            int pattern = Random.Range(0, 4); // 0 = 메테오, 1, 2, 3 = 브레스
            animator.SetBool("onAttack", true);

            switch (pattern)
            {
                case 0:
                    StartCoroutine("Meteor");
                    break;
                default:
                    StartCoroutine("Breath");
                    break;
            }
        }
    }

    IEnumerator Chase(int dir)
    {
        onPattern = true;
        float moveTime = chaseTime;

        while (moveTime > 0)
        {
            moveTime -= Time.deltaTime;
            Vector2 moveDir = new Vector2(target.position.x - transform.position.x, target.position.y + chaseOffsetY - transform.position.y).normalized;
            rigid.velocity = moveDir * chaseSpeed;

            if (Mathf.Abs(target.transform.position.x - transform.position.x) < 5f && Mathf.Abs(target.transform.position.y + chaseOffsetY - transform.position.y) < 1f)
            {
                rigid.velocity = Vector2.zero;
                onPattern = false;
                Think();
                yield break;
            }

            yield return null;
        }

        // 이동 시간이 지나도 거리가 멀면 텔레포트 시전
        Vector2 targetVec = target.position;
        rigid.velocity = Vector2.zero;
        yield return StartCoroutine("Teleport", targetVec);
        onPattern = false;
        Think();
    }

    IEnumerator Teleport(Vector2 vec)
    {
        Color color = sprite.color;
        color.a = 0;

        yield return new WaitForSeconds(0.5f);
        Vector2 point = new Vector2(vec.x, vec.y + chaseOffsetY);
        Instantiate(teleportEffect, point, Quaternion.identity);
        transform.position = point;
        float delay = teleportDelay;

        while (delay > 0)
        {
            delay -= Time.deltaTime;
            sprite.color = new Color(1, 1, 1, 1 - delay);
            yield return null;
        }
    }

    IEnumerator Breath()
    {
        onPattern = true;
        yield return new WaitForSeconds(0.5f);
        animator.SetTrigger("attack_Breath");
        int type = Random.Range(0, 2); // 0 = 전진하면서 브레스, 1 = 좌우 반복이동하며 브레스
        float attackDir = sightDir > 0 ? 3.5f : -3.5f;
        float currentPos = transform.position.x;
        float attackTime = Random.Range(3f, 5f);

        switch (type)
        {
            case 0:
                while (attackTime > 0)
                {
                    currentPos += Time.deltaTime * attackDir;
                    transform.position = new Vector2(currentPos, transform.position.y); ;
                    attackTime -= Time.deltaTime;

                    yield return null;
                }
                break;

            case 1:
                float rightMax = currentPos + 3;
                float leftMax = currentPos - 3;

                while (attackTime > 0)
                {
                    currentPos += Time.deltaTime * attackDir;

                    if (currentPos >= rightMax)
                        attackDir *= -1;
                    else if (currentPos <= leftMax)
                        attackDir *= -1;

                    transform.position = new Vector2(currentPos, transform.position.y);
                    attackTime -= Time.deltaTime;

                    yield return null;
                }
                break;
        }

        hitBox.SetActive(false);
        onPattern = false;
        animator.SetBool("onAttack", false);

        yield return new WaitForSeconds(Random.Range(patternDelayMin, patternDelayMax));
        onPattern = false;
        Think();
    }

    IEnumerator Meteor()
    {
        onPattern = true;
        animator.SetTrigger("attack_Meteor");

        yield return new WaitForSeconds(meteorCast);
        int meteorCount = Random.Range(meteorMinNum, meteorMaxNum + 1); // 메테오 개수를 랜덤으로 뽑는다
        Vector2 castPos = transform.position;

        for (int i = 0; i < meteorCount; i++)
        {
            Vector2 meteorPos = new Vector2(Random.Range(castPos.x - 7.5f, castPos.x + 7.5f), Random.Range(castPos.y + 3f, castPos.y + 5f));
            GameObject meteor = Instantiate(meteorObject, meteorPos, Quaternion.identity);
            meteor.GetComponent<Rigidbody2D>().velocity = new Vector2(Random.Range(meteorSpeedMin, meteorSpeedMax) * sightDir, -Random.Range(meteorSpeedMin, meteorSpeedMax));
        }

        yield return new WaitForSeconds(meteorCast);
        onPattern = false;

        yield return new WaitForSeconds(Random.Range(patternDelayMin, patternDelayMax));
        animator.SetBool("onAttack", false);
        onPattern = false;
        Think();
    }

    public void OnHitBox()
    {
        hitBox.SetActive(true);
    }

    public void OffHitBox()
    {
        hitBox.SetActive(false);
    }
}
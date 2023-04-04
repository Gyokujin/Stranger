using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gaiten : Enemy
{
    [Header("Action")]
    private float moveTiem = 1.5f;
    private float waitTime = 4f;
    private float curWaitTime;
    private bool onMove;

    [Header("RayCheck")]
    [SerializeField]
    private Transform sight;
    private RaycastHit2D detectCheck;
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

    void Start()
    {
        curWaitTime = waitTime;
        StartCoroutine("Think");
    }

    void Update()
    {
        BlinkReady();
        RayCheck();
    }

    // #1. 공격거리에 플레이어가 없을시 공격을 제외한 랜덤 패턴 실행
    IEnumerator Think()
    {
        yield return new WaitForSeconds(1f);
        int pattern = Random.Range(0, 4); // 0,1,2 : 대기, 3 : 이동

        if (pattern < 3)
            StartCoroutine("Think");
        else
            StartCoroutine("Move");
    }

    // #2. 패턴1: 주위 이동
    IEnumerator Move()
    {
        onMove = true;
        float time = moveTiem;
        int dir = Random.Range(0, 2);
        Vector2 direction = dir == 0 ? Vector2.left : Vector2.right;
        animator.SetBool("onMove", true);

        while (time > 0)
        {
            time -= Time.deltaTime;
            yield return null;
        }

        onMove = false;
        curWaitTime = waitTime;
        animator.SetBool("onMove", false);
        StartCoroutine("Think");
    }

    void BlinkReady()
    {
        if (onDetect)
        {
            curWaitTime -= Time.deltaTime;

            if (curWaitTime <= 0)
            {
                StopAllCoroutines();
                // StopCoroutine("Think");
                // StopCoroutine("Move");
                StartCoroutine("Blink");
            }
        }
    }

    // #3. 패턴2 : 점멸 이동
    IEnumerator Blink()
    {
        Debug.Log("점멸 이동");
        onMove = true;
        int pos = Random.Range(0, 2); // 0 : 플레이어 왼쪽, 1 : 플레이어 오른쪽
        Vector2 targetPos = new Vector2(Player.instance.transform.position.x + (pos == 0 ? -5 : 5), transform.position.y);
        
        yield return new WaitForSeconds(2f);
        animator.SetTrigger("doBlink");

        yield return new WaitForSeconds(1f);
        transform.position = targetPos;

        yield return new WaitForSeconds(5f);
        onMove = false;
        curWaitTime = waitTime;
        StartCoroutine("Think");
    }

    void RayCheck()
    {
        // #. FrontDetect
        Debug.DrawRay(sight.position, Vector2.right * sightDir * frontSight, Color.red);
        detectCheck = Physics2D.Raycast(sight.position, Vector2.right * sightDir, frontSight, LayerMask.GetMask("Player"));
        
        if (detectCheck)
            onDetect = true;

        // #. MeleeCheck
        Debug.DrawRay(transform.position, Vector2.left * (-sightDir) * 1.4f, Color.blue);
        meleeCheck = Physics2D.Raycast(transform.position, Vector2.left * (-sightDir), 1.4f, LayerMask.GetMask("Player"));
    }
}
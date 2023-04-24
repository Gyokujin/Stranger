using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gaiten : MonoBehaviour
{
    [Header("Status")]
    public int atk;
    [SerializeField]
    private int maxHP;
    private int hp;

    [Header("Action")]
    // #1. 이동
    public Vector2 moveDir = Vector2.left;
    [SerializeField]
    private float moveTime;
    [SerializeField]
    private float moveSpeed;
    // #2. 피격
    private bool onDamage;
    private bool onDie;
    [SerializeField]
    private float hitTime;
    // #3. 순간이동
    [SerializeField]
    private float blinkTime;
    // #4. 딜레이
    [SerializeField]
    private float patternDelay = 0.7f;
    private WaitForSeconds patternWait;

    [Header("Detect")]
    [SerializeField]
    private Transform sight;
    [SerializeField]
    private float detectRange;
    [SerializeField]
    private float attackRange;
    [SerializeField]
    private float blinkRange;
    private RaycastHit2D meleeDetect;
    private RaycastHit2D leftDetect;
    private RaycastHit2D rightDetect;

    [Header("Attack")]
    // 어택 박스
    [SerializeField]
    private GameObject attackBox;
    public bool onAttack;
    // #1. 대시 공격
    [SerializeField]
    private float dashTime;
    [SerializeField]
    private float dashSpeed;
    [SerializeField]
    private float dashSight;
    // #2. 점프 공격
    [SerializeField]
    private float jumpTime;
    [SerializeField]
    private float jumpDelay;
    [SerializeField]
    private float jumpAttackTime;
    [SerializeField]
    private float jumpFallTime;
    [SerializeField]
    private float jumpAttackDelay;
    [SerializeField]
    private float jumpForceX;
    [SerializeField]
    private float jumpForceY;
    [SerializeField]
    private float jumpAttackForce;
    // #3. 앉아 공격
    [SerializeField]
    private float crouchTime;
    [SerializeField]
    private float crouchMoveSpeed;
    [SerializeField]
    private float crouchSight;
    [SerializeField]
    private float crouchAttackDelay;
    // #4. 염주 발사
    [SerializeField]
    private GameObject rosary;
    [SerializeField]
    private float shotDelay;
    // #5. 딜레이
    [SerializeField]
    private float attackDelay;
    private WaitForSeconds attackWait;

    [Header("Collider")]
    private Vector2 idleColOff = new Vector2(-0.08087763f, -0.2648114f); // 평소 Collider2D의 Offset
    private Vector2 idleColSize = new Vector2(0.4757214f, 1.345377f); // 평소 Collider2D의 Size
    private Vector2 jAColOff = new Vector2(0.2912853f, 0.01643795f); // 점프 공격 상태에서의 Collider2D의 Offset
    private Vector2 jAleColSize = new Vector2(1.094467f, 0.7854486f); // 점프 공격 상태에서의 Collider2D의 Size
    private Vector2 crouchColOff = new Vector2(-0.2080189f, -0.4854448f); // 앉은 상태에서의 Collider2D의 Offset
    private Vector2 crouchColSize = new Vector2(0.5657727f, 0.9041105f); // 앉은 상태에서의 Collider2D의 Size

    [Header("Component")]
    private SpriteRenderer sprite;
    private Rigidbody2D rigid;
    private BoxCollider2D collider;
    private Animator animator;

    void Awake()
    {
        patternWait = new WaitForSeconds(patternDelay);
        attackWait = new WaitForSeconds(attackDelay);
        sprite = GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        hp = maxHP;
        collider.offset = idleColOff;
        collider.size = idleColSize;
        Think();
    }

    // #1. 탐지 결과를 바탕으로 대기 또는 이동
    void Think()
    {
        if (onDamage)
            return;

        moveDir = Detect();

        if (moveDir == Vector2.zero)
        {
            Invoke("Think", patternDelay);
        }
        else
        {
            int pattern = 0;

            if (hp <= 25)
                pattern = Random.Range(0, 4); // 3이 나올경우 염주발사
            else
                pattern = Random.Range(0, 3);

            switch (pattern)
            {
                case 0:
                case 1:
                case 2:
                    StartCoroutine("Move");
                    break;
                case 3:
                    StartCoroutine("RosarySpawn");
                    break;
            }
        }
    }

    // #2. 탐지 명령
    Vector2 Detect()
    {
        Vector2 detect = Vector2.left;

        // #. PlayerScan (좌)
        Debug.DrawRay(sight.position, Vector2.left * detectRange, Color.yellow);
        leftDetect = Physics2D.Raycast(sight.position, Vector2.left, detectRange, LayerMask.GetMask("Player"));

        // #. PlayerScan (우)
        Debug.DrawRay(sight.position, Vector2.right * detectRange, Color.yellow);
        rightDetect = Physics2D.Raycast(sight.position, Vector2.right, detectRange, LayerMask.GetMask("Player"));

        if (leftDetect)
        {
            detect = Vector2.left;
            transform.localScale = new Vector3(-0.9f, 0.9f, 1);
        }
        else if (rightDetect)
        {
            detect = Vector2.right;
            transform.localScale = new Vector3(0.9f, 0.9f, 1);
        }
        else
            detect = Vector2.zero;

        return detect;
    }

    // #3. 이동 명령
    IEnumerator Move()
    {
        float moveT = moveTime;
        animator.SetBool("onMove", true);

        while (moveT > 0)
        {
            moveT -= Time.deltaTime;
            rigid.velocity = moveDir * moveSpeed;

            // #. 이동중 Physics2D로 플레이어 탐지
            Debug.DrawRay(sight.position, moveDir * attackRange, Color.red);
            meleeDetect = Physics2D.Raycast(sight.position, moveDir, attackRange, LayerMask.GetMask("Player"));
            
            // #. 사정거리에 접근시 공격 명령
            if (meleeDetect)
                AttackReady();

            yield return null;
        }

        rigid.velocity = Vector2.zero;

        // #. 이동이 끝난후 Blink를 위한 Physics2D로 플레이어 탐지 (플레이어가 반대쪽에 있을시에 추격한다)
        Debug.DrawRay(sight.position, moveDir * blinkRange, Color.yellow);
        RaycastHit2D blinkDetect = Physics2D.Raycast(sight.position, moveDir, blinkRange, LayerMask.GetMask("Player"));
        
        // #. 탐지시에 플레이어에게 순간이동
        if (blinkDetect)
        {
            float target = blinkDetect.collider.transform.position.x;
            StartCoroutine("Blink", target);
        }
        // #. 찾지 못했을 경우 Think 실행
        else
        {
            animator.SetBool("onMove", false);
            yield return patternWait;
            Think();
        }
    }

    // #4. 공격 준비
    void AttackReady()
    {
        StopCoroutine("Move");
        rigid.velocity = Vector2.zero;
        animator.SetBool("onMove", false);
        animator.SetBool("onAttack", true);

        onAttack = true;
        int attackType = 0;
        if (transform.position.x < -16 || transform.position.x > 22)
            attackType = Random.Range(0, 8);
        else
            attackType = Random.Range(0, 10);

        animator.SetBool("onAttack", true);
        GaitenAttackBox attack = attackBox.GetComponent<GaitenAttackBox>();

        switch (attackType)
        {
            // #. 정권 지르기
            case 0:
            case 1:
            case 2:
                attack.ChangeCollider(0);
                StartCoroutine("PistAttack");
                break;
            // #. 내려차기
            case 3:
            case 4:
            case 5:
            case 6:
                attack.ChangeCollider(1);
                StartCoroutine("KickAttack");
                break;
            // #. 앉아 발차기
            case 7:
                attack.ChangeCollider(2);
                StartCoroutine("CrouchAttack");
                break;
            // #. 공중 발차기
            case 8:
            case 9:
                attack.ChangeCollider(3);
                StartCoroutine("JumpAttack");
                break;
        }
    }
    
    // #5. 정권 지르기
    IEnumerator PistAttack()
    {
        animator.SetTrigger("doPistAttack");

        yield return attackWait;
        animator.SetBool("onAttack", false);

        yield return patternWait;
        Think();
    }

    // #6. 내려차기
    IEnumerator KickAttack()
    {
        animator.SetTrigger("doDash");
        float time = dashTime;
        RaycastHit2D dashDetect;

        // #. 공격 유효거리 확보를 위해 플레이어에게 대시
        while (time > 0 && !onDamage)
        {
            time -= Time.deltaTime;
            rigid.velocity = moveDir * dashSpeed;

            // #. 대시중 Physics2D로 플레이어 탐지
            Debug.DrawRay(sight.position, moveDir * dashSight, Color.red);
            dashDetect = Physics2D.Raycast(sight.position, moveDir, dashSight, LayerMask.GetMask("Player"));
            
            if (dashDetect || transform.position.x < -16 || transform.position.x > 22)
                break;

            yield return null;
        }

        rigid.velocity = Vector2.zero;
        animator.SetTrigger("doKickAttack");

        yield return attackWait;
        animator.SetBool("onAttack", false);

        yield return patternWait;
        Think();
    }

    // #7. 공중 발차기
    IEnumerator JumpAttack()
    {
        animator.SetTrigger("doJump");
        float time = jumpTime;
        int curAtk = atk;
        collider.isTrigger = true;

        // #. 공격전 도약
        while (time > 0)
        {
            time -= Time.deltaTime;
            rigid.velocity = moveDir * jumpForceX + Vector2.up * jumpForceY;
            yield return null;
        }

        rigid.velocity = Vector2.zero;
        rigid.gravityScale = 0;
        Vector2 dir = Detect();
        if (dir != Vector2.zero)
            moveDir = dir;

        // #. 공격 시전
        yield return new WaitForSeconds(jumpDelay);
        animator.SetTrigger("doJumpAttack");
        atk *= 2;
        collider.offset = jAColOff;
        collider.size = jAleColSize;
        OnAttackBox();
        time = jumpAttackTime;
        
        while (time > 0 && !onDamage)
        {
            time -= Time.deltaTime;
            rigid.velocity = moveDir * jumpAttackForce;
            
            if (transform.position.x < -16 || transform.position.x > 22)
                break;

            yield return null;
        }

        rigid.velocity = Vector2.zero;

        // #. 공격 판정이 끝난 후 낙하
        yield return new WaitForSeconds(jumpAttackTime);
        atk = curAtk;
        OffAttackBox();
        collider.isTrigger = false;
        rigid.velocity = Vector2.zero;
        rigid.gravityScale = 1;

        // #. 바닥에 닿을때 착지
        yield return new WaitForSeconds(jumpFallTime);
        collider.offset = idleColOff;
        collider.size = idleColSize;
        animator.SetBool("onCrouch", true);
        animator.SetBool("onAttack", false);

        yield return new WaitForSeconds(jumpAttackDelay);
        animator.SetBool("onCrouch", false);

        yield return patternWait;
        Think();
    }

    // #8. 앉아 발차기
    IEnumerator CrouchAttack()
    {
        animator.SetBool("onCrouch", true);
        collider.offset = crouchColOff;
        collider.size = crouchColSize;
        float time = crouchTime;
        RaycastHit2D crouchDetect;

        // #. 공격 유효거리 확보를 위해 플레이어에 짧게 이동
        while (time > 0 && !onDamage)
        {
            time -= Time.deltaTime;
            rigid.velocity = moveDir * crouchMoveSpeed;

            // #. 이동중  Physics2D로 플레이어 탐지
            Debug.DrawRay(sight.position, moveDir * crouchSight, Color.red);
            crouchDetect = Physics2D.Raycast(sight.position, moveDir, crouchSight, LayerMask.GetMask("Player"));

            if (crouchDetect || transform.position.x < -16 || transform.position.x > 22)
                break;

            yield return null;
        }

        rigid.velocity = Vector2.zero;

        // #. 공격 시전
        yield return new WaitForSeconds(crouchAttackDelay);
        animator.SetTrigger("onCrouchAttack");

        yield return attackWait;
        animator.SetBool("onAttack", false);

        yield return patternWait;
        animator.SetBool("onCrouch", false);
        collider.offset = idleColOff;
        collider.size = idleColSize;
        Think();
    }

    // #9. 공격 판정 활성화
    public void OnAttackBox()
    {
        attackBox.GetComponent<BoxCollider2D>().enabled = true;
    }

    // #10. 공격 판정 비활성화
    public void OffAttackBox()
    {
        onAttack = false;
        attackBox.GetComponent<BoxCollider2D>().enabled = false;
    }

    // #11. 염주 발사 (2페이즈 추가 패턴)
    IEnumerator RosarySpawn()
    {
        GameObject spawnRosary = Instantiate(rosary, new Vector2(transform.position.x + moveDir.x * 0.75f, transform.position.y), Quaternion.identity);
        spawnRosary.SetActive(true);

        yield return attackWait;
        Vector2 shotDir = Detect();

        if (shotDir == Vector2.zero)
            shotDir = moveDir;
        
        spawnRosary.GetComponent<Rosary>().BulletSpawn(shotDir);

        yield return new WaitForSeconds(shotDelay);
        yield return patternWait;
        Think();
    }

    // #12. 순간 이동
    IEnumerator Blink(float dest)
    {
        int dir = Random.Range(0, 2); // 0 : 플레이어 왼쪽, 1 : 플레이어 오른쪽
        Vector3 blinkPos = new Vector3(dest + (dir == 0 ? -1.25f : 1.25f), transform.position.y, 1);
        animator.enabled = false;

        yield return new WaitForSeconds(0.5f);
        Color color = sprite.color;
        color.a = 1;
        float delay = blinkTime;

        // #. 가이텐을 사라지게 한다
        while (delay > 0)
        {
            delay -= Time.deltaTime;
            sprite.color = new Color(1, 1, 1, delay);
            yield return null;
        }

        transform.localScale = new Vector3(dir == 0 ? 0.9f : -0.9f, 0.9f, 1);

        yield return new WaitForSeconds(0.1f);
        animator.enabled = true;
        transform.position = blinkPos;
        delay = blinkTime;

        // #. 가이텐을 다시 등장시킨다
        while (delay > 0)
        {
            delay -= Time.deltaTime;
            sprite.color = new Color(1, 1, 1, 1 - delay);
            yield return null;
        }

        Think();
    }

    // #13. 피격 처리
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (onDie)
            return;

        if (collision.gameObject.CompareTag("PlayerHitBox"))
        {
            int damage = collision.gameObject.GetComponentInParent<Player>().playerATK;
            hp -= damage;
            Vector2 dir = Vector2.zero;
            dir = collision.gameObject.transform.position.x > transform.position.x ? Vector2.left : Vector2.right;
            StartCoroutine("HitProcess", dir);
        }
    }

    IEnumerator HitProcess(Vector2 hitDir)
    {
        if (hp <= 0)
        {
            onDie = true;
            StartCoroutine("DieProcess");
        }

        if (!onAttack)
        {
            onDamage = true;
            rigid.AddForce(hitDir * 1.5f, ForceMode2D.Impulse);
            animator.SetBool("onHit", true);

            yield return new WaitForSeconds(hitTime);
            animator.SetBool("onHit", false);
            onDamage = false;
            Think();
        }
    }

    // #14. 죽음 처리
    IEnumerator DieProcess()
    {
        StopCoroutine("HitProcess");
        PatternStop();
        animator.SetBool("onHit", true);

        yield return new WaitForSeconds(5f);
    }

    void PatternStop()
    {
        rigid.velocity = Vector2.zero;
        animator.SetBool("onMove", false);
        animator.SetBool("onAttack", false);
        animator.SetBool("onGround", true);
        animator.SetBool("onCrouch", false);
        animator.ResetTrigger("doJump");
        animator.ResetTrigger("doDash");
        animator.ResetTrigger("doPistAttack");
        animator.ResetTrigger("doKickAttack");
        animator.ResetTrigger("doJumpAttack");
        animator.ResetTrigger("doCrouchAttack");
        StopCoroutine("Move");
        StopCoroutine("PistAttack");
        StopCoroutine("KickAttack");
        StopCoroutine("JumpAttack");
        StopCoroutine("CrouchAttack");
    }
}
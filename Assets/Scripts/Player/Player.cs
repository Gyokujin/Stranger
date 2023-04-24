using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance = null;

    [Header("Status")]
    public int maxHP;
    public int playerHP;
    [Range(0, 10)]
    public int playerATK;
    [SerializeField][Range(0, 10)]
    private float runSpeed = 3;
    private float defaultSpeed;
    [SerializeField][Range(0, 12)]
    private float jumpForce;

    [Header("State")]
    [HideInInspector]
    public bool onMove;
    private bool onGround;
    private bool onCrouch;
    [HideInInspector]
    public bool onDash;
    private bool isWall;
    private bool isWallJump;
    private bool onAttack;
    private bool onDamaged;
    private bool onDie;

    [Header("Move")]
    [SerializeField]
    private int jumpCount;
    private int jumpCnt;
    [SerializeField]
    private float checkDistance;
    private float inputX;
    private float inputY;
    [HideInInspector]
    public float isRight = 1; // 바라보는 방향 1 = 오른쪽, -1 = 왼쪽
    [SerializeField]
    private float slidingSpeed;
    [SerializeField]
    private float wallJumpPower;

    [Header("Action")]
    [SerializeField]
    private float dashSpeed;
    [SerializeField]
    private float defaultDashTime;
    private float dashTime;
    private bool dashCool;
    [SerializeField]
    private float dashCooldown;
    public GameObject targetObject;

    [Header("Attack")]
    [SerializeField]
    private int atkCount;
    private int atkCnt;
    [SerializeField]
    private GameObject attackBox;

    [Header("Hit")]
    [SerializeField]
    private float crouchHitPos; // 앉은 상태에서의 HitBox Y좌표
    private float invincibilityTime = 0;
    [SerializeField]
    private GameObject hitBox;
    [SerializeField]
    private Sprite hitPose;

    [Header("Physics")]
    [SerializeField]
    private Transform groundCheckFront; // 바닥 체크 position
    [SerializeField]
    private Transform groundCheckBack; // 바닥 체크 position
    private float standColOffsetY = -0.1564108f;
    private float standColSizeY = 1.030928f;
    private float crouchColOffsetY = -0.4838978f;
    private float crouchColSizeY = 0.3759539f;
    [SerializeField]
    private Transform wallCheck;
    [SerializeField]
    private float wallCheckDistance;
    [SerializeField]
    private LayerMask groundLayer;
    [SerializeField]
    private LayerMask wallLayer;

    [Header("Component")]
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rigid;
    private BoxCollider2D collider;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (instance != this)
                Destroy(this.gameObject);
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        defaultSpeed = runSpeed;
        dashCool = true;
        jumpCnt = jumpCount;
        atkCnt = atkCount;
    }

    void Update()
    {
        GetInput();
        Crouch();
        Jump();
        Sliding();
        Interaction();
        Invincibilit();
    }

    void FixedUpdate()
    {
        Move();
        //LadderMove();
    }

    // #1. 조작을 위한 키 입력받기
    void GetInput()
    {
        inputX = Input.GetAxisRaw("Horizontal");
        inputY = Input.GetAxisRaw("Vertical");

        if (Input.GetButtonDown("Attack") && atkCount > 0 && !onMove && onGround && !onCrouch && !onDamaged && !onDie && !onDash)
        {
            Attack();
        }

        if (Input.GetButtonDown("Dash") && !onAttack && onGround && !onDamaged && !onDie && !onDash && dashCool)
        {
            StartCoroutine("Dash");
        }
    }

    // #2. 이동 명령
    void Move()
    {
        if (onCrouch || onAttack || onDamaged || onDie || isWallJump || onDash)
            return;

        // #. 캐릭터 이동
        rigid.velocity = new Vector2((inputX) * runSpeed, rigid.velocity.y);
        onMove = rigid.velocity.x != 0 ? true : false;

        // #. 캐릭터의 앞쪽과 뒤쪽의 바닥 체크를 진행
        bool groundFront = Physics2D.Raycast(groundCheckFront.position, Vector2.down, checkDistance, groundLayer);
        bool groundBack = Physics2D.Raycast(groundCheckBack.position, Vector2.down, checkDistance, groundLayer);

        // #. 점프 상태에서 앞 또는 뒤쪽에 바닥이 감지되면 바닥에 붙어서 이동하도록 변경
        if (!onGround && (groundFront || groundBack))
            rigid.velocity = new Vector2(rigid.velocity.x, 0);

        // #. 앞 또는 뒤쪽의 바닥이 감지되면 isGround를 참으로
        if (groundFront || groundBack)
        {
            onGround = true;
            jumpCnt = jumpCount;
        }
        else
            onGround = false;

        animator.SetBool("onGround", onGround);

        if (inputX != 0 && !isWallJump)
        {
            // #. 방향키가 눌리는 방향과 캐릭터가 바라보는 방향이 다르면 캐릭터의 방향을 전환
            if ((inputX > 0 && isRight < 0) || (inputX < 0 && isRight > 0))
            {
                FlipPlayer();
            }

            animator.SetBool("onMove", true);
        }
        else
        {
            animator.SetBool("onMove", false);
        }
    }

    // #. 방향 전환
    void FlipPlayer()
    {
        // #. 방향을 전환
        transform.eulerAngles = new Vector3(0, Mathf.Abs(transform.eulerAngles.y - 180), 0);
        isRight *= -1;
    }

    // #3. 앉기
    void Crouch()
    {
        if (onMove || !onGround || onAttack || onDamaged || onDie || isWall || onDash)
            return;

        // #. 캐릭터 숙이기
        onCrouch = inputY < 0 ? true : false;
        animator.SetBool("onCrouch", onCrouch);

        if (onCrouch)
        {
            hitBox.transform.localPosition = new Vector2(0, crouchHitPos);
            rigid.velocity = new Vector2(0, rigid.velocity.y);
            collider.offset = new Vector2(collider.offset.x, crouchColOffsetY);
            collider.size = new Vector2(collider.size.x, crouchColSizeY);
        }
        else
        {
            hitBox.transform.localPosition = Vector2.zero;
            collider.offset = new Vector2(collider.offset.x, standColOffsetY);
            collider.size = new Vector2(collider.size.x, standColSizeY);
        }
    }

    // #4. 점프
    void Jump()
    {
        if (Input.GetButtonDown("Jump") && jumpCnt > 0 && !onAttack && !onDamaged && !onDie && !onCrouch)
        {
            // #. 캐릭터 점프
            rigid.velocity = Vector2.up * jumpForce;
            animator.SetTrigger("doJump");
        }
        if (Input.GetButtonUp("Jump"))
        {
            jumpCnt--;
        }
    }

    // #5. 대시
    IEnumerator Dash()
    {
        dashCool = false;
        onDash = true;
        invincibilityTime += 0.7f;
        gameObject.layer = 9;
        animator.SetBool("onDash", true);
        dashTime = defaultDashTime;

        if (onMove && !onCrouch)
        {
            while (dashTime > 0 && inputX != 0)
            {
                rigid.velocity = new Vector2(inputX * dashSpeed, rigid.velocity.y);
                dashTime -= Time.deltaTime;
                yield return null;
            }

            if (dashTime > 0)
            {
                dashTime = 0;
                gameObject.layer = 3;
                animator.SetBool("onMove", false);
            }

            yield return new WaitForSeconds(dashTime);
            runSpeed = defaultSpeed;
            animator.SetBool("onDash", false);
        }
        else if (!onMove && onCrouch)
        {
            onCrouch = true;
            animator.SetBool("onCrouch", true);
            rigid.AddForce(Vector2.left * -isRight * 280);

            yield return new WaitForSeconds(dashTime);
            rigid.velocity = new Vector2(0, rigid.velocity.y);
            animator.SetBool("onDash", false);
        }
        else if (!onMove && !onCrouch)
        {
            while (dashTime > 0 && inputX == 0)
            {
                dashTime -= Time.deltaTime;
                rigid.velocity = new Vector2(dashSpeed * isRight, 0);
                animator.SetBool("onMove", true);
                animator.SetBool("onDash", true);
                yield return null;
            }

            dashTime = 0;
            rigid.velocity = new Vector2(rigid.velocity.x, rigid.velocity.y);
            animator.SetBool("onMove", false);
            animator.SetBool("onDash", false);
        }
        onDash = false;
        gameObject.layer = 3;

        yield return new WaitForSeconds(dashCooldown);
        dashCool = true;
    }

    // #6. 벽 타기
    void Sliding()
    {
        if (!onGround)
        {
            isWall = Physics2D.Raycast(wallCheck.position, Vector2.right * isRight, wallCheckDistance, wallLayer);
            animator.SetBool("onSliding", isWall);

            if (isWall)
            {
                rigid.velocity = new Vector2(rigid.velocity.x, rigid.velocity.y * slidingSpeed);
                isWallJump = false;

                if (Input.GetButtonDown("Jump"))
                {
                    isWallJump = true;
                    animator.SetTrigger("doJump");
                    Invoke("FreezeX", 0.3f);
                    rigid.velocity = new Vector2(-isRight * wallJumpPower, 0.9f * wallJumpPower);
                    FlipPlayer();
                }
            }
        }
        else
        {
            animator.SetBool("onSliding", false);
        }
    }

    void FreezeX()
    {
        isWallJump = false;
    }

    // #7. 상호작용
    void Interaction()
    {
        if (Input.GetButtonDown("Interaction") && targetObject != null)
        {
            switch (targetObject.GetComponent<Object>().objectType.ToString())
            {
                case "Gate":
                    targetObject.GetComponent<Gate>().UseGate();
                    break;

                case "VillageGate":
                    targetObject.GetComponent<VillageGate>().UseVillageGate();
                    break;

                case "PortalRing":
                    Vector2 destination = targetObject.GetComponent<Portal>().targetPortal;
                    Vector2 offset = targetObject.GetComponent<Portal>().offsetBackground;
                    StartCoroutine(GameManager.instance.Teleport());
                    break;

                case "TreasureBox":
                    targetObject.GetComponent<TreasureBox>().Spawn();
                    break;
                /*
                case "Ladder":
                    transform.position = new Vector2(targetObject.transform.position.x, transform.position.y);     
                    onLadder = true;
                    break;
                */
            }
        }
    }

    // #8. 공격 명령
    void Attack()
    {
        onAttack = true;
        atkCnt--;
        animator.SetTrigger("doAttack");
    }

    // #. 어택 박스 활성화
    public IEnumerator OnAttackBox()
    {
        attackBox.SetActive(true);

        yield return new WaitForSeconds(0.1f);
        attackBox.SetActive(false);
    }

    // #. 공격 상태 비활성화
    void OffAttack()
    {
        onAttack = false;
        atkCnt = atkCount;
    }

    // #9. 피격 명령
    public void OnDamaged(Vector2 targetPos, int damage)
    {
        if (onDamaged || onDie)
            return;

        playerHP -= damage;
        rigid.velocity = Vector2.zero;

        if (playerHP <= 0)
            Die();
        else
        {
            onDamaged = true;
            GameManager.instance.HPSetting("Damage");
            invincibilityTime += 2f;

            // #. 레이어 변경 (Invincibility)
            gameObject.layer = 9;

            // #. 컬러 변경
            spriteRenderer.color = new Color(1, 1, 1, 0.6f);

            // #. 넉백
            int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1;
            rigid.AddForce(new Vector2(dirc, 0) * 2, ForceMode2D.Impulse);
            animator.SetBool("onCrouch", false);
            animator.SetTrigger("doDamaged");

            // #. 피격 해제 실행
            StartCoroutine("OffDamaged");
        }
    }

    // #. 잡기 피격 명령
    public IEnumerator Holding(int damage, Vector2 knockback)
    {
        if (!onDamaged)
        {
            onDamaged = true;
            animator.enabled = false;
            spriteRenderer.sprite = hitPose;

            yield return new WaitForSeconds(2f);
            playerHP -= damage;
            GameManager.instance.HPSetting("Damage");

            // #. 넉백
            collider.isTrigger = true;
            rigid.gravityScale = 0;
            animator.enabled = true;
            animator.SetTrigger("doDamaged");
            rigid.AddForce(knockback * 2, ForceMode2D.Impulse);

            yield return new WaitForSeconds(0.5f);
            collider.isTrigger = false;
            rigid.gravityScale = 1.6f;
            onDamaged = false;
        }
    }

    // #. 데미지 처리후 무적상태
    void Invincibilit()
    {
        float time = Mathf.Clamp(invincibilityTime, 0, 2);

        if (time > 0)
        {
            invincibilityTime -= Time.deltaTime;
            hitBox.SetActive(false);
        }
        else
        {
            hitBox.SetActive(true);
        }
    }

    // #. 피격 상태 비활성화
    IEnumerator OffDamaged()
    {
        yield return new WaitForSeconds(0.5f);
        onDamaged = false;

        yield return new WaitForSeconds(2f);
        gameObject.layer = 3;
        spriteRenderer.color = new Color(1, 1, 1, 1);

        // 공격중 피격 버그에 대한 버그 방지
        if (onAttack)
            OffAttack();
    }

    // #10. 플레이어 죽음 명령
    void Die()
    {
        onDie = true;
        gameObject.layer = 9;
        animator.SetTrigger("doDie");

        StartCoroutine(GameManager.instance.GameOver());
    }

    // #11. 플레이어 부활
    public void Resurrection()
    {
        onDie = false;
        gameObject.layer = 3;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Object"))
            targetObject = collision.gameObject;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Object"))
            targetObject = null;
    }

    // #. 바닥 체크 Ray를 씬화면에 표시
    void OnDrawGizmos()
    {
        if (onGround)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(groundCheckFront.position, Vector2.down * checkDistance);
            Gizmos.DrawRay(groundCheckBack.position, Vector2.down * checkDistance);
        }
        else if (isWall)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(wallCheck.position, Vector2.right * isRight * wallCheckDistance);
        }
    }

    /*
    void LadderMove()
    {
        if (onLadder)
        {
            gameObject.layer = 11;
            onGround = false;
            animator.SetBool("ladderMove", true);
            animator.SetBool("onMove", inputY != 0 ? true : false);
            rigid.gravityScale = 0;
            bool ladderCheck = false;

            if (inputY != 0)
            {
                animator.speed = 1f;
                rigid.velocity = new Vector2(0, inputY * defaultSpeed * 0.4f);

                switch (inputY)
                {
                    case 1:
                        ladderCheck = Physics2D.Raycast(ladderCheckTop.position, Vector2.up, checkDistance * 0.6f, groundLayer);
                        break;
                    case -1:
                        ladderCheck = Physics2D.Raycast(ladderCheckBot.position, Vector2.down, checkDistance * 0.6f, groundLayer);
                        break;
                }

                if (ladderCheck)
                {
                    gameObject.layer = 3;
                    rigid.gravityScale = 1.6f;
                    float moveDir = inputY == 1 ? 0.7f : 0.2f;
                    transform.position = new Vector2(transform.position.x, transform.position.y + moveDir);

                    Debug.Log("바닥감지");
                    animator.SetBool("ladderMove", false);
                    onGround = true;
                    onLadder = false;
                }
            }
            else
            {
                animator.speed = 0f;
                rigid.velocity = Vector2.zero;
            }
        }
    }
    */
}
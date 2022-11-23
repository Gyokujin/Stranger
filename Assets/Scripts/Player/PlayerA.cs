using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
public class Player : MonoBehaviour
{
    public static Player instance = null;

    [Range(0, 100)]
    public float playerHP;
    [Range(0, 10)]
    public float playerATK;

    public Transform wallCheck;
    public float wallCheckDistance;
    public LayerMask wallLayer;

    [SerializeField][Range(0, 25)]
    private float speed;
    [SerializeField][Range(10, 15)]
    private float jumpPower;
    public float slidingSpeed;
    public float wallJumpPower;

    public int comboCount;
    [SerializeField]
    private GameObject hitBox;

    private bool onGround;
    [SerializeField]
    private bool isWall;
    private bool onAttack;
    private bool onDamaged;
    private bool onDie;

    private Rigidbody2D rigid;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

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

        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (onDamaged || onDie || onAttack)
            return;

        // #. Stop Speed
        if (Input.GetButtonUp("Horizontal"))
        {
            rigid.velocity = new Vector2(0, rigid.velocity.y);
        }

        // #. Direction Sprite
        if (Input.GetButton("Horizontal"))
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;

        // #. Jump
        if (Input.GetButtonDown("Jump") && onGround)
        {
            onGround = false;
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            animator.SetTrigger("doJump");
        }

        // #. Attack
        if (Input.GetMouseButtonDown(0) && onGround && comboCount == 0)
        {
            onAttack = true;
            StartCoroutine("Attack");
        }

        // #. Animation
        if (Mathf.Abs(rigid.velocity.x) > 0.3f)
            animator.SetBool("onMove", true);
        else
            animator.SetBool("onMove", false);
    }

    void FixedUpdate()
    {
        // #1. 이동 입력
        if (!onDamaged && !onDie && !onAttack)
        {
            float h = Input.GetAxisRaw("Horizontal");
            rigid.AddForce(Vector2.right * h * speed * Time.deltaTime, ForceMode2D.Impulse);
        }

        // #. 바닥 착지
        Debug.DrawRay(rigid.position, Vector3.down * 1.2f, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1.2f, LayerMask.GetMask("Platform"));

        if (rayHit.collider != null && rayHit.distance < 1.0f) // 바닥을 감지
        {
            onGround = true;
            isWall = false;
            rigid.mass = 1;
            animator.SetBool("onGround", true);
        }
        else // 공중에 있을 때
        {
            onGround = false;
            rigid.mass = 1.75f;
            animator.SetBool("onGround", false);

            isWall = Physics2D.Raycast(wallCheck.position, spriteRenderer.flipX == true ? Vector2.left : Vector2.right, wallCheckDistance, wallLayer);
            // animator.SetBool("isSliding", isWall);
        }

        // #. 벽 타기
        if (isWall)
        {
            rigid.velocity = new Vector2(rigid.velocity.x, rigid.velocity.y * slidingSpeed);
            rigid.mass = 1.2f;

            if (Input.GetButtonDown("Jump"))
            {
                Debug.Log("점프");
                rigid.velocity = new Vector2(0, wallJumpPower);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // #8. 몬스터 피격
        if (collision.gameObject.CompareTag("HitBox"))
        {
            OnDamaged(collision.transform.position, collision.gameObject.GetComponentInParent<Enemy>().atk);
        }
    }
    
    void OnTriggerStay2D(Collider2D collision)
    {
        // #. 오브젝트 상호작용
        if (collision.gameObject.CompareTag("Object"))
        {
            if(Input.GetButtonDown("Interaction"))
            {
                Object.ObjectType objectType = collision.gameObject.GetComponent<Object>().objectType;

                switch (objectType)
                {
                    case Object.ObjectType.TreasureBox:
                        collision.gameObject.GetComponent<TreasureBox>().Spawn();
                        break;

                    case Object.ObjectType.PortalRing:
                        collision.gameObject.GetComponent<Portal>().Teleport(gameObject);
                        break;
                }
            }
        }
    }

    // #. 공격
    IEnumerator Attack()
    {
        comboCount++;
        hitBox.transform.localScale = spriteRenderer.flipX ? new Vector3(-1, 1, 1) : new Vector3(1, 1, 1);
        animator.SetTrigger("doAttack");

        yield return new WaitForSeconds(1.3f);
        onAttack = false;
        comboCount = 0;
    }

    IEnumerator ComboActivation()
    {
        while (comboCount == 1)
        {
            if (Input.GetMouseButtonDown(0))
            {
                hitBox.SetActive(false);
                StopCoroutine("Attack");
                StartCoroutine("Attack");
            }

            yield return null;
        }
    }

    IEnumerator OnHitBox()
    {
        hitBox.SetActive(true);

        yield return new WaitForSeconds(0.1f);
        hitBox.SetActive(false);
    }

    // #. 피격 처리
    public void OnDamaged(Vector2 targetPos, float damage)
    {
        playerHP -= damage;

        if (playerHP <= 0)
            OnDie();
        else
        {
            onDamaged = true;
            GameManager.instance.HPSetting("Damage");

            // #. 레이어 변경 (Invincibility)
            gameObject.layer = 9;

            // #. 컬러 변경
            spriteRenderer.color = new Color(1, 1, 1, 0.6f);

            // #. 넉백
            int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1;
            rigid.AddForce(new Vector2(dirc, 0) * 3, ForceMode2D.Impulse);
            animator.SetTrigger("doDamaged");

            // #. 피격 해제 실행
            StartCoroutine("OffDamaged");
        }
    }

    // #. 피격 해제
    IEnumerator OffDamaged()
    {
        yield return new WaitForSeconds(1f);
        onDamaged = false;

        yield return new WaitForSeconds(1f);
        gameObject.layer = 3;
        spriteRenderer.color = new Color(1, 1, 1, 1);
    }

    void OnDie()
    {
        onDie = true;
        gameObject.layer = 9;
        animator.SetTrigger("doDie");

        GameManager.instance.GameOver();
    }
}
*/
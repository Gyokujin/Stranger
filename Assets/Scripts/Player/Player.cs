using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float maxSpeed;
    [SerializeField]
    private float jumpPower;
    private bool onGround;

    private Rigidbody2D rigid;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // #3. Stop Speed
        if (Input.GetButtonUp("Horizontal"))
        {
            rigid.velocity = new Vector2(0, rigid.velocity.y);
        }

        // #4. Direction Sprite
        if (Input.GetButton("Horizontal"))
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;

        // #5. Jump
        if (Input.GetButtonDown("Jump") && onGround)
        {
            onGround = false;
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            animator.SetTrigger("doJump");
        }

        // #6. Animation
        if (Mathf.Abs(rigid.velocity.x) > 0.3f)
            animator.SetBool("onMove", true);
        else
            animator.SetBool("onMove", false);
    }

    void FixedUpdate()
    {
        // #1. 이동 입력
        float h = Input.GetAxisRaw("Horizontal");
        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        // #2. 속도 제한
        if (rigid.velocity.x > maxSpeed) // Right Max Speed
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        else if (rigid.velocity.x < -maxSpeed) // Left Max Speed
            rigid.velocity = new Vector2(-maxSpeed, rigid.velocity.y);

        // #7. 바닥 착지
        Debug.DrawRay(rigid.position, Vector3.down * 1.2f, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1.2f, LayerMask.GetMask("Platform"));

        if (rayHit.collider != null && rayHit.distance < 0.75f) // 바닥을 감지
        {
            onGround = true;
            animator.SetBool("onGround", true);
        }
        else // 공중에 있을 때
        {
            onGround = false;
            animator.SetBool("onGround", false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // #8. 몬스터 피격
        if (collision.gameObject.CompareTag("HitBox"))
        {
            OnDamaged(collision.transform.position);
        }
    }
    
    void OnTriggerStay2D(Collider2D collision)
    {
        // #9. 오브젝트 상호작용
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

    // #10. 피격 처리
    void OnDamaged(Vector2 targetPos)
    {
        // #. 레이어 변경 (Invincibility)
        gameObject.layer = 9;

        // #. 컬러 변경
        spriteRenderer.color = new Color(1, 1, 1, 0.6f);

        // #. 넉백
        int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1;
        rigid.AddForce(new Vector2(dirc, 1) * 5, ForceMode2D.Impulse);

        // #. 피격 해제 실행
        Invoke("OffDamaged", 2);
    }

    // #11. 피격 해제
    void OffDamaged()
    {
        gameObject.layer = 3;
        spriteRenderer.color = new Color(1, 1, 1, 1);
    }
}
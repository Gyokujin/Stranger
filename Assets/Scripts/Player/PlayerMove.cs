using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
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
        // #1. Move Speed
        float h = Input.GetAxisRaw("Horizontal");
        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        // #2. Max Speed
        if (rigid.velocity.x > maxSpeed) // Right Max Speed
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        else if (rigid.velocity.x < -maxSpeed) // Left Max Speed
            rigid.velocity = new Vector2(-maxSpeed, rigid.velocity.y);

        // #7. Landing Platform
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
}
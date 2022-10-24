using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int moveVec; // 이동 방향
    public float patternDelay; // 패턴 주기

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

        Invoke("Think", patternDelay);
    }

    void FixedUpdate()
    {
        Move();
    }

    // #1. Set Direction
    void Think()
    {
        moveVec = Random.Range(-1, 2);

        // #. Flip Sprite
        if (moveVec != 0)
        {
            spriteRenderer.flipX = moveVec == 1;
        }

        // #. Sprite Animation
        animator.SetInteger("WalkSpeed", moveVec);

        // #. Recursive
        Invoke("Think", patternDelay);
    }

    // #2. Move
    void Move()
    {
        // #. Move
        rigid.velocity = new Vector2(moveVec, rigid.velocity.y);

        // #. Platform Check
        Vector2 frontVec = new Vector2(rigid.position.x + moveVec * 0.5f, rigid.position.y);
        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Platform"));

        if (rayHit.collider == null)
            Turn();
    }

    // #3. Turn
    void Turn()
    {
        moveVec *= -1;
        spriteRenderer.flipX = !spriteRenderer.flipX;

        CancelInvoke();
        Invoke("Think", patternDelay);
    }
}
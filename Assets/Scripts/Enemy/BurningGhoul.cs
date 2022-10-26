using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurningGhoul : Enemy
{
    [SerializeField]
    private float moveTime;
    private bool onChase;

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

        // Move(moveVec, moveTime);
    }

    void FixedUpdate()
    {

    }

    // #. 랜덤으로 이동 방향 설정
    /*
    IEnumerator Think()
    {
        yield return new WaitForSeconds(patternDelay);

        int moveDir = Random.Range(0, 2);

        switch (moveDir)
        {
            case 0: // 왼쪽
                base.moveVec = -1;
                break;
            case 1: // 오른쪽
                base.moveVec = 1;
                break;
        }
    }
    */

    // #1. Move
    public void Move(float dir, float time)
    {
        while (time > 0)
        {
            time -= Time.deltaTime;
            rigid.velocity = new Vector2(dir, rigid.velocity.y);
            Debug.Log("이동중");
        }
        Debug.Log("이동끝");
    }

    /*
    // #. Move
        rigid.velocity = new Vector2(moveVec, rigid.velocity.y);

        // #. Platform Check
        Vector2 frontVec = new Vector2(rigid.position.x + moveVec * 0.5f, rigid.position.y);
        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Platform"));

        if (rayHit.collider == null)
            Turn();

    // #1. Set Direction
    public void Think()
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



    // #3. Turn
    void Turn()
    {
        moveVec *= -1;
        StopAllCoroutines();
        spriteRenderer.flipX = !spriteRenderer.flipX;
    }
    */
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingDemon : MonoBehaviour
{
    [SerializeField]
    private float patternDelayMin;
    [SerializeField]
    private float patternDelayMax;
    [SerializeField]
    private float chaseSpeed;
    private bool onPattern;
    [SerializeField]
    private GameObject[] meteors;

    private Rigidbody2D rigid;
    private Animator animator;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void OnEnable()
    {
        Invoke("PatternCycle", 5f);
    }

    void FixedUpdate()
    {
        Chase();
    }

    void Chase()
    {
        if (onPattern)
            rigid.velocity = Vector2.zero;
        else
            rigid.velocity = new Vector2(chaseSpeed, 0);
    }

    void PatternCycle()
    {
        int pattern = Random.Range(0, 2); // 0 = 메테오, 1 = 브레스
        animator.SetBool("onAttack", true);
        onPattern = true;

        switch (pattern)
        {
            case 0:
                StartCoroutine("Meteor");
                break;
            case 1:
                StartCoroutine("Breath");
                break;
        }
    }

    IEnumerator Meteor()
    {
        animator.SetTrigger("attack_Meteor");
        int count = Random.Range(3, 6);
        float locationX = transform.position.x;
        float locationY = transform.position.y;

        yield return new WaitForSeconds(2f);

        for (int i = 0; i < count; i++)
        {
            meteors[i].transform.position = new Vector2(Random.Range(locationX, locationX + 7.5f), locationY + 5);
            meteors[i].SetActive(true);
            Rigidbody2D meteorRigid = meteors[i].GetComponent<Rigidbody2D>();
            meteorRigid.velocity = new Vector2(1, -1);
        }

        yield return new WaitForSeconds(2f);

        for (int i = 0; i < meteors.Length; i++)
        {
            meteors[i].SetActive(false);
        }

        onPattern = false;

        yield return new WaitForSeconds(Random.Range(patternDelayMin, patternDelayMax));
        animator.SetBool("onAttack", false);
        PatternCycle();
    }

    IEnumerator Breath()
    {
        animator.SetTrigger("attack_Breath");
        float attackDir = 2;
        float currentPos = transform.position.x;
        float attackTime = Random.Range(3.5f, 5f);

        float rightMax = currentPos + 2;
        float leftMax = currentPos - 2;

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

        onPattern = false;
        animator.SetBool("onAttack", false);

        yield return new WaitForSeconds(Random.Range(patternDelayMin, patternDelayMax));
        PatternCycle();
    }
}
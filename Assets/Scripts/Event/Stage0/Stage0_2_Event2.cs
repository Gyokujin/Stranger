using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage0_2_Event2 : MonoBehaviour
{
    [SerializeField]
    private Sprite moribundStance;

    [SerializeField]
    private GameObject laurence;
    private Animator laurenceAni;

    [SerializeField]
    private GameObject iraff;
    private Animator iraffAni;

    [SerializeField]
    private GameObject[] enemies;

    void Awake()
    {
        laurenceAni = laurence.GetComponent<Animator>();
        iraffAni = iraff.GetComponent<Animator>();
    }

    void Start()
    {
        StartCoroutine("Cut1");
    }

    IEnumerator Cut1()
    {
        MoveCamera.instance.enabled = true;
        Player.instance.GetComponent<Animator>().enabled = false;
        Player.instance.GetComponent<SpriteRenderer>().flipX = true;
        Player.instance.GetComponent<SpriteRenderer>().sprite = moribundStance;

        yield return new WaitForSeconds(5f);
        laurence.SetActive(true);
        laurenceAni.Play("Fall");
        MoveCamera.instance.target = laurence.transform;

        yield return new WaitForSeconds(0.9f);
        laurenceAni.Play("Crouch");

        yield return new WaitForSeconds(0.75f);
        laurenceAni.Play("Run");

        while (laurence.transform.position.x < -12f)
        {
            laurence.GetComponent<Rigidbody2D>().velocity = new Vector2(4, 0);
            yield return null;
        }

        laurence.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        laurence.GetComponent<Rigidbody2D>().AddForce(Vector2.right * 8.5f, ForceMode2D.Impulse);
        laurenceAni.Play("Attack");

        yield return new WaitForSeconds(0.1f);
        laurence.GetComponent<SpriteRenderer>().enabled = false;
        StartCoroutine(enemies[0].GetComponent<Zombie>().DieProcess());

        yield return new WaitForSeconds(0.8f);
        laurence.GetComponent<SpriteRenderer>().enabled = true;
        StartCoroutine(enemies[1].GetComponent<EliteZombie>().DieProcess());

        yield return new WaitForSeconds(1f);
        StartCoroutine("Cut2");
    }
    IEnumerator Cut2()
    {
        yield return new WaitForSeconds(2f);
        MoveCamera.instance.target = iraff.transform;
        iraff.SetActive(true);
        iraffAni.Play("Run");
        laurenceAni.Play("Idle");

        while (iraff.transform.position.x > 3.25f)
        {
            iraff.GetComponent<Rigidbody2D>().velocity = new Vector2(-2.5f, 0);
            yield return null;
        }

        iraff.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        iraff.GetComponent<Rigidbody2D>().AddForce(Vector2.left * 5.5f, ForceMode2D.Impulse);
        iraffAni.Play("Attack");

        yield return new WaitForSeconds(0.1f);
        iraff.GetComponent<SpriteRenderer>().enabled = false;

        yield return new WaitForSeconds(0.75f);
        iraff.GetComponent<SpriteRenderer>().enabled = true;
        StartCoroutine(enemies[2].GetComponent<Zombie>().DieProcess());

        yield return new WaitForSeconds(0.5f);
        iraffAni.Play("AttackEnd");

        yield return new WaitForSeconds(2f);
        laurenceAni.speed = 0;
        laurence.GetComponent<SpriteRenderer>().flipX = true;

        yield return new WaitForSeconds(3f);
        MoveCamera.instance.target = Player.instance.transform;
        StartCoroutine(GameManager.instance.StageTransition(5));
    }
}
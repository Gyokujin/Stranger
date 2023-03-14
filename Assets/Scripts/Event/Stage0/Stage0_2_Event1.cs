using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage0_2_Event1 : MonoBehaviour
{
    private bool onRun;
    [SerializeField]
    private Sprite idleStance;
    [SerializeField]
    private Sprite battleStance;
    private Animator playerAni;
    [SerializeField]
    private GameObject flyingDemon;
    [SerializeField]
    private GameObject[] meteors;
    [SerializeField]
    private GameObject teleportEffect;
    [SerializeField]
    private Vector3 scenePoint;

    void Awake()
    {
        playerAni = Player.instance.GetComponent<Animator>();
    }

    void Start()
    {
        playerAni.enabled = true;
        StartCoroutine("Cut1");
    }

    void FixedUpdate()
    {
        if (onRun)
            Player.instance.GetComponent<Rigidbody2D>().velocity = new Vector2(1.3f, 0);
    }

    void MeteorSpawn()
    {
        for (int i = 0; i < meteors.Length; i++)
        {
            meteors[i].SetActive(true);
            meteors[i].GetComponent<Rigidbody2D>().velocity = new Vector2(1, -1);
        }
    }

    IEnumerator Teleport()
    {
        SpriteRenderer sprite = flyingDemon.GetComponent<SpriteRenderer>();
        Color color = sprite.color;
        color.a = 0;

        yield return new WaitForSeconds(0.5f);
        Vector2 point = new Vector2(Player.instance.transform.position.x - 3, -6.09f);
        Instantiate(teleportEffect, point, Quaternion.identity);
        flyingDemon.transform.position = point;
        float delay = 1f;

        while (delay > 0)
        {
            delay -= Time.deltaTime;
            sprite.color = new Color(1, 1, 1, 1 - delay);
            yield return null;
        }
    }

    IEnumerator Cut1()
    {
        onRun = true;
        playerAni.SetBool("onMove", true);
        Player.instance.gameObject.layer = 9;
        flyingDemon.GetComponent<Rigidbody2D>().velocity = new Vector2(1.1f, 0);

        yield return new WaitForSeconds(3f);
        flyingDemon.GetComponent<Animator>().Play("Meteor");
        flyingDemon.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        yield return new WaitForSeconds(1.5f);
        MeteorSpawn();
        MoveCamera.instance.offsetX = 2.5f;

        yield return new WaitForSeconds(6.4f);
        Time.timeScale = 0.5f;
        onRun = false;
        Player.instance.GetComponent<Rigidbody2D>().AddForce(new Vector2(1, 1) * 5f, ForceMode2D.Impulse);
        playerAni.SetTrigger("doJump");
        playerAni.SetBool("onGround", false);

        yield return new WaitForSeconds(0.5f);
        onRun = true;
        playerAni.SetBool("onGround", true);

        yield return new WaitForSeconds(0.5f);
        onRun = false;
        playerAni.SetBool("onMove", false);
        playerAni.SetBool("onCrouch", true);

        yield return new WaitForSeconds(0.2f);
        playerAni.SetBool("onDash", true);
        Player.instance.GetComponent<Rigidbody2D>().AddForce(Vector2.right * 6f, ForceMode2D.Impulse);

        yield return new WaitForSeconds(0.4f);
        playerAni.SetBool("onDash", false);

        yield return new WaitForSeconds(3f);
        playerAni.SetBool("onCrouch", false);
        StartCoroutine("Cut2");

    }

    IEnumerator Cut2()
    {
        Time.timeScale = 1;
        onRun = true;
        playerAni.SetBool("onMove", true);

        yield return new WaitForSeconds(7f);
        onRun = false;
        playerAni.SetBool("onMove", false);
        playerAni.enabled = false;
        Player.instance.GetComponent<SpriteRenderer>().sprite = idleStance;
        Player.instance.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        MoveCamera.instance.offsetX = 4f;

        yield return new WaitForSeconds(4f);
        Player.instance.GetComponent<SpriteRenderer>().flipX = true;
        MoveCamera.instance.offsetX = 1.5f;
        yield return StartCoroutine("Teleport");

        yield return new WaitForSeconds(1.5f);
        Player.instance.GetComponent<SpriteRenderer>().sprite = battleStance;

        yield return new WaitForSeconds(0.5f);
        flyingDemon.GetComponent<Animator>().enabled = true;
        flyingDemon.GetComponent<Animator>().SetBool("onAttack", true);
        flyingDemon.GetComponent<Animator>().SetTrigger("attack_Breath");

        yield return new WaitForSeconds(0.75f);
        MoveCamera.instance.enabled = false;
        Time.timeScale = 0.2f;
        playerAni.enabled = true;
        playerAni.Play("FallOff");
        Player.instance.GetComponent<BoxCollider2D>().enabled = false;
        Player.instance.GetComponent<Rigidbody2D>().AddForce(new Vector2(1, 1) * 5f, ForceMode2D.Impulse);

        yield return new WaitForSeconds(1f);
        flyingDemon.GetComponent<Animator>().speed = 0.2f;
        Time.timeScale = 1;

        yield return StartCoroutine(UIManager.instance.FadeOut());
        StartCoroutine("Cut3");
    }

    IEnumerator Cut3()
    {
        MoveCamera.instance.transform.position = scenePoint;
        Player.instance.GetComponent<BoxCollider2D>().enabled = true;
        yield return StartCoroutine(UIManager.instance.FadeIn());

        yield return new WaitForSeconds(0.4f);
        StartCoroutine(GameManager.instance.StageTransition(4));
    }
}
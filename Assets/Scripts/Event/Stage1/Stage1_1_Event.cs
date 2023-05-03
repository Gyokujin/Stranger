using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage1_1_Event : MonoBehaviour
{
    private Animator playerAni;
    [SerializeField]
    private Sprite idleStance;
    [SerializeField]
    private Sprite battleStance;
    [SerializeField]
    private GameObject gaiten;
    private Animator gaitenAni;
    [SerializeField]
    private GameObject[] rosaryBullet;

    void Awake()
    {
        playerAni = Player.instance.GetComponent<Animator>();
        gaitenAni = gaiten.GetComponent<Animator>();
    }

    void Start()
    {
        Player.instance.enabled = false;
        StartCoroutine("Cut1");
    }

    IEnumerator Cut1()
    {
        while (Player.instance.transform.position.x < 4f)
        {
            playerAni.SetBool("onMove", true);
            Player.instance.GetComponent<Rigidbody2D>().AddForce(Vector2.right * Time.deltaTime * 180f);
            yield return null;
        }

        Player.instance.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        playerAni.SetBool("onMove", false);
        playerAni.enabled = false;
        Player.instance.GetComponent<SpriteRenderer>().sprite = GameManager.instance.idleStance;

        yield return new WaitForSeconds(3f);

        for (int i = 0; i < rosaryBullet.Length; i++)
        {
            rosaryBullet[i].SetActive(true);
            rosaryBullet[i].GetComponent<Rigidbody2D>().velocity = (Vector2.left + Vector2.down) * 5f;
            yield return new WaitForSeconds(0.2f);
        }

        yield return new WaitForSeconds(0.1f);
        playerAni.enabled = true;
        Player.instance.GetComponent<Rigidbody2D>().AddForce(Vector2.left * 7.5f + Vector2.up * 6f, ForceMode2D.Impulse);
        playerAni.SetBool("onGround", false);
        playerAni.SetTrigger("doJump");

        yield return new WaitForSeconds(0.6f);
        playerAni.SetBool("onGround", true);
        playerAni.SetBool("onMove", false);

        yield return new WaitForSeconds(0.1f);
        playerAni.SetBool("onCrouch", true);
        float time = 1f;

        while (time > 0)
        {
            time -= Time.deltaTime;
            Player.instance.GetComponent<Rigidbody2D>().velocity = Vector2.left * 0.3f;
            yield return null;
        }

        Player.instance.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        playerAni.SetBool("onCrouch", false);

        yield return new WaitForSeconds(1f);
        gaiten.SetActive(true);
        gaitenAni.SetTrigger("doDash");

        while (gaiten.transform.position.x > 1.7f)
        {
            gaiten.GetComponent<Rigidbody2D>().velocity = (Vector2.left * 9f);
            yield return null;
        }

        StartCoroutine("Cut2");
    }

    IEnumerator Cut2()
    {
        Time.timeScale = 0.2f;

        yield return new WaitForSeconds(0.03f);
        gaiten.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        gaitenAni.SetBool("onAttack", true);
        gaitenAni.SetTrigger("doKickAttack");
        playerAni.SetBool("onMove", true);
        playerAni.SetBool("onDash", true);

        float time = 0.65f;

        while (time > 0)
        {
            time -= Time.deltaTime;
            Player.instance.GetComponent<Rigidbody2D>().velocity = Vector2.right * 3f;
            yield return null;
        }

        Time.timeScale = 1f;
        playerAni.SetBool("onMove", false);
        playerAni.SetBool("onDash", false);
        gaitenAni.SetBool("onAttack", false);
        gaiten.GetComponent<SpriteRenderer>().flipX = true;
        Player.instance.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        yield return new WaitForSeconds(0.2f);
        Player.instance.GetComponent<SpriteRenderer>().flipX = true;

        yield return new WaitForSeconds(1.5f);
        playerAni.SetTrigger("doAttack");
        gaitenAni.SetBool("onAttack", true);
        gaitenAni.SetTrigger("doJump");
        gaiten.GetComponent<Rigidbody2D>().AddForce(Vector2.left * 3f + Vector2.up * 3.5f, ForceMode2D.Impulse);

        yield return new WaitForSeconds(0.5f);
        SpriteRenderer gaitenSprite = gaiten.GetComponent<SpriteRenderer>();
        Color color = gaitenSprite.color;
        color.a = 1;
        float delay = 1f;

        // #. 가이텐을 사라지게 한다
        while (delay > 0)
        {
            delay -= Time.deltaTime;
            gaitenSprite.color = new Color(1, 1, 1, delay);
            yield return null;
        }

        gaitenAni.SetBool("onAttack", false);
        gaiten.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        yield return new WaitForSeconds(1f);
        MoveCamera.instance.offsetX = 1.5f;

        yield return new WaitForSeconds(3f);
        playerAni.enabled = false;
        Player.instance.GetComponent<SpriteRenderer>().sprite = GameManager.instance.idleStance;
        Player.instance.GetComponent<SpriteRenderer>().flipX = false;
        gaiten.transform.position = new Vector2(10, -4.15f);
        gaitenSprite.flipX = false;
        gaitenAni.SetBool("onMove", true);

        delay = 3f;

        while (delay > 0)
        {
            delay -= Time.deltaTime;
            gaitenSprite.color = new Color(1, 1, 1, 1 - delay/3);
            gaiten.GetComponent<Rigidbody2D>().velocity = Vector2.left;
            yield return null;
        }

        gaiten.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        gaitenAni.SetBool("onMove", false);
        Player.instance.GetComponent<SpriteRenderer>().sprite = battleStance;

        yield return new WaitForSeconds(2f);
        StartCoroutine(GameManager.instance.StageTransition(11));
    }
}
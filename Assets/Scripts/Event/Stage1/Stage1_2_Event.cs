using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage1_2_Event : MonoBehaviour
{
    private Animator playerAni;
    private SpriteRenderer playerSprite;
    [SerializeField]
    private Sprite idleStance;
    [SerializeField]
    private Sprite playerStance1;
    [SerializeField]
    private Sprite playerStance2;

    [SerializeField]
    private GameObject gaiten;
    private Animator gaitenAni;
    SpriteRenderer gaitenSprite;
    [SerializeField]
    private Sprite gaitenStance1;
    [SerializeField]
    private GameObject rosary;
    [SerializeField]
    private GameObject[] bullets;
    [SerializeField]
    private float[] shotDirX;
    [SerializeField]
    private float[] shotDirY;

    [SerializeField]
    private GameObject credit;
    [SerializeField]
    private Animator creditText1;
    [SerializeField]
    private Animator creditText2;
    [SerializeField]
    private GameObject quitButton;

    void Awake()
    {
        playerAni = Player.instance.GetComponent<Animator>();
        playerSprite = Player.instance.GetComponent<SpriteRenderer>();
        gaitenAni = gaiten.GetComponent<Animator>();
        gaitenSprite = gaiten.GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        Player.instance.enabled = false;
        StartCoroutine("Cut1");
    }

    IEnumerator BulletsSpawn()
    {
        for (int i = 0; i < bullets.Length; i++)
        {
            bullets[i].SetActive(true);
            bullets[i].GetComponent<Rigidbody2D>().velocity = new Vector2(-1 * shotDirX[i], shotDirY[i]);
            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator Cut1()
    {
        yield return new WaitForSeconds(2f);
        playerAni.SetTrigger("doAttack");

        yield return new WaitForSeconds(0.3f);
        playerAni.enabled = false;
        gaitenAni.SetBool("onHit", true);
        gaiten.GetComponent<Rigidbody2D>().AddForce(Vector2.right * 5f, ForceMode2D.Impulse);

        yield return new WaitForSeconds(0.4f);
        playerSprite.sprite = playerStance1;
        gaiten.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        gaitenAni.SetBool("onHit", false);

        yield return new WaitForSeconds(0.3f);
        gaitenAni.SetBool("onAttack", true);
        gaitenAni.SetTrigger("doDash");

        float time = 0.3f;

        while (time > 0)
        {
            time -= Time.deltaTime;
            gaiten.GetComponent<Rigidbody2D>().velocity = Vector2.left * 3f;
            gaitenSprite.color = new Color(1, 1, 1, time / 0.3f);
            yield return null;
        }

        gaiten.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        gaitenAni.SetTrigger("doKickAttack");

        yield return new WaitForSeconds(0.1f);
        gaitenAni.SetBool("onAttack", false);
        gaitenAni.enabled = false;
        gaitenSprite.flipX = true;
        gaitenSprite.sprite = gaitenStance1;
        gaiten.transform.position = new Vector2(2.717f, -4.15f);
        gaitenAni.enabled = true;
        gaitenAni.SetBool("onCrouch", true);

        time = 0.5f;

        while (time > 0)
        {
            time -= Time.deltaTime;
            gaitenSprite.color = new Color(1, 1, 1, 1 - time / 0.5f);
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);
        gaitenAni.SetBool("onAttack", true);
        gaitenAni.SetTrigger("onCrouchAttack");

        yield return new WaitForSeconds(0.1f);
        playerSprite.flipX = true;
        playerSprite.sprite = playerStance2;

        time = 4f;

        while (time > 0)
        {
            time -= Time.deltaTime;
            
            if ((int)time % 2 == 0)
            {
                Player.instance.GetComponent<Rigidbody2D>().velocity = Vector2.right * 0.03f;
                gaiten.GetComponent<Rigidbody2D>().velocity = Vector2.right * 0.03f;
            }
            else
            {
                Player.instance.GetComponent<Rigidbody2D>().velocity = Vector2.left * 0.03f;
                gaiten.GetComponent<Rigidbody2D>().velocity = Vector2.left * 0.03f;
            }

            yield return null;
        }

        gaiten.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        playerAni.enabled = true;
        playerAni.Play("Player_AttackB");

        yield return new WaitForSeconds(0.1f);
        playerAni.enabled = false;
        playerSprite.sprite = playerStance1;
        gaitenAni.enabled = true;
        gaitenAni.SetBool("onCrouch", false);
        gaitenAni.SetBool("onAttack", false);
        gaitenAni.SetBool("onHit", true);
        time = 1f;

        while (time > 0)
        {
            time -= Time.deltaTime;
            gaiten.GetComponent<Rigidbody2D>().velocity = Vector2.left * 1.5f;
            gaitenSprite.color = new Color(1, 1, 1, time);
            yield return null;
        }

        gaitenAni.SetBool("onHit", false);
        gaiten.transform.position = new Vector2(19.25f, -4.15f);
        gaitenSprite.flipX = false;
        gaitenSprite.color = new Color(1, 1, 1, 1);
        gaiten.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        StartCoroutine("Cut2");
    }

    IEnumerator Cut2()
    {
        yield return new WaitForSeconds(1f);
        rosary.SetActive(true);
        playerSprite.flipX = false;
        MoveCamera.instance.target = gaiten.transform;

        yield return new WaitForSeconds(2f);
        StartCoroutine("BulletsSpawn");
        MoveCamera.instance.offsetX = 2f;
        MoveCamera.instance.target = Player.instance.transform;
        playerAni.enabled = true;
        playerAni.SetBool("onMove", true);
        gaitenAni.SetBool("onAttack", true);
        gaitenAni.SetTrigger("doJump");
        float time = 6.5f;

        while (time > 0)
        {
            time -= Time.deltaTime;
            Player.instance.GetComponent<Rigidbody2D>().velocity = Vector2.right * 2f;
            yield return null;
        }

        Player.instance.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        Player.instance.GetComponent<Rigidbody2D>().AddForce(Vector2.right * 2.5f);
        playerAni.SetBool("onDash", true);

        yield return new WaitForSeconds(1f);
        gaitenAni.SetTrigger("doJumpAttack");
        Time.timeScale = 0.2f;
        time = 1f;
        
        while (time > 0)
        {
            time -= Time.deltaTime;
            Player.instance.GetComponent<Rigidbody2D>().velocity = Vector2.right * 0.25f;
            gaiten.GetComponent<Rigidbody2D>().velocity = Vector2.left * 1.55f;
            yield return null;
        }

        Time.timeScale = 1f;
        playerAni.Play("DashAttack");

        yield return new WaitForSeconds(0.2f);
        Player.instance.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        gaitenAni.SetBool("onHit", true);
        gaiten.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        gaiten.GetComponent<Rigidbody2D>().AddForce(Vector2.right * 4f, ForceMode2D.Impulse);

        yield return new WaitForSeconds(1f);
        MoveCamera.instance.offsetX = 6f;
        playerAni.Play("DashAttackDelay");
        gaiten.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        rosary.GetComponent<Animator>().enabled = true;

        yield return new WaitForSeconds(1f);
        time = 1f;

        while (time > 0)
        {
            time -= Time.deltaTime;
            gaitenSprite.color = new Color(1, 1, 1, time);
            yield return null;
        }

        yield return new WaitForSeconds(2f);
        playerAni.enabled = false;
        playerSprite.sprite = idleStance;

        yield return new WaitForSeconds(4f);
        yield return StartCoroutine(UIManager.instance.FadeOut());
        credit.SetActive(true);

        yield return StartCoroutine(UIManager.instance.FadeIn());
        creditText1.enabled = true;
        creditText2.enabled = true;

        yield return new WaitForSeconds(2f);
        quitButton.SetActive(true);
    }

    public void GameQuit()
    {
        GameManager.instance.GameQuit();
    }
}
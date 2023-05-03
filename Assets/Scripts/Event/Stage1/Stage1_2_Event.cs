using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage1_2_Event : MonoBehaviour
{
    private Animator playerAni;
    [SerializeField]
    private GameObject gaiten;
    private Animator gaitenAni;

    void Awake()
    {
        playerAni = Player.instance.GetComponent<Animator>();
        gaitenAni = gaiten.GetComponent<Animator>();
    }

    void Start()
    {
        StartCoroutine("Cut1");
    }

    IEnumerator Cut1()
    {
        yield return new WaitForSeconds(2f);
        playerAni.SetTrigger("doAttack");

        yield return new WaitForSeconds(0.3f);
        gaitenAni.SetBool("onHit", true);
        gaiten.GetComponent<Rigidbody2D>().AddForce(Vector2.right * 3.5f, ForceMode2D.Impulse);

        yield return new WaitForSeconds(0.7f);
        gaitenAni.SetBool("onHit", false);
        gaitenAni.SetBool("onAttack", true);
        gaitenAni.SetTrigger("doDash");

        while (gaiten.transform.position.x > 4f)
        {
            gaiten.GetComponent<Rigidbody2D>().velocity = Vector2.left * 3f;
            yield return null;
        }

        gaiten.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        gaitenAni.SetTrigger("doKickAttack");
    }
}
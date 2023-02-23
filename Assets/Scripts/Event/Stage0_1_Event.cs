using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stage0_1_Event : MonoBehaviour
{
    [SerializeField]
    private Sprite battleStance;
    [SerializeField]
    private GameObject flyingDemon;

    void Awake()
    {
        UIManager.instance.HideUI();
    }

    void Start()
    {
        StartCoroutine("Stage0_1Event");
    }

    IEnumerator Stage0_1Event()
    {
        Player.instance.GetComponent<SpriteRenderer>().sprite = GameManager.instance.idleStance;
        Player.instance.GetComponent<SpriteRenderer>().flipX = true;

        while (Player.instance.transform.position.x < 2f)
        {
            Player.instance.GetComponent<Animator>().SetBool("onMove", true);
            Player.instance.GetComponent<Rigidbody2D>().AddForce(Vector2.right * Time.deltaTime * 180f);
            yield return null;
        }

        Player.instance.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        Player.instance.GetComponent<Animator>().SetBool("onMove", false);
        Player.instance.GetComponent<SpriteRenderer>().sprite = GameManager.instance.idleStance;
        Player.instance.GetComponent<Animator>().speed = 0;

        yield return new WaitForSeconds(4f);
        flyingDemon.SetActive(true);
        Player.instance.GetComponent<SpriteRenderer>().flipX = false;

        while (flyingDemon.transform.position.x < -5f)
        {
            flyingDemon.GetComponent<Rigidbody2D>().AddForce(Vector2.right * Time.deltaTime * 20f);
            yield return null;
        }

        flyingDemon.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        
        yield return new WaitForSeconds(3f);
        flyingDemon.GetComponent<Animator>().speed = 0.75f;
        flyingDemon.GetComponent<Animator>().SetBool("onAttack", true);
        flyingDemon.GetComponent<Animator>().SetTrigger("attack_Meteor");
        Player.instance.GetComponent<Animator>().speed = 1;
        Player.instance.GetComponent<SpriteRenderer>().sprite = battleStance;
        Player.instance.GetComponent<Animator>().speed = 0;

        yield return new WaitForSeconds(1f);
        GameManager.instance.stageNum = 2;
        StartCoroutine(GameManager.instance.StageTransition(GameManager.instance.stageNum));
    }
}
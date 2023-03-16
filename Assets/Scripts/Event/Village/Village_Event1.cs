using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Village_Event1 : MonoBehaviour
{
    private Vector2 bedPos;
    [SerializeField]
    private Sprite bedStance0;
    [SerializeField]
    private Sprite bedStance1;
    [SerializeField]
    private GameObject tyche;
    private Animator tycheAni;

    private void Awake()
    {
        tycheAni = tyche.GetComponent<Animator>();
    }

    void Start()
    {
        bedPos = new Vector2(GameManager.instance.startPointX[5], GameManager.instance.startPointY[5]);
        Player.instance.GetComponent<BoxCollider2D>().enabled = false;
        Player.instance.GetComponent<Animator>().enabled = false;
        Player.instance.GetComponent<Rigidbody2D>().gravityScale = 0;
        Player.instance.transform.position = bedPos;
        Player.instance.GetComponent<SpriteRenderer>().sprite = bedStance0;
        StartCoroutine("Cut1");
    }

    IEnumerator Cut1()
    {
        yield return new WaitForSeconds(5f);
        Player.instance.GetComponent<SpriteRenderer>().sprite = bedStance1;

        yield return new WaitForSeconds(2f);
        tycheAni.Play("Move");

        while (tyche.transform.position.x > 2f)
        {
            tyche.GetComponent<Rigidbody2D>().velocity = new Vector2(-1.7f, 0);
            yield return null;
        }

        tyche.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        tycheAni.Play("Idle");

        yield return new WaitForSeconds(4f);
        StartCoroutine(GameManager.instance.StageTransition(6));
    }
}
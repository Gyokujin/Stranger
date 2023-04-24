using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage1_2_Event : MonoBehaviour
{
    [SerializeField]
    private GameObject gaiten;

    void Start()
    {
        StartCoroutine("Cut1");
    }

    IEnumerator Cut1()
    {
        while (Player.instance.transform.position.x < -5.5f)
        {
            Player.instance.GetComponent<Animator>().SetBool("onMove", true);
            Player.instance.GetComponent<Rigidbody2D>().AddForce(Vector2.right * Time.deltaTime * 180f);
            yield return null;
        }

        Player.instance.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        Player.instance.GetComponent<Animator>().SetBool("onMove", false);
        Player.instance.GetComponent<Animator>().enabled = false;
        Player.instance.GetComponent<SpriteRenderer>().sprite = GameManager.instance.idleStance;

        yield return new WaitForSeconds(2f);
        gaiten.SetActive(true);
    }
}
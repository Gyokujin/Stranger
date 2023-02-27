using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage0_2_Event : MonoBehaviour
{
    [SerializeField]
    private Sprite moribundStance;

    void Start()
    {
        StartCoroutine("Stage0_2Event");
    }

    IEnumerator Stage0_2Event()
    {
        Player.instance.GetComponent<Animator>().enabled = false;
        Player.instance.GetComponent<SpriteRenderer>().sprite = moribundStance;
        Player.instance.GetComponent<SpriteRenderer>().color = new Color(171, 170, 170);

        yield return new WaitForSeconds(5f);
        StartCoroutine(GameManager.instance.StageTransition(4));
    }
}
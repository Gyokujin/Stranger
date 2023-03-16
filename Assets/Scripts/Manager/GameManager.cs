using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    public Sprite idleStance;
    public float[] startPointX;
    public float[] startPointY;
    public int stagePoint;
    public int stageNum = 0;
    public int gold = 0;
    public int crystal = 0;
    public string[] watchEvent;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (instance != this)
                Destroy(this.gameObject);
        }
    }

    void Start()
    {
        StartCoroutine("StageOpen");
    }

    IEnumerator StageOpen()
    {
        AnimatorInitialization();
        Player.instance.GetComponent<BoxCollider2D>().enabled = true;
        Player.instance.GetComponent<Rigidbody2D>().gravityScale = 1.6f;
        Player.instance.transform.position = new Vector2(startPointX[stageNum], startPointY[stageNum]);
        Player.instance.transform.rotation = Quaternion.Euler(0, 0, 0);
        Player.instance.isRight = 1;
        Player.instance.GetComponent<SpriteRenderer>().flipX = false;
        Player.instance.GetComponent<SpriteRenderer>().sprite = idleStance;
        MoveCamera.instance.CameraSetting(stageNum);
        Debug.Log(stageNum);

        if (stageNum == 1 || stageNum == 3 || stageNum == 4 || stageNum == 5 || stageNum == 7) // 이벤트로만 진행되는 씬
        {
            UIManager.instance.HideUI();
            UIManager.instance.EventCut(true);
            Player.instance.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            yield return StartCoroutine(UIManager.instance.FadeIn());
        }
        else // 캐릭터를 직접 조작하여 진행하는 씬
        {
            UIManager.instance.ShowUI();
            UIManager.instance.EventCut(false);
            yield return StartCoroutine(UIManager.instance.FadeIn());
            Player.instance.enabled = true;
            
            yield return StartCoroutine(UIManager.instance.StageNameFade());
        }
    }

    public IEnumerator StageTransition(int num)
    {
        Player.instance.enabled = false;
        yield return StartCoroutine(UIManager.instance.FadeOut());
        stageNum = num;
        SceneManager.LoadScene(num);
        StartCoroutine("StageOpen");
    }

    public void Increase(string type, int amount)
    {
        switch (type)
        {
            case "gold":
                gold += amount;
                break;

            case "crystal":
                crystal += amount;
                break;
        }
    }

    public void HPSetting(string type)
    {
        switch (type)
        {
            case "Recovery":
                break;

            case "Damage":
                break;

            case "DeadZone":
                break;
        }

        Debug.Log("현재 체력은 " + Player.instance.playerHP);     
    }

    void AnimatorInitialization()
    {
        Animator playerAnimator = Player.instance.GetComponent<Animator>();
        playerAnimator.enabled = true;
        playerAnimator.SetBool("onMove", false);
        playerAnimator.SetBool("onGround", true);
        playerAnimator.SetBool("onCrouch", false);
        playerAnimator.SetBool("onSliding", false);
        playerAnimator.SetBool("onDash", false);
    }


    public IEnumerator Teleport()
    {
        Portal portal = Player.instance.targetObject.GetComponent<Portal>();
        Player.instance.enabled = false;

        yield return StartCoroutine(UIManager.instance.FadeOut());
        Player.instance.transform.position = portal.targetPortal;

        yield return StartCoroutine(UIManager.instance.FadeIn());
        Player.instance.enabled = true;
    }

    public void GameOver()
    {
        Debug.Log("Game Over");
    }
}
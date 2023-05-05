using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    [SerializeField]
    private string lobbyScene;
    public Sprite idleStance;
    public float[] startPointX;
    public float[] startPointY;
    public int stagePoint;
    public int stageNum = 0;
    public int gold = 0;
    public int crystal = 0;
    public string[] watchEvent;
    private bool eventStage;

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
        Player.instance.playerHP = Player.instance.maxHP;
        UIManager.instance.SetHP(Player.instance.playerHP);
        Player.instance.GetComponent<BoxCollider2D>().enabled = true;
        Player.instance.GetComponent<Rigidbody2D>().gravityScale = 1.6f;
        Player.instance.transform.position = new Vector2(startPointX[stageNum], startPointY[stageNum]);
        Player.instance.transform.rotation = Quaternion.Euler(0, 0, 0);
        Player.instance.isRight = 1;
        Player.instance.GetComponent<SpriteRenderer>().flipX = false;
        Player.instance.GetComponent<SpriteRenderer>().sprite = idleStance;
        MoveCamera.instance.CameraSetting(stageNum);

        switch (stageNum)
        {
            // 캐릭터를 직접 조작하여 진행하는 씬
            case 0:
            case 2:
            case 9:
            case 11:
                eventStage = false;
                UIManager.instance.ShowUI();
                UIManager.instance.EventCut(false);
                yield return null;
                SpawnManager.instance.ReadSpawnFile();
                yield return StartCoroutine(SpawnManager.instance.SpawnEnemies());
                break;

            // 마을등 몬스터가 없는 씬
            case 6:
            case 8:
                eventStage = false;
                UIManager.instance.ShowUI();
                UIManager.instance.EventCut(false);
                break;

            // 이벤트로만 진행되는 씬
            case 1:
            case 3:
            case 4:
            case 5:
            case 7:
            case 10:
            case 12:
                eventStage = true;
                UIManager.instance.HideUI();
                UIManager.instance.EventCut(true);
                Player.instance.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                break;
        }

        yield return StartCoroutine(UIManager.instance.FadeIn());

        if (!eventStage)
            Player.instance.enabled = true;
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

        UIManager.instance.SetHP(Player.instance.playerHP);
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
        playerAnimator.Play("Idle");
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

    public IEnumerator GameOver()
    {
        Player.instance.enabled = false;
        yield return new WaitForSeconds(3f);
        Time.timeScale = 0;
        UIManager.instance.ShowGameOver();
    }

    public void GameRestart()
    {
        Time.timeScale = 1;
        StartCoroutine("ReStartRoutine");
    }

    private IEnumerator ReStartRoutine()
    {
        yield return StartCoroutine(UIManager.instance.FadeOut());
        Player.instance.Resurrection();
        yield return null;
        AnimatorInitialization();
        SpawnManager.instance.EnemyClear();
        yield return StartCoroutine(SpawnManager.instance.SpawnEnemies());
        yield return StartCoroutine(UIManager.instance.FadeIn());
        Player.instance.enabled = true;
    }

    public void GoVillage()
    {
        StartCoroutine(StageTransition(8));
    }

    public void GoLobby()
    {
        UIManager.instance.HideUI();
        SceneManager.LoadScene(lobbyScene);
    }

    public void GameQuit()
    {
        Application.Quit();
    }
}
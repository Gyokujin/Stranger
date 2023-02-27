using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    public Sprite idleStance;
    public float[] startPointX = { -2.5f, -7f, -8.55f, -2.5f };
    private string[] stageNames = { "Stage0_1", "Stage0_1_Event", "Stage0_2", "Stage0_2_Event", "Village" };
    public int stagePoint;
    public int stageNum = 0;
    public int gold = 0;
    public int crystal = 0;

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
        Player.instance.transform.position = new Vector2(startPointX[stageNum], -2.3f);
        Player.instance.transform.rotation = Quaternion.Euler(0, 0, 0);
        Player.instance.isRight = 1;
        Player.instance.GetComponent<SpriteRenderer>().flipX = false;
        Player.instance.GetComponent<SpriteRenderer>().sprite = idleStance;
        MoveCamera.instance.CameraSetting(stageNum);
        Debug.Log(stageNum);

        if (stageNum == 1 || stageNum == 3) // 이벤트로만 진행되는 씬
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
        stageNum = num;
        yield return StartCoroutine(UIManager.instance.FadeOut());
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
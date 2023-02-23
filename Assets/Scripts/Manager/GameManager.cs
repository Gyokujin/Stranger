using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    public Sprite idleStance;
    public float[] startPointX = { -2.5f, -7f, -8.55f };
    private string[] stageNames = { "Stage0_1", "Stage0_1_Event", "Stage0_2", "Village" };
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
        Player.instance.transform.position = new Vector2(startPointX[stageNum], -2.3f);
        Player.instance.GetComponent<SpriteRenderer>().sprite = idleStance;
        MoveCamera.instance.SwitchOffset(stageNum);
        Debug.Log(stageNum);

        if (stageNum == 1 || stageNum == 3) // 이벤트로만 진행되는 씬
        {
            UIManager.instance.EventCut(true);
            Player.instance.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            yield return StartCoroutine(UIManager.instance.FadeIn());
        }
        else // 캐릭터를 직접 조작하여 진행하는 씬
        {
            yield return StartCoroutine(UIManager.instance.FadeIn());
            UIManager.instance.EventCut(false);
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

    public IEnumerator Teleport()
    {
        Portal portal = Player.instance.targetObject.GetComponent<Portal>();
        Player.instance.enabled = false;

        yield return StartCoroutine(UIManager.instance.FadeOut());
        Player.instance.transform.position = portal.targetPortal;
        UIManager.instance.Relocation(portal.targetPortal, portal.offsetBackground);

        yield return StartCoroutine(UIManager.instance.FadeIn());
        Player.instance.enabled = true;
    }

    public void GameOver()
    {
        Debug.Log("Game Over");
    }
}
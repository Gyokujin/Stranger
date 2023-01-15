using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    public Vector2 startPoint;
    public int stagePoint;
    public int curScene;

    [SerializeField]
    private Reposition backgroundImage;
    [SerializeField]
    private float fadeTime;
    [SerializeField]
    private Image fadeImage;

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
        StartCoroutine("Fade");
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

    public void Teleport(Vector2 destination, Vector2 offset)
    {
        StartCoroutine("Fade");

        // #. 플레이어 이동
        Player.instance.transform.position = destination;

        // #. 배경 재배치
        for (int i = 0; i < backgroundImage.transform.childCount; i++)
        {
            Debug.Log(backgroundImage.transform.childCount);
            float dir = 0;

            switch (i)
            {
                case 0:
                    dir = -24;
                    break;
                case 1:
                    dir = -12;
                    break;
                case 2:
                    dir = 0;
                    break;
                case 3:
                    dir = 12;
                    break;
                case 4:
                    dir = 24;
                    break;
            }
            backgroundImage.backgrounds[i].transform.position = new Vector2(destination.x + dir + offset.x, destination.y + offset.y);
        }
    }

    IEnumerator Fade()
    {
        float count = 0;
        float alpha = 0;
        fadeImage.color = new Color(0, 0, 0, 1);

        yield return new WaitForSeconds(1.5f);

        while (count < fadeTime)
        {
            count += Time.deltaTime;
            alpha = count / fadeTime;
            fadeImage.color = new Color(0, 0, 0, 1 - alpha);

            yield return null;
        }
    }

    public void StageLoad(int stage)
    {
        SceneManager.LoadScene(stage);
        Player.instance.transform.position = new Vector2(0, -2.25f);
    }

    public void GameOver()
    {
        Debug.Log("Game Over");
    }
}
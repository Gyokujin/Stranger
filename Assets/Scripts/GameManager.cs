using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    public Vector2 startPoint;
    public int stagePoint;
    public int curScene;

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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance = null;

    [SerializeField]
    private Reposition backgroundImage;
    [SerializeField]
    private GameObject statusPanel;
    [SerializeField]
    private GameObject itemPanel;
    [SerializeField]
    private GameObject stageName;
    [SerializeField]
    private Sprite[] stageNameSprites;
    [SerializeField]
    private GameObject fadeImage;
    [SerializeField]
    private GameObject eventCut;
    [SerializeField]
    private GameObject gameOverPanel;
    [SerializeField]
    private GameObject returnButton;

    public float fadeTime = 3.5f;

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

    public IEnumerator FadeIn()
    {
        fadeImage.SetActive(true);
        Image fade = fadeImage.GetComponent<Image>();
        float count = fadeTime;

        yield return new WaitForSeconds(1f);

        // #. 이미지 페이드인
        while (count > 0)
        {
            count -= Time.deltaTime;
            fade.color = new Color(0, 0, 0, count);

            yield return null;
        }

        fadeImage.SetActive(fade);

        yield return new WaitForSeconds(0.75f);

        if (stageNameSprites[GameManager.instance.stageNum] != null)
            StartCoroutine("StageNameFade");
    }

    public IEnumerator FadeOut()
    {
        fadeImage.SetActive(true);
        Image fade = fadeImage.GetComponent<Image>();
        float count = fadeTime;

        yield return new WaitForSeconds(1.0f);

        // #. 이미지 페이드아웃
        while (count > 0)
        {
            count -= Time.deltaTime;
            fade.color = new Color(0, 0, 0, 1 - count);

            yield return null;
        }
    }

    public void EventCut(bool use)
    {
        eventCut.SetActive(use);
    }

    public IEnumerator StageNameFade()
    {
        stageName.SetActive(true);
        Image stageNameImage = stageName.GetComponent<Image>();
        stageNameImage.sprite = stageNameSprites[GameManager.instance.stageNum];
        float count = fadeTime;

        if (stageNameSprites[GameManager.instance.stageNum] != null)
        {
            // #. 스테이지 이름 페이드인
            while (count > 0)
            {
                count -= Time.deltaTime;
                stageNameImage.color = new Color(1, 1, 1, 1 - count);

                yield return null;
            }

            count = fadeTime;

            // #. 스테이지 이름 페이드아웃
            while (count > 0)
            {
                count -= Time.deltaTime;
                stageNameImage.color = new Color(1, 1, 1, count);

                yield return null;
            }
        }

        stageName.SetActive(false);
    }

    public void HideUI()
    {
        statusPanel.SetActive(false);
        itemPanel.SetActive(false);
    }

    public void ShowUI()
    {
        statusPanel.SetActive(true);
        itemPanel.SetActive(true);
    }

    public void ShowGameOver()
    {
        gameOverPanel.SetActive(true);

        if (GameManager.instance.stageNum > 5)
            returnButton.SetActive(true);
        else
            returnButton.SetActive(false);
    }
}
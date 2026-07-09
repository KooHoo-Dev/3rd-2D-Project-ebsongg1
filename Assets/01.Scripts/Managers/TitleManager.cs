using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    [Header("타이틀 배경 이미지 컴포넌트")]
    public Image backgroundImage;

    [Header("바꿀 이미지(스프라이트) 소스")]
    public Sprite normalBackground; // 1회차 
    public Sprite ruinedBackground; // 2회차 

    [Header("옵션창 UI")]
    public GameObject optionUIPanel;

    [Header("BGM")]
    public AudioClip titleBGM;
    public AudioClip titleBGM_2;

    [Header("씬 이동 페이드 연출")]
    public CanvasGroup fadeScreen; 
    public string firstStageName = "Scenes_Stage_01"; // 이동할 스테이지 씬 

    public void Start()
    {
        // 2회차 이상일 때
        if (GameManager.instance != null && GameManager.instance.clearCount >= 1)
        {
            backgroundImage.sprite = ruinedBackground;

            // 2회차 브금 재생
            if (titleBGM_2 != null && BGMManager.instance != null)
                BGMManager.instance.PlayBGM(titleBGM_2);
        }
        // 1회차일 때 
        else
        {
            backgroundImage.sprite = normalBackground;

            // 1회차 브금 재생
            if (titleBGM != null && BGMManager.instance != null)
                BGMManager.instance.PlayBGM(titleBGM);
        }
    }

    public void OnClickStartButton()
    {
        StartCoroutine(FadeAndLoadRoutine());
    }

    IEnumerator FadeAndLoadRoutine()
    {
        if (fadeScreen != null)
        {
            fadeScreen.gameObject.SetActive(true);
            float timer = 0f;

            // 1초 동안 화면 서서히 까맣게 만들기
            while (timer < 1f)
            {
                timer += Time.deltaTime;
                fadeScreen.alpha = timer;
                yield return null;
            }
        }

        // 화면이 완전히 까매지면 다음 씬으로 부드럽게 넘어가기
        SceneManager.LoadScene(firstStageName);
    }

    public void OnClickSettingButton()
    {
        if (optionUIPanel != null)
        {
            optionUIPanel.SetActive(true); // 옵션창 켜기
        }
    }

    public void OnClickExitButton()
    {
        Debug.Log("게임을 종료합니다.");
        Application.Quit();
    }
}
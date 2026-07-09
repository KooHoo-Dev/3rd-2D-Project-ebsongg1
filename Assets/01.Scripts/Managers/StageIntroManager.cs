using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class StageIntroManager : MonoBehaviour
{
    [Header("시작 양피지 UI")]
    public GameObject parchmentUI;

    [Header("BGM")]
    public AudioMixer mainMixer;

    public AudioClip stageBGM;
    public AudioClip dungeonBGM_1; 
    public AudioClip dungeonBGM_2; 

    public void Start()
    {
        if (parchmentUI != null)
        {
            // 씬이 로드되자마자 양피지 UI를 켬
            parchmentUI.SetActive(true);

            // 시간을 0으로 만들어 게임을 완전히 정지
            Time.timeScale = 0f;
        }
    }

    public void OnClickStartBattle()
    {
        if (parchmentUI != null)
        {
            // 양피지 숨기기
            parchmentUI.SetActive(false);
        }

        Time.timeScale = 1f;

        if (stageBGM != null && BGMManager.instance != null)
        {
            BGMManager.instance.PlayBGM(stageBGM);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (GameManager.instance.clearCount == 0)
            {
                // CrossFadeBGM 사용 (2초 동안 부드럽게)
                BGMManager.instance.CrossFadeBGM(dungeonBGM_1, 2f);
            }
            else
            {
                BGMManager.instance.CrossFadeBGM(dungeonBGM_2, 2f);
            }
        }
    }
}

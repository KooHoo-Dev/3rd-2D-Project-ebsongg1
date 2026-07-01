using UnityEngine;

public class StageIntroManager : MonoBehaviour
{
    [Header("시작 양피지 UI")]
    public GameObject parchmentUI; 

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
    }
}

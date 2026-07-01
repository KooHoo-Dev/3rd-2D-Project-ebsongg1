using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameClearManager : MonoBehaviour
{
    [Header("엔딩 연출 UI")]
    public GameObject clearUIPanel; // 반투명 검은 배경 + 타이틀 복귀 버튼

    [Header("왕국군 (궁수) 연출 세팅")]
    public GameObject archerPrefab;   // 왕국군 병사 프리팹
    public GameObject arrowPrefab;    // 화살 프리팹
    public Transform spawnPointRight; // 오른쪽 화면 밖 스폰 위치

    private Transform player;

    void Start()
    {
        if (clearUIPanel != null) clearUIPanel.SetActive(false);

        // 주인공(레이) 위치 미리 찾아두기
        GameObject pObj = GameObject.FindGameObjectWithTag("Player");
        if (pObj != null) player = pObj.transform;
    }

    // BossHealth.cs에서 보스가 죽을 때 호출하는 함수
    public void TriggerGameClear()
    {
        Debug.Log("엔딩 시퀀스 시작!"); // 콘솔창에 이게 뜨면 정상 작동 중인 것!
        StartCoroutine(EndingSequence());
    }

    IEnumerator EndingSequence()
    {
        // 1. 보스 처치 후 클리어의 기쁨을 1.5초간 만끽
        yield return new WaitForSeconds(1.5f);

        // 5. 비장하게 반투명 클리어 UI 팝업!
        if (clearUIPanel != null) clearUIPanel.SetActive(true);

        // 2. 오른쪽에서 병사(궁수) 3명 스폰!
        if (archerPrefab != null && spawnPointRight != null)
        {
            for (int i = 0; i < 3; i++)
            {
                // 3명이 겹치지 않게 약간씩 위아래로 흩어져서 스폰
                Vector2 spawnPos = spawnPointRight.position + new Vector3(Random.Range(0f, 1f), Random.Range(-1.5f, 1.5f), 0);
                Instantiate(archerPrefab, spawnPos, Quaternion.identity);
            }
        }

        // 병사들이 폼 잡고 조준할 시간 1.5초 대기
        yield return new WaitForSeconds(1.5f);

        // 3. 화살 발사! (플레이어를 향해)
        if (arrowPrefab != null && spawnPointRight != null && player != null)
        {
            for (int i = 0; i < 3; i++)
            {
                // 화살 생성
                Vector2 arrowPos = spawnPointRight.position + new Vector3(0, Random.Range(-0.5f, 0.5f), 0);
                GameObject arrow = Instantiate(arrowPrefab, arrowPos, Quaternion.identity);

                // 화살 스크립트에게 주인공 위치로 날아가라고 명령
                RoyalArrow arrowScript = arrow.GetComponent<RoyalArrow>();
                if (arrowScript != null)
                {
                    arrowScript.SetTarget(player.position);
                }

                // 타다닥! 쏘는 느낌을 위해 0.2초 간격 발사
                yield return new WaitForSeconds(0.2f);
            }
        }

        // 4. 레이가 화살에 맞고 쓰러지며 멘붕할 시간 2초 대기
        yield return new WaitForSeconds(2.0f);

        
    }

    public void OnClickReturnToTitle()
    {
        // 처치수 초기화 후 타이틀 씬으로 복귀
        DataManager.killCount = 0;
        SceneManager.LoadScene("TitleScene");
    }
}

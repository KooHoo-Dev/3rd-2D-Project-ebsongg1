using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameClearManager : MonoBehaviour
{
    [Header("엔딩 연출 UI")]
    public CanvasGroup fadeGroup; // 검은색 전체 화면 캔버스 그룹
    public float fadeSpeed = 0.5f;

    [Header("왕국군 (궁수) 연출 세팅")]
    public GameObject archerPrefab;   // 왕국군 병사 프리팹
    public GameObject arrowPrefab;    // 화살 프리팹
    public Transform spawnPointRight; // 오른쪽 화면 밖 스폰 위치

    private Transform player;
    private Camera mainCam;

    public void Start()
    {
        mainCam = Camera.main;
        if (fadeGroup != null) fadeGroup.alpha = 0; // 시작할 땐 화면이 투명하게

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
        // 플레이어가 죽어도 게임 오버 씬으로 가지 않도록 설정
        if (player != null)
        {
            PlayerHealth ph = player.GetComponent<PlayerHealth>();
            if (ph != null) ph.isEnding = true; // 아까 만든 변수를 여기서 true로 켬!

            // 물리 엔진을 아예 꺼버려서(simulated = false) 이동/점프를 물리적으로 불가능하게 만듦
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero; // 미끄러짐 방지
            }

            MonoBehaviour moveScript = player.GetComponent("PlayerMovement") as MonoBehaviour;
            if (moveScript != null) moveScript.enabled = false;

            Animator pAnim = player.GetComponentInChildren<Animator>();
            if (pAnim != null)
            {
                pAnim.SetBool("Move", false);
                pAnim.Play("IDLE");
            }

            // 보스 처치 후 클리어의 기쁨을 1.5초간 만끽
            yield return new WaitForSeconds(1.5f);

            // 오른쪽에서 병사(궁수) 3명 스폰
            Vector3 baseSpawnPos = spawnPointRight.position;

            if (archerPrefab != null && spawnPointRight != null)
            {
                for (int i = 0; i < 3; i++)
                {
                    // Y축(높이)은 기준점과 완벽히 동일하게 두고, X축(가로)으로만 안 겹치게 살짝 흩어지게 함
                    Vector2 spawnPos = baseSpawnPos + new Vector3(Random.Range(0f, 1.5f), 0, 0);
                    Instantiate(archerPrefab, spawnPos, Quaternion.identity);
                }
            }

            // 병사들이 폼 잡고 조준할 시간 1.5초 대기
            yield return new WaitForSeconds(1.5f);

            Vector3 dynamicSpawnCenter = player.position + new Vector3(15f, -0.038f, 0);

            // 화살 발사
            if (archerPrefab != null)
            {
                for (int i = 0; i < 3; i++)
                {
                    Vector2 spawnPos = dynamicSpawnCenter + new Vector3(Random.Range(0f, 1.5f), 0, 0);
                    Instantiate(archerPrefab, spawnPos, Quaternion.identity);
                }
            }

            yield return new WaitForSeconds(2.5f);

            if (arrowPrefab != null && player != null)
            {
                for (int i = 0; i < 3; i++)
                {
                    Vector2 arrowPos = dynamicSpawnCenter + new Vector3(0, Random.Range(-0.2f, 0.2f), 0);
                    GameObject arrow = Instantiate(arrowPrefab, arrowPos, Quaternion.identity);
                    RoyalArrow arrowScript = arrow.GetComponent<RoyalArrow>();
                    if (arrowScript != null) arrowScript.SetTarget(player.position);


                    // 쏘는 느낌을 위해 0.2초 간격 발사
                    yield return new WaitForSeconds(0.2f);
                }
            }

            // 화살에 맞고 쓰러지며 2초 대기
            yield return new WaitForSeconds(2.0f);

            // 카메라가 왕국군(스폰 지점) 쪽으로 이동함
            // 카메라가 덜덜 떨리지 않도록 기존 카메라 추적 코드를 잠시 꺼줌
            PlayerCameraFollow camFollow = mainCam.GetComponent<PlayerCameraFollow>();
            if (camFollow != null) camFollow.enabled = false;

            float t = 0;
            Vector3 startPos = mainCam.transform.position;
            // 카메라 Z축(보통 -10)은 유지하면서 왕국군 쪽으로 위치 이동
            Vector3 endPos = new Vector3(dynamicSpawnCenter.x, dynamicSpawnCenter.y, startPos.z);


            while (t < 2.0f)
            {
                t += Time.deltaTime;
                // Lerp로 2초 동안 부드럽게 이동
                mainCam.transform.position = Vector3.Lerp(startPos, endPos, t / 2.0f);
                yield return null;
            }

            yield return new WaitForSeconds(1.0f); // 이동 후 왕국군을 1초간 비춰줌

            // 서서히 검은 화면으로 페이드 아웃
            if (fadeGroup != null)
            {
                while (fadeGroup.alpha < 1.0f)
                {
                    fadeGroup.alpha += Time.deltaTime * fadeSpeed;
                    yield return null;
                }
            }

            // 완전히 화면이 까매지고 여운을 주는 1.5초 대기
            yield return new WaitForSeconds(1.5f);

            // 데이터 초기화 후 타이틀 씬으로 자동 복귀
            DataManager.killCount = 0;
            SceneManager.LoadScene("Scenes_Title");
        }


    }
}

 



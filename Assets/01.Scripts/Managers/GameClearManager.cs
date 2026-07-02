using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameClearManager : MonoBehaviour
{
    [Header("엔딩 연출 UI")]
    public CanvasGroup fadeGroup; // 검은색 전체 화면 캔버스 그룹
    public float fadeSpeed = 0.5f;

    [Header("왕국군 연출 세팅")]
    public GameObject archerPrefab;   // 왕국군 병사 프리팹
    public GameObject arrowPrefab;    // 화살 프리팹
    public Transform royalArmySpawnPoint; // 투명 발판

    private Transform player;
    private Camera mainCam;

    public void Start()
    {
        mainCam = Camera.main;
        if (fadeGroup != null) fadeGroup.alpha = 0; // 시작할 땐 화면이 투명하게

        // 주인공 위치 미리 찾아두기
        GameObject pObj = GameObject.FindGameObjectWithTag("Player");
        if (pObj != null) player = pObj.transform;
    }

    // BossHealth.cs에서 보스가 죽을 때 호출하는 함수
    public void TriggerGameClear()
    {
        Debug.Log("엔딩 시퀀스 시작!"); // 콘솔창에 이게 뜨면 정상 작동 중인 것
        StartCoroutine(EndingSequence());
    }

    IEnumerator EndingSequence()
    {
        // 1. 플레이어 제어권 박탈 (이동 불가, 물리 엔진 고정)
        if (player != null)
        {
            PlayerHealth ph = player.GetComponent<PlayerHealth>();
            if (ph != null) ph.isEnding = true;

            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null) rb.linearVelocity = Vector2.zero;

            MonoBehaviour moveScript = player.GetComponent("PlayerMovement") as MonoBehaviour;
            if (moveScript != null) moveScript.enabled = false;

            Animator pAnim = player.GetComponentInChildren<Animator>();
            if (pAnim != null)
            {
                pAnim.SetBool("Move", false);
                pAnim.Play("IDLE");
            }
        }

        // 보스 처치 후 여운 1.5초
        yield return new WaitForSeconds(1.5f);

        // 엉뚱한 동적 계산 다 버리고, 투명 발판 위치(baseSpawnPos)에서 스폰
        Vector3 baseSpawnPos = royalArmySpawnPoint.position;

        if (archerPrefab != null && royalArmySpawnPoint != null)
        {
            for (int i = 0; i < 3; i++)
            {
                // Y축(높이)은 투명 발판과 완벽히 동일, X축만 안 겹치게 흩어짐
                Vector2 spawnPos = baseSpawnPos + new Vector3(Random.Range(0f, 1.5f), 0, 0);
                Instantiate(archerPrefab, spawnPos, Quaternion.identity);
            }
        }

        yield return new WaitForSeconds(2.5f);

        if (arrowPrefab != null && player != null)
        {
            for (int i = 0; i < 3; i++)
            {
                // 화살도 투명 발판 높이에서 나감
                Vector2 arrowPos = baseSpawnPos + new Vector3(0, Random.Range(-0.2f, 0.2f), 0);
                GameObject arrow = Instantiate(arrowPrefab, arrowPos, Quaternion.identity);

                RoyalArrow arrowScript = arrow.GetComponent<RoyalArrow>();
                if (arrowScript != null) arrowScript.SetTarget(player.position);

                yield return new WaitForSeconds(0.2f);
            }
        }

        yield return new WaitForSeconds(2.0f);
        ;

        // 4. 카메라 이동 (왕국군 쪽으로)
        PlayerCameraFollow camFollow = mainCam.GetComponent<PlayerCameraFollow>();
        if (camFollow != null) camFollow.enabled = false;

        float t = 0;
        Vector3 startPos = mainCam.transform.position;
        // 카메라도 투명 발판 쪽으로 이동
        Vector3 endPos = new Vector3(baseSpawnPos.x, baseSpawnPos.y, startPos.z);

        while (t < 2.0f)
        {
            t += Time.deltaTime;
            mainCam.transform.position = Vector3.Lerp(startPos, endPos, t / 2.0f);
            yield return null;
        }

        yield return new WaitForSeconds(1.0f);

        // 5. 검은 화면 페이드 아웃
        if (fadeGroup != null)
        {
            while (fadeGroup.alpha < 1.0f)
            {
                fadeGroup.alpha += Time.deltaTime * fadeSpeed;
                yield return null;
            }
        }

        yield return new WaitForSeconds(1.5f);

        // 6. 타이틀 씬 복귀
        DataManager.killCount = 0;
        SceneManager.LoadScene("Scenes_Title");


    }
}

 



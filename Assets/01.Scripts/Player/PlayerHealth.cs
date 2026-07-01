using System.Collections;
using UnityEngine;
using UnityEngine.UI; // UI 사용을 위해 필수
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("체력 세팅")]
    public int maxHealth = 10;
    public int currentHealth;

    [Header("UI 세팅")]
    public GameObject[] hpSlots; // 배열 사용

    [Header("피격 무적(i-frame) 세팅")]
    public float iFrameDuration = 1.5f; // 맞았을 때 무적 유지 시간
    private bool isInvincible = false;  // 지금 무적 상태인가?
    private bool isDead = false;

    public bool isEnding = false;

    // 배열로 변경: 그림 조각들을 담음
    private SpriteRenderer[] spriteRenderers;
    private Animator anim;

    public void Start()
    {
        currentHealth = maxHealth;
        // 내 몸을 포함해 자식들(머리, 팔, 다리 등)에 있는 모든 그림 부품을 다 찾아옴
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        UpdateHealthUI(); // 시작할 때 UI 꽉 차 있게 업데이트

        anim = GetComponentInChildren<Animator>();
    }

    // 고블린이 레이를 때릴 때 부를 함수
    public void TakeDamage(int damage)
    {
        // 이미 죽은 상태라면 고블린이 아무리 때려도 이 밑의 코드는 절대 실행 안 됨
        if (isDead) return;
        if (isInvincible) return;

        // 체력 깎기
        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;

        Debug.Log($"아야! 체력 : {currentHealth} / {maxHealth}");

        //  UI 즉시 갱신
        UpdateHealthUI();

        // 사망 or 무적 돌입 체크
        if (currentHealth == 0)
        {
            Die();
        }
        else
        {
            // 안 죽었으면 깜빡깜빡 무적 코루틴 실행
            StartCoroutine(InvincibilityRoutine());
        }
    }

    // 체력바 이미지 갱신 
    public void UpdateHealthUI()
    {
        for (int i = 0; i < hpSlots.Length; i++)
        {
            if (i < currentHealth)
            {
                hpSlots[i].SetActive(true);
            }
            else
            {
                hpSlots[i].SetActive(false);
            }
        }
    }

    // 피격 시 깜빡거리며 무적 상태를 만들어주는 코루틴
    IEnumerator InvincibilityRoutine()
    {
        isInvincible = true;

        // 1.5초 동안 깜빡깜빡 효과 주기 (5번 반복)
        for (int i = 0; i < 5; i++)
        {
            // 모든 조각들을 아예 화면에서 안 보이게 끔 (투명도 대신 스위치 Off)
            foreach (SpriteRenderer sr in spriteRenderers)
            {
                if (sr != null) sr.enabled = false;
            }
            yield return new WaitForSeconds(iFrameDuration / 10f);

            // 모든 조각들을 다시 화면에 보이게 켬 (스위치 On)
            foreach (SpriteRenderer sr in spriteRenderers)
            {
                if (sr != null) sr.enabled = true;
            }
            yield return new WaitForSeconds(iFrameDuration / 10f);
        }

        // 혹시 모르니 한 번 더 확실하게 스위치 켜주고 무적 해제
        foreach (SpriteRenderer sr in spriteRenderers)
        {
            if (sr != null) sr.enabled = true;
        }

        isInvincible = false; // 이제 정상적으로 무적 해제
    }

    public void Die()
    {
        isDead = true;
        Debug.Log("쓰러졌습니다...(게임 오버)");

        // 죽음 애니메이션 실행
        if (anim != null)
        {
            anim.SetTrigger("Death"); 
        }

        // 죽었으니 더 이상 조종하지 못하게 스크립트 체크박스를 꺼버림
        if (GetComponent<PlayerController>() != null) GetComponent<PlayerController>().enabled = false;
        if (GetComponent<PlayerAttack>() != null) GetComponent<PlayerAttack>().enabled = false;

        // 씬 이동 코루틴을 실행
        StartCoroutine(GoToGameOverScene());
    }

    // 1.5초 뒤에 "GameOverScene"으로 넘어가는 타이머
    IEnumerator GoToGameOverScene()
    {
        if (!isEnding)
        {
            yield return new WaitForSeconds(1.5f);
            SceneManager.LoadScene("Scenes_GameOver");
        }
    }

}

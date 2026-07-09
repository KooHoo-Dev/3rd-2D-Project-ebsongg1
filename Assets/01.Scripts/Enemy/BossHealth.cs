using UnityEngine;
using UnityEngine.UI;

public class BossHealth : MonoBehaviour
{
    [Header("보스 체력 세팅")]
    public int maxHealth = 5; // 테스트용으로 낮게 세팅
    public int currentHealth;

    [Header("UI 세팅")]
    public GameObject hpbarUI;
    public Image hpbarFill;

    private Animator anim;
    private BossAI bossAI;
    private bool isDead = false;

    public void Start()
    {
        currentHealth = maxHealth;
        anim = GetComponentInChildren<Animator>();
        bossAI = GetComponent<BossAI>();

        UpdateHealthUI();
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return; // 이미 죽었으면 무시

        currentHealth -= damage;

        // 체력이 0 밑으로 안 내려가게 방어(0 이하가 되면 0으로 고정)
        currentHealth = Mathf.Max(currentHealth, 0);

        Debug.Log($"보스 피격! 남은 체력: {currentHealth} / {maxHealth}");

        UpdateHealthUI();

        if (anim != null) anim.SetTrigger("TakeHit");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void UpdateHealthUI()
    {
        if (hpbarFill != null)
        {
            // 현재 체력 / 최대 체력 = 0.0 ~ 1.0 사이의 비율 구하기
            hpbarFill.fillAmount = (float)currentHealth / maxHealth;
        }
    }

    public void Die()
    {
        isDead = true;
        Debug.Log("보스 처치 완료!");

        if (BGMManager.instance != null)
        {
            BGMManager.instance.FadeOutBGM(1.0f);
        }

        GameManager.instance.AddClearCount();

        if (hpbarUI != null) hpbarUI.SetActive(false);

        if (anim != null) anim.SetTrigger("Death");

        // 죽었으니 보스 AI 조종기 압수
        if (bossAI != null) bossAI.enabled = false;

        // 작업 지시서 연동: 처치한 적의 수 +1 증가
        DataManager.killCount++;
        Debug.Log($"현재 총 처치한 적의 수: {DataManager.killCount}");

        GameClearManager clearManager = FindFirstObjectByType<GameClearManager>();
        if (clearManager != null)
        {
            clearManager.TriggerGameClear();
        }

        // 리지드바디의 시뮬레이션을 꺼서 공중에 고정시킴
        Rigidbody2D rbody = GetComponent<Rigidbody2D>();
        if (rbody != null)
        {
            rbody.simulated = false; // 물리 계산 중지 (중력 무시 + 그 자리 고정)
        }

        // 물리 충돌체 꺼버리기
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        if (rbody != null) rbody.linearVelocity = Vector2.zero; // 밀림 멈추기

        DataManager.killCount++;

        // 보스 처치 완료 카메라 원래대로 복귀
        PlayerCameraFollow camScript = Camera.main.GetComponent<PlayerCameraFollow>();
        if (camScript != null)
        {
            camScript.EndBossMode();
        }


    }
}

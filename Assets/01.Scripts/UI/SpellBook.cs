using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections; //코루틴

public class SpellBook : MonoBehaviour
{
    [Header("SFX")]
    public AudioClip glitchSFX;
    public AudioClip resumeBGM;

    [Header("마법책 스킬 종류")]
    public PlayerAttack.MagicType bookMagicType;

    [Header("타이머 연동")]
    public GameObject timerUI;

    [Header("이 책을 먹었을 때 켤 UI")]
    public GameObject myBookUI;

    [Header("UI 안의 텍스트(글씨) 오브젝트")]
    public TextMeshProUGUI loreTextUI; // 책 안의 글씨를 조종할 변수 추가

    [Header("다회차 텍스트 연출 세팅")]
    public int bookNumber = 1; // 1이면 단어 변환, 2면 문장 추가

    [Header("1번 책 전용 (붉게 물든 전체 텍스트)")]
    [TextArea(3, 5)]
    public string corruptedText = "왕국의 <color=red>심</color>장부인 <color=red>중</color>앙 광장은 언제나 <color=red>백</color>성들의 안식처였다.";

    [Header("2번 책 전용 (밑에 추가될 붉은 문장)")]
    [TextArea(3, 5)]
    public string addedSentence = "<color=red>...하지만 그들은 진실을 몰랐지.</color>";

    [Header("책 닫고 나서 띄울 안내 문구")]
    public string messageAfterClose = "새로운 마법 획득! [Q] 키로 장착/해제하세요.";

    // UI에 직접 적어둔 글씨를 안전하게 보관할 창고
    private string originalTextBackup;

    private bool isTyping = false;

    public void Start()
    {
        if (timerUI != null) timerUI.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerAttack attackScript = collision.GetComponent<PlayerAttack>();

            if (attackScript != null )
            {
                attackScript.UnlockMagic(bookMagicType); // 스토리 주고 ui 열기
            }

            if (myBookUI != null)
            {
                myBookUI.SetActive(true);
                Time.timeScale = 0f;

                // UI에 직접 적어둔 글씨를 스크립트가 알아서 복사해 둠
                if (loreTextUI != null)
                {
                    originalTextBackup = loreTextUI.text;
                }

                // 2회차라면 연출 시작
                if (GameManager.instance != null && GameManager.instance.clearCount >= 1)
                {
                    StartCoroutine(GlitchTextRoutine());
                }
            }

            Debug.Log("마법책 획득");
            if (timerUI != null) timerUI.SetActive(true);

            GetComponent<Collider2D>().enabled = false;
            if (GetComponent<SpriteRenderer>() != null)
            {
                GetComponent<SpriteRenderer>().enabled = false;
            }

        }

    }

    IEnumerator GlitchTextRoutine()
    {
        isTyping = true;

        // 배경음 끄기
        if (GameManager.instance != null && GameManager.instance.clearCount >= 1)
        {
            if (BGMManager.instance != null) BGMManager.instance.StopBGM();
        }

        // 잠깐 원본 글씨를 읽게 두기
        yield return new WaitForSecondsRealtime(2f);

        // 빨간 글씨 뜰 때 효과음
        if (glitchSFX != null && BGMManager.instance != null)
            BGMManager.instance.PlaySFX(glitchSFX);

        if (loreTextUI != null)
        {
            if (bookNumber == 1)
            {
                // 1번 책: 태그를 달아둔 텍스트로 갈아끼우기 (특정 글자만 빨갛게)
                loreTextUI.text = corruptedText;
            }
            else if (bookNumber == 2)
            {
                // 2번 책: UI에 적어둔 원본 글씨 밑에 새로운 붉은 문장을 덧붙이기
                loreTextUI.text = originalTextBackup + "\n\n" + addedSentence;
            }
        }

        isTyping = false;
    }

    public void OnClickCloseBook()
    {
        if (isTyping == true) return;

        // 책 덮었으니 시간 다시 1배속으로 복구
        Time.timeScale = 1f;

        // 양피지 UI 화면에서 끄기
        if (myBookUI != null)
        {
            myBookUI.SetActive(false);
        }

        // 책을 닫을 때 글씨를 원래대로 원상복구
        // (안 그러면 다음에 다른 책을 열었을 때 빨간 글씨가 남아있을 수 있음)
        if (loreTextUI != null && !string.IsNullOrEmpty(originalTextBackup))
        {
            loreTextUI.text = originalTextBackup;
        }

        if (GameManager.instance != null && GameManager.instance.clearCount >= 1)
        {
            if (resumeBGM != null && BGMManager.instance != null)
            {
                BGMManager.instance.PlayBGM(resumeBGM);
            }
        }

        // 주인공을 찾아서 하단 메시지 띄우기
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerAttack attackScript = player.GetComponent<PlayerAttack>();

            // 빈칸이 아닐 때만 메시지를 띄움
            if (attackScript != null && !string.IsNullOrEmpty(messageAfterClose))
            {
                attackScript.ShowSystemMessage(messageAfterClose);
            }
        }
    }

}

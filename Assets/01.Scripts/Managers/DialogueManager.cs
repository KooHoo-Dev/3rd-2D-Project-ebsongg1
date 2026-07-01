using System.Collections;
using TMPro; // TextMeshPro 사용을 위해 필요
using UnityEngine;
using UnityEngine.UI;

public class CinematicDialogueManager : MonoBehaviour
{
    [Header("UI 연결")]
    public GameObject cinematicUI;      // 전체 시네마틱 UI 그룹
    public RectTransform topBar;        // 위쪽 검은 띠
    public RectTransform bottomBar;     // 아래쪽 검은 띠
    public TextMeshProUGUI dialogueText;// 대사 출력 텍스트

    [Header("대화 내용")]
    public string[] bossDialogues;      // 보스의 대사 리스트
    public float textSpeed = 0.05f;     // 글자가 타다닥 나타나는 속도

    private bool isDialogueActive = false;
    private bool isTyping = false;
    private int currentLine = 0;

    public void Start()
    {
        if (cinematicUI != null) cinematicUI.SetActive(false);
    }

    public void Update()
    {
        // 대화 중일 때 스페이스바(또는 마우스 클릭)로 대사 넘기기
        if (isDialogueActive && Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
            {
                // 글씨가 써지는 중이면 한 번에 다 보여줌
                StopAllCoroutines();
                dialogueText.text = bossDialogues[currentLine];
                isTyping = false;
            }
            else
            {
                // 글씨가 다 써졌으면 다음 대사로
                NextLine();
            }
        }
    }

    // 보스전 진입 시 호출할 함수
    public void StartCinematic()
    {
        isDialogueActive = true;
        currentLine = 0;
        cinematicUI.SetActive(true);

        // 시간 정지 (원한다면)
        Time.timeScale = 0f;

        // 대사 시작
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        dialogueText.text = ""; // 텍스트 비우기
                                // 한 글자씩 타다닥 출력 (타이프라이터 효과)
        foreach (char c in bossDialogues[currentLine].ToCharArray())
        {
            dialogueText.text += c;
            // Time.unscaledDeltaTime을 쓰면 Time.timeScale = 0 상태에서도 정상 작동함
            yield return new WaitForSecondsRealtime(textSpeed);
        }

        isTyping = false;
    }

    public void NextLine()
    {
        if (currentLine < bossDialogues.Length - 1)
        {
            currentLine++;
            StartCoroutine(TypeLine());
        }
        else
        {
            EndCinematic();
        }
    }

    public void EndCinematic()
    {
        isDialogueActive = false;
        cinematicUI.SetActive(false); // 시네마틱 종료
        dialogueText.text = "";

        // 멈췄던 시간 다시 흐르게
        Time.timeScale = 1f;

        // 여기서 BossAI를 깨워서 공격을 시작하게 하면 완벽해
    }
}
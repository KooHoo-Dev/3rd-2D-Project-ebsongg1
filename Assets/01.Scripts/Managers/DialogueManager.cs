using System.Collections;
using TMPro; 
using UnityEngine;
using UnityEngine.UI;
using static System.TimeZoneInfo;

public class DialogueManager : MonoBehaviour
{
    [Header("UI 연결")]
    public GameObject cinematicUI;      // 전체 시네마틱 UI 그룹
    public RectTransform topBar;        // 위쪽 검은 띠
    public RectTransform bottomBar;     // 아래쪽 검은 띠
    public TextMeshProUGUI dialogueText;// 대사 출력 텍스트

    [Header("선택지 UI")]
    public GameObject battleStartButton;
    public TextMeshProUGUI battleButtonText;

    [Header("플레이어 제어")]
    public PlayerController playerController;
    
    [Header("보스 제어")]
    public BossAI bossAI;
    public PlayerCameraFollow cameraFollow; // 카메라 스크립트 연결
    public Transform bossRoomCenter;       // 보스방 중앙 위치
    public float bossCameraZoom = 8f;      // 대화 중 확대할 줌아웃 크기

    [Header("대화 내용")]
    public string[] bossDialogues;      // 보스의 대사 리스트
    public float textSpeed = 0.05f;     // 글자가 나타나는 속도

    [Header("시작 설정 (체크 시 활성화 즉시 시작)")]
    public bool playOnStart = false;    // 이 값이 true면 씬이 켜지자마자 대화가 시작됨

    private bool isDialogueActive = false;
    private bool isTyping = false;
    private int currentLine = 0;
    private bool hasStarted = false;    // 대화가 중복으로 시작되는 것을 방지

    public void Start()
    {
        if (cinematicUI != null) cinematicUI.SetActive(false);

        // 자동 시작 옵션이 켜져있다면 즉시 대화 시퀀스 오픈
        if (playOnStart)
        {
            StartCinematic();
        }
    }

    // 보스방 입구에 오브젝트를 만들고 BoxCollider2D (Is Trigger 체크)를 달아두면
    // 플레이어가 그 구역에 걸어 들어오는 순간 자동으로 대화가 시작되는 함수
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (!hasStarted && other.CompareTag("Player"))
        {
            StartCinematic();
        }
    }

    public void Update()
    {
        // 버튼이 화면에 떠 있다면 스페이스바 입력을 무시함
        bool isButtonActive = (battleStartButton != null && battleStartButton.activeSelf);

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
        if (hasStarted) return;
        
        hasStarted = true;

        isDialogueActive = true;
        currentLine = 0;
        if (cinematicUI != null) cinematicUI.SetActive(true);

        // 대화가 시작되자마자 카메라를 보스방 줌아웃 모드로 전환
        if (cameraFollow != null && bossRoomCenter != null)
        {
            cameraFollow.StartBossMode(bossRoomCenter.position, bossCameraZoom);
        }

        // 대화창 전체를 화면에 킴
        if (cinematicUI != null) cinematicUI.SetActive(true);

        // 대화창이 켜지자마자 버튼은 숨김
        if (battleStartButton != null) battleStartButton.SetActive(false);

        // 주인공을 찾아서 완벽하게 물리/애니메이션 굳히기
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            // 물리 속도 완전 제로로 만들기 (미끄러짐 방지)
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }

            // 애니메이터 파라미터 강제 리셋 (달리기 애니메이션 끄기)
            Animator pAnim = player.GetComponentInChildren<Animator>();
            if (pAnim != null)
            {
                pAnim.SetBool("Move", false);      
                
                // 확실하게 대기(IDLE) 애니메이션 상태로 강제 전환
                pAnim.Play("IDLE"); 
            }

            // 물리와 애니메이션을 다 정리한 '후'에 최종적으로 스크립트 끄기
            MonoBehaviour playerController = player.GetComponent("PlayerController") as MonoBehaviour;
            if (playerController != null)
            {
                playerController.enabled = false;
            }
        }

        // 대사 시작
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        dialogueText.text = ""; // 텍스트 비우기
                                // 한 글자씩 출력 (타이프라이터)
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
            dialogueText.text = "";

            // 대사가 끝나고 버튼을 띄울 때 다회차 분기 태우기
            if (battleStartButton != null)
            {
                battleStartButton.SetActive(true);

                // GameManager에서 클리어 횟수를 확인
                if (GameManager.instance.clearCount == 0)
                {
                    Debug.Log("★ 디버그: 현재 1회차입니다. [공격] 버튼 세팅");
                    // 1회차: 평범한 공격 버튼
                    if (battleButtonText != null)
                    {
                        battleButtonText.text = "*공격";
                        battleButtonText.color = Color.white; 
                    }
                }
                else
                {
                    // 2회차 이상: 텍스트 변환 연출 시작
                    if (battleButtonText != null)
                    {
                        Debug.Log("★ 디버그: 현재 2회차 이상입니다! 글리치 코루틴 시작!");
                        StartCoroutine(GlitchButtonText());
                    }
                }
            }
            else
            {
                EndCinematic();
            }
        }
    }

    public void OnClickBattleStart()
    {
        if (battleStartButton != null) battleStartButton.SetActive(false); // 버튼 다시 숨기기
        EndCinematic(); // 대화창 끄고 전투 시작
    }

    public void EndCinematic()
    {
        isDialogueActive = false;
        if (cinematicUI != null) cinematicUI.SetActive(false);
        dialogueText.text = "";

        if (bossAI != null)
        {
            // 대화가 끝난 직후 보스가 곧바로 플레이어를 쫓아가도록 상태 강제 변경 (시점 초기화 방지)
            // BossAI 상태 변수명이나 구조에 맞춰 대문자/이름을 살짝 조절해야 할 수 있음
            bossAI.currentState = BossAI.BossState.Move;
            bossAI.enabled = true;
        }

        playerController.enabled = true;
    }

    IEnumerator GlitchButtonText()
    {
        Debug.Log("★ 디버그: GlitchButtonText 코루틴 진입 성공!");
        // 1. 처음엔 1회차처럼 똑같이 [ 공격 ]으로 시작해서 안심시킴
        battleButtonText.text = "*공격";
        battleButtonText.color = Color.white;

        // 0.5초 대기 (플레이어가 '어? 공격버튼이네' 하고 인식할 찰나의 시간)
        yield return new WaitForSecondsRealtime(1.5f);

        // 2. 글자가 지지직거리며 깨지는 느낌
        battleButtonText.text = "*공..";
        yield return new WaitForSecondsRealtime(1.5f);

        battleButtonText.text = "ㄱ..";
        yield return new WaitForSecondsRealtime(1.5f);

        // 최종적으로 색상이 붉은색(또는 어두운색)으로 변하며 침묵의 [...] 으로 변경
        battleButtonText.text = "<color=red>* ... </color>";

        // (선택 사항) 여기서 소름 돋는 치직! 하는 효과음을 재생해도 엄청 멋있음
    }
}
using System.Collections;
using System.Collections.Generic; // List를 쓰기 위해
using TMPro; 
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
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

    [Header("브금 설정")]
    public AudioClip bossBattleBGM;
    public AudioClip choiceChangeSFX;
    public AudioClip secretEndingBGM;
    public AudioClip endingBookBGM;

    [Header("진엔딩 연출용 재료")]
    public GameObject meteorPrefab;
    public Transform royalArmySpawnPoint; // 왕국군이 스폰될 중심점
    public GameObject archerPrefab;       // 왕국군 프리팹

    [Header("진엔딩 화면 연출 (블랙아웃)")]
    public CanvasGroup blackScreen;       // 화면을 덮을 패널 (CanvasGroup 컴포넌트 필요)
    public GameObject ruinedBackground;   // 배경 이미지 오브젝트
    public GameObject endingBookUI;       
    public CanvasGroup endingBookCanvasGroup;

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

    // 보스방 입구에 오브젝트(BoxCollider2D)
    // 플레이어가 그 구역에 들어오는 순간 자동으로 대화가 시작
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

        // 보스 대화가 시작되는 바로 이 순간에 브금을 끔
        if (BGMManager.instance != null) BGMManager.instance.FadeOutBGM(1.5f);

        // 대화가 시작되자마자 카메라를 보스방 줌아웃 모드로 전환
        if (cameraFollow != null && bossRoomCenter != null)
        {
            cameraFollow.StartBossMode(bossRoomCenter.position, bossCameraZoom);
        }

        // 대화창 전체를 화면에 킴
        if (cinematicUI != null) cinematicUI.SetActive(true);

        // 대화창이 켜지자마자 버튼은 숨김
        if (battleStartButton != null) battleStartButton.SetActive(false);

        // 주인공을 찾아서 물리/애니메이션 굳히기
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
            // (Time.unscaledDeltaTime을 쓰면 Time.timeScale = 0 상태에서도 정상 작동)
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

        if (GameManager.instance.clearCount == 0)
        {
            // 1회차: 원래 하던 대로 대화창 끄고 전투 시작
            if (bossBattleBGM != null) BGMManager.instance.PlayBGM(bossBattleBGM);
            Debug.Log("1회차: 보스전 시작!");
            Time.timeScale = 1f;
            EndCinematic();
        }
        else
        {
            // 2회차 이상: 전투 대신 진엔딩 자동 연극 시작
            Debug.Log("2회차 진입: 비밀 엔딩 연출 시작!");
            StartCoroutine(SecretEndingRoutine());
        }
    }

    public void EndCinematic()
    {
        isDialogueActive = false;
        if (cinematicUI != null) cinematicUI.SetActive(false);
        dialogueText.text = "";

        if (bossAI != null)
        {
            // 대화가 끝난 직후 보스가 곧바로 플레이어를 쫓아가도록 상태 강제 변경 (시점 초기화 방지)
            bossAI.currentState = BossAI.BossState.Move;
            bossAI.enabled = true;
        }

        playerController.enabled = true;
    }

    IEnumerator GlitchButtonText()
    {

        Debug.Log("★ 디버그: GlitchButtonText 코루틴 진입 성공!");
        // 처음엔 1회차처럼 똑같이 [ 공격 ]으로 시작
        battleButtonText.text = "*공격";
        battleButtonText.color = Color.white;

        // 몇초 대기 (인식할 찰나의 시간)
        yield return new WaitForSecondsRealtime(1.5f);

        battleButtonText.text = "*공..";
        yield return new WaitForSecondsRealtime(1.5f);

        battleButtonText.text = "*ㄱ..";
        yield return new WaitForSecondsRealtime(1.5f);

        if (choiceChangeSFX != null && BGMManager.instance != null)
        {
            BGMManager.instance.PlaySFX(choiceChangeSFX);
        }

        // 색상이 붉은색으로 변하며 [...] 으로 변경
        battleButtonText.text = "<color=red>* ... </color>";

    }


    // 2회차 엔딩 자동 연출 코루틴
    IEnumerator SecretEndingRoutine()
    {

        if (secretEndingBGM != null)
        {
            BGMManager.instance.PlayBGM(secretEndingBGM);
        }

        battleStartButton.SetActive(false);
        Time.timeScale = 1f;

        // 카메라 추적 기능 강제로 끄기 (고무줄 현상 방지)
        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            MonoBehaviour camScript = mainCam.GetComponent("PlayerCameraFollow") as MonoBehaviour;
            if (camScript != null) camScript.enabled = false;
        }

        // 왕국군 스폰 
        List<GameObject> armyList = new List<GameObject>();
        Vector3 baseSpawnPos = royalArmySpawnPoint.position;

        if (archerPrefab != null && royalArmySpawnPoint != null)
        {
            for (int i = 0; i < 5; i++)
            {
                Vector3 spawnPos = baseSpawnPos + new Vector3(Random.Range(-1.5f, 1.5f), 0, 0);
                GameObject archer = Instantiate(archerPrefab, spawnPos, Quaternion.identity);
                armyList.Add(archer);
            }
        }

        yield return new WaitForSeconds(1.5f);

        // 카메라가 왕국군 쪽으로 부드럽게 이동
        Vector3 startCamPos = mainCam.transform.position;
        Vector3 targetCamPos = new Vector3(baseSpawnPos.x, startCamPos.y, startCamPos.z);

        float camMoveTime = 0f;
        while (camMoveTime < 1f)
        {
            camMoveTime += Time.deltaTime;
            mainCam.transform.position = Vector3.Lerp(startCamPos, targetCamPos, camMoveTime);
            yield return null;
        }

        // 머리 위에 운석 추락
        if (meteorPrefab != null)
        {
            foreach (GameObject archer in armyList)
            {
                if (archer != null)
                {
                    Vector3 skyPos = archer.transform.position + new Vector3(0, 5f, 0);
                    Instantiate(meteorPrefab, skyPos, Quaternion.identity);
                }
            }
        }

        yield return new WaitForSeconds(1.0f);

        // 운석 직격, 왕국군 전멸 
        foreach (GameObject archer in armyList)
        {
            if (archer != null) archer.transform.rotation = Quaternion.Euler(0, 0, -90f);
        }

        yield return new WaitForSeconds(2f);

        // 카메라 다시 주인공 쪽으로 원상복귀
        camMoveTime = 0f;
        while (camMoveTime < 1f)
        {
            camMoveTime += Time.deltaTime;
            mainCam.transform.position = Vector3.Lerp(targetCamPos, startCamPos, camMoveTime);
            yield return null;
        }

        yield return new WaitForSeconds(1f);

        //보스 대사
        dialogueText.text = "...";
        yield return new WaitForSeconds(2f);

        dialogueText.text = "흠...";
        yield return new WaitForSeconds(2f);

        dialogueText.text = "...";
        yield return new WaitForSeconds(2f);

        dialogueText.text = "아무래도 우리의 목적은 같아 보이는군.";
        yield return new WaitForSeconds(3f);

        // 화면부터 먼저 서서히 까맣게 덮음
        float fadeAlpha = 0f;
        if (blackScreen != null)
        {
            blackScreen.gameObject.SetActive(true);
            while (fadeAlpha < 1f)
            {
                fadeAlpha += Time.deltaTime / 1.5f;
                blackScreen.alpha = fadeAlpha;
                yield return null;
            }
        }

        // 화면이 까말 때, 뒤에서 시네마틱 UI 끄기 
        dialogueText.transform.parent.gameObject.SetActive(false);

        // 까만 상태로 1.5초 유지
        yield return new WaitForSeconds(1.5f);

        // 까만 화면 뒤에서 배경과 책 UI 켜기 (책은 투명도 0으로 시작)
        if (ruinedBackground != null) ruinedBackground.SetActive(true);
        if (endingBookUI != null)
        {
            endingBookUI.SetActive(true);
            endingBookUI.transform.SetAsLastSibling();
            if (endingBookCanvasGroup != null) endingBookCanvasGroup.alpha = 0f; 
        }

        // 까만 화면 서서히 걷어내기
        while (fadeAlpha > 0f)
        {
            fadeAlpha -= Time.deltaTime / 1.5f;
            if (blackScreen != null) blackScreen.alpha = fadeAlpha;
            yield return null;
        }

        // 배경이 다 나오면, 책 UI 서서히 띄우기
        if (endingBookCanvasGroup != null)
        {
            BGMManager.instance.PlayBGM(endingBookBGM);

            float bookAlpha = 0f;
            while (bookAlpha < 1f)
            {
                bookAlpha += Time.deltaTime / 1.5f;
                endingBookCanvasGroup.alpha = bookAlpha;
                yield return null;
            }
        }
    }

    public void GoToTitleScene()
    {
        // 팍 넘어가지 않고, 서서히 까매지는 코루틴 시작
        StartCoroutine(FadeOutToTitleRoutine());
    }

    IEnumerator FadeOutToTitleRoutine()
    {
        // 다시 까만 패널을 서서히 진하게 만듦
        if (blackScreen != null)
        {
            blackScreen.transform.SetAsLastSibling(); // 까만 화면을 책보다 맨 앞으로 끄집어냄
            float fadeAlpha = 0f;
            while (fadeAlpha < 1f)
            {
                fadeAlpha += Time.deltaTime / 1.5f;
                blackScreen.alpha = fadeAlpha;
                yield return null;
            }
        }

        // 화면이 완전히 까매지면 타이틀로 이동
        Time.timeScale = 1f;
        SceneManager.LoadScene("Scenes_Title");
    }

  
}
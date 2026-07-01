using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BossAI : MonoBehaviour
{
    // 보스의 뇌 구조(상태 정의)
    public enum BossState
    {
        Idle,       // 대기
        Move,       // 거리 조절 (이동)
        RangeAttack,// 패턴 1: 유령탄 쏘기
        MeleeAttack,// 패턴 2: 지팡이 치기
        CastMagic,  // 패턴 3: 흑염 기둥 소환
        CastMeteor,
        Dead        // 사망
    }

    [Header("현재 상태 (인스펙터에서 확인용)")]
    public BossState currentState = BossState.Idle;

    [Header("타겟 세팅")]
    public Transform player;

    [Header("거리 및 속도 세팅")]
    public float moveSpeed = 2f;
    public float meleeRange = 3.5f;       // 패턴 2: 근접 공격 발동 거리 (코앞)
    public float rangeAttackRange = 7f; // 패턴 1: 유령탄 발동 거리 (원거리)
    public float detectRange = 12f;     // 인식 거리 (이 안으로 오면 전투 시작)

    [Header("공격력 세팅")]
    public int meleeDamage = 2; // 지팡이로 맞았을 때 깎일 체력

    private bool isAttacking = false;   // 지금 공격 중인지 체크하는 스위치
    private Animator anim;

    [Header("발사 세팅")]
    public GameObject projectilePrefab; 
    public Transform firePoint;

    [Header("필살기 세팅")]
    public GameObject warningPrefab;    
    public GameObject flamePillarPrefab;
    public float magicCooldown = 6f;    
    private float magicTimer = 0f;

    [Header("운석 패턴 세팅")]
    public Transform platformPoint;  
    public GameObject meteorPrefab;

    private float groundY; // 보스가 원래 있던 땅바닥 높이 기억용

    [Header("카메라 연출")]
    public Transform bossRoomCenter; // 보스방 중앙 위치
    public float bossCameraZoom = 8f; // 줌아웃 크기 (기본이 5라면 8~10 정도로 설정)

    public void Start()
    {
        anim = GetComponentInChildren<Animator>();

        // 씬에 있는 플레이어를 자동으로 찾아옴
        // Tag가 "Player"로 설정되어 있어야 함
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        // 보스가 처음 서 있던 바닥의 Y축 높이를 기억해둔다 (나중에 내려올 때 씀)
        groundY = transform.position.y;
   
}

    public void Update()
    {
        // 타겟이 없거나, 보스가 죽었으면 아무것도 안 함
        if (player == null || currentState == BossState.Dead)
        {
            ManageHealthBar(false);
            ManageCameraMode(false);
            return;
        }

        // 보스가 플레이어를 발견하고 전투 중일 때만 필살기 타이머가 흐르게 제한
        // 이제 Idle 상태나 CastMagic중에는 시간이 멈춤
        if (currentState == BossState.Move || currentState == BossState.RangeAttack || currentState == BossState.MeleeAttack)
        {
            magicTimer += Time.deltaTime;
        }

        // 쿨타임이 찼고, 현재 다른 공격 중이 아닐 때만 필살기 발동
        if (magicTimer >= magicCooldown && !isAttacking && currentState != BossState.Idle)
        {
            // 50% 확률로 불기둥, 50% 확률로 운석 패턴 발동
            int randomUlt = Random.Range(0, 2);
            if (randomUlt == 0)
                StartCoroutine(CastMagicRoutine());
            else
                StartCoroutine(CastMeteorRoutine());
        }

        // 상태에 따른 실시간 체력바 자동 제어
        // 대기(Idle) 상태일 때는 무조건 끄고, 그 외의 모든 전투 상태(이동, 평타, 필살기 등)일 때는 무조건 킴
        if (currentState == BossState.Idle)
        {
            ManageHealthBar(false);
            ManageCameraMode(false);
        }
        else
        {
            ManageHealthBar(true);
            ManageCameraMode(true);
        }

        // 뇌 상태(currentState)에 따라 행동을 완벽하게 분리하는 스위치
        switch (currentState)
        {
            case BossState.Idle:
                UpdateIdle();
                break;
            case BossState.Move:
                UpdateMove();
                break;
            // 공격 중일 때는 Update에서 따로 제어할 게 없음 (코루틴이 알아서 함)
            case BossState.RangeAttack:
            case BossState.MeleeAttack:
            case BossState.CastMagic:
            case BossState.CastMeteor:
                break;
        }

        if (player == null || currentState == BossState.Dead) return;

        //Debug.Log("현재 거리: " + Vector2.Distance(transform.position, player.position));
    }

    public void ManageHealthBar(bool isActive)
    {
        BossHealth bossHealth = GetComponent<BossHealth>();
        if (bossHealth != null && bossHealth.hpbarUI != null)
        {
            // 현재 슬롯의 활성화 상태와 다를 때만 SetActive를 실행하여 과부하 방지
            if (bossHealth.hpbarUI.activeSelf != isActive)
            {
                bossHealth.hpbarUI.SetActive(isActive);
            }
        }
    }

    // 카메라 줌아웃/줌인을 안전하게 처리해주는 전용 제어 함수
    public void ManageCameraMode(bool isBossMode)
    {
        PlayerCameraFollow camScript = Camera.main.GetComponent<PlayerCameraFollow>();
        if (camScript != null)
        {
            if (isBossMode && bossRoomCenter != null)
            {
                // 전투 상태면 보스방 중앙 비추기
                camScript.StartBossMode(bossRoomCenter.position, bossCameraZoom);
            }
            else
            {
                // 대기 상태거나 도망쳤으면 레이 추적 모드로 복귀
                camScript.EndBossMode();
            }
        }
    }

    public void UpdateIdle()
    {
        if (anim != null) anim.SetBool("isRunning", false); // 대기 애니메이션

        // 인식 거리 안에 들어오면 전투 시작 -> Move 상태로 변경
        float dist = Vector2.Distance(transform.position, player.position);
        if (dist <= detectRange)
        {
            currentState = BossState.Move;
        }

    }

    public void UpdateMove()
    {
        if (isAttacking) return; // 공격 중이면 멈추기

        float dist = Vector2.Distance(transform.position, player.position);

        // 플레이어가 있는 방향 쳐다보기 (자동 뒤집기 기능)
        LookAtPlayer();

        // 거리 계산해서 패턴 발동
        if (dist <= meleeRange)
        {
            // 코앞까지 다가왔다 -> 근접 지팡이 공격
            StartCoroutine(MeleeAttackRoutine());
        }
        else if (dist <= rangeAttackRange)
        {
            // 적정 사거리에 들어왔다 -> 유령탄 발사
            StartCoroutine(RangeAttackRoutine());
        }
        else if (dist <= detectRange)
        {
            // 인식 거리 안에 있으면 쫓아가기
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(player.position.x, transform.position.y), moveSpeed * Time.deltaTime);
            if (anim != null) anim.SetBool("isRunning", true);
        }
        else
        {
            // 플레이어가 너무 멀리 도망쳐서 인식 범위를 벗어났을 때 (Idle로 복귀)
            currentState = BossState.Idle;
            if (anim != null) anim.SetBool("isRunning", false);

            // 플레이어가 도망쳤으니 화면 위에 떠 있던 보스 체력바도 다시 부드럽게 숨겨주기
            BossHealth bossHealth = GetComponent<BossHealth>();
            if (bossHealth != null && bossHealth.hpbarUI != null)
            {
                bossHealth.hpbarUI.SetActive(false);
            }
        }
    }

    // 유령탄 발사 코루틴
    IEnumerator RangeAttackRoutine()
    {
        isAttacking = true;
        currentState = BossState.RangeAttack; // 상태 고정 (버그 방지)
        if (anim != null) anim.SetBool("isRunning", false);

        Debug.Log("패턴 1: 원거리 유령탄 발사!");
        if (anim != null) anim.SetTrigger("RangeAttack");

        // 지팡이를 휘두르고 유령이 날아가는 모션 타이밍까지 잠깐 대기
        yield return new WaitForSeconds(0.6f);

        // 총알 생성 
        if (projectilePrefab != null && firePoint != null)
        {
            Instantiate(projectilePrefab, firePoint.position, transform.rotation);
        }

        // 남은 애니메이션 시간 대기
        yield return new WaitForSeconds(1.4f);

        isAttacking = false;
        currentState = BossState.Move; // 쏘고 나면 다시 이동 상태로
    }

    // 패턴 2: 근접 지팡이 넉백 공격 코루틴
    IEnumerator MeleeAttackRoutine()
    {
        isAttacking = true;
        currentState = BossState.MeleeAttack;
        if (anim != null) anim.SetBool("isRunning", false);

        Debug.Log("패턴 2: 저리 가! 지팡이 후려치기!");
        if (anim != null) anim.SetTrigger("MeleeAttack");

        yield return new WaitForSeconds(0.5f);

        // 아직도 사거리 안에 있다면 데미지를 줌 (피했으면 안 맞음)
        float dist = Vector2.Distance(transform.position, player.position);
        if (dist <= meleeRange)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(meleeDamage);
            }
        }

        // 공격 후 딜레이 (나머지 시간)
        yield return new WaitForSeconds(1.0f);

        isAttacking = false;
        currentState = BossState.Move;
    }

    // 흑염 기둥 소환 코루틴
    IEnumerator CastMagicRoutine()
    {
        isAttacking = true;
        currentState = BossState.CastMagic;
        magicTimer = 0f; // 쿨타임 초기화

        if (anim != null) anim.SetBool("isRunning", false);

        Debug.Log("패턴 3: 흑염 기둥 시전!"); 

        // 지금 플레이어가 서 있는 발밑 위치 기억하기
        Vector3 targetPos = player.position;
        targetPos.y -= 0.5f; // 발밑으로 살짝 내림 (캐릭터 크기에 맞게 조절)

        // 그 자리에 경고 장판 생성
        GameObject warning = Instantiate(warningPrefab, targetPos, Quaternion.identity);

        // 1.5초 기다려줌
        yield return new WaitForSeconds(1.5f);

        // 시간이 지나면 장판은 지우고, 그 자리에 진짜 불기둥 
        Destroy(warning);
        GameObject pillar = Instantiate(flamePillarPrefab, targetPos, Quaternion.identity);

        // 불기둥이 화면에 유지되는 시간 
        yield return new WaitForSeconds(1.0f);
        if (pillar != null) Destroy(pillar);

        // 애니메이션 후딜레이 좀 더 기다렸다가 다시 걷기 모드로
        yield return new WaitForSeconds(0.5f);
        isAttacking = false;
        currentState = BossState.Move;
    }

    // 공중 발판으로 도약 후 운석 소환
    IEnumerator CastMeteorRoutine()
    {
        isAttacking = true;
        currentState = BossState.CastMeteor;
        magicTimer = 0f;

        if (anim != null) anim.SetBool("isRunning", false);
        Debug.Log("패턴 4: 공중 운석 소환!");

        // 공중 발판으로 점프(이동)
        if (platformPoint != null)
        {
            // (점프 애니메이션이 있다면 여기서 실행)
            // if (anim != null) anim.SetTrigger("Jump");

            transform.position = platformPoint.position;
        }

        yield return new WaitForSeconds(1.0f);

        // 머리 위 높은 곳(하늘)에서 운석을 여러 개 떨어뜨림
        for (int i = 0; i < 5; i++) // 운석 5개 연속 발사
        {
            // 현재 X좌표 + 하늘(Y + 10) 위치를 타겟으로
            Vector2 spawnPos = new Vector2(player.position.x, player.position.y + 10f);

            // 약간의 오차(랜덤)를 줘서 플레이어 주변에 떨어지게 함
            spawnPos.x += Random.Range(-2f, 2f);

            Instantiate(meteorPrefab, spawnPos, Quaternion.identity);

            yield return new WaitForSeconds(0.4f); // 0.4초 간격으로 투하
        }

        // 마법 다 쏘고 사이 시간
        yield return new WaitForSeconds(1.5f);

        // 다시 원래 있던 바닥으로 착지
        // 원래 X 위치는 유지하고, Y 위치만 아까 기억해둔 바닥 높이로 복귀
        transform.position = new Vector2(transform.position.x, groundY);

        // 착지 후 딜레이
        yield return new WaitForSeconds(0.5f);

        isAttacking = false;
        currentState = BossState.Move;
    }

    public void LookAtPlayer()
    {
        // 플레이어가 보스보다 왼쪽에 있는지, 오른쪽에 있는지 판별해서 몸을 뒤집음
        if (player.position.x < transform.position.x)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0); // 왼쪽 보기
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);   // 오른쪽 보기
        }
    }

    private void OnDrawGizmos()
    {
        // 노란색: 원거리 사거리
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rangeAttackRange);

        // 빨간색: 근거리 사거리
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, meleeRange);
    }

}

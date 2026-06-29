using UnityEngine;
using System.Collections;

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
        Dead        // 사망
    }

    [Header("현재 상태 (인스펙터에서 확인용)")]
    public BossState currentState = BossState.Idle;

    [Header("타겟 세팅")]
    public Transform player;

    [Header("거리 및 속도 세팅")]
    public float moveSpeed = 2f;
    public float meleeRange = 2f;       // 패턴 2: 근접 공격 발동 거리 (코앞)
    public float rangeAttackRange = 7f; // 패턴 1: 유령탄 발동 거리 (원거리)
    public float detectRange = 12f;     // 인식 거리 (이 안으로 오면 전투 시작)

    private bool isAttacking = false;   // 지금 공격 중인지 체크하는 스위치
    private Animator anim;

    void Start()
    {
        anim = GetComponentInChildren<Animator>();

        // 씬에 있는 플레이어를 자동으로 찾아옴
        // Tag가 "Player"로 설정되어 있어야 함
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    void Update()
    {
        // 타겟이 없거나, 보스가 죽었으면 아무것도 안 함
        if (player == null || currentState == BossState.Dead) return;

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
                break;
        }
    }

    void UpdateIdle()
    {
        // 인식 거리 안에 들어오면 전투 시작 -> Move 상태로 변경
        float dist = Vector2.Distance(transform.position, player.position);
        if (dist <= detectRange)
        {
            currentState = BossState.Move;
        }
    }

    void UpdateMove()
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
            // 아직 멀었다 -> 다가가기
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(player.position.x, transform.position.y), moveSpeed * Time.deltaTime);
            // 걷기 애니메이션 (나중에 파라미터 이름 맞춰서 수정)
            // if (anim != null) anim.SetBool("Move", true);
        }
        else
        {
            // 너무 멀어지면 다시 대기 상태로
            currentState = BossState.Idle;
            // if (anim != null) anim.SetBool("Move", false);
        }
    }

    // 유령탄 발사 코루틴
    IEnumerator RangeAttackRoutine()
    {
        isAttacking = true;
        currentState = BossState.RangeAttack; // 상태 고정 (버그 방지)

        Debug.Log("패턴 1: 원거리 유령탄 발사!");
        // if (anim != null) anim.SetTrigger("RangeAttack"); 

        // 마법 쏘는 애니메이션이 끝날 때까지 딜레이 주기 (임시로 2초)
        yield return new WaitForSeconds(2f);

        isAttacking = false;
        currentState = BossState.Move; // 쏘고 나면 다시 이동 상태로
    }

    // 패턴 2: 근접 지팡이 넉백 공격 코루틴
    IEnumerator MeleeAttackRoutine()
    {
        isAttacking = true;
        currentState = BossState.MeleeAttack;

        Debug.Log("패턴 2: 저리 가! 지팡이 후려치기!");
        // if (anim != null) anim.SetTrigger("MeleeAttack");

        yield return new WaitForSeconds(1.5f);

        isAttacking = false;
        currentState = BossState.Move;
    }

    void LookAtPlayer()
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

}

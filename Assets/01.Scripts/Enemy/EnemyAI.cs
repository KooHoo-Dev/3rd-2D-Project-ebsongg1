using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("SFX")]
    public AudioClip goblinAttackSFX;

    [Header("이동 세팅")]
    public float patrolSpeed = 2f;         // 이동 속도
    public bool movingRight = false;

    [Header("추적(Run) 세팅")]
    public float runSpeed = 4.5f;        // 발견했을 때 달려오는 속도
    public float detectRange = 5f;       // 플레이어 감지 시야 거리
    private Transform player;            // 위치를 기억할 빈칸

    [Header("공격 세팅")]
    public float attackRange = 1.2f;     // 공격이 닿는 좁은 사거리
    public float attackCooldown = 2.0f;  // 한 번 때리고 다음 공격까지 기다리는 시간
    private float attackTimer = 0f;      // 쿨타임 초시계

    [Header("지형 감지 (Raycast)")]
    public Transform checkPoint;         // 고블린 발 앞쪽에 달아줄 센서 위치
    public float checkDistance = 0.5f;   // 레이저를 쏠 거리
    public LayerMask groundLayer;        // 땅으로 인식할 레이어 (Tilemap 등)

    private Rigidbody2D rbody;
    private Animator anim;

    public void Start()
    {
        rbody = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        // 게임이 시작되면 태그가 "Player"인 오브젝트를 스스로 찾아서 기억함
        GameObject target = GameObject.FindGameObjectWithTag("Player");
        if (target != null)
        {
            player = target.transform;
        }

        // 코드로 시작 방향을 잡기
        if (movingRight == false)
        {
            // 왼쪽으로 가야 하면 시작하자마자 몸을 180도 돌려버림
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
    }

    public void Update()
    {
        // 공격 쿨타임 초시계는 매 프레임마다 줄어듬
        if (attackTimer > 0f)
        {
            attackTimer -= Time.deltaTime;
        }

        float dist = 100f; // 플레이어와의 기본 거리 (아주 멀게 세팅)
        if (player != null)
        {
            dist = Vector2.Distance(transform.position, player.position);
        }

        // 행동 결정: 거리에 따라 3가지 상태로 나뉨
        if (dist <= attackRange)
        {
            // [공격 모드] 사거리 안에 들어왔다
            rbody.linearVelocity = new Vector2(0f, rbody.linearVelocity.y); // 제자리에 멈춤
            if (anim != null) anim.SetBool("isRunning", false); // 뛰기 애니메이션 끔

            // 쿨타임이 다 돌았다면 공격
            if (attackTimer <= 0f)
            {
                AttackPlayer();
            }
        }
        else if (dist <= detectRange)
        {
            // [추적 모드] 시야에는 들어왔지만 아직 멀다
            bool playerIsRight = player.position.x > transform.position.x;
            if (movingRight != playerIsRight)
            {
                Flip();
            }

            rbody.linearVelocity = new Vector2(movingRight ? runSpeed : -runSpeed, rbody.linearVelocity.y);
            if (anim != null) anim.SetBool("isRunning", true);
        }
        else
        {
            // [순찰 모드] 평화롭게 걷기
            RaycastHit2D groundInfo = Physics2D.Raycast(checkPoint.position, Vector2.down, checkDistance, groundLayer);
            Vector2 forwardDirection = movingRight ? Vector2.right : Vector2.left;
            RaycastHit2D wallInfo = Physics2D.Raycast(checkPoint.position, forwardDirection, checkDistance, groundLayer);

            if (groundInfo.collider == false || wallInfo.collider == true)
            {
                Flip();
            }

            rbody.linearVelocity = new Vector2(movingRight ? patrolSpeed : -patrolSpeed, rbody.linearVelocity.y);
            if (anim != null) anim.SetBool("isRunning", true);
        }


    }

    public void AttackPlayer()
    {
        BGMManager.instance.PlaySFX(goblinAttackSFX);

        // 초시계 리셋
        attackTimer = attackCooldown;

        // 공격 애니메이션 실행
        if (anim != null)
        {
            anim.SetTrigger("attack");
        }

        // PlayerHealth 부품을 찾아서 체력을 10 깎기
        if (player != null)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(1);
            }
        }
    }

    public void Flip()
    {
        movingRight = !movingRight;
        // 주인공한테 썼던 180도 회전 코드 
        transform.Rotate(0f, 180f, 0f);
    }

    // 유니티 에디터 화면에서 레이저가 잘 나가는지 눈으로 보기 위한 기즈모
    private void OnDrawGizmos()
    {
        if (checkPoint != null)
        {
            Gizmos.color = Color.red;
            // 아래로 쏘는 레이저 시각화
            Gizmos.DrawLine(checkPoint.position, checkPoint.position + Vector3.down * checkDistance);
            // 앞으로 쏘는 레이저 시각화
            Vector3 forward = movingRight ? Vector3.right : Vector3.left;
            Gizmos.DrawLine(checkPoint.position, checkPoint.position + forward * checkDistance);
        }

        // 추적 감지 범위(노란색 동그라미)를 그려줌
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRange);

        // 공격 사거리 기즈모 추가 (빨간색 좁은 원)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}

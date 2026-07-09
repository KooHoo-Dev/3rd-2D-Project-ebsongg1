using UnityEngine.Tilemaps;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerController : MonoBehaviour
{
    [Header("이동 및 점프 설정")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    [Header("바닥 감지 설정")]
    public Transform groundCheck; // 발밑 위치 체크 오브젝트
    public float groundCheckRadius = 0.2f; // 바닥을 감지할 반경
    public LayerMask groundLayer; // 어떤 레이어를 바닥으로 인식할 지

    private Rigidbody2D rbody;
    private float moveInput;
    private bool isGrounded;
    private bool facingRight = true;

    private Animator anim;

    public void Start()
    {
        rbody = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
    }

    public void Update()
    {
        if (GetComponent<PlayerHealth>().isEnding) return;

        moveInput = Input.GetAxisRaw("Horizontal"); // 좌우 입력 받기

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer); // 바닥에 닿아있는지 원형 범위로 체크

        if(Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rbody.linearVelocity = new Vector2(rbody.linearVelocity.x, jumpForce);
        }

        if (moveInput > 0 && !facingRight) Flip();
        else if(moveInput < 0 && facingRight) Flip();

        // 좌우 입력값(X)이 0이 아니면 움직이는 중
        if (anim != null)
        {
            // 키보드 좌우 방향키를 누르고 있다면 true, 손을 떼면 false
            bool isMoving = (Input.GetAxisRaw("Horizontal") != 0);
            anim.SetBool("Move", isMoving);
        }


    }

    public void FixedUpdate()
    {
        rbody.linearVelocity = new Vector2(moveInput * moveSpeed, rbody.linearVelocity.y);
    }

    public void Flip()
    {
        facingRight = !facingRight;

        transform.Rotate(0f, 180f, 0f);
       
    }
}

using System.Collections;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [Header("이동 및 점프")]
    public float speed = 5f; // Translate가 아니므로 단위를 크게 변경 (예: 5)
    public float jumpPower = 10f; // Rigidbody 속도에 맞게 값 조절 필요 (예: 10)
    private int maxJumpCount = 2;
    public int currentJumpCount = 0;
    public bool isGround = true;

    [Header("대시")]
    public float dashPower = 24f;
    public float dashTime = 0.2f;
    public float dashingCooldown = 1f;
    bool canDash = true;
    bool dashing = false;

    [Header("스킬")]
    public GameObject mask; // 자식 오브젝트로 설정하고 Rigidbody 제거

    [Header("컴포넌트 및 상태")]
    public Rigidbody2D rigid;
    private float moveInput; // 좌우 입력을 저장할 변수
    private bool isFacingRight = true; // 현재 바라보는 방향

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // --- 입력 (Input)은 Update에서 처리 ---

        // 1. 좌우 이동 입력 받기
        moveInput = 0f;
        if (Input.GetKey(KeyCode.RightArrow))
        {
            moveInput = 1f;
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            moveInput = -1f;
        }

        // 2. 점프 입력 (물리와 상관없는 횟수 체크)
        if (Input.GetKeyDown(KeyCode.Space) && currentJumpCount < maxJumpCount)
        {
            Jump();
        }

        // 3. 대시 입력 (코루틴 시작)
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash()); // StartCoroutine으로 호출!
        }

        // 4. 스킬(마스크) 토글
        if (Input.GetKeyDown(KeyCode.Q))
        {
            playerAttack();
        }

        // 5. 캐릭터 방향 뒤집기 (대시 중이 아닐 때만)
        if (!dashing)
        {
            Flip();
        }
    }

    void FixedUpdate()
    {
        // --- 물리 (Physics)는 FixedUpdate에서 처리 ---

        // 대시 중이 아닐 때만 좌우 이동 속도를 적용
        if (!dashing)
        {
            rigid.linearVelocity = new Vector2(moveInput * speed, rigid.linearVelocity.y);
        }
    }

    void Jump()
    {
        // 현재 수평 속도(rigid.velocity.x)를 유지하면서 y축 속도만 변경
        rigid.linearVelocity = new Vector2(rigid.linearVelocity.x, jumpPower);
        currentJumpCount++;
        isGround = false;
    }

    void Flip()
    {
        // moveInput을 기반으로 방향 전환
        if (isFacingRight && moveInput < 0f || !isFacingRight && moveInput > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f; // x 스케일 뒤집기
            transform.localScale = localScale;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "floor")
        {
            currentJumpCount = 0;
            isGround = true;
        }
    }

    void playerAttack()
    {
        // Q키로 마스크 활성화/비활성화 토글
        mask.SetActive(!mask.activeSelf);
    }

    IEnumerator Dash()
    {
        canDash = false;
        dashing = true;
        float originalGravity = rigid.gravityScale;
        rigid.gravityScale = 0f; // 중력 일시 정지

        // 바라보는 방향(isFacingRight)을 기반으로 대시 방향 결정
        float dashDirection = isFacingRight ? 1f : -1f;
        rigid.linearVelocity = new Vector2(dashDirection * dashPower, 0f); // Vector2 사용

        yield return new WaitForSeconds(dashTime);

        // 대시가 끝나면 중력과 속도 복구
        rigid.gravityScale = originalGravity;
        rigid.linearVelocity = new Vector2(0, 0); // 대시 후 속도를 0으로 할지, 아니면 이전 속도를 유지할지 결정 (지금은 0)
        dashing = false;

        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }
}
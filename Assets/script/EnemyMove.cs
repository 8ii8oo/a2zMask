using UnityEngine;
using System.Collections;
using Spine.Unity;

public class EnemyMove : MonoBehaviour
{
    [SerializeField] public SkeletonAnimation spinePlayer;
    protected Rigidbody2D rigid;

    [Header("== 이동 설정 ==")]
    public float speed = 1f;
    public int nextMove = 1;                // 1: 오른쪽, -1: 왼쪽
    protected bool isStopping = false;      // 벽 끝에서 멈춰 있는 상태
    protected string currentAnim = "";

    // 자식이 부모 AI를 켜고 끌 수 있게 (공격 시 false)
    [HideInInspector] public bool isActiveAI = true;

    protected virtual void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        if (spinePlayer != null) SetAnim("idle");
        
        // 3초마다 배회 로직 시작
        InvokeRepeating(nameof(Think), 1f, 3f);
    }
    
    // 배회 로직: 랜덤하게 방향 변경
    protected virtual void Think()
    {
        if (!isActiveAI) return; // 자식 AI 활성화 시 중단
        if (isStopping) return;

        int[] moves = { -1, 1 };
        nextMove = moves[Random.Range(0, moves.Length)];
    }

    protected virtual void FixedUpdate()
    {
        // 자식 AI (아처 공격) 활성화 시 부모 로직 완전 중단
        if (!isActiveAI) return;

        // 실제 이동 및 애니메이션 설정
        if (!isStopping)
        {
            rigid.linearVelocity = new Vector2(nextMove * speed, rigid.linearVelocity.y);
            if (nextMove != 0)
                SetAnim("walk");
        }
        else
        {
            rigid.linearVelocity = Vector2.zero;
            SetAnim("idle");
        }

        // ⭐ 걷는 방향(nextMove)에 따라 스파인 방향(ScaleX) 동기화 (걷기/방향 문제 해결 핵심)
        if (nextMove != 0 && spinePlayer != null)
        {
            // 스파인 셋업에 따라 nextMove * -1 또는 nextMove를 사용
            spinePlayer.skeleton.ScaleX = nextMove; 
        }

        // 낭떠러지 체크 (앞쪽에서 아래로 Raycast)
        Vector2 frontVec = new Vector2(rigid.position.x + nextMove * 0.3f, rigid.position.y);
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector2.down, 1f, LayerMask.GetMask("Ground"));

        if (!isStopping && rayHit.collider == null)
        {
            StartCoroutine(StopAndTurn());
        }
    }

    // 벽 끝/낭떠러지에서 1초 정지 후 방향 전환
    protected IEnumerator StopAndTurn()
    {
        isStopping = true;
        SetAnim("idle");
        rigid.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(1f);

        nextMove *= -1;
        isStopping = false;
    }

    // 애니 재생 헬퍼
    protected void SetAnim(string animName, bool loop = true)
    {
        // 현재 재생 중인 애니메이션과 다를 때만 재생
        if (spinePlayer && spinePlayer.AnimationName != animName)
        {
            spinePlayer.AnimationState.SetAnimation(0, animName, loop);
            currentAnim = animName;
        }
    }
}
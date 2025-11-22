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
    protected bool isStopping = false;      
    protected string currentAnim = "";

    // 자식이 부모 AI를 켜고 끌 수 있게
    [HideInInspector] public bool isActiveAI = true;

    protected virtual void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();

        if (spinePlayer != null)
        {
            SetAnim("idle");
        }
        
        // ⭐ 배회 로직: 3초마다 Think 함수 호출
        InvokeRepeating(nameof(Think), 1f, 3f);
    }
    
    // ⭐ 배회 로직: nextMove를 랜덤하게 변경
    protected virtual void Think()
    {
        if (!isActiveAI) return;
        if (isStopping) return;

        int[] moves = { -1, 1 };
        nextMove = moves[Random.Range(0, moves.Length)];
    }

    protected virtual void FixedUpdate()
    {
        // 부모 AI가 비활성화 되어있다면 이동/애니 제어를 하지 않음.
        if (!isActiveAI) return;

        // 실제 이동 및 애니메이션 설정
        if (!isStopping)
        {
            // 걷는 애니메이션은 나오지만 걷지 않던 문제의 핵심: nextMove에 따른 이동 적용
            rigid.linearVelocity = new Vector2(nextMove * speed, rigid.linearVelocity.y);
            if (nextMove != 0)
                SetAnim("walk");
        }
        else
        {
            rigid.linearVelocity = Vector2.zero;
            SetAnim("idle");
        }

        // ⭐ 걷는 방향(nextMove)에 따라 스파인 방향(ScaleX) 동기화 (걷기 문제 해결 핵심)
        if (nextMove != 0 && spinePlayer != null)
        {
            // nextMove가 1일 때 ScaleX=1, -1일 때 ScaleX=-1이 되도록 설정
            spinePlayer.skeleton.ScaleX = nextMove; 
        }

        // 낭떠러지 체크 로직
        Vector2 frontVec = new Vector2(rigid.position.x + nextMove * 0.3f, rigid.position.y);
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector2.down, 1f, LayerMask.GetMask("Ground"));

        if (!isStopping && rayHit.collider == null)
        {
            StartCoroutine(StopAndTurn());
        }
    }

    protected IEnumerator StopAndTurn()
    {
        isStopping = true;
        SetAnim("idle");
        rigid.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(1f);

        nextMove *= -1;
        isStopping = false;
    }

    protected void SetAnim(string animName, bool loop = true)
    {
        if (spinePlayer && spinePlayer.AnimationName != animName)
        {
            spinePlayer.AnimationState.SetAnimation(0, animName, loop);
            currentAnim = animName;
        }
    }
}
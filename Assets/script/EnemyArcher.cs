using UnityEngine;

using Spine.Unity;

using System.Collections;



public class EnemyArcher : EnemyMove

{

    public float detectRange = 6f;

    public float attackCooldown = 2f;



    public GameObject arrowPrefab;

    public float arrowSpeed = 10f;



    private Transform player;

    private bool isAttacking = false;



    protected override void Awake()

    {

        base.Awake();



        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)

            player = playerObj.transform;



        spinePlayer.AnimationState.Complete += OnAnimComplete;

    }



    void Update()
{
    if (player == null) return;

    float distX = Mathf.Abs(player.position.x - transform.position.x);
    float distY = Mathf.Abs(player.position.y - transform.position.y);

    // ★ y축 거리 제한 추가 (예: 1.0f 이내만 감지)
    if (distY > 1.0f) 
    {
        isActiveAI = true;
        isAttacking = false;
        isStopping = false;

        nextMove = spinePlayer.skeleton.ScaleX > 0 ? 1 : -1;
        return;
    }

    if (distX <= detectRange)
    {
        // 추적 모드
        isActiveAI = false;
        rigid.linearVelocity = Vector2.zero;

        LookAtPlayer();
        Attack();
    }
    else
    {
        // 평상시 이동
        isActiveAI = true;
        isAttacking = false;
        isStopping = false;

        nextMove = spinePlayer.skeleton.ScaleX > 0 ? 1 : -1;
    }
}



    void LookAtPlayer()

    {

        float dir = player.position.x - transform.position.x;



        spinePlayer.skeleton.ScaleX = dir > 0 ? 1 : -1;

    }



    void Attack()

    {

        if (!isAttacking)

        {

            isAttacking = true;

            SetAnim("attack", false);

        }

    }



    void OnAnimComplete(Spine.TrackEntry track)

    {

        if (track.Animation.Name == "attack")

        {

            ShootArrow();

            StartCoroutine(AttackDelay());

            SetAnim("idle");

        }

    }



    void ShootArrow()

    {

        float direction = spinePlayer.skeleton.ScaleX;



        GameObject arrow = Instantiate(arrowPrefab, transform.position, Quaternion.identity);

        Rigidbody2D rb = arrow.GetComponent<Rigidbody2D>();



        rb.linearVelocity = new Vector2(direction * arrowSpeed, 0f);

        arrow.transform.localScale = new Vector3(direction, 1, 1);

    }



    IEnumerator AttackDelay()

    {

        yield return new WaitForSeconds(attackCooldown);

        isAttacking = false;

    }

}
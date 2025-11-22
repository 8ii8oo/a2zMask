using UnityEngine;
using System.Collections;
using Spine.Unity;

public class EnemyMove : MonoBehaviour
{
    [SerializeField] private SkeletonAnimation spinePlayer;
    Rigidbody2D rigid;

    [Header("== 이동 설정 ==")]
    public float speed = 1f;
    public int nextMove = 0; 
    bool isStopping = false;
    string currentAnim = "";

    [Header("== 공격 설정 ==")]
    public GameObject BulletPrefab;
    public float bulletSpeed = 10f; // 총알 속도
    public float shootingInterval = 3f; // 발사 간격 (초)
    private bool canShoot = true; // 발사 쿨다운 체크

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();

        if (spinePlayer != null)
        {
            SetAnim("idle");
            spinePlayer.skeleton.ScaleX = 1; 
        }

        InvokeRepeating("think", 1f, 3f); 
    }

    void FixedUpdate()
    {
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

        if (nextMove != 0)
        {
            spinePlayer.skeleton.ScaleX = Mathf.Sign(nextMove * -1); 
        }

        Vector2 frontVec = new Vector2(rigid.position.x + nextMove * 0.3f, rigid.position.y);
        Debug.DrawRay(frontVec, Vector3.down, Color.green);
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1f, LayerMask.GetMask("Ground"));

        if (!isStopping && rayHit.collider == null)
        {
            StartCoroutine(StopAndTurn());
        }

        if (canShoot)
        {
            ShootBullet();
        }
    }

    IEnumerator StopAndTurn()
    {
        isStopping = true;
        SetAnim("idle");
        rigid.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(1f);

        nextMove *= -1; 

        isStopping = false;
    }

    void think()
    {
        if (!isStopping)
        {
            int[] moves = { -1, 1 };
            nextMove = moves[Random.Range(0, moves.Length)];
        }
    }

    void ShootBullet()
    {
        canShoot = false;
        
        float facingDirection = spinePlayer.skeleton.ScaleX;
        float directionSign = Mathf.Sign(facingDirection); 

        Vector3 spawnPosition = transform.position;

        GameObject bullet = Instantiate(BulletPrefab, spawnPosition, Quaternion.identity);
        Rigidbody2D bulletRigid = bullet.GetComponent<Rigidbody2D>();
        
        if (bulletRigid != null && directionSign != 0)
        {
            bulletRigid.linearVelocity = new Vector2(directionSign * bulletSpeed, 0f);
            
            bullet.transform.localScale = new Vector3(directionSign * Mathf.Abs(bullet.transform.localScale.x), 
            bullet.transform.localScale.y, 
            bullet.transform.localScale.z);
        }
        
        SetAnim("attack"); 

        StartCoroutine(ShootCooldown());
    }

    IEnumerator ShootCooldown()
    {
        yield return new WaitForSeconds(shootingInterval);
        canShoot = true;
        
        if (!isStopping) SetAnim("walk");
        else SetAnim("idle");
    }

    void SetAnim(string animName)
    {
        if (spinePlayer != null && spinePlayer.AnimationName != animName)
        {
            spinePlayer.AnimationState.SetAnimation(0, animName, true);
            currentAnim = animName;
        }
    }
}
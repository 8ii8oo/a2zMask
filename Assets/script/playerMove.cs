using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{


    [SerializeField] private SkeletonAnimation spinePlayer;

    [Header("이동 및 점프")]
    public float speed = 5f;
    public float jumpPower = 10f;
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
    public GameObject mask;

    [Header("컴포넌트 및 상태")]
    public Rigidbody2D rigid;
    private float moveInput = 0f;
    private bool isFacingRight = true;


    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        if (spinePlayer != null)
        {

            SetAnimationState("idle"); 
        }
    }

    void Update()
    {

        
        moveInput = 0f;
        if (Input.GetKey(KeyCode.RightArrow))
        {
            moveInput = 1f;
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            moveInput = -1f;
        }


        if (isGround && !dashing)
        {
            if (moveInput != 0f)
            {
                SetAnimationState("walk");
            }
            else
            {
                SetAnimationState("idle");
            }
        }



        if(Input.GetKeyDown(KeyCode.A)) 
        {
            if(!dashing)
            {

                SetAnimationState("attack", false);
                

                if (moveInput != 0 )
                {
                    spinePlayer.AnimationState.AddAnimation(0, "walk", true, 0f);
                }
                else
                {
                    spinePlayer.AnimationState.AddAnimation(0, "idle", true, 0f);
                }
            }
        }


        if (Input.GetKeyDown(KeyCode.Space) && currentJumpCount < maxJumpCount)
        {
            Jump();
        }


        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }


        if (Input.GetKeyDown(KeyCode.Q))
        {
            playerAttack();
        }


        if (!dashing)
        {
            Flip();
        }
    }

    void FixedUpdate()
    {
        if (!dashing)
        {

            rigid.linearVelocity = new Vector2(moveInput * speed, rigid.linearVelocity.y);
        }
    }

    void Jump()
    {

        SetAnimationState("jump"); 
        
        rigid.linearVelocity = new Vector2(rigid.linearVelocity.x, jumpPower);
        currentJumpCount++;
        isGround = false;
    }

    void Flip()
    {

        if (isFacingRight && moveInput < 0f || !isFacingRight && moveInput > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f; 
            transform.localScale = localScale;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("floor")) // CompareTag 사용 권장
        {
            spinePlayer.AnimationState.SetAnimation(0, "landing", false);

            currentJumpCount = 0;
            isGround = true;


            if (!dashing)
            {
                if(moveInput != 0f)
                {
                    SetAnimationState("walk");
                }
                else
                {
                    SetAnimationState("idle");
                }
            }

        }
    }

    void playerAttack()
    {
        // mask.SetActive(!mask.activeSelf);
    }

    IEnumerator Dash()
    {
        canDash = false;
        dashing = true;


        float originalGravity = rigid.gravityScale;
        rigid.gravityScale = 0f;

        float dashDirection = isFacingRight ? 1f : -1f;
        rigid.linearVelocity = new Vector2(dashDirection * dashPower, 0f);

        yield return new WaitForSeconds(dashTime);

    
        rigid.gravityScale = originalGravity;
     
        if (moveInput == 0f)
        {
            rigid.linearVelocity = new Vector2(0f, rigid.linearVelocity.y);
            SetAnimationState("idle"); 
        } 
        else
        {
            rigid.linearVelocity = new Vector2(moveInput * speed, rigid.linearVelocity.y);
            SetAnimationState("walk"); 
        }

        dashing = false;

        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }

    void SetAnimationState(string animName, bool loop = true)
    {
        if (spinePlayer != null && spinePlayer.AnimationName != animName)
        {
            spinePlayer.AnimationState.SetAnimation(0, animName, loop);
        }
    }

}
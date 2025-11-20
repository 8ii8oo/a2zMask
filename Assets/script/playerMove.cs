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
    private float moveInput = 0f; // 좌우 입력을 저장할 변수
    private bool isFacingRight = true;


    void Start()
    {
        
        rigid = GetComponent<Rigidbody2D>();
        if (spinePlayer != null)
        {
            spinePlayer.AnimationState.SetAnimation(0, "idle", true);
        }
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.LeftArrow))
        {

            if (!dashing && spinePlayer.AnimationName != "walk")
            {
                spinePlayer.AnimationState.SetAnimation(0, "walk", true);
            }
        }
        
        // 키를 떼었을 때
        if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.LeftArrow))
        {
            if (!Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.LeftArrow))
            {
                if (!dashing && spinePlayer.AnimationName != "idle")
                {
                    spinePlayer.AnimationState.SetAnimation(0, "idle", true);
                }
            }
        }


        moveInput = 0f;
        if (Input.GetKey(KeyCode.RightArrow))
        {
            moveInput = 1f;
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            moveInput = -1f;
        }

        if(Input.GetKeyDown(KeyCode.A)) //기본공격
        {
            if(!dashing)
            {
            spinePlayer.AnimationState.SetAnimation(0, "attack", false);
            
            
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
        if (collision.gameObject.tag == "floor")
        {
            currentJumpCount = 0;
            isGround = true;
         
        }
    }

    void playerAttack()
    {
        

        //mask.SetActive(!mask.activeSelf);
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
            spinePlayer.AnimationState.SetAnimation(0, "idle", true); // idle로 복귀
        } 
        else
        {
            rigid.linearVelocity = new Vector2(moveInput * speed, rigid.linearVelocity.y);
            spinePlayer.AnimationState.SetAnimation(0, "walk", true); 
        }

        dashing = false;

        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }
}

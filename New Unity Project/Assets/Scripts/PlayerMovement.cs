using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //组件
    private Rigidbody2D rb;
    private BoxCollider2D coll;

    //移动参数
    public float speed = 8f;
    public float crouchSpeedDivisor = 3f;
    public float xVelocity;

    //跳跃参数
    public float jumpForce = 6.3f;
    public float jumpHoldForce = 1.9f;
    public float jumpHoldDuration = 0.1f;
    public float crouchJumpBoost = 2.5f;
    public float hangingJumpForce = 15f;

    float jumpTime;

    //状态
    public bool isCrouch = false;
    public bool isOnGround;
    public bool isJump;
    public bool isHeadBlocked;
    public bool isHanging;

    //环境检测
    public LayerMask groundLayer;
    public float footOffset = 0.4f;
    public float groundDistance = 0.2f;
    public float headClearance = 0.5f;
    float playerHeight;
    public float eyeHeight = 1.5f;
    public float grabDistance = 0.4f;
    public float reachOffset = 0.7f;

    //按键设置
    bool jumpPressed;
    bool jumpHeld;
    bool crouchHeld;
    bool crouchPressed;

    //碰撞体尺寸
    Vector2 colliderStandSize;
    Vector2 colliderStandOffset;
    Vector2 colliderCrouchSize;
    Vector2 colliderCrouchOffset;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
        colliderStandSize = coll.size;
        colliderStandOffset = coll.offset;
        colliderCrouchSize = new Vector2(coll.size.x, coll.size.y / 2f);
        colliderCrouchOffset = new Vector2(coll.offset.x, coll.offset.y / 2f);

        playerHeight = coll.size.y;
    }

    void Update()
    {
        if (GameManager.GameOver())
        {
            return;
        }
        if (Input.GetButtonDown("Jump"))
        {
            jumpPressed = true;
        }
        if (Input.GetButton("Jump"))
        {
            jumpHeld = true;
        }

        crouchHeld = Input.GetButton("Crouch");
        crouchPressed = Input.GetButtonDown("Crouch");

    }

    private void FixedUpdate()
    {
        if (GameManager.GameOver())
        {
            xVelocity = 0f;
            rb.bodyType = RigidbodyType2D.Static;
            return;
        }
        PhysicsCheck();
        GroundMovement();
        FilpDirction();
        MidAirMovement();
    }

    //检测碰撞环境
    void PhysicsCheck()
    {
        //人物是否在地面上
        RaycastHit2D leftCheck = Raycast(new Vector2(-footOffset, 0f), Vector2.down, groundDistance, groundLayer);
        RaycastHit2D rightCheck = Raycast(new Vector2(footOffset, 0f), Vector2.down, groundDistance, groundLayer);
        if (leftCheck || rightCheck)
        {
            isOnGround = true;
        }
        else
        {
            isOnGround = false;
        }
        //人物头是否顶到墙
        RaycastHit2D headCheck = Raycast(new Vector2(0f, coll.size.y), Vector2.up, headClearance, groundLayer);
        if (headCheck)
        {
            isHeadBlocked = true;
        }
        else
        {
            isHeadBlocked = false;
        }

        float direction = transform.localScale.x;
        Vector2 grabDir = new Vector2(direction, 0f);
        //悬挂检测
        RaycastHit2D blockedCheck = Raycast(new Vector2(footOffset * direction, playerHeight), grabDir, grabDistance, groundLayer);
        RaycastHit2D wallCheck = Raycast(new Vector2(footOffset * direction, eyeHeight), grabDir, grabDistance, groundLayer);
        RaycastHit2D ledgeCheck = Raycast(new Vector2(reachOffset * direction, playerHeight), Vector2.down, grabDistance, groundLayer);
        if(!isOnGround && rb.velocity.y <= 0 && ledgeCheck && wallCheck && !blockedCheck)
        {
            Vector3 pos = transform.position;
            pos.x += (wallCheck.distance - 0.05f) * direction;
            pos.y -= ledgeCheck.distance;
            transform.position = pos;
            rb.bodyType = RigidbodyType2D.Static;
            isHanging = true;
        }
    }

    //移动
    void GroundMovement()
    {
        if (isHanging)
        {
            return;
        }

        if (crouchHeld && !isCrouch && isOnGround)
        {
            Crouch();
        }
        else if(!crouchHeld && isCrouch && !isHeadBlocked)
        {
            StandUp();
            jumpPressed = false;
        }
        else if(!isOnGround && isCrouch)
        {
            StandUp();
        }

        xVelocity = Input.GetAxis("Horizontal");//-1f 1f

        if (isCrouch)
        {
            xVelocity /= crouchSpeedDivisor;
        }

        rb.velocity = new Vector2(xVelocity * speed, rb.velocity.y);
    }

    //跳跃
    void MidAirMovement()
    {
        if (isHanging)
        {
            if (jumpPressed)
            {
                rb.bodyType = RigidbodyType2D.Dynamic;
                rb.velocity = new Vector2(rb.velocity.x, hangingJumpForce);
                isHanging = false;
                jumpPressed = false;
            }
            if (crouchHeld)
            {
                rb.bodyType = RigidbodyType2D.Dynamic;
                isHanging = false;
                crouchHeld = false;
            }
        }

        if (jumpPressed && isOnGround && !isJump && !isHeadBlocked)
        {
            if (isCrouch)
            {
                StandUp();
                rb.AddForce(new Vector2(0f, crouchJumpBoost), ForceMode2D.Impulse);
            }
            isOnGround = false;
            isJump = true;

            jumpTime = Time.time + jumpHoldDuration;

            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);

            AudioManager.PlayJumpAudio();

            jumpPressed = false;
        }
        else if (isJump)
        {
            if (jumpHeld)
            {
                rb.AddForce(new Vector2(0f, jumpHoldForce), ForceMode2D.Impulse);
                jumpHeld = false;
            }
            if (jumpTime < Time.time)
            {
                isJump = false;
            }
        }
    }

    //根据速度改变朝向
    void FilpDirction()
    {
        if(xVelocity < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        if (xVelocity > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    //蹲下
    void Crouch()
    {
        isCrouch = true;
        coll.size = colliderCrouchSize;
        coll.offset = colliderCrouchOffset;
    }
    //站立
    void StandUp()
    {
        isCrouch = false;
        coll.size = colliderStandSize;
        coll.offset = colliderStandOffset;
    }

    //各个部位射线碰撞
    RaycastHit2D Raycast(Vector2 offSet, Vector2 rayDiraction, float length, LayerMask layer)
    {
        Vector2 pos = transform.position;

        RaycastHit2D hit = Physics2D.Raycast(pos + offSet, rayDiraction, length, layer);

        Color color = hit ? Color.red : Color.green; 

        Debug.DrawRay(pos + offSet, rayDiraction * length, color);

        return hit;
    }
}
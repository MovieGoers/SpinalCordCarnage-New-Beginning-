using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private static PlayerController instance;

    public GameObject eyes;
    public GameObject groundChecker;

    public Rigidbody rigidBody;

    Vector3 moveDirection;

    bool isGrounded;
    bool isOnSlope;

    enum State
    {
        running,
        dashing,
        air
    }

    State state;

    [Header("Ground Check")]
    public float groundCheckingRadius;
    public LayerMask whatIsGround;

    [Header ("Basic Movement")]
    public float speedNormal;
    public float groundDrag;
    public float airDrag;
    public float airSpeedRatio;

    [Header("Jump")]
    public float jumpForce;
    public int maxJumpCount;
    int jumpCount;
    public float jumpCountResetTime;
    float jumpCountResetTimer;
    bool canResetJumpCount;

    [Header("Dash")]
    public float dashSideForce;
    public float dashUpForce;
    public float dashCooltime;
    public float dashStateTime;
    float dashCoolTimer;
    float dashStateTimer;
    bool canDash;
    bool isDashing;

    [Header("Slope")]
    public float maxSlopeAngle;
    public float slopeSpeedRatio;
    GameObject slopeGameObject;

    public static PlayerController Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        if (instance)
        {
            Destroy(instance);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        jumpCount = maxJumpCount;
        jumpCountResetTimer = jumpCountResetTime;
        dashStateTimer = dashStateTime;
        canResetJumpCount = true;
        canDash = true;
    }

    private void Update()
    {
        transform.localRotation = InputManager.Instance.xQuat;

        float horizontalInput = InputManager.Instance.horizontalInput;
        float verticalInput = InputManager.Instance.VerticalInput;

        // 플레이어 이동 방향에 대한 normal vector 구하기.
        moveDirection = transform.forward * verticalInput + transform.right * horizontalInput;
        moveDirection.Normalize();

        isGrounded = IsGrounded();
        isOnSlope = IsOnSlope();

        if (isOnSlope)
            rigidBody.useGravity = false;
        else
            rigidBody.useGravity = true;

        if (isDashing)
        {
            rigidBody.drag = 0;
        }
        else if (isGrounded)
        {
            rigidBody.drag = groundDrag;
            if (canResetJumpCount)
                ResetJump();
        }
        else
        {
            rigidBody.drag = airDrag;
        }

        if (Input.GetKeyDown(InputManager.Instance.jumpKey) && jumpCount > 0)
        {
            Jump();
            canResetJumpCount = false;
        }

        if(jumpCountResetTimer > 0 && !canResetJumpCount)
        {
            jumpCountResetTimer -= Time.deltaTime;
        }

        if(jumpCountResetTimer <= 0)
        {
            if (!canResetJumpCount)
            {
                canResetJumpCount = true;
                jumpCountResetTimer = jumpCountResetTime;
            }
        }

        if (Input.GetKeyDown(InputManager.Instance.dashKey) && moveDirection.magnitude > 0.1f && canDash)
        {
            Dash();
            canDash = false;
            isDashing = true;
        }

        if(!canDash && dashCoolTimer > 0)
        {
            dashCoolTimer -= Time.deltaTime;
        }

        if(dashCoolTimer <= 0)
        {
            if (!canDash)
            {
                canDash = true;
                dashCoolTimer = dashCooltime;
            }
        }

        if(isDashing && dashStateTimer > 0)
        {
            dashStateTimer -= Time.deltaTime;
        }

        if(dashStateTimer <= 0)
        {
            if (isDashing)
            {
                isDashing = false;
                dashStateTimer = dashStateTime;
            }
        }

        HandleState();
    }

    void HandleState()
    {
        if (isDashing) 
        {
            state = State.dashing;
        }
        else if (isGrounded)
        {
            state = State.running;
        }
        else
        {
            state = State.air;
        }
    }

    void MovePlayer()
    {
        if(isOnSlope)
            rigidBody.AddForce(GetVectorOnSlope(moveDirection) * speedNormal * slopeSpeedRatio, ForceMode.Force);
        else if(isGrounded)
            rigidBody.AddForce(moveDirection * speedNormal, ForceMode.Force);
        else
            rigidBody.AddForce(moveDirection * speedNormal * airSpeedRatio, ForceMode.Force);
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(groundChecker.transform.position, groundCheckingRadius);
    }

    bool IsGrounded()
    {
        return Physics.CheckSphere(groundChecker.transform.position, groundCheckingRadius, whatIsGround);
    }

    void Jump()
    {
        rigidBody.velocity = new Vector3(rigidBody.velocity.x, 0, rigidBody.velocity.z);
        rigidBody.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        jumpCount -= 1;
    }

    void ResetJump()
    {
        jumpCount = maxJumpCount;
    }

    bool IsOnSlope()
    {
        Collider[] col = Physics.OverlapSphere(groundChecker.transform.position, groundCheckingRadius);
        if(col.Length != 0)
        {
            slopeGameObject = col[0].gameObject;

            // 플레이어가 닿은 땅의 angle 구하기.
            float groundAngle = Vector3.Angle(Vector3.up, slopeGameObject.transform.up);

            if (5f < groundAngle && groundAngle < maxSlopeAngle)
                return true;
        }
        return false;
    }

    Vector3 GetVectorOnSlope(Vector3 vector)
    {
        Vector3 newVector;

        if (isOnSlope)
            newVector = Vector3.ProjectOnPlane(vector, slopeGameObject.transform.up).normalized;
        else
            newVector = vector;

        return newVector;
    }

    void Dash()
    {
        rigidBody.velocity = new Vector3(0, 0, 0);
        rigidBody.AddForce(moveDirection * dashSideForce + transform.up * dashUpForce, ForceMode.Impulse);
    }
}

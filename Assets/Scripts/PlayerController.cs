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

    [Header("Ground Check")]
    public float groundCheckingRadius;
    public LayerMask whatIsGround;

    [Header ("Basic Movement")]
    public float speedNormal;
    public float groundDrag;
    public float airDrag;

    [Header("Jump")]
    public float jumpForce;

    [Header("Slope")]
    public float maxSlopeAngle;
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

        if (isGrounded)
        {
            rigidBody.drag = groundDrag;
        }
        else
        {
            rigidBody.drag = airDrag;
        }

        if (Input.GetKeyDown(InputManager.Instance.jumpKey) && isGrounded)
        {
            Jump();
        }
    }

    void MovePlayer()
    {
        if(isOnSlope)
            rigidBody.AddForce(GetVectorOnSlope(moveDirection) * speedNormal, ForceMode.Force);
        else
            rigidBody.AddForce(moveDirection * speedNormal, ForceMode.Force);
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
        rigidBody.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    bool IsOnSlope()
    {
        Collider[] col = Physics.OverlapSphere(groundChecker.transform.position, groundCheckingRadius);
        if(col.Length != 0)
        {
            slopeGameObject = col[0].gameObject;

            // 플레이어가 닿은 땅의 angle 구하기.
            float groundAngle = Vector3.Angle(Vector3.up, slopeGameObject.transform.up);

            Debug.Log(groundAngle);

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
}

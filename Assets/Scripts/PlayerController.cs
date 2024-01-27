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

    [Header("Ground Check")]
    public float groundCheckingRadius;
    public LayerMask whatIsGround;

    [Header ("Basic Movement")]
    public float speedNormal;
    public float groundDrag;
    public float airDrag;

    [Header("Jump")]
    public float jumpForce;

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

        moveDirection = transform.forward * verticalInput + transform.right * horizontalInput;

        isGrounded = IsGrounded();

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

    private void FixedUpdate()
    {
        rigidBody.AddForce(moveDirection * speedNormal, ForceMode.Force);


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
}

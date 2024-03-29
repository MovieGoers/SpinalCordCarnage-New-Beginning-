using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private static InputManager instance;

    const string xAxis = "Mouse X";
    const string yAxis = "Mouse Y";

    public Vector2 rotation = Vector2.zero;

    [Range(0.1f, 9f)] public float sensitivity;

    public float horizontalInput, VerticalInput;
    public Quaternion xQuat, yQuat;
    public Vector2 inputXY;

    [Header("Key Bind")]
    public KeyCode jumpKey;
    public KeyCode dashKey;
    public KeyCode shootKey;
    public KeyCode reloadkey;

    public static InputManager Instance
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
        LockMouse();
    }

    private void Update()
    {
        inputXY.x = Input.GetAxis(xAxis);
        inputXY.y = Input.GetAxis(yAxis);

        rotation.x += inputXY.x * sensitivity;
        rotation.y += inputXY.y * sensitivity;
        rotation.y = Mathf.Clamp(rotation.y, -90f, 90f);

        xQuat = Quaternion.AngleAxis(rotation.x, Vector3.up);
        yQuat = Quaternion.AngleAxis(rotation.y, Vector3.left);

        horizontalInput = Input.GetAxisRaw("Horizontal");
        VerticalInput = Input.GetAxisRaw("Vertical");
    }

    void LockMouse()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void UnlockMouse()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}

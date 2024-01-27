using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunManager : MonoBehaviour
{
    private static GunManager instance;

    public GameObject gunHolder;

    public Animator animator;

    [Header("Gun Mechanism")]
    public float gunDamage;
    public float reloadTime;
    public float reloadTimer;
    public bool isReloaded;

    [Header("Gun Movement")]
    public float mouseInputMultiplier;


    RaycastHit raycastHit;
    public static GunManager Instance
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
        isReloaded = true;
        reloadTimer = reloadTime;
    }

    private void Update()
    {
        animator.SetBool("isReloading", !isReloaded);
        gunHolder.transform.localRotation = Quaternion.Slerp(gunHolder.transform.localRotation, GetSwayRotation(), Time.deltaTime * 10f);

        if (Input.GetKeyDown(InputManager.Instance.shootKey) && isReloaded)
        {
            bool isHit = Physics.Raycast(PlayerController.Instance.eyes.transform.position, PlayerController.Instance.eyes.transform.forward, out raycastHit, 50f);
            if (isHit)
            {
                if (raycastHit.collider.CompareTag("Enemy"))
                {
                    ShootEnemy(raycastHit.collider.gameObject);

                }
            }
            isReloaded = false;
        }

        if (!isReloaded && reloadTimer > 0)
        {
            reloadTimer -= Time.deltaTime;
        }

        if (reloadTimer <= 0)
        {
            if (!isReloaded)
            {
                isReloaded = true;
                reloadTimer = reloadTime;
            }
        }
    }

    void ShootEnemy(GameObject Enemy)
    {
        Enemy.gameObject.GetComponent<EnemyScript>().AddHP(-1 * gunDamage);
    }

    Quaternion GetSwayRotation()
    {
        // 마우스가 움직이는 반대 방향의 Quaternion 반환.
        Quaternion xQuat = Quaternion.AngleAxis(InputManager.Instance.inputXY.x * -1 * mouseInputMultiplier, Vector3.up);
        Quaternion yQuat = Quaternion.AngleAxis(InputManager.Instance.inputXY.y * -1 * mouseInputMultiplier, Vector3.left);
        return xQuat * yQuat;
    }
}

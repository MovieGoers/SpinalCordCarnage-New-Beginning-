using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunManager : MonoBehaviour
{
    private static GunManager instance;

    public GameObject gunHolder;
    public GameObject gun;

    public Animator anim;

    public LayerMask enemyLayerMask;

    [Header("Gun Mechanism")]
    public float gunDamage;
    public float gunMaxRange;
    public float maxGunAmmo;
    float gunAmmo;
    public float gunShootResetTime;
    public float gunReloadTime;
    bool isGunReset;
    bool isGunReloaded;


    [Header("Gun Movement")]
    public float mouseInputMultiplier;
    public float bobMultiplier;
    public float bobSpeed;
    float sincosInput;
    Vector3 originalGunHolderPos;


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
        isGunReloaded = true;
        isGunReset = true;
        sincosInput = 0;
        gunAmmo = maxGunAmmo;
        UIManager.Instance.SetAmmoText("AMMO\n" + gunAmmo);

        originalGunHolderPos = gunHolder.transform.localPosition;
    }

    private void Update()
    {
        UIManager.Instance.SetAmmoText("AMMO\n" + gunAmmo);
        // sway, bob weapon
        gunHolder.transform.localRotation = Quaternion.Slerp(gunHolder.transform.localRotation, GetSwayRotation(), Time.deltaTime * 10f);
        gunHolder.transform.localPosition = Vector3.Slerp(gunHolder.transform.localPosition, GetBobPosition() + originalGunHolderPos, Time.deltaTime * 10f);

        if(Input.GetKeyDown(InputManager.Instance.reloadkey) && gunAmmo != maxGunAmmo && isGunReloaded)
        {
            isGunReloaded = false;
            anim.Play("Winchester_BeforeReloading");
            Invoke("ReloadGun", gunReloadTime - 1);

        }

        if (Input.GetKeyDown(InputManager.Instance.shootKey) && isGunReset && isGunReloaded)
        {
            isGunReset = false;
            
            bool isHit = Physics.Raycast(CameraController.Instance.transform.position, CameraController.Instance.transform.forward, out raycastHit, gunMaxRange);
            if (isHit)
            {
                Debug.Log(raycastHit.collider.gameObject.name);
                if (raycastHit.collider.CompareTag("Enemy"))
                {
                    ShootEnemy(raycastHit.collider.gameObject);

                }
            }
            gunAmmo -= 1;

            anim.Play("Winchester_Shoot");

            if (gunAmmo <= 0)
            {
                isGunReloaded = false;
                anim.SetTrigger("TriggerReload");
                Invoke("ReloadGun", gunReloadTime);
            }

            Invoke("ResetGunShoot", gunShootResetTime);
        }
    }

    void ReloadGun()
    {
        isGunReloaded = true;
        gunAmmo = maxGunAmmo;
    }
    
    void ResetGunShoot()
    {
        isGunReset = true;
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

    Vector3 GetBobPosition()
    {
        sincosInput += Time.deltaTime * bobSpeed;

        float groundedValue;

        if (PlayerController.Instance.isGrounded && PlayerController.Instance.rigidBody.velocity.magnitude > 0.1f)
            groundedValue = 1;
        else
            groundedValue = 0;

        Vector3 bobPosition = Vector3.zero;
        bobPosition.y = Mathf.Cos(sincosInput) * bobMultiplier * groundedValue * PlayerController.Instance.rigidBody.velocity.normalized.magnitude;

        return bobPosition;
    }
}

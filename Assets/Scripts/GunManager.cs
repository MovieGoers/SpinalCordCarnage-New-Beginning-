using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunManager : MonoBehaviour
{
    private static GunManager instance;

    public GameObject winchester;

    public Animator animator;

    public float gunDamage;
    public float reloadTime;
    public float reloadTimer;
    public bool isReloaded;

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
}

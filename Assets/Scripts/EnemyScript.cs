using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public GameObject projectile;

    public float hp;
    bool isReadyToAttack;
    public float attackCooltime;
    float attackCoolTimer;
    public float attackProjectileSpeed;
    public float attackProjectileLifetime;
    public float attackProjectilePushForce;
    void Start()
    {
        SetHP(100f);
        attackCoolTimer = attackCooltime;
        isReadyToAttack = true;
    }

    private void Update()
    {
        if(hp <= 0f)
        {
            Destroy(gameObject);
        }

        if (isReadyToAttack)
        {
            Attack();

        }

        if(!isReadyToAttack && attackCoolTimer > 0)
        {
            attackCoolTimer -= Time.deltaTime;
        }

        if(attackCoolTimer <= 0)
        {
            if (!isReadyToAttack)
            {
                ResetAttack();
            }
        }
    }

    public void SetHP(float newHP)
    {
        hp = newHP;
    }

    public void AddHP(float newHP)
    {
        hp += newHP;
    }

    public void Attack()
    {
        isReadyToAttack = false;

        GameObject newProjectile = Instantiate(projectile);
        newProjectile.transform.position = transform.position;

        newProjectile.SetActive(true);
        newProjectile.GetComponent<EnemyProjectileScript>().direction = (PlayerController.Instance.transform.position - transform.position).normalized;
        newProjectile.GetComponent<EnemyProjectileScript>().speed = attackProjectileSpeed;
        newProjectile.GetComponent<EnemyProjectileScript>().lifetime = attackProjectileLifetime;
        newProjectile.GetComponent<EnemyProjectileScript>().pushForce = attackProjectilePushForce;
    }

    public void ResetAttack()
    {
        isReadyToAttack = true;
        attackCoolTimer = attackCooltime;
    }
}

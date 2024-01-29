using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectileScript : MonoBehaviour
{
    public bool isOriginal;
    public float speed;
    public float pushForce;
    public Vector3 direction;
    public float lifetime;
    float lifeTimer;

    private void Start()
    {
        lifeTimer = lifetime;
    }

    private void Update()
    {

        if(lifeTimer > 0)
        {
            lifeTimer -= Time.deltaTime;
        }

        if(lifeTimer <= 0)
        {
            if(!isOriginal)
                Destroy(this.gameObject);
        }
    }

    private void FixedUpdate()
    {
        transform.Translate(direction * speed);
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject go = other.gameObject;
        if (go.CompareTag("Player"))
        {
            go.GetComponent<Rigidbody>().AddForce(direction * pushForce, ForceMode.Impulse);
        }

        if(go.CompareTag("Player") || go.CompareTag("Ground"))
            Destroy(this.gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public float hp;
    void Start()
    {
        SetHP(100f);
    }

    private void Update()
    {
        if(hp <= 0f)
        {
            Destroy(gameObject);
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
}

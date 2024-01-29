using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    private static MapManager instance;

    public GameObject mapTile;
    public GameObject enemy;
    public GameObject projectile;

    public int totalTileCount;
    public int enemyPerTile;
    public float distanceBetweenTiles;
    public float heightBetweenTiles;
    public float randomAngleRange;

    Vector3 tileGenerateDirection;
    public static MapManager Instance
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
        tileGenerateDirection = Vector3.forward;

        GenerateMap();
    }

    void GenerateMap()
    {
        for(int i = 0; i < totalTileCount; i++)
        {
            GenerateTile(i);
        }
    }

    void GenerateTile(int index)
    {
        GameObject newTile = Instantiate(mapTile);

        float angle = GetRandomAngle();
        Quaternion quat = Quaternion.AngleAxis(angle, Vector3.up);

        float height = GetRandomHeight();

        tileGenerateDirection = quat * tileGenerateDirection;

        mapTile.transform.Translate(tileGenerateDirection * distanceBetweenTiles + Vector3.up * height);

        if (index % enemyPerTile == 0)
        {
            GameObject newEnemy = Instantiate(enemy);
            GameObject newProjectile = Instantiate(projectile);
            newEnemy.SetActive(true);
            newEnemy.transform.position = mapTile.transform.position + Vector3.up * 10f;
        }
    }

    float GetRandomAngle()
    {
        return Random.Range(-randomAngleRange, randomAngleRange);
    }

    float GetRandomHeight()
    {
        return Random.Range(-heightBetweenTiles, heightBetweenTiles);
    }
}

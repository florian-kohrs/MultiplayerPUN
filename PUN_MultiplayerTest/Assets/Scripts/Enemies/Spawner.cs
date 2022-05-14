using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : PunLocalBehaviour
{

    public float enemyInterval = 3;

    public float massSpawnIntervall = 15; //720

    public float massSpawnShortIntervall = 1; //720

    public float massSpawnEnemyNumbers = 10;

    protected Transform enemyParent;

    [SerializeField]
    protected GameObject enemyPrefab;

    protected PoolOf<GameObject> enemyPool;

    protected override void OnStart()
    {
        enemyParent = new GameObject("Enemies").transform;
        enemyPool = new PoolOf<GameObject>(SpawnEnemy);
        StartCoroutine(SpawnEnemies());
        //StartCoroutine(FloodWithEnemies());
    }

    protected IEnumerator FloodWithEnemies()
    {
        yield return new WaitForSeconds(massSpawnIntervall);
        for (int i = 0; i < massSpawnEnemyNumbers; i++)
        {
            SpawnEnemyFromPool();
            yield return new WaitForSeconds(massSpawnShortIntervall);
        }
        yield return FloodWithEnemies();
    }

    protected IEnumerator SpawnEnemies()
    {
        yield return new WaitForSeconds(enemyInterval);
        SpawnEnemyFromPool();
        yield return SpawnEnemies();
    }

    protected void SpawnEnemyFromPool()
    {
        GameObject enemy = enemyPool.GetItemFromPool();
        enemy.SetActive(true);
        enemy.transform.position = transform.position + transform.forward + transform.up * 4;
    }

    protected GameObject SpawnEnemy()
    {
        GameObject enemy;
        if (PhotonNetwork.IsConnected)
        {
            enemy = PhotonNetwork.Instantiate(enemyPrefab.name, Vector3.zero, Quaternion.identity);
        }
        else
        {
            enemy = Instantiate(enemyPrefab);
        }
        enemy.transform.localScale *= 2;
        enemy.transform.parent = enemyParent;
        return enemy;
    }

}

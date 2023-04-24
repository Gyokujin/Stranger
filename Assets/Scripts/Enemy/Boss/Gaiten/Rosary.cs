using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rosary : MonoBehaviour
{
    [SerializeField]
    private GameObject[] bullets;
    [SerializeField]
    private int bulletCount;
    [SerializeField]
    private Vector2 spawnPoint;

    private float shotDir;
    [SerializeField]
    private float shotOffset;
    [SerializeField]
    private float shotSpeed;
    [SerializeField]
    private float shotDelayMin;
    [SerializeField]
    private float shotDelayMax;
    [SerializeField]
    private GameObject rosaryBullet;
    [SerializeField]
    private float remainTime;

    public void BulletSpawn(Vector2 direction)
    {
        bullets = new GameObject[bulletCount];
        shotDir = direction.x;

        for (int i = 0; i < bulletCount; i++)
        {
            GameObject bullet = Instantiate(rosaryBullet, spawnPoint, Quaternion.identity);
            bullet.transform.SetParent(transform, false);
            bullets[i] = bullet;
        }

        StartCoroutine("BulletShot");
    }

    IEnumerator BulletShot()
    {
        int count = bullets.Length;

        for (int i = 0; i < count; i++)
        {
            bullets[i].SetActive(true);
            float dirY = Random.Range(-shotOffset, shotOffset);
            float delay = Random.Range(shotDelayMin, shotDelayMax);
            bullets[i].GetComponent<Rigidbody2D>().velocity = new Vector2(shotDir, dirY) * shotSpeed;
            
            yield return new WaitForSeconds(delay);
        }

        yield return new WaitForSeconds(remainTime);
        Destroy(gameObject);
    }
}
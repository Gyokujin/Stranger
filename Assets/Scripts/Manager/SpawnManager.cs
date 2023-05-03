using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager instance = null;

    private List<Spawn> spawnList;
    private int spawnCount;
    public GameObject[] enemyObjs;
    public List<GameObject> spawnEnemy = new List<GameObject>();

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (instance != this)
                Destroy(this.gameObject);
        }

        spawnList = new List<Spawn>();
    }

    public void EnemyClear()
    {
        for (int i = 0; i < spawnEnemy.Count; i++)
        {
            Destroy(spawnEnemy[i].gameObject);
        }
    }

    public void ReadSpawnFile()
    {
        // #1. 변수 초기화
        spawnCount = 0;
        spawnList.Clear();

        // #2. 리스폰 파일 읽기
        string sceneName = SceneManager.GetActiveScene().name.ToString();
        Debug.Log(sceneName);
        TextAsset textFile = Resources.Load(sceneName) as TextAsset;
        StringReader stringReader = new StringReader(textFile.text);

        while (stringReader != null)
        {
            string line = stringReader.ReadLine();

            if (line == null)
                break;

            // #. 리스폰 데이터 생성
            Spawn spawnData = new Spawn();
            spawnData.type = int.Parse(line.Split(',')[0]);
            spawnData.positionX = float.Parse(line.Split(',')[1]);
            spawnData.positionY = float.Parse(line.Split(',')[2]);
            spawnData.direction = int.Parse(line.Split(',')[3]);
            spawnList.Add(spawnData);
            spawnCount++;
        }

        // #. 텍스트 파일 닫기
        //stringReader.Close();
    }

    public IEnumerator SpawnEnemies()
    {
        spawnEnemy.Clear();

        for (int i = 0; i < spawnCount; i++)
        {
            GameObject spawn = enemyObjs[spawnList[i].type].gameObject;
            Vector2 spawnPos = new Vector2(spawnList[i].positionX, spawnList[i].positionY);
            int dir = spawnList[i].direction;

            GameObject enemy = Instantiate(spawn, spawnPos, Quaternion.identity);
            enemy.transform.localScale = new Vector3(dir == -1 ? enemy.transform.localScale.x : enemy.transform.localScale.x * -1, enemy.transform.localScale.y, enemy.transform.localScale.z);
            spawnEnemy.Add(enemy);
            yield return null;
        }
    }
}
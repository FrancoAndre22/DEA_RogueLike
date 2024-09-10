using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomInitializer : MonoBehaviour
{
    public RoomType roomType;
    [System.Serializable]
    public enum Direction { North, South, East, West }
    public GameObject northWall;
    public GameObject southWall;
    public GameObject eastWall;
    public GameObject westWall;
    public int maxEnemies = 5;
    int currentEnemies;
    public List<Enemy> enemyPrefabs;
    public GameObject exitPrefab;
    public List<GameObject> finalBosses;
    public Transform[] spawnPoints;
    public List<string> openedDoors = new List<string>();
    int level = 0;

    public void InitializeRoom(int level)
    {
        this.level = level;
        currentEnemies = Mathf.Max(maxEnemies, level);
        PlayerPrefs.SetInt("Weapon", 0);
        int weapon = PlayerPrefs.GetInt("Weapon");
        //int enemyCount = Mathf.Min(maxEnemies, level); // Incrementa la cantidad de enemigos seg�n el nivel
        //SpawnEnemies(enemyCount);
        // l�gica adicional para modificar la habitaci�n seg�n el nivel
    }

    public void EnemyKilled()
    {
        currentEnemies--;
        if (currentEnemies <= 0)
        {
            foreach (var door in openedDoors)
            {
                OpenWall(door);
                roomType = RoomType.ClearedRoom;
            }
        }
    }

    public void OpenWall(Direction direction)
    {
        switch (direction)
        {
            case Direction.North:
                northWall.SetActive(false);
                openedDoors.Add("northWall");
                break;
            case Direction.South:
                southWall.SetActive(false);
                openedDoors.Add("southWall");
                break;
            case Direction.East:
                eastWall.SetActive(false);
                openedDoors.Add("eastWall");
                break;
            case Direction.West:
                westWall.SetActive(false);
                openedDoors.Add("westWall");
                break;
        }
    }

    public void OpenWall(string direction)
    {
        switch (direction)
        {
            case "northWall":
                northWall?.SetActive(false);
                break;
            case "southWall":
                southWall?.SetActive(false);
                break;
            case "eastWall":
                eastWall?.SetActive(false);
                break;
            case "westWall":
                westWall?.SetActive(false);
                break;
        }
    }

    public void CloseWall(string direction)
    {
        switch (direction)
        {
            case "northWall":
                northWall.SetActive(true);
                break;
            case "southWall":
                southWall.SetActive(true);
                break;
            case "eastWall":
                eastWall.SetActive(true);
                break;
            case "westWall":
                westWall.SetActive(true);
                break;
        }
    }

    IEnumerator SpawnEnemies(int enemyCount)
    {
        for (int i = 0; i < enemyCount; i++)
        {
            yield return new WaitForSeconds(Random.Range(0.5f, 1.5f));
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            int enemy = Random.Range(0, enemyPrefabs.Count);
            Enemy e = Instantiate(enemyPrefabs[enemy], spawnPoint.position, Quaternion.Euler(0, Random.Range(0, 360), 0));
            e.myRoom = this;
        }
    }

    public void SetExitRoom()
    {
        gameObject.name = "Exit";
        roomType = RoomType.ExitRoom;
        GameObject exit = Instantiate(exitPrefab, transform.position, Quaternion.identity);
        exit.transform.SetParent(transform);
    }

    public void SetBossRoom()
    {
        gameObject.name = "Boss";
        roomType = RoomType.BossRoom;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (roomType == RoomType.NormalRoom)
            {
                foreach (var door in openedDoors)
                {
                    CloseWall(door);
                }
                StartCoroutine(SpawnEnemies(currentEnemies));
            }
            else if (roomType == RoomType.BossRoom)
            {
                int boss = Random.Range(0, finalBosses.Count);
                Instantiate(finalBosses[boss], transform.position, Quaternion.identity);
            }
        }
    }
}

public enum RoomType
{
    PlayerRoom,
    NormalRoom,
    ClearedRoom,
    EventRoom,
    BossRoom,
    ExitRoom
}
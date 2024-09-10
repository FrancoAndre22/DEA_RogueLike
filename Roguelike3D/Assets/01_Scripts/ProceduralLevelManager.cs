using UnityEngine;
using System.Collections.Generic;

public class ProceduralLevelManager : MonoBehaviour
{
    public int level = 1;
    public int initialRoomCount = 10;
    public List<RoomInitializer> roomPrefabs;
    public Transform playerTransform;

    private Dictionary<Vector2Int, RoomInitializer> generatedRooms = new Dictionary<Vector2Int, RoomInitializer>();
    public static ProceduralLevelManager instance;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        GenerateLevel(level); // Empieza en el nivel 1
    }

    public void UpdateLevel()
    {
        level++;
        GenerateLevel(level);
        playerTransform.position = transform.position;
    }

    public void GenerateLevel(int lvl)
    {
        ClearPreviousLevel();
        int roomCount = initialRoomCount + (lvl/5); // Aumenta la cantidad de habitaciones por nivel

        List<Vector2Int> roomPositions = GenerateRoomPositions(roomCount);
        GenerateRooms(roomPositions, lvl);
        ConnectRooms(roomPositions);

        
    }

    private void ClearPreviousLevel()
    {
        foreach (var room in generatedRooms.Values)
        {
            Destroy(room.gameObject);
        }
        generatedRooms.Clear();
    }

    private List<Vector2Int> GenerateRoomPositions(int roomCount)
    {
        List<Vector2Int> positions = new List<Vector2Int>();
        HashSet<Vector2Int> occupiedPositions = new HashSet<Vector2Int>();

        // Generar la posición inicial
        Vector2Int startPos = Vector2Int.zero;
        positions.Add(startPos);
        occupiedPositions.Add(startPos);

        for (int i = 1; i < roomCount; i++)
        {
            Vector2Int newPos;
            do
            {
                Vector2Int lastRoomPos = positions[Random.Range(0, positions.Count)];
                newPos = lastRoomPos + GetRandomDirection() * 2;
            } while (occupiedPositions.Contains(newPos));

            positions.Add(newPos);
            occupiedPositions.Add(newPos);
        }

        return positions;
    }

    private Vector2Int GetRandomDirection()
    {
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        return directions[Random.Range(0, directions.Length)];
    }

    private void GenerateRooms(List<Vector2Int> roomPositions, int level)
    {
        bool firstRoom = true;
        foreach (Vector2Int pos in roomPositions)
        {
            RoomInitializer roomPrefab = roomPrefabs[Random.Range(0, roomPrefabs.Count)];
            Vector3 roomPosition = new Vector3(pos.x * 10, 0, pos.y * 10); // Ajustar según el tamaño de la habitación
            RoomInitializer room = Instantiate(roomPrefab, roomPosition, Quaternion.identity);
            if (firstRoom )
            {
                room.roomType = RoomType.PlayerRoom;
                firstRoom = false;
            }
            else
            {
                if (Random.Range(0, 10) < 1)
                {
                    room.roomType = RoomType.EventRoom;
                }
                else
                {
                    room.roomType = RoomType.NormalRoom;
                }
            }
            generatedRooms.Add(pos, room);
            // Inicializar la habitación según el nivel actual
            //room.InitializeRoom(level);
        }
        if (level % 5 == 0)
        {
            PlaceBossRoom(roomPositions);
        }
        else
        {
            PlaceExit(roomPositions);
        }
        for (int i = 0; i < roomPositions.Count; i++)
        {
            generatedRooms[roomPositions[i]].InitializeRoom(level);
        }
    }

    private void ConnectRooms(List<Vector2Int> roomPositions)
    {
        foreach (Vector2Int pos in roomPositions)
        {
            RoomInitializer room = generatedRooms[pos];
            RoomInitializer roomInitializer = room.GetComponent<RoomInitializer>();

            if (roomInitializer != null)
            {
                // Chequear habitaciones adyacentes y abrir paredes en consecuencia
                if (generatedRooms.ContainsKey(pos + Vector2Int.up * 2))
                {
                    roomInitializer.OpenWall(RoomInitializer.Direction.North);
                }
                if (generatedRooms.ContainsKey(pos + Vector2Int.down * 2))
                {
                    roomInitializer.OpenWall(RoomInitializer.Direction.South);
                }
                if (generatedRooms.ContainsKey(pos + Vector2Int.left * 2))
                {
                    roomInitializer.OpenWall(RoomInitializer.Direction.West);
                }
                if (generatedRooms.ContainsKey(pos + Vector2Int.right * 2))
                {
                    roomInitializer.OpenWall(RoomInitializer.Direction.East);
                }
            }
        }
    }

    private void PlaceExit(List<Vector2Int> roomPositions)
    {
        Vector2Int exitPos = roomPositions[Random.Range(1, roomPositions.Count)];
        generatedRooms[exitPos].SetExitRoom();
    }

    private void PlaceBossRoom(List<Vector2Int> roomPositions)
    {
        Vector2Int bossPos = roomPositions[Random.Range(0, roomPositions.Count)];
        generatedRooms[bossPos].SetBossRoom();
    }
}
using System;
using UnityEngine;

public class ObjectsSpawner : MonoBehaviour
{
    [Header("Objects Spawned")]
    [SerializeField] private ObjectSpawned[] cakeObjectsSpawner;
    [SerializeField] private ObjectSpawned[] candyObjectsSpawner;
    [SerializeField] private ObjectSpawned[] giftBoxObjectsSpawner;

    [Header("Objects Prefab")]
    [SerializeField] private GameObject cakePrefab;
    [SerializeField] private GameObject candyPrefab;
    [SerializeField] private GameObject giftBoxPrefab;

    public static ObjectsSpawner Instance;
    private int width, height;
    public GameObject[,] gridArray;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        width = GridMap.Instance.width; height = GridMap.Instance.height;
        gridArray = new GameObject[width, height];

        SpriteRenderer spriteRenderer = GridMap.Instance.tileBoard.GetComponent<SpriteRenderer>();
        Vector2 tileSize = spriteRenderer.bounds.size;

        Vector2 gridOffset = new Vector2((width - 1) * tileSize.x / 2, (height - 1) * tileSize.y / 2);

        SpawnObjects(tileSize, gridOffset);
    }

    private void SpawnObjects(Vector2 tileSize, Vector2 gridOffet)
    {
        for(int i = 0; i < cakeObjectsSpawner.Length; i++)
        {
            Vector2 positionSpawn = new Vector2(cakeObjectsSpawner[i].posX, cakeObjectsSpawner[i].posY) * tileSize - gridOffet;
            GameObject goPrefab = Instantiate(cakePrefab, positionSpawn, Quaternion.identity, transform);
            gridArray[cakeObjectsSpawner[i].posX, cakeObjectsSpawner[i].posY] = goPrefab; 
        }

        for (int i = 0; i < candyObjectsSpawner.Length; i++)
        {
            Vector2 positionSpawn = new Vector2(candyObjectsSpawner[i].posX, candyObjectsSpawner[i].posY) * tileSize - gridOffet;
            GameObject goPrefab = Instantiate(candyPrefab, positionSpawn, Quaternion.identity, transform);
            gridArray[candyObjectsSpawner[i].posX, candyObjectsSpawner[i].posY] = goPrefab;
        }

        for (int i = 0; i < giftBoxObjectsSpawner.Length; i++)
        {
            Vector2 positionSpawn = new Vector2(giftBoxObjectsSpawner[i].posX, giftBoxObjectsSpawner[i].posY) * tileSize - gridOffet;
            GameObject goPrefab = Instantiate(giftBoxPrefab, positionSpawn, Quaternion.identity, transform);
            gridArray[giftBoxObjectsSpawner[i].posX, giftBoxObjectsSpawner[i].posY] = goPrefab;
        }
    }
}

[Serializable]
public class ObjectSpawned
{
    [SerializeField] public int posX;
    [SerializeField] public int posY;
}
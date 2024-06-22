using UnityEngine;

public class GridMap : MonoBehaviour
{
    [SerializeField] public int width, height;
    [SerializeField] public GameObject tileBoard;

    public static GridMap Instance;
    public int[,] gridArray;

    public Vector2 tileSize;
    public Vector2 gridOffset;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        gridArray = new int[width, height];

        SpriteRenderer spriteRenderer = tileBoard.GetComponent<SpriteRenderer>();
        tileSize = spriteRenderer.bounds.size;

        gridOffset = new Vector2((width - 1) * tileSize.x / 2, (height - 1) * tileSize.y / 2);

        for (int i=0; i< gridArray.GetLength(0); i++)
        {
            for(int j=0; j< gridArray.GetLength(1); j++)
            {
                Vector2 positionGrid = new Vector2(i, j) * tileSize - gridOffset;
                SpawnTileBoard(tileBoard, positionGrid, transform);
            }
        }
    }

    private void SpawnTileBoard(GameObject tileBoard, Vector2 position, Transform transformParent)
    {
        Instantiate(tileBoard, position, Quaternion.identity, transformParent);
    }
}

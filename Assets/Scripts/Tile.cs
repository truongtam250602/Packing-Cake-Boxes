using UnityEngine;

public class Tile : MonoBehaviour
{
    private Vector2 _position;

    public Tile(Vector2 position)
    {
        this._position = position;
    }   

    public void SpawnTile(GameObject gridPrefab, Transform transformParent)
    {
        Instantiate(gridPrefab, _position, Quaternion.identity, transformParent);
        Debug.Log(_position);
    }
}


using System.Collections.Generic;
using UnityEngine;

public class HandleInputController : MonoBehaviour
{
    [SerializeField] private float speedSwipe;

    private GameObject[,] _gridArray;
    private Vector2 _startTouchPosition;
    private Vector2 _endTouchPosition;
    private Vector2 _tileSize;
    private Vector2 _gridOffset;

    List<GameObject> goCakes;
    GameObject goGiftBox;
    void Start()
    {
        goCakes = new List<GameObject>();
    }

    void Update()
    {
        if (_gridArray == null)
        {
            if (ObjectsSpawner.Instance.gridArray != null)
            {
                _gridArray = ObjectsSpawner.Instance.gridArray;
                for (int i = 0; i < _gridArray.GetLength(0); i++)
                {
                    for (int j = 0; j < _gridArray.GetLength(1); j++)
                    {
                        if (_gridArray[i, j] != null)
                        {
                            if (_gridArray[i, j].CompareTag("Cake"))
                            {
                                goCakes.Add(_gridArray[i, j]);
                            }
                            else if (_gridArray[i, j].CompareTag("GiftBox"))
                            {
                                goGiftBox = _gridArray[i, j];
                            }
                        }
                    }
                }
            }
        }

        if (_tileSize == Vector2.zero)
        {
            _tileSize = GridMap.Instance.tileSize;
        }

        if (_gridOffset == Vector2.zero)
        {
            _gridOffset = GridMap.Instance.gridOffset;
        }

        if (goCakes != null && goGiftBox != null)
        {
            for (int i = 0; i < goCakes.Count; i++)
            {
                if (goCakes[i].transform.position == goGiftBox.transform.position)
                {
                    DestroyCakeObject(goCakes[i]);
                }
            }
        }

        if (goCakes.Count <= 0)
        {
            GameManager.Instance.WinState();
        }

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    _startTouchPosition = touch.position;
                    break;
                case TouchPhase.Ended:
                    _endTouchPosition = touch.position;
                    Vector2 swipeDirection = _endTouchPosition - _startTouchPosition;

                    if (Mathf.Abs(swipeDirection.x) > Mathf.Abs(swipeDirection.y))
                    {
                        if (swipeDirection.x > 0)
                        {
                            MoveObject(Vector2Int.right);
                        }
                        else
                        {
                            MoveObject(Vector2Int.left);
                        }
                    }
                    else
                    {
                        if (swipeDirection.y > 0)
                        {
                            MoveObject(Vector2Int.up);
                        }
                        else
                        {
                            MoveObject(Vector2Int.down);
                        }
                    }
                    break;
            }
        }
    }
    
    private void DestroyCakeObject(GameObject cake)
    {
        goCakes.Remove(cake);
        Destroy(cake);
    }

    private void MoveObject(Vector2Int direction)
    {
        for (int i = 0; i < _gridArray.GetLength(0); i++)
        {
            for(int j = 0; j < _gridArray.GetLength(1); j++)
            {
                if (_gridArray[i, j] != null)
                {
                    for(int k = 0; k < goCakes.Count; k++)
                    {
                        if (_gridArray[i, j] == goCakes[k])
                        {
                            Vector2Int coordinateCake = new Vector2Int(i, j);
                            int distance = CaculateDistanceMoving(_gridArray[i, j], direction);
                            Vector2Int newCoordinateCake = coordinateCake + direction * distance;
                            Vector2 targetPosition = newCoordinateCake * _tileSize - _gridOffset;
                            goCakes[k].GetComponent<MovingObject>().targetPosition = targetPosition;
                            SwapCoordinateObject(coordinateCake, newCoordinateCake);
                        }
                    }

                    if (_gridArray[i, j] == goGiftBox)
                    {
                        Vector2Int coordinateGiftBox = new Vector2Int(i, j);
                        int distance = CaculateDistanceMoving(_gridArray[i, j], direction);
                        Vector2Int newCoordinateGiftBox = coordinateGiftBox + direction * distance;
                        Vector2 targetPosition = newCoordinateGiftBox * _tileSize - _gridOffset;
                        goGiftBox.GetComponent<MovingObject>().targetPosition = targetPosition;
                        SwapCoordinateObject(coordinateGiftBox, newCoordinateGiftBox);
                    }
                }
            }
        }
    }

    private void SwapCoordinateObject(Vector2Int go1Coordinate, Vector2Int go2Coordinate)
    {
        Debug.Log(go1Coordinate + "/" + go2Coordinate);
        GameObject temp = _gridArray[go1Coordinate.x, go1Coordinate.y];
        _gridArray[go1Coordinate.x, go1Coordinate.y] = _gridArray[go2Coordinate.x, go2Coordinate.y];
        _gridArray[go2Coordinate.x, go2Coordinate.y] = temp;
    }

    private int CaculateDistanceMoving(GameObject go, Vector2Int direction)
    {
        int distance = 0;
        
        if (go.CompareTag("Cake"))
        {
            Vector2Int cakeCoordinate = GetElementCoordinate(go);
            if (go.TryGetComponent(out MovingObject moving))
            {
                if (moving.IsCandy(direction, out RaycastHit2D hitCandy))
                {
                    Vector2Int goCandyCoordinate = GetElementCoordinate(hitCandy.collider.gameObject);
                    distance = Mathf.Abs((int)Vector2Int.Distance(cakeCoordinate, goCandyCoordinate)) - 1;
                }
                else if (moving.IsGiftBox(direction, out RaycastHit2D hitGiftBox))
                {
                    if (direction != Vector2Int.down)
                    {
                        Vector2Int newCoordinateHitGB;
                        int distanceMovingHitGB = CaculateDistanceMoving(hitGiftBox.collider.gameObject, direction) - 1;
                        newCoordinateHitGB = GetElementCoordinate(hitGiftBox.collider.gameObject)
                            + direction * distanceMovingHitGB;
                        Vector2Int goCoordinate = GetElementCoordinate(go);
                        distance = (int)Vector2Int.Distance(goCoordinate, newCoordinateHitGB);
                    }
                    else
                    {
                        return GetElementCoordinate(go).y - GetElementCoordinate(hitGiftBox.collider.gameObject).y;
                    }
                }
                else if (moving.IsCake(direction, out RaycastHit2D hitCake1))
                {
                    Vector2Int newCoordinateHitCake;
                    int distanceMovingHitCake = CaculateDistanceMoving(hitCake1.collider.gameObject, direction) - 1;
                    newCoordinateHitCake = GetElementCoordinate(hitCake1.collider.gameObject)
                        + direction * distanceMovingHitCake;
                    Vector2Int goCoordinate = GetElementCoordinate(go);
                    distance = (int)Vector2Int.Distance(goCoordinate, newCoordinateHitCake);
                }
                else
                {
                    if (direction == Vector2Int.right)
                    {
                        distance = _gridArray.GetLength(0) - cakeCoordinate.x - 1;
                    }
                    else if (direction == Vector2Int.left)
                    {
                        distance = cakeCoordinate.x;
                    }
                    else if (direction == Vector2.up)
                    {
                        distance = _gridArray.GetLength(1) - cakeCoordinate.y - 1;
                    }
                    else if (direction == Vector2.down)
                    {
                        distance = cakeCoordinate.y;
                    }
                }
            }
            else
            {
                return 0;
            }
            if (distance < 0)
            {
                distance = 0;
            }
            return distance;
        }
        else if (go.CompareTag("GiftBox")){
            Vector2Int giftBoxCoordinate = GetElementCoordinate(go);
            if (go.TryGetComponent(out MovingObject moving))
            {
                if (moving.IsCandy(direction, out RaycastHit2D hitCandy))
                {
                    Vector2Int goCandyCoordinate = GetElementCoordinate(hitCandy.collider.gameObject);
                    distance = Mathf.Abs((int)Vector2Int.Distance(giftBoxCoordinate, goCandyCoordinate)) - 1;
                }
                else if (moving.IsCake(direction, out RaycastHit2D hitCake2))
                {
                    if (direction != Vector2Int.up)
                    {
                        Vector2Int newCoordinateHitCake;
                        int distanceMovingHitCake = CaculateDistanceMoving(hitCake2.collider.gameObject, direction) - 1;

                        newCoordinateHitCake = GetElementCoordinate(hitCake2.collider.gameObject)
                            + direction * distanceMovingHitCake;

                        Vector2Int goCoordinate = GetElementCoordinate(go);
                        distance = (int)Vector2Int.Distance(goCoordinate, newCoordinateHitCake);
                    }
                    else
                    {
                        return GetElementCoordinate(go).y - GetElementCoordinate(hitCake2.collider.gameObject).y;
                    }
                }
                else
                {
                    if (direction == Vector2Int.right)
                    {
                        distance = _gridArray.GetLength(0) - giftBoxCoordinate.x - 1;
                    }
                    else if (direction == Vector2Int.left)
                    {
                        distance = giftBoxCoordinate.x;
                    }
                    else if (direction == Vector2.up)
                    {
                        distance = _gridArray.GetLength(1) - giftBoxCoordinate.y - 1;
                    }
                    else if (direction == Vector2.down)
                    {
                        distance = giftBoxCoordinate.y;
                    }
                }
            }
            else
            {
                return 0;
            }
            if (distance < 0)
            {
                distance = 0;
            }
            return distance;
        }
        else
        {
            return 0;
        }
    }

    private Vector2Int GetElementCoordinate(GameObject element)
    {
        for (int i = 0; i < _gridArray.GetLength(0); i++)
        {
            for (int j = 0; j < _gridArray.GetLength(1); j++)
            {
                if (_gridArray[i, j] != null)
                {
                    if (_gridArray[i, j] == element)
                    {
                        return new Vector2Int(i, j);
                    }
                }    
            }
        }
        return new Vector2Int(-1, -1);
    }

}



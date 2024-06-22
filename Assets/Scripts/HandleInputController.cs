using UnityEngine;

public class HandleInputController : MonoBehaviour
{
    [SerializeField] private float speedSwipe;

    private GameObject[,] _gridArray;
    private Vector2 _startTouchPosition;
    private Vector2 _endTouchPosition;
    private Vector2 _tileSize;
    private Vector2 _gridOffset;

    GameObject goCake, goGiftBox;
    void Start()
    {

    }

    void Update()
    {
        if(_gridArray == null)
        {
            if (ObjectsSpawner.Instance.gridArray != null)
            {
                _gridArray = ObjectsSpawner.Instance.gridArray;
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

        if (goCake == null && goGiftBox == null)
        {
            goCake = GameObject.FindGameObjectWithTag("Cake");
            goGiftBox = GameObject.FindGameObjectWithTag("GiftBox");
        }

        if (goCake != null && goGiftBox != null)
        {
            if (goCake.transform.position == goGiftBox.transform.position)
            {
                GameManager.Instance.WinState();
            }
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
    
    private void MoveObject(Vector2Int direction)
    {
        for (int i = 0; i < _gridArray.GetLength(0); i++)
        {
            for(int j = 0; j < _gridArray.GetLength(1); j++)
            {
                if (_gridArray[i, j] != null)
                {
                    if (_gridArray[i, j].CompareTag("Cake"))
                    {
                        Vector2Int coordinateCake = new Vector2Int(i, j);
                        int distance = CaculateDistanceMoving(_gridArray[i, j], direction);
                        Vector2Int newCoordinateCake = coordinateCake + direction * distance;
                        Vector2 targetPosition = newCoordinateCake * _tileSize - _gridOffset;
                        _gridArray[i, j].GetComponent<MovingObject>().targetPosition = targetPosition;
                        SwapCoordinateObject(coordinateCake, newCoordinateCake);
                    }

                    else if (_gridArray[i, j].CompareTag("GiftBox"))
                    {
                        Vector2Int coordinateGiftBox = new Vector2Int(i, j);
                        int distance = CaculateDistanceMoving(_gridArray[i, j], direction);
                        Vector2Int newCoordinateGiftBox = coordinateGiftBox + direction * distance;
                        Vector2 targetPosition = newCoordinateGiftBox * _tileSize - _gridOffset;
                        _gridArray[i, j].GetComponent<MovingObject>().targetPosition = targetPosition;
                        SwapCoordinateObject(coordinateGiftBox, newCoordinateGiftBox);
                    }
                }
            }
        }
    }

    private void SwapCoordinateObject(Vector2Int go1Coordinate, Vector2Int go2Coordinate)
    {
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
                RaycastHit2D hitCandy, hitGiftBox;
                if (moving.IsCandy(direction, out hitCandy))
                {
                    Vector2Int goCandyCoordinate = GetElementCoordinate(hitCandy.collider.gameObject);
                    distance = Mathf.Abs((int)Vector2Int.Distance(cakeCoordinate, goCandyCoordinate)) - 1;
                }
                else if (moving.IsGiftBox(direction, out hitGiftBox))
                {
                    if (direction != Vector2Int.down)
                    {
                        Vector2Int newCoordinateHitGB;
                        int distanceMovingHitGB = CaculateDistanceMoving(hitGiftBox.collider.gameObject, direction);
                        if (distanceMovingHitGB >= 0)
                        {
                            newCoordinateHitGB = GetElementCoordinate(hitGiftBox.collider.gameObject)
                            + direction * distanceMovingHitGB;
                        }
                        else
                        {
                            newCoordinateHitGB = GetElementCoordinate(hitGiftBox.collider.gameObject);
                        }
                        Vector2Int goCoordinate = GetElementCoordinate(go);
                        if (newCoordinateHitGB.x == goCoordinate.x)
                        {
                            distance = newCoordinateHitGB.y - goCoordinate.y - 1;
                        }
                        if (newCoordinateHitGB.y == goCoordinate.y)
                        {
                            distance = newCoordinateHitGB.x - goCoordinate.x - 1;
                        }
                    }
                    else
                    {
                        return GetElementCoordinate(go).y - GetElementCoordinate(hitGiftBox.collider.gameObject).y;
                    }
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
                RaycastHit2D hitCandy, hitCake;
                if (moving.IsCandy(direction, out hitCandy))
                {
                    Vector2Int goCandyCoordinate = GetElementCoordinate(hitCandy.collider.gameObject);
                    distance = Mathf.Abs((int)Vector2Int.Distance(giftBoxCoordinate, goCandyCoordinate)) - 1;
                }
                else if (moving.IsCake(direction, out hitCake))
                {
                    if (direction != Vector2Int.up)
                    {
                        Vector2Int newCoordinateHitCake;
                        int distanceMovingHitCake = CaculateDistanceMoving(hitCake.collider.gameObject, direction);
                        if (distanceMovingHitCake >= 0)
                        {
                            newCoordinateHitCake = GetElementCoordinate(hitCake.collider.gameObject)
                            + direction * distanceMovingHitCake;
                        }
                        else
                        {
                            newCoordinateHitCake = GetElementCoordinate(hitCake.collider.gameObject);
                        }

                        Vector2Int goCoordinate = GetElementCoordinate(go);
                        if (newCoordinateHitCake.x == goCoordinate.x)
                        {
                            distance = newCoordinateHitCake.y - goCoordinate.y - 1;
                        }
                        if (newCoordinateHitCake.y == goCoordinate.y)
                        {
                            distance = newCoordinateHitCake.x - goCoordinate.x - 1;
                        }
                    }
                    else
                    {
                        return GetElementCoordinate(go).y - GetElementCoordinate(hitCake.collider.gameObject).y;
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



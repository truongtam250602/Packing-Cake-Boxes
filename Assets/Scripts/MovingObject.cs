using JetBrains.Annotations;
using UnityEngine;

public class MovingObject : MonoBehaviour
{
    [CanBeNull][SerializeField] private LayerMask candyLayerMask;
    [CanBeNull][SerializeField] private LayerMask giftBoxLayerMask;
    [CanBeNull][SerializeField] private LayerMask cakeLayerMask;
    [SerializeField] private Collider2D coll;

    [HideInInspector]
    public Vector2 targetPosition;
    public float speed;
    private Vector2 lastTargetPosition;
    Color color;
    private void Start()
    {
        targetPosition = transform.position;
        lastTargetPosition = transform.position;
    }

    private void Update()
    {
        if (lastTargetPosition != targetPosition)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            if (Vector2.Distance(transform.position, targetPosition) <= 0f)
            {
                lastTargetPosition = targetPosition;
            }
        }

        DrawDebugRay(Vector2.up);
        DrawDebugRay(Vector2.down);
        DrawDebugRay(Vector2.right);
        DrawDebugRay(Vector2.left);
    }

    public bool IsCandy(Vector2 direction, out RaycastHit2D hit)
    {
        coll.enabled = false;
        hit = Physics2D.Raycast(transform.position, direction, candyLayerMask);
        coll.enabled = true;
        if (hit.collider != null && hit.collider.CompareTag("Candy"))
        {
            return true;
        }
        return false;
    }

    public bool IsCake(Vector2 direction, out RaycastHit2D hit)
    {
        coll.enabled = false;
        hit = Physics2D.Raycast(transform.position, direction, cakeLayerMask);
        coll.enabled = true;
        if (hit.collider != null && hit.collider.CompareTag("Cake"))
        {
            return true;
        }
        return false;
    }

    public bool IsGiftBox(Vector2 direction, out RaycastHit2D hit)
    {
        coll.enabled = false;
        hit = Physics2D.Raycast(transform.position, direction, giftBoxLayerMask);
        coll.enabled = true;
        if (hit.collider != null && hit.collider.CompareTag("GiftBox"))
        {
            return true;
        }
        return false;
    }
    private void DrawDebugRay(Vector2 direction)
    {
        if (IsCandy(direction, out RaycastHit2D hit))
        {
            color = Color.red;
        }
        else
        {
            color = Color.green;
        }
        Debug.DrawLine(transform.position,transform.position + (Vector3)direction * 10f, color);
    }
}

using UnityEngine;

public class DeliverySquare : MonoBehaviour
{
    [SerializeField] public int OrderID;
    private float _deliverInteractionTimeSeconds = 2f;

    private float _interactionTimer;
    
    public delegate void OnDeliver();
    public OnDeliver DeliverEvent;
    public Vector3 ParentDropOffPoint;

    public void Setup(int orderID, float deliverTimeSeconds, Transform parentTransform)
    {
        OrderID = orderID;
        _deliverInteractionTimeSeconds = deliverTimeSeconds;
        _interactionTimer = _deliverInteractionTimeSeconds;
        ParentDropOffPoint = parentTransform.position + ((transform.position - parentTransform.position) / 3f);
        ParentDropOffPoint.y = transform.position.y;
    }
    
    private void OnTriggerStay(Collider other)
    {
        _interactionTimer -= Time.deltaTime;
        if (_interactionTimer <= 0f)
        {
            var carriedOrders = other.gameObject.GetComponentsInChildren<Order>();

            foreach (var order in carriedOrders)
            {
                if (order.GetInstanceID() == OrderID)
                {
                    order.Complete();
                    OrderID = 0;
                    DeliverEvent.Invoke();
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        _interactionTimer = _deliverInteractionTimeSeconds;
    }
}

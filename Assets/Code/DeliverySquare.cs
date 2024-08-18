using UnityEngine;

public class DeliverySquare : MonoBehaviour
{
    [SerializeField] private int OrderID;
    private float _deliverInteractionTimeSeconds = 2f;

    private float _interactionTimer;
    
    public delegate void OnDeliver();
    public OnDeliver DeliverEvent;

    public void Setup(int orderID, float deliverTimeSeconds)
    {
        Debug.Log("setup");
        OrderID = orderID;
        _deliverInteractionTimeSeconds = deliverTimeSeconds;
        _interactionTimer = _deliverInteractionTimeSeconds;
    }
    
    private void OnTriggerStay(Collider other)
    {
        _interactionTimer -= Time.deltaTime;
        if (_interactionTimer <= 0f)
        {
            var carriedOrders = other.gameObject.GetComponentsInChildren<Order>();

            foreach (var order in carriedOrders)
            {
                if (order.gameObject.GetInstanceID() == OrderID)
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

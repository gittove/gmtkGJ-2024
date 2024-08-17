using System;
using UnityEngine;

public class Customer : MonoBehaviour
{
    private bool _occupied;
    [SerializeField] private int OrderID;

    public bool Occupied => _occupied;
    private SphereCollider _collider;

    private void Start()
    {
        _collider = GetComponent<SphereCollider>();
        _collider.enabled = false;
    }

    public void Activate(int orderID)
    {
        OrderID = orderID;
        _collider.enabled = true;
        _occupied = true;
    }

    private void Reset()
    {
        OrderID = 0;
        _collider.enabled = false;
        _occupied = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Input.GetButtonDown("Interact"))
        {
            var carriedOrders = other.gameObject.GetComponentsInChildren<Order>();

            foreach (var order in carriedOrders)
            {
                if (order.GetInstanceID() == OrderID)
                {
                    Debug.Log("Delivered:)");
                    order.Complete();
                    Reset();
                }
            }
            // compare orderIDs
        }
    }

    private void OnDrawGizmos()
    {
        if (_occupied)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position + _collider.center, _collider.radius);
        }
    }
}

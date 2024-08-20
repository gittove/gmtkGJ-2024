using System;
using UnityEngine;

public class Customer : MonoBehaviour
{
    private bool _occupied;
    private DeliverySquare _deliverSquare;
    private GameObject _order;

    [SerializeField] private float _deliverInteractionTimeSeconds = 2f;
    public bool Occupied => _occupied;

    private void Start()
    {
        _deliverSquare = GetComponentInChildren<DeliverySquare>(true);
        _deliverSquare.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (_occupied && _order == null)
        {
            Reset();
        }
    }

    public void Activate(int orderID, GameObject order)
    {
        _deliverSquare.gameObject.SetActive(true);
        _deliverSquare.Setup(orderID, _deliverInteractionTimeSeconds, transform);
        _deliverSquare.DeliverEvent += OnDeliver;

        _order = order;
        _occupied = true;
    }

    private void Reset()
    {
        _deliverSquare.DeliverEvent -= OnDeliver;
        _deliverSquare.gameObject.SetActive(false);
        _occupied = false;
    }

    private void OnDeliver()
    {
        Reset();
    }
}

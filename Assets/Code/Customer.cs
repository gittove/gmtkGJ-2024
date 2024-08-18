using UnityEngine;

public class Customer : MonoBehaviour
{
    private bool _occupied;
    private DeliverySquare _deliverSquare;

    [SerializeField] private float _deliverInteractionTimeSeconds = 2f;
    public bool Occupied => _occupied;

    private void Start()
    {
        _deliverSquare = GetComponentInChildren<DeliverySquare>(true);
        _deliverSquare.gameObject.SetActive(false);
    }

    public void Activate(int orderID)
    {
        _deliverSquare.gameObject.SetActive(true);
        _deliverSquare.Setup(orderID, _deliverInteractionTimeSeconds);
        _deliverSquare.DeliverEvent += OnDeliver;
        
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

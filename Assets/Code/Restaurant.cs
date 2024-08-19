using UnityEngine;

public class Restaurant : MonoBehaviour
{
    public GameObject OrderPrefab;
    public GameObject OrderSpawnAlign;
    
    private float _weight;
    private float _depletionWeight;
    private GameObject _orderObject;

    public float Weight => _weight * _depletionWeight;

    void Start()
    {
        _depletionWeight = 1f;
        _weight = Random.Range(0.1f, 0.9f);
    }

    void Update()
    {

    }

    public void ActivateOrder()
    {
        _depletionWeight = 1f;

        var newOrder = Instantiate(OrderPrefab, OrderSpawnAlign.transform);
        newOrder.GetComponent<Order>().Setup(Order.ORDER_TIME);
    }

    public void Deplete()
    {
        _depletionWeight *= 0.5f;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawCube(transform.position, Vector3.one);
    }
}

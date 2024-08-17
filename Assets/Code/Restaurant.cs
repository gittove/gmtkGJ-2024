using System;
using UnityEngine;

public class Restaurant : MonoBehaviour
{
    public GameObject OrderPrefab;
    
    private float _weight;
    private float _depletionWeight;

    public float Weight => _weight * _depletionWeight;

    void Start()
    {
        _depletionWeight = 1f;
        _weight = UnityEngine.Random.Range(0.1f, 0.9f);
    }

    void Update()
    {
        
    }

    public void ActivateOrder()
    {
        _depletionWeight = 1f;

        var newOrder = Instantiate(OrderPrefab, transform);
        newOrder.GetComponent<Order>().Setup(30f);
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

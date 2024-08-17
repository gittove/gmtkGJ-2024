using UnityEngine;

public class Restaurant : MonoBehaviour
{
    private float _weight;
    private float _depletionWeight;

    public float Weight => _weight * _depletionWeight;
    
    private Order _orderComp;

    void Start()
    {
        _depletionWeight = 1f;
        _weight = UnityEngine.Random.Range(0.1f, 0.9f);
        _orderComp = GetComponentInChildren<Order>();
        _orderComp.gameObject.SetActive(false);
    }

    void Update()
    {
        
    }

    public void ActivateOrder()
    {
        _depletionWeight = 1f;
        
        _orderComp.gameObject.SetActive(true);
        _orderComp.Setup(15f);
    }

    public void Deplete()
    {
        _depletionWeight *= 0.5f;
    }
}

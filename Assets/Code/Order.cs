using Unity.Mathematics;
using UnityEngine;

public class Order : MonoBehaviour
{
    private float _timer;

    public float3 Position => gameObject.transform.position;

    public void Setup(float timer)
    {
        _timer = timer;
    }

    public void Complete()
    {
        Destroy(gameObject);
    }

    private void OnPickup(Transform newParent)
    {
        transform.parent = newParent;
        
        int orderID = gameObject.GetInstanceID();

        var customers = FindObjectsByType<Customer>(FindObjectsSortMode.None);
        int pickIndex = UnityEngine.Random.Range(0, customers.Length - 1);
        
        while (customers[pickIndex].GetComponent<Customer>().Occupied)
        {
            pickIndex = UnityEngine.Random.Range(0, customers.Length - 1);
        }
            
        customers[pickIndex].GetComponent<Customer>().Activate(orderID);
        
    }
    
    void Update()
    {
        _timer -= Time.deltaTime;

        if (_timer <= 0f)
        {
            Destroy(this);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (Input.GetButtonDown("Interact"))
        {
            Debug.Log("Picked up order");
            
            OnPickup(other.gameObject.transform);
        }
    }

    private void OnDrawGizmos()
    {
        if (enabled)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(transform.position + transform.up * 1.5f, 0.2f);
            
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position + GetComponent<SphereCollider>().center, GetComponent<SphereCollider>().radius);
        }
    }
}

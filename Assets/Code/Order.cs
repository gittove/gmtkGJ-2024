using System;
using Unity.Mathematics;
using UnityEngine;

public class Order : MonoBehaviour
{
    private float _timer;
    
    [SerializeField] private float _pickupInteractionSeconds = 2f;
    [SerializeField] private GameObject _pickupMesh;

    private float _interactionTimer;

    public float3 Position => gameObject.transform.position;

    public void Setup(float timer)
    {
        _interactionTimer = _pickupInteractionSeconds;
        _timer = timer;
    }

    public void Complete()
    {
        Destroy(this.gameObject);
    }

    private void OnPickup(Transform newParent)
    {
        transform.parent = newParent;
        
        int orderID = GetInstanceID();

        var customers = FindObjectsByType<Customer>(FindObjectsSortMode.None);
        int pickIndex = UnityEngine.Random.Range(0, customers.Length - 1);

        int attempts = 0;
        while (customers[pickIndex].GetComponent<Customer>().Occupied)
        {
            pickIndex = UnityEngine.Random.Range(0, customers.Length - 1);
            attempts++;
            if (attempts > 5)
            {
                return;
            }
        }
            
        customers[pickIndex].GetComponent<Customer>().Activate(orderID);
        
        _interactionTimer = _pickupInteractionSeconds;
        GetComponent<BoxCollider>().enabled = false;
        _pickupMesh.gameObject.SetActive(false);
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
        _interactionTimer -= Time.deltaTime;
        
        if (_interactionTimer <= 0f)
        {
            OnPickup(other.gameObject.transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        _interactionTimer = _pickupInteractionSeconds;
    }

    private void OnDrawGizmos()
    {
        if (enabled)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(transform.position + transform.up * 1.5f, 0.2f);

            if (GetComponent<BoxCollider>().enabled)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawWireCube(transform.position + GetComponent<BoxCollider>().center, GetComponent<BoxCollider>().size);
            }
        }
    }
}

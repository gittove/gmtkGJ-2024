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
    
    void Update()
    {
        _timer -= Time.deltaTime;

        if (_timer <= 0f)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (Input.GetButtonDown("Interact"))
        {
            Debug.Log("Picked up order");
            gameObject.SetActive(false);
        }
    }

    private void OnDrawGizmos()
    {
        if (enabled)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(transform.position + transform.up * 1.5f, 0.2f);
            
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, GetComponent<SphereCollider>().radius);
        }
    }
}

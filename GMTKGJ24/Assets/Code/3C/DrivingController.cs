using Unity.Mathematics.Geometry;
using UnityEngine;

public class DrivingController : MonoBehaviour
{
    public float MovementSpeed = 5f;
    public float TurnSpeed = 3f;
    
    void Start()
    {
        
    }

    void Update()
    {
        var verticalInput = Input.GetAxis("Vertical");
        var horizontalInput = Input.GetAxis("Horizontal");
        var deltaTime = Time.deltaTime;

        gameObject.transform.Rotate(new Vector3(0, 1, 0), TurnSpeed * deltaTime * verticalInput);
        gameObject.transform.position += gameObject.transform.forward * MovementSpeed * deltaTime * horizontalInput;
    }
}

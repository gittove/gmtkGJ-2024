using Unity.Mathematics;
using Unity.Mathematics.Geometry;
using UnityEngine;

public class DrivingController : MonoBehaviour
{
    public float MovementSpeed = 5f;
    public float TurnSpeed = 3f;
    public float MaxVelocity = 20f;
    public Rigidbody Rigidbody;

    void Update()
    {
        var verticalInput = Input.GetAxis("Vertical");
        var horizontalInput = Input.GetAxis("Horizontal");
        var deltaTime = Time.deltaTime;
        Vector3 moveVector = transform.forward * verticalInput * deltaTime * MovementSpeed;

        var rotatedTransform = gameObject.transform;
        rotatedTransform.Rotate(new Vector3(0, 1, 0), TurnSpeed * deltaTime * horizontalInput);
        Rigidbody.MoveRotation(rotatedTransform.rotation);
        Rigidbody.linearVelocity += moveVector;
        var clampedVelocity = math.clamp(Rigidbody.linearVelocity, -MaxVelocity, MaxVelocity);
        Rigidbody.linearVelocity = clampedVelocity;
    }
}

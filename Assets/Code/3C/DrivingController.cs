using System;
using Unity.Mathematics;
using Unity.Mathematics.Geometry;
using Unity.VisualScripting;
using UnityEngine;

public class DrivingController : MonoBehaviour
{
    public float MovementSpeed = 5f;
    public float TurnSpeed = 3f;
    public float MinimumTurnSpeedMultiplier = 0.3f;
    public float MaxVelocity = 20f;
    public Rigidbody Rigidbody;
    public float ForwardDrag = 0.4f;
    public float SidewayDrag = 0.3f;
    public float DriftForwardDragMultiplier = 0.5f;
    public float DriftSidewayDragMultiplier = 0.1f;
    public float DriftingTurnSpeedMultiplier = 1.2f;
    
    void Update()
    {
        var verticalInput = Input.GetAxis("Vertical");
        var horizontalInput = Input.GetAxis("Horizontal");
        var drifting = Input.GetButton("Jump");
        var deltaTime = Time.deltaTime;
        var forwardDrag = ForwardDrag;
        var sidewayDrag = SidewayDrag;
        float currentTurnSpeed = TurnSpeed;

        if (drifting)
        {
            currentTurnSpeed = TurnSpeed * DriftingTurnSpeedMultiplier;
            forwardDrag *= DriftForwardDragMultiplier;
            sidewayDrag *= DriftSidewayDragMultiplier;
        }
        
        var turnAngle = horizontalInput * currentTurnSpeed;
        Vector3 moveVector = transform.forward * verticalInput * MovementSpeed;
        Debug.Log($"Move vector: {moveVector}");
        
        var turnedDirection = Quaternion.AngleAxis(turnAngle, Vector3.up) * transform.forward;
        var desiredDirection = transform.forward + turnedDirection;
        var speedMultiplier = math.unlerp(MaxVelocity, 0f, Rigidbody.linearVelocity.magnitude);
        speedMultiplier = math.clamp(speedMultiplier, MinimumTurnSpeedMultiplier, 1f);
        var rotatedForward = Vector3.RotateTowards(transform.forward, desiredDirection.normalized, currentTurnSpeed * deltaTime * speedMultiplier, MaxVelocity);
        if(Rigidbody.linearVelocity.magnitude > 0.3f)
            transform.rotation = Quaternion.LookRotation(rotatedForward.normalized);

        var preMoveVel = Rigidbody.linearVelocity;
        Rigidbody.linearVelocity += moveVector * deltaTime;
        Rigidbody.linearVelocity += ((transform.forward * Rigidbody.linearVelocity.magnitude) - preMoveVel) * sidewayDrag * deltaTime;
        var preDragVel = Rigidbody.linearVelocity;
        Rigidbody.linearVelocity -= Rigidbody.linearVelocity.normalized * forwardDrag * deltaTime;
        if (math.dot(preDragVel, Rigidbody.linearVelocity) < 0f)
            Rigidbody.linearVelocity = Vector3.zero;
        var clampedVelocity = math.clamp(Rigidbody.linearVelocity, -MaxVelocity, MaxVelocity);
        Rigidbody.linearVelocity = clampedVelocity;
    }
}
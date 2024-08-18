using System;
using Unity.Mathematics;
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
    public ParticleSystem DrivingEmitter;
    private bool isRunning;
    private bool wasRunning;

    private void Start()
    {
        DrivingEmitter.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }

    void Update()
    {
        var verticalInput = Input.GetAxis("Vertical");
        var horizontalInput = Input.GetAxis("Horizontal");
        var drifting = Input.GetButton("Jump");
        var deltaTime = Time.deltaTime;
        var forwardDrag = ForwardDrag;
        var sidewayDrag = SidewayDrag;
        float currentTurnSpeed = TurnSpeed;
        var moveDot = math.dot(transform.forward, Rigidbody.linearVelocity.normalized);

        isRunning = verticalInput != 0;
        if (isRunning && isRunning != wasRunning)
            DrivingEmitter.Play(true);
        else if (!isRunning && isRunning != wasRunning)
            DrivingEmitter.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        wasRunning = isRunning;


        if (drifting)
        {
            currentTurnSpeed = TurnSpeed * DriftingTurnSpeedMultiplier;
            forwardDrag *= DriftForwardDragMultiplier;
            sidewayDrag *= DriftSidewayDragMultiplier;
        }
        
        var turnAngle = horizontalInput * currentTurnSpeed;
        var turnedDirection = Quaternion.AngleAxis(turnAngle, Vector3.up) * transform.forward;
        var desiredDirection = transform.forward + turnedDirection;
        var speedMultiplier = math.unlerp(0f, MaxVelocity, Rigidbody.linearVelocity.magnitude);
        speedMultiplier = math.clamp(speedMultiplier, MinimumTurnSpeedMultiplier, 1f);
        if (Rigidbody.linearVelocity.magnitude == 0f)
            speedMultiplier = 0f;
        var rotatedForward = Vector3.RotateTowards(transform.forward, desiredDirection.normalized, currentTurnSpeed * deltaTime * speedMultiplier, MaxVelocity);
        /*if(Rigidbody.linearVelocity.magnitude > 0.3f)
            transform.rotation = Quaternion.LookRotation(rotatedForward.normalized);*/
        
        Vector3 moveVector = rotatedForward * verticalInput * MovementSpeed;
        Rigidbody.linearVelocity += moveVector * deltaTime;
        Rigidbody.linearVelocity += ((transform.forward * moveDot * Rigidbody.linearVelocity.magnitude) - Rigidbody.linearVelocity) * sidewayDrag * deltaTime;
        var preDragVel = Rigidbody.linearVelocity;
        Rigidbody.linearVelocity -= Rigidbody.linearVelocity.normalized * forwardDrag * deltaTime;
        if (math.dot(preDragVel, Rigidbody.linearVelocity) < 0f)
            Rigidbody.linearVelocity = Vector3.zero;
        var clampedVelocity = math.clamp(Rigidbody.linearVelocity, -MaxVelocity, MaxVelocity);
        Rigidbody.linearVelocity = clampedVelocity;
        
        if(Rigidbody.linearVelocity.magnitude > 0.3f)
            transform.rotation = Quaternion.LookRotation(rotatedForward.normalized);
    }
}
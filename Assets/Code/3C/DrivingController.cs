using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

public class DrivingController : MonoBehaviour
{
    public float MovementSpeed = 5f;
    public float TurnSpeed = 3f;
    public float MinimumTurnSpeedMultiplier = 0.3f;
    public float MaxVelocity = 20f;
    public float MaxVelocityBoost = 20f;
    public float BoostTurnMultiplier = 0.25f;

    private ScaleReciever Scaler;
    
    public Rigidbody Rigidbody;
    public float ForwardDrag = 0.4f;
    public float SidewayDrag = 0.3f;
    
    public float DriftForwardDragMultiplier = 0.5f;
    public float DriftSidewayDragMultiplier = 0.1f;
    public float DriftingTurnSpeedMultiplier = 1.2f;
    
    public ParticleSystem DrivingEmitter;
    private bool isRunning;
    private bool wasRunning;

    public UnityEvent<bool> _brakeChanging;
    private bool braking;

    private void Start()
    {
        Scaler = GetComponent<ScaleReciever>();
        DrivingEmitter.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }

    void Update()
    {
        gameObject.transform.SetPositionAndRotation(new Vector3(transform.position.x, -0.155f * transform.localScale.y, transform.position.z), transform.rotation);
        
        var verticalInput = Input.GetAxis("Vertical");
        var horizontalInput = Input.GetAxis("Horizontal");
        var drifting = Input.GetButton("Jump");
        var boosting = Input.GetKey(KeyCode.LeftShift);
        
        var deltaTime = Time.deltaTime;
        var forwardDrag = ForwardDrag;
        var sidewayDrag = SidewayDrag;
        float currentTurnSpeed = TurnSpeed;
        float currentMovementSpeed = MovementSpeed;
        float currentMaxVelocity = MaxVelocity;
        
        if (boosting)
        {
            currentMaxVelocity = MaxVelocityBoost;
        }

        if (verticalInput < 0 && !braking)
        {
            braking = true;
            _brakeChanging.Invoke(true);
        }

        if (verticalInput >= 0 && braking)
        {
            braking = false;
            _brakeChanging.Invoke(false);
        }
        
        var moveDot = math.dot(transform.forward, Rigidbody.linearVelocity.normalized);

        isRunning = verticalInput != 0;
        if (isRunning && isRunning != wasRunning)
            DrivingEmitter.Play(true);
        else if (!isRunning && isRunning != wasRunning)
            DrivingEmitter.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        
        wasRunning = isRunning;

        if (Scaler.CurrentScale == PlayerScale.Big)
        {
            currentMovementSpeed *= Scaler.BigSpeedMultiplier;
            currentMaxVelocity *= Scaler.BigMaxVelocityMultiplier;
            forwardDrag *= Scaler.BigSpeedMultiplier;
            sidewayDrag *= Scaler.BigSpeedMultiplier;
        }
        else if (Scaler.CurrentScale == PlayerScale.Small)
        {
            currentMovementSpeed *= Scaler.SmallSpeedMultiplier;
            currentMaxVelocity *= Scaler.SmallMaxVelocityMultiplier;
            forwardDrag *= Scaler.SmallSpeedMultiplier;
            sidewayDrag *= Scaler.SmallSpeedMultiplier;
        }

        if (drifting)
        {
            currentTurnSpeed = TurnSpeed * DriftingTurnSpeedMultiplier;
            forwardDrag *= DriftForwardDragMultiplier;
            sidewayDrag *= DriftSidewayDragMultiplier;
        }
        
        var turnAngle = horizontalInput * currentTurnSpeed * moveDot;
        var turnedDirection = Quaternion.AngleAxis(turnAngle, Vector3.up) * transform.forward;
        var desiredDirection = transform.forward + turnedDirection;
        var speedMultiplier = math.unlerp(0f, MaxVelocity, Rigidbody.linearVelocity.magnitude);
        speedMultiplier = math.clamp(speedMultiplier, MinimumTurnSpeedMultiplier, 1f);
        if (Rigidbody.linearVelocity.magnitude == 0f)
            speedMultiplier = 0f;
        var rotatedForward = Vector3.RotateTowards(transform.forward, desiredDirection.normalized, currentTurnSpeed * deltaTime * speedMultiplier, MaxVelocity);
        /*if(Rigidbody.linearVelocity.magnitude > 0.3f)
            transform.rotation = Quaternion.LookRotation(rotatedForward.normalized);*/
        
        Vector3 moveVector = rotatedForward * verticalInput * currentMovementSpeed;
        if (Rigidbody.linearVelocity.magnitude < currentMaxVelocity)
        {
            Rigidbody.linearVelocity += moveVector * deltaTime;
        }
        
        Rigidbody.linearVelocity += ((transform.forward * moveDot * Rigidbody.linearVelocity.magnitude) - Rigidbody.linearVelocity) * sidewayDrag * deltaTime;
        var preDragVel = Rigidbody.linearVelocity;
        Rigidbody.linearVelocity -= Rigidbody.linearVelocity.normalized * forwardDrag * deltaTime;
        if (math.dot(preDragVel, Rigidbody.linearVelocity) < 0f)
            Rigidbody.linearVelocity = Vector3.zero;
        
        // var clampedVelocity = math.clamp(Rigidbody.linearVelocity, -currentMaxVelocity, currentMaxVelocity);
        // Rigidbody.linearVelocity = clampedVelocity;
        
        if(Rigidbody.linearVelocity.magnitude > 0.3f)
            transform.rotation = Quaternion.LookRotation(rotatedForward.normalized);
    }
}
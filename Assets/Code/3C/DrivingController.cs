using System;
using Unity.Mathematics;
using UnityEngine;

public class DrivingController : MonoBehaviour
{
    public float MovementSpeed = 5f;
    public float TurnSpeed = 3f;
    public float MinimumTurnSpeedMultiplier = 0.3f;
    public float MaxVelocity = 20f;

    private ScaleReciever Scaler;
    
    public Rigidbody Rigidbody;
    public float ForwardDrag = 0.4f;
    public float SidewayDrag = 0.3f;
    
    public float DriftForwardDragMultiplier = 0.5f;
    public float DriftSidewayDragMultiplier = 0.1f;
    public float DriftingTurnSpeedMultiplier = 1.2f;

    private float BoostAmount;
    public float MaxBoosAmount = 1.5f;
    public float BoostRechargeSpeed = 5f;
    public float BoostingMovementSpeedMultiplier = 2f;
    public float BoostingMaxVelocityMultiplier = 2f;
    
    public ParticleSystem DrivingEmitter;
    private bool isRunning;
    private bool wasRunning;

    private void Start()
    {
        Scaler = GetComponent<ScaleReciever>();
        DrivingEmitter.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        BoostAmount = MaxBoosAmount;
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

        if (boosting && BoostAmount > 0f)
        {
            currentMovementSpeed *= BoostingMovementSpeedMultiplier;
            currentMaxVelocity *= BoostingMaxVelocityMultiplier;
            BoostAmount -= deltaTime;
        }
        else if (!boosting)
        {
            BoostAmount += deltaTime / BoostRechargeSpeed;
        }

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
        
        Vector3 moveVector = rotatedForward * verticalInput * currentMovementSpeed;
        Rigidbody.linearVelocity += moveVector * deltaTime;
        Rigidbody.linearVelocity += ((transform.forward * moveDot * Rigidbody.linearVelocity.magnitude) - Rigidbody.linearVelocity) * sidewayDrag * deltaTime;
        var preDragVel = Rigidbody.linearVelocity;
        Rigidbody.linearVelocity -= Rigidbody.linearVelocity.normalized * forwardDrag * deltaTime;
        if (math.dot(preDragVel, Rigidbody.linearVelocity) < 0f)
            Rigidbody.linearVelocity = Vector3.zero;
        var clampedVelocity = math.clamp(Rigidbody.linearVelocity, -currentMaxVelocity, currentMaxVelocity);
        Rigidbody.linearVelocity = clampedVelocity;
        
        if(Rigidbody.linearVelocity.magnitude > 0.3f)
            transform.rotation = Quaternion.LookRotation(rotatedForward.normalized);
    }
}
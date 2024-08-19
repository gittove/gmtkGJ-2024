using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

public class DrivingController : MonoBehaviour
{
    public AnimationCurve AccelerationCurve;
    public AnimationCurve TurningCurve;
    public AnimationCurve GripCurve;
    
    public float Acceleration = 5f;
    public float EngineBrake = 3f;
    public float BrakeSpeed = 5;
    public float TurnAcceleration = 180f;
    public float MaxTurnSpeed = 90f;
    public float MinimumSpeedToTurn = 0.3f;
    public float MaxGrip = 45f;
    public float HandbrakeGripFactor = 0.75f;
    
    public float MaxVelocity = 20f;
    public float MaxVelocityBoost = 40f;

    private ScaleReciever Scaler;
    
    public Rigidbody Rigidbody;
    
    public ParticleSystem DrivingEmitter;
    private bool isRunning;
    private bool wasRunning;

    public UnityEvent<bool> _driftChange;
    private bool braking;

    private float Speed;
    
    public float Slippage;
    private bool _drift;
    private bool _forceDrift;

    private void Start()
    {
        Scaler = GetComponent<ScaleReciever>();
        DrivingEmitter.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }

    private void Update()
    {
        //gameObject.transform.SetPositionAndRotation(new Vector3(transform.position.x, -0.155f * transform.localScale.y, transform.position.z), transform.rotation);
        
        var handbrake = Input.GetAxis("Jump");
        _forceDrift = handbrake > 0;
        
        var boosting = Input.GetKey(KeyCode.LeftShift);
        
        float maxVelocity = MaxVelocity;
        
        if (boosting)
        {
            maxVelocity = MaxVelocityBoost;
        }
        
        var facingDotVelocity = math.dot(transform.forward, Rigidbody.linearVelocity.normalized);
        Slippage = facingDotVelocity;
        
        // if (Scaler.CurrentScale == PlayerScale.Big)
        // {
        //     currentMovementSpeed *= Scaler.BigSpeedMultiplier;
        //     maxVelocity *= Scaler.BigMaxVelocityMultiplier;
        //     forwardDrag *= Scaler.BigSpeedMultiplier;
        //     sidewayDrag *= Scaler.BigSpeedMultiplier;
        // }
        // else if (Scaler.CurrentScale == PlayerScale.Small)
        // {
        //     currentMovementSpeed *= Scaler.SmallSpeedMultiplier;
        //     maxVelocity *= Scaler.SmallMaxVelocityMultiplier;
        //     forwardDrag *= Scaler.SmallSpeedMultiplier;
        //     sidewayDrag *= Scaler.SmallSpeedMultiplier;
        // }

        // if (handbrake)
        // {
        //     turnSpeed = TurnSpeed * DriftingTurnSpeedMultiplier;
        //     forwardDrag *= DriftForwardDragMultiplier;
        //     sidewayDrag *= DriftSidewayDragMultiplier;
        // }
        //
        // if (handbrake && !_drift)
        // {
        //     _drift = true;
        //     _driftChange.Invoke(true);
        // }
        //
        // if (!handbrake && _drift)
        // {
        //     _drift = false;
        //     _driftChange.Invoke(false);
        // }
        
        // var turnedDirection = Quaternion.AngleAxis(turnAngle, Vector3.up) * transform.forward;
        var desiredDirection = transform.forward;
        var speedMultiplier = math.unlerp(0f, MaxVelocity, Rigidbody.linearVelocity.magnitude);
        speedMultiplier = math.clamp(speedMultiplier, MinimumSpeedToTurn, 1f);
        if (Rigidbody.linearVelocity.magnitude == 0f)
            speedMultiplier = 0f;

        Accelerate();
        Turning();
        //KeepUpright();
        Compensate();

        const float driftFactor = 0.88f;
        if ((Slippage < driftFactor && ! _drift || _forceDrift) && Rigidbody.linearVelocity.magnitude > 0.1f)
        {
            _drift = true;
            _driftChange.Invoke(_drift);
        }
        if (Slippage > driftFactor && _drift)
        {
            if (!_forceDrift)
            {
                _drift = false;
                _driftChange.Invoke(_drift);
            }
        }
        
        // var rotatedForward = Vector3.RotateTowards(transform.forward, desiredDirection.normalized, turnSpeed * deltaTime * speedMultiplier, MaxVelocity);
        // /*if(Rigidbody.linearVelocity.magnitude > 0.3f)
        //     transform.rotation = Quaternion.LookRotation(rotatedForward.normalized);*/
        //
        // Vector3 moveVector = rotatedForward * (accelInput * acceleration);
        // if (Rigidbody.linearVelocity.magnitude < maxVelocity)
        // {
        //     Rigidbody.linearVelocity += moveVector * deltaTime;
        // }
        //
        //
        // Rigidbody.linearVelocity += (transform.forward * (facingDotVelocity * Rigidbody.linearVelocity.magnitude) - Rigidbody.linearVelocity) * (sidewayDrag * deltaTime);
        // var preDragVel = Rigidbody.linearVelocity;
        // Rigidbody.linearVelocity -= Rigidbody.linearVelocity.normalized * (forwardDrag * deltaTime);
        // if (math.dot(preDragVel, Rigidbody.linearVelocity) < 0f)
        //     Rigidbody.linearVelocity = Vector3.zero;
        
        // var clampedVelocity = math.clamp(Rigidbody.linearVelocity, -maxVelocity, maxVelocity);
        // Rigidbody.linearVelocity = clampedVelocity;

        // if (Rigidbody.linearVelocity.magnitude > 0.3f)
        // {
        //     transform.rotation = Quaternion.LookRotation(rotatedForward.normalized);
        // }
    }

    private void Compensate()
    {
        var handbrake = Input.GetAxis("Jump");
        var velocity = Rigidbody.linearVelocity;
        var speed = velocity.magnitude;
        var expectedVelocity = transform.forward.normalized * speed;

        var grip = GripCurve.Evaluate(speed / MaxVelocity) * MaxGrip;
        if (handbrake > 0)
        {
            grip *= HandbrakeGripFactor;
        }

        Debug.Log($"{grip}");
        Rigidbody.linearVelocity = Vector3.RotateTowards(velocity, expectedVelocity, grip * Mathf.Deg2Rad * Time.deltaTime, Acceleration * Time.deltaTime);
    }

    private void Accelerate()
    {
        var accelInput = Input.GetAxis("Acceleration");
        var brake = Input.GetAxis("Brake"); 
        var forward = transform.forward.normalized;
        var velocity = Rigidbody.linearVelocity;

        if (brake == 0)
        {
            if (accelInput > 0)
            {
                var speed = Rigidbody.linearVelocity.magnitude;
                var acceleration = AccelerationCurve.Evaluate(speed / MaxVelocity) * Acceleration;
                velocity += forward * (acceleration * accelInput * Time.deltaTime);
            }
            else if (accelInput == 0)
            {
                var magnitude = velocity.magnitude;
                magnitude = Mathf.MoveTowards(magnitude, 0, EngineBrake * Time.deltaTime);
                velocity = velocity.normalized * magnitude;
            }
        }
        else
        {
            var magnitude = velocity.magnitude;
            magnitude = Mathf.MoveTowards(magnitude, 0, BrakeSpeed * Time.deltaTime);
            velocity = velocity.normalized * magnitude;
        }

        velocity.y = 0;
        velocity = Vector3.ClampMagnitude(velocity, MaxVelocity);
        Rigidbody.linearVelocity = velocity;
    }

    private void Turning()
    {
        var velocity = Rigidbody.linearVelocity;
        
        var maxTurnSpeed = MaxTurnSpeed * Mathf.Deg2Rad; 
        var horizontalInput = Input.GetAxis("Horizontal");
        var turnSpeed = horizontalInput * TurnAcceleration * Mathf.Deg2Rad;
        
        var angVel = Rigidbody.angularVelocity;
        if (horizontalInput != 0 && velocity.magnitude > MinimumSpeedToTurn)
        {
            if (horizontalInput > 0)
            {
                angVel.y = Mathf.MoveTowards(angVel.y, maxTurnSpeed, turnSpeed * Time.deltaTime);
            }
            else
            {
                angVel.y = Mathf.MoveTowards(angVel.y, -maxTurnSpeed, -turnSpeed * Time.deltaTime);
            }
        }
        else
        {
            angVel.y = Mathf.MoveTowards(angVel.y, 0, TurnAcceleration * Mathf.Deg2Rad * Time.deltaTime);
        }

        angVel.x = 0;
        angVel.z = 0;
        Rigidbody.angularVelocity = angVel;
    }

    private void KeepUpright()
    {
        var forward = transform.forward;
        var flatForward = new Vector3(forward.x, 0, forward.z).normalized;

        if (flatForward != Vector3.zero)
        {
            var wantedRotation = Quaternion.LookRotation(flatForward, Vector3.up);
            var rot = Quaternion.RotateTowards(transform.rotation, wantedRotation, 360f * Time.deltaTime);
            Rigidbody.MoveRotation(rot);
        }
    }
}
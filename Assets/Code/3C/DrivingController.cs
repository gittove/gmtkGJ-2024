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
    public float HandbrakeTurnFactor = 1.25f;

    public float SmallSpeedMultiplier;
    
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

        Rigidbody.linearVelocity = Vector3.RotateTowards(velocity, expectedVelocity, grip * Mathf.Deg2Rad * Time.deltaTime, Acceleration * Time.deltaTime);
    }

    private void Accelerate()
    {
        var accelInput = Input.GetAxis("Acceleration");
        var brake = Input.GetAxis("Brake"); 
        var forward = transform.forward.normalized;
        var velocity = Rigidbody.linearVelocity;

        var maxVelocity = MaxVelocity;
        if (Scaler.CurrentScale == PlayerScale.Small)
        {
            maxVelocity *= SmallSpeedMultiplier;
        }
        
        if (brake == 0)
        {
            if (accelInput > 0)
            {
                var speed = Rigidbody.linearVelocity.magnitude;
                var acceleration = AccelerationCurve.Evaluate(speed / maxVelocity) * Acceleration;
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
        velocity = Vector3.ClampMagnitude(velocity, maxVelocity);
        Rigidbody.linearVelocity = velocity;
    }

    private void Turning()
    {
        var velocity = Rigidbody.linearVelocity;
        
        var maxTurnSpeed = MaxTurnSpeed * Mathf.Deg2Rad; 
        var horizontalInput = Input.GetAxis("Horizontal");
        var turnSpeed = horizontalInput * TurnAcceleration * Mathf.Deg2Rad;
        if (_drift)
        {
            maxTurnSpeed *= HandbrakeTurnFactor;
            turnSpeed *= HandbrakeTurnFactor;
        }
        
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
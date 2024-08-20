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
    public float MaxReverseVelocity = 5f;

    private ScaleReciever Scaler;
    
    public Rigidbody Rigidbody;
    
    public ParticleSystem DrivingEmitter;
    private bool isRunning;
    private bool wasRunning;

    public UnityEvent<bool> _reverse;
    public UnityEvent<bool> _driftChange;
    public UnityEvent _startDriving;
    public UnityEvent _endDriving;
    private bool braking;

    private float Speed;
    
    public float Slippage;
    private bool _drift;
    private bool _forceDrift;
    private bool _driving;

    private void Start()
    {
        Scaler = GetComponent<ScaleReciever>();
        DrivingEmitter.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }

    private void Update()
    {
        var handbrake = Input.GetAxis("Jump");
        _forceDrift = handbrake > 0;
        
        var facingDotVelocity = math.dot(transform.forward, Rigidbody.linearVelocity.normalized);
        Slippage = facingDotVelocity;
        
        if (Rigidbody.linearVelocity.magnitude < 0.1f)
        {
            Slippage = 1;
        }

        Accelerate();
        Turning();
        //KeepUpright();
        Compensate();

        const float driftFactor = 0.88f;
        if ((Slippage < driftFactor && ! _drift || _forceDrift) && Rigidbody.linearVelocity.magnitude > 0.25f)
        {
            _drift = true;
            _driftChange.Invoke(_drift);
        }
        if (Slippage > driftFactor && _drift)
        {
            if (!_forceDrift || Rigidbody.linearVelocity.magnitude < 0.25f)
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
        var accel = Input.GetAxis("Acceleration");
        var brake = Input.GetAxis("Brake"); 
        var forward = transform.forward.normalized;
        var velocity = Rigidbody.linearVelocity;

        var maxVelocity = MaxVelocity;
        var maxReverse = MaxReverseVelocity;
        if (Scaler.CurrentScale == PlayerScale.Small)
        {
            maxVelocity *= SmallSpeedMultiplier;
            maxReverse *= SmallSpeedMultiplier;
        }
        
        if (accel > 0)
        {
            if (!_driving)
            {
                _driving = true;
                _startDriving.Invoke();
            }

            if (!_forceDrift)
            {
                var speed = Rigidbody.linearVelocity.magnitude;
                var acceleration = AccelerationCurve.Evaluate(speed / maxVelocity) * Acceleration;
                if (Scaler.CurrentScale == PlayerScale.Small)
                {
                    acceleration *= SmallSpeedMultiplier;
                }

                velocity += forward * (acceleration * accel * Time.deltaTime);
            }
            else
            {
                var magnitude = velocity.magnitude;
                magnitude = Mathf.MoveTowards(magnitude, 0, EngineBrake * Time.deltaTime);
                velocity = velocity.normalized * magnitude;
            }
        }
        else
        {
            var magnitude = velocity.magnitude;
            magnitude = Mathf.MoveTowards(magnitude, 0, EngineBrake * Time.deltaTime);
            velocity = velocity.normalized * magnitude;
        }
        if (brake > 0)
        {
            _reverse.Invoke(true);
            var reverse = BrakeSpeed;
            if (Scaler.CurrentScale == PlayerScale.Small)
            {
                reverse *= SmallSpeedMultiplier;
            }

            if (velocity.magnitude < maxReverse)
            {
                velocity += -forward * (reverse * brake * Time.deltaTime);
            }
            else
            {
                var magnitude = velocity.magnitude;
                magnitude = Mathf.MoveTowards(magnitude, 0, BrakeSpeed * Time.deltaTime);
                velocity = velocity.normalized * magnitude;
            }
        }
        else
        {
            _reverse.Invoke(false);
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
            var finalTurn = TurningCurve.Evaluate(velocity.magnitude / MaxVelocity) * turnSpeed;
            if (horizontalInput > 0)
            {
                angVel.y = Mathf.MoveTowards(angVel.y, maxTurnSpeed, finalTurn * Time.deltaTime);
            }
            else
            {
                angVel.y = Mathf.MoveTowards(angVel.y, -maxTurnSpeed, -finalTurn * Time.deltaTime);
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
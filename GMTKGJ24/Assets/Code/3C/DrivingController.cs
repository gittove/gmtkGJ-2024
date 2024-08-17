using System;
using Unity.Mathematics;
using Unity.Mathematics.Geometry;
using UnityEngine;

public class DrivingController : MonoBehaviour
{
    public float MovementSpeed = 5f;
    public float TurnSpeed = 3f;
    public float MaxVelocity = 20f;
    public Rigidbody Rigidbody;
    public float ForwardDrag = 0.4f;
    public float SidewayDrag = 0.3f;
    public float DriftForwardDragMultiplier = 0.5f;
    public float DriftSidewayDragMultiplier = 0.1f;
    public float DriftingTurnSpeedMultiplier = 1.2f;
    
    private float currentTurnSpeed;

    private void Start()
    {
        currentTurnSpeed = TurnSpeed;
    }

    void Update()
    {
        var verticalInput = Input.GetAxis("Vertical");
        var horizontalInput = Input.GetAxis("Horizontal");
        var driftActivated = Input.GetButtonDown("Jump");
        var drifting = Input.GetButton("Jump");
        var driftDeactivated = Input.GetButtonUp("Jump");
        var deltaTime = Time.deltaTime;

        var turnAngle = horizontalInput * TurnSpeed;
        Vector3 moveVector = transform.forward * verticalInput * MovementSpeed;
        var turnedDirection = Quaternion.AngleAxis(turnAngle, Vector3.up) * transform.forward;
        var desiredDirection = transform.forward + turnedDirection;
        var speedMultiplier = math.unlerp(0f, MaxVelocity, Rigidbody.linearVelocity.magnitude);
        var rotatedForward = Vector3.RotateTowards(transform.forward, desiredDirection.normalized, TurnSpeed * deltaTime * speedMultiplier, MaxVelocity);
        transform.rotation = Quaternion.LookRotation(rotatedForward.normalized);
        
        //Debug.DrawLine(transform.position, transform.position + desiredDirection.normalized * 2f, Color.red);
        Rigidbody.linearVelocity += ((transform.forward * Rigidbody.linearVelocity.magnitude) - Rigidbody.linearVelocity) * SidewayDrag * deltaTime;
        Rigidbody.linearVelocity += moveVector * deltaTime;
        Rigidbody.linearVelocity -= Rigidbody.linearVelocity.normalized * ForwardDrag * deltaTime;
        var clampedVelocity = math.clamp(Rigidbody.linearVelocity, -MaxVelocity, MaxVelocity);
        Rigidbody.linearVelocity = clampedVelocity;

        /*if (driftActivated)
        {
            currentTurnSpeed = TurnSpeed * DriftingTurnSpeedMultiplier;
        }
        else if (driftDeactivated)
        {
            currentTurnSpeed = TurnSpeed;
        }
        
        var rotatedTransform = gameObject.transform;
        rotatedTransform.Rotate(new Vector3(0, 1, 0), currentTurnSpeed * deltaTime * horizontalInput * math.clamp(math.unlerp(0f, 1f, Rigidbody.linearVelocity.magnitude), 0f, 1f));
        Rigidbody.MoveRotation(rotatedTransform.rotation);
        var oldVelocity = Rigidbody.linearVelocity;
        Rigidbody.linearVelocity += moveVector;
        Rigidbody.linearVelocity = Rigidbody.linearVelocity.magnitude * gameObject.transform.forward;
        var clampedVelocity = math.clamp(Rigidbody.linearVelocity, -MaxVelocity, MaxVelocity);
        Rigidbody.linearVelocity = clampedVelocity;

        var velocityDot = math.dot(oldVelocity.normalized, gameObject.transform.forward);
        var sideDrag = SidewayDrag * (1f - velocityDot);
        var forwardDrag = ForwardDrag * velocityDot;
        if (drifting)
        {
            sideDrag *= DriftSidewayDragMultiplier;
            forwardDrag *= DriftForwardDragMultiplier;
        }

        var offOffset = new Vector3(0, 1, 0);
        var dragDirecion = Rigidbody.linearVelocity.normalized - oldVelocity.normalized;
        Debug.DrawLine(transform.position, transform.position - dragDirecion.normalized * (sideDrag + forwardDrag), Color.red);
        var directionDrag = oldVelocity.normalized * (sideDrag + forwardDrag);
        Debug.DrawLine(transform.position + offOffset, transform.position + offOffset - directionDrag * 3f, Color.blue);
        Debug.Log($"side: {sideDrag}, forward: {forwardDrag}, current drag: {transform.TransformDirection(directionDrag)}, drag direction: {dragDirecion}");

        var clampedDrag = math.clamp(Rigidbody.linearVelocity, -MaxVelocity, MaxVelocity);
        Rigidbody.linearVelocity += dragDirecion.normalized * (sideDrag + forwardDrag) * deltaTime;*/
    }
}

using System;
using UnityEngine;

public class ProgradeTracker : MonoBehaviour
{
    [SerializeField] private Vector3 _constantOffset = new Vector2(2, 1);
    [SerializeField] private float _velocityMultiplier = 0.3f;
    [SerializeField] private Rigidbody _target;

    [SerializeField] private bool _onlyFollow;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            _onlyFollow = !_onlyFollow;
        }
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        var targetDir = _target.transform.forward;
        var constantPoint = targetDir * _constantOffset.x;
        constantPoint.y += _constantOffset.y;

        var velocity = _target.linearVelocity;
        var velocityPoint = velocity * _velocityMultiplier;

        var finalPoint = (constantPoint + velocityPoint) / 2f;
        var wantedPoint = _target.position + finalPoint;

        if (_onlyFollow)
        {
            transform.position = _target.position;
        }
        else
        {
            transform.position = wantedPoint;
        }
    }
}

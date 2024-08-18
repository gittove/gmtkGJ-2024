using System;
using UnityEngine;
using UnityEngine.Events;

public class ImpactTracker : MonoBehaviour
{
    [SerializeField] private Vector3 _thresholds;

    [SerializeField] private UnityEvent _smallImpact;
    [SerializeField] private UnityEvent _mediumImpact;
    [SerializeField] private UnityEvent _bigImpact;

    private void OnCollisionEnter(Collision other)
    {
        var mag = other.relativeVelocity.magnitude; 
        if (mag > _thresholds.z)
        {
            _bigImpact.Invoke();
            _mediumImpact.Invoke();
            _smallImpact.Invoke();
        }
        else if (mag > _thresholds.y)
        {
            _mediumImpact.Invoke();
            _smallImpact.Invoke();
        }
        else if (mag > _thresholds.x)
        {
            _smallImpact.Invoke();
        }
    }
}

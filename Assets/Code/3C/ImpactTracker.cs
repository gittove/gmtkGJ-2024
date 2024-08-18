using System;
using UnityEngine;

public class ImpactTracker : MonoBehaviour
{
    [SerializeField] private ParticleSystem _smallHit;
    [SerializeField] private float _smallHitThreshold;

    private void OnCollisionEnter(Collision other)
    {
        if (other.relativeVelocity.magnitude > _smallHitThreshold)
        { 
            _smallHit.Play();
        }
    }
}

using UnityEngine;
using UnityEngine.Events;

public class ImpactTracker : MonoBehaviour
{
    [SerializeField] private Vector3 _thresholds;

    [SerializeField] private UnityEvent _smallImpact;
    [SerializeField] private UnityEvent _mediumImpact;
    [SerializeField] private UnityEvent _bigImpact;
    private ScaleReciever _scaler;

    private void Start()
    {
        _scaler = GetComponent<ScaleReciever>();
    }

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

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Collided with layer: {other.gameObject.layer}, looking for {LayerMask.NameToLayer("Shortcut")}");
        if (other.gameObject.layer == LayerMask.NameToLayer("Shortcut"))
        {
            Debug.Log($"Entered shortcut");
            _scaler.CanScale = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Shortcut"))
        {
            Debug.Log($"Exited shortcut");
            _scaler.CanScale = true;
        } 
        
    }
}

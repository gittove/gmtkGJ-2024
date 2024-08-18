using UnityEngine;

public class EngineSoundController : MonoBehaviour
{
    [SerializeField] private Rigidbody _bodySource;
    [SerializeField] private AudioSource _audio;

    [SerializeField] private Vector2 _pitchRange;
    [SerializeField] private Vector2 _cutoffPoints;
    
    // Update is called once per frame
    private void Update()
    {
        var vel = _bodySource.linearVelocity.magnitude;
        var unlerp = Mathf.InverseLerp(_cutoffPoints.x, _cutoffPoints.y, vel);
        _audio.pitch = Mathf.Lerp(_pitchRange.x, _pitchRange.y, unlerp);
    }
}

using System;
using Unity.Cinemachine;
using UnityEngine;

public class CameraFOV : MonoBehaviour
{
    public CinemachineCamera Camera;
    public ScaleReciever Scale;

    public float SmallFOV;

    private float _startFov;
    private float _targetFov;
    private float _fov;

    private void Awake()
    {
        Scale = FindFirstObjectByType<ScaleReciever>();
    }

    private void Start()
    {
        _startFov = Camera.Lens.FieldOfView;
        _targetFov = _startFov;
    }

    private void Update()
    {
        if (Scale.CurrentScale == PlayerScale.Medium)
        {
            _targetFov = _startFov;
        }
        else
        {
            _targetFov = SmallFOV;
        }
        var diff = Mathf.Abs(_fov - _targetFov);
        _fov = Mathf.MoveTowards(_fov, _targetFov, 2.34f + Mathf.Pow(diff, 1.3f) * Time.deltaTime);
        Camera.Lens.FieldOfView = _fov;
    }
}

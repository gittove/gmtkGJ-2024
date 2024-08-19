using System;
using Unity.Cinemachine;
using UnityEngine;

public class CameraFOV : MonoBehaviour
{
    public CinemachineCamera Camera;
    public ScaleReciever Scale;

    public float SmallFOV;

    private float _startFov;

    private void Awake()
    {
        Scale = FindFirstObjectByType<ScaleReciever>();
    }

    private void Start()
    {
        _startFov = Camera.Lens.FieldOfView;
    }

    private void Update()
    {
        if (Scale.CurrentScale == PlayerScale.Medium)
        {
            Camera.Lens.FieldOfView = _startFov;
        }
        else
        {
            Camera.Lens.FieldOfView = SmallFOV;
        }
    }
}

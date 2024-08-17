using System;
using UnityEngine;

public enum PlayerScale
{
    Big,
    Medium,
    Small,
}

public class PlayerScaler : MonoBehaviour
{
    public PlayerScale Scale = PlayerScale.Medium;
    public BoxCollider TriggerBox;

    private void OnTriggerEnter(Collider other)
    {
        var scaler = other.gameObject.GetComponent<ScaleReciever>();
        if (scaler == null)
            return;

        scaler.CurrentScale = Scale;
    }
}

using System;
using UnityEngine;

public class EditorSnap : MonoBehaviour
{
    [SerializeField] private float _increment;
    
    private void OnDrawGizmosSelected()
    {
        var pos = transform.position;
        pos.x = Mathf.Round(pos.x / _increment) * _increment;
        pos.z = Mathf.Round(pos.z / _increment) * _increment;
        transform.position = pos;
    }
}

using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    public GameObject FollowTarget;

    public Vector3 PositionOffset;

    public Vector3 LookRotation;
    
    void LateUpdate()
    {
        var newPosition = FollowTarget.transform.position + PositionOffset;
        gameObject.transform.position = newPosition;
        gameObject.transform.rotation = Quaternion.Euler(LookRotation);
    }
}

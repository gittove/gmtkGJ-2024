using Unity.Mathematics;
using UnityEngine;

public class ThrownFood : MonoBehaviour
{
    public AudioSource ImpactSound;
    public Vector3 SourcePosition;
    public Vector3 TargetPosition;

    private float lerpTimer;
    private const float lerpTime = 0.8f;
    private bool isDone;

    void Update()
    {
        if (isDone)
        {
            lerpTimer -= Time.deltaTime;
            if(lerpTimer <= 0f)
                Destroy(this);
            return;
        }
        transform.position = math.lerp(SourcePosition, TargetPosition, lerpTimer);
        lerpTimer += Time.deltaTime / lerpTime;
        if (lerpTimer >= 1f)
        {
            lerpTimer = 1f;
            ImpactSound.Play();
            isDone = true;
        }
    }
}

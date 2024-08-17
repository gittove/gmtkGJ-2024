using UnityEngine;

public class ScaleReciever : MonoBehaviour
{
    public PlayerScale CurrentScale = PlayerScale.Medium;
    private PlayerScale previousScale;
    
    void Start()
    {
        previousScale = CurrentScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (previousScale == CurrentScale)
            return;

        switch (CurrentScale)
        {
            case PlayerScale.Big:
                transform.localScale = new Vector3(3, 3, 3);
                break;
            case PlayerScale.Medium:
                transform.localScale = new Vector3(1, 1, 1);
                break;
            case PlayerScale.Small:
                transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
                break;
        }

        previousScale = CurrentScale;
        Debug.Log($"Player scale: {CurrentScale}");
    }
}

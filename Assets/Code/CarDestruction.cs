using UnityEngine;

public class CarDestruction : MonoBehaviour
{
    public Rigidbody[] DestructionParts;

    private int _index = 0;

    private void Start()
    {
        _index = 0;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            var part = DestructionParts[_index];

            part.isKinematic = false;
            part.AddExplosionForce(1000f, part.gameObject.transform.position, 1f);

            _index++;
        }
    }
}

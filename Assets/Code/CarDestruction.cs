using UnityEngine;

public class CarDestruction : MonoBehaviour
{
    public GameObject[] DestructionParts;

    private DrivingController _drivingController;
    private int _index = 0;
    private bool _done;
    
    [SerializeField] private float _destructionCooldown;
    private float _destructionCooldownTimer;

    private void Start()
    {
        _done = false;
        _index = 0;
        _drivingController = GetComponent<DrivingController>();
        _destructionCooldownTimer = _destructionCooldown;
    }

    private void Update()
    {
        _destructionCooldownTimer -= Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_done || collision.relativeVelocity.magnitude < 3 || _destructionCooldownTimer > 0f)
        {
            return;
        }

        DestructionParts[_index].transform.parent = null;
        var part = DestructionParts[_index].AddComponent<Rigidbody>();
        DestructionParts[_index].AddComponent<BoxCollider>();

        part.AddExplosionForce(100f, part.gameObject.transform.position, 1f);

        _index++;
        // _drivingController.MaxVelocity += 2;
        _destructionCooldownTimer = _destructionCooldown;

        if (_index >= DestructionParts.Length)
        {
            _done = true;
        }
    }
}

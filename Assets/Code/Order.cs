using Unity.Mathematics;
using UnityEngine;

public class Order : MonoBehaviour
{
    public float PICKUP_TIME = 30f;
    public float DELIVER_TIME = 60f;
    private HUD playerHUD;
    private float _deliveryTimer;
    private float _pickupTimer;
    private float _score;
    public bool IsPickedUp = false;
    public GameObject ThrownFood;

    [SerializeField] private float _pickupInteractionSeconds = 2f;
    [SerializeField] private GameObject _pickupMesh;

    private float _interactionTimer;

    public float3 Position => gameObject.transform.position;

    public void Setup()
    {
        _interactionTimer = _pickupInteractionSeconds;
        _pickupTimer = PICKUP_TIME;
        _deliveryTimer = DELIVER_TIME;

        IsPickedUp = false;
    }

     public float GetTimer()
     {
         if (IsPickedUp)
         {
             return _deliveryTimer / DELIVER_TIME;
         }
        
         return _pickupTimer / PICKUP_TIME;
    }

    private void Start()
    {
        playerHUD = FindFirstObjectByType<HUD>();
        playerHUD.AddOrderIndicator(this);

        _score = 1000;
    }

    public void Complete()
    {
        _score += (1000 * _deliveryTimer / DELIVER_TIME) * 0.5f;
        playerHUD.GainScore((int)_score);
        playerHUD.RemoveIndicator(this);
        Destroy(this.gameObject);
        ThrowFood();
    }

    private void OnPickup(Transform newParent)
    {
        IsPickedUp = true;
        transform.parent = newParent;
        _score += (1000 * _pickupTimer / PICKUP_TIME) * 0.5f;
        
        int orderID = GetInstanceID();

        var customers = FindObjectsByType<Customer>(FindObjectsSortMode.None);
        int pickIndex = UnityEngine.Random.Range(0, customers.Length - 1);

        int attempts = 0;
        while (customers[pickIndex].GetComponent<Customer>().Occupied)
        {
            pickIndex = UnityEngine.Random.Range(0, customers.Length - 1);
            attempts++;
            if (attempts > 5)
            {
                return;
            }
        }
        
        customers[pickIndex].GetComponent<Customer>().Activate(orderID, this.gameObject);
        
        _interactionTimer = _pickupInteractionSeconds;
        GetComponent<BoxCollider>().enabled = false;
        _pickupMesh.gameObject.SetActive(false);
        
        playerHUD.AddIndicator(this);
        playerHUD.RemoveOrderIndicator(this);
    }

    void ThrowFood()
    {
        var food = Instantiate(ThrownFood);
        var thrown = food.GetComponent<ThrownFood>();
        thrown.SourcePosition = FindFirstObjectByType<DrivingController>().gameObject.transform.position;
        var squares = FindObjectsByType<DeliverySquare>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        foreach (var square in squares)
        {
            if (square.OrderID == GetInstanceID())
            {
                thrown.TargetPosition = square.ParentDropOffPoint;
                break;
            }
        }
    }
    
    void Update()
    {
        if (IsPickedUp)
        {
            _deliveryTimer -= Time.deltaTime;
        }
        else
        {
            _pickupTimer -= Time.deltaTime;
        }
        if (_pickupTimer <= 0f || _deliveryTimer <= 0f)
        {
            playerHUD.RemoveIndicator(this);
            playerHUD.RemoveOrderIndicator(this);
            playerHUD.AddFailedOrderIcon(this);
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        _interactionTimer -= Time.deltaTime;
        
        if (_interactionTimer <= 0f)
        {
            OnPickup(other.gameObject.transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        _interactionTimer = _pickupInteractionSeconds;
    }

    private void OnDrawGizmos()
    {
        if (enabled)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(transform.position + transform.up * 1.5f, 0.2f);

            if (GetComponent<BoxCollider>().enabled)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawWireCube(transform.position + GetComponent<BoxCollider>().center, GetComponent<BoxCollider>().size);
            }
        }
    }
}

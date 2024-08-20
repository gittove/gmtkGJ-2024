using System;
using System.Collections.Generic;
using UnityEngine;

public class ResturantManager : MonoBehaviour
{
    [SerializeField] private float _orderSpawnTimer = 15;
    
    public int MaxActiveRestaurants = 2;
    
    private float _timer;
    
    private Restaurant[] _restaurants;

    private Queue<Restaurant> _orderQueue;

    void Start()
    {
        _timer = 5f;
        _orderQueue = new Queue<Restaurant>();

        _restaurants = FindObjectsByType<Restaurant>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        GenerateOrderQueue();
    }

    void Update()
    {
        _timer -= Time.deltaTime;
        
        if (_orderQueue.Count == 0)
        {
            GenerateOrderQueue();
            return;
        }

        if (_timer <= 0)
        {
            TryActivateOrder();
            _timer = _orderSpawnTimer;
        }
    }

    private void TryActivateOrder()
    {
        var orders = FindObjectsByType<Order>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        int activeOrdersCount = orders.Length;

        if (activeOrdersCount >= MaxActiveRestaurants)
        {
            return;
        }
        
        int attemptCount = 0;
        var pickedRestaurant = _orderQueue.Dequeue();
        while (pickedRestaurant.gameObject.GetComponentInChildren<Order>() != null && attemptCount < _restaurants.Length)
        {
            attemptCount++;
            pickedRestaurant = _orderQueue.Dequeue();

            if (_orderQueue.Count == 0)
            {
                GenerateOrderQueue();
            }
        }
        
        pickedRestaurant.ActivateOrder();
    }

    private void GenerateOrderQueue()
    {
        for (int i = 0; i < 10; i++)
        {
            int indexLargestWeight = 0;
            float largestWeight = float.MinValue;
            for (int k = 0; k < _restaurants.Length; k++)
            {
                var restaurant = _restaurants[k];
                if (restaurant.Weight > largestWeight)
                {
                    indexLargestWeight = k;
                    largestWeight = restaurant.Weight;
                }
            }

            var pickedRestaurant = _restaurants[indexLargestWeight];
            pickedRestaurant.Deplete();
            _orderQueue.Enqueue(pickedRestaurant);
        }
    }
}

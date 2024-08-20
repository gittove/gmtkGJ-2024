using System;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HUD : MonoBehaviour
{
    private class ScreenIndicator
    {
        public GameObject Indicator;
        public Order Order;
    }
    
    private class FailedScreenIndicator
    {
        public GameObject Indicator;
        public Vector3 OrderPosition;
        public float FailTimer = 2.5f;
    }
    
    private DrivingController Player;
    private DeliverySquare[] DeliverySquares;
    private Order[] OrderSquares;
    private Camera MainCamera;
    private WinScreen WinScreen;
    
    public TMP_Text ScoreText;
    public TMP_Text GameTimerText;
    public double GameTime = 5.15;
    private float gameTimer;

    public AudioSource ScoreSound;
    public AudioSource PickupSound;
    public AudioSource CallSound;
    public AudioSource FailSound;

    public GameObject DeliveryScreenIndicator;
    public GameObject OrderScreenIndicator;
    public GameObject FailedOrderScreenIndicator;
    private List<ScreenIndicator> DeliveryIndicators;
    private List<ScreenIndicator> OrderIndicators;
    private List<FailedScreenIndicator> FailedOrderIndicators;

    public int Score;
    public int CompletedDeliveries;
    public int FailedDeliveries;

    void Start()
    {
        MainCamera = Camera.main;
        WinScreen = FindFirstObjectByType<WinScreen>(FindObjectsInactive.Include);

        DeliveryIndicators = new List<ScreenIndicator>();
        OrderIndicators = new List<ScreenIndicator>();
        FailedOrderIndicators = new List<FailedScreenIndicator>();
        DeliveryScreenIndicator.SetActive(false);
        OrderScreenIndicator.SetActive(false);
        FailedOrderScreenIndicator.SetActive(false);
        Player = FindFirstObjectByType<DrivingController>(FindObjectsInactive.Include);
        ScoreText.text = "0";
        TimeSpan timeSpan = TimeSpan.FromMinutes(GameTime);
        string timeText = string.Format("{0:0}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
        GameTimerText.text = timeText;
        gameTimer = (float)timeSpan.TotalSeconds;
    }

    public void AddIndicator(Order order)
    {
        var canvas = GetComponent<Canvas>();
        GameObject newObj = Instantiate(DeliveryScreenIndicator);
        newObj.GetComponent<RectTransform>().SetParent(canvas.transform, false);
        newObj.SetActive(true);
        var indicator = new ScreenIndicator();
        indicator.Indicator = newObj;
        indicator.Order = order;
        DeliveryIndicators.Add(indicator);
        RemoveOrderIndicator(order);
        PickupSound.Play();
    }

    public void AddOrderIndicator(Order order)
    {
        var canvas = GetComponent<Canvas>();
        GameObject newObj = Instantiate(OrderScreenIndicator);
        newObj.GetComponent<RectTransform>().SetParent(canvas.transform, false);
        newObj.SetActive(true);
        var indicator = new ScreenIndicator();
        indicator.Indicator = newObj;
        indicator.Order = order;
        OrderIndicators.Add(indicator);
        CallSound.Play();
    }

    public void AddFailedOrderIcon(Order order)
    {
        var canvas = GetComponent<Canvas>();
        GameObject newObj = Instantiate(FailedOrderScreenIndicator);
        newObj.GetComponent<RectTransform>().SetParent(canvas.transform, false);
        newObj.SetActive(true);
        var indicator = new FailedScreenIndicator();
        indicator.Indicator = newObj;
        if (!order.IsPickedUp)
        {
            foreach (var square in OrderSquares)
            {
                if(square.IsPickedUp)
                    continue;
                if (square.GetInstanceID() == order.GetInstanceID())
                {
                    indicator.OrderPosition = square.transform.parent.position;
                    break;
                }
            }
        }
        else
        {
            foreach (var square in DeliverySquares)
            {
                if (square.OrderID == order.GetInstanceID())
                {
                    indicator.OrderPosition = square.transform.position;
                    break;
                }
            }
        }

        FailedOrderIndicators.Add(indicator); 
        FailSound.Play();
    }

    public void RemoveIndicator(Order order)
    {
        foreach (var indicator in DeliveryIndicators)
        {
            if (indicator.Order == order)
            {
                if (order.GetTimer() <= 0f)
                    FailedDeliveries++;
                DeliveryIndicators.Remove(indicator);
                Destroy(indicator.Indicator);
                break;
            }
        }
    }

    public void RemoveOrderIndicator(Order order)
    {
        foreach (var indicator in OrderIndicators)
        {
            if (indicator.Order == order)
            {
                if (order.GetTimer() <= 0f)
                    FailedDeliveries++;
                OrderIndicators.Remove(indicator);
                Destroy(indicator.Indicator);
                break;
            }
        } 
    }
    
    void Update()
    {
        DeliverySquares = FindObjectsByType<DeliverySquare>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        OrderSquares = FindObjectsByType<Order>(FindObjectsInactive.Exclude, FindObjectsSortMode.None); 
        gameTimer -= Time.deltaTime;
        TimeSpan timeSpan = TimeSpan.FromSeconds(gameTimer);
        string timeText = string.Format("{0:D1}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
        GameTimerText.text = timeText;

        UpdateIndicators(OrderIndicators, true);
        UpdateIndicators(DeliveryIndicators, false);
        UpdateFailedIndicators(FailedOrderIndicators);

        foreach (var fail in FailedOrderIndicators)
        {
            fail.FailTimer -= Time.deltaTime;
            if (fail.FailTimer <= 0f)
            {
                FailedOrderIndicators.Remove(fail);
                Destroy(fail.Indicator);
                break;
            }
        }

        if (gameTimer <= 0f)
        {
            WinScreen.Show();
        }
    }

    void UpdateIndicators(in List<ScreenIndicator> indicators, bool isOrder)
    {
        foreach (var delivery in indicators)
        {
            var deliveryPos = Vector3.zero;
            if (isOrder)
            {
                foreach (var order in OrderSquares)
                {
                    if(order.IsPickedUp)
                        continue;
                    if (order.GetInstanceID() == delivery.Order.GetInstanceID())
                    {
                        deliveryPos = order.transform.parent.position;
                        break;
                    }
                }
            }
            else
            {
                foreach (var square in DeliverySquares)
                {
                    if (square.OrderID == delivery.Order.GetInstanceID())
                    {
                        deliveryPos = square.transform.position;
                        break;
                    }
                }
            }

            var indicatorTimer = delivery.Indicator.GetComponentInChildren<Slider>();
            indicatorTimer.value = delivery.Order.GetTimer();
            
            var deliveryDirection = deliveryPos - Player.gameObject.transform.position;
            var depth = Vector3.Dot(deliveryPos - MainCamera.transform.position, MainCamera.transform.forward);
            var deliveryScreenPos = MainCamera.WorldToScreenPoint(deliveryPos);
            if (depth < 0)
            {
                deliveryScreenPos.x = Screen.width - deliveryScreenPos.x;
                deliveryScreenPos.y = Screen.height - deliveryScreenPos.y;
            }
            
            var canvas = GetComponent<Canvas>();
            var indicatorImage = delivery.Indicator.GetComponent<ImageRef>().ImageReference;
            var indicatorRect = delivery.Indicator.GetComponent<RectTransform>();
            var indicatorHalfWidth = (indicatorRect.rect.width * canvas.scaleFactor) / 2f;
            var indicatorHalfHeight = (indicatorRect.rect.height * canvas.scaleFactor) / 2f;
            if (deliveryScreenPos.x > 0f + indicatorHalfWidth && deliveryScreenPos.x < Screen.width - indicatorHalfWidth 
                && deliveryScreenPos.y > 0f + indicatorHalfHeight && deliveryScreenPos.y < Screen.height - indicatorHalfHeight)
            {
                delivery.Indicator.transform.position = deliveryScreenPos;
                indicatorImage.enabled = false;
                continue;
            }

            if (!indicatorImage.enabled)
                indicatorImage.enabled = true;
            
            deliveryScreenPos.x = math.clamp(deliveryScreenPos.x, 0f + indicatorHalfWidth, Screen.width - indicatorHalfWidth);
            deliveryScreenPos.y = math.clamp(deliveryScreenPos.y, 0f + indicatorHalfHeight, Screen.height - indicatorHalfHeight);
            delivery.Indicator.transform.position = deliveryScreenPos;
            var angle = Vector3.SignedAngle(Vector3.forward, deliveryDirection.normalized, Vector3.up);
            indicatorImage.rectTransform.rotation = Quaternion.Euler(new Vector3(0, 0, -angle));
        }
    }
    
        void UpdateFailedIndicators(in List<FailedScreenIndicator> indicators)
    {
        foreach (var fail in indicators)
        {
            var deliveryPos = fail.OrderPosition;
            
            var deliveryDirection = deliveryPos - Player.gameObject.transform.position;
            var depth = Vector3.Dot(deliveryPos - MainCamera.transform.position, MainCamera.transform.forward);
            var deliveryScreenPos = MainCamera.WorldToScreenPoint(deliveryPos);
            if (depth < 0)
            {
                deliveryScreenPos.x = Screen.width - deliveryScreenPos.x;
                deliveryScreenPos.y = Screen.height - deliveryScreenPos.y;
            }
            
            var canvas = GetComponent<Canvas>();
            var indicatorImage = fail.Indicator.GetComponent<ImageRef>().ImageReference;
            var indicatorRect = fail.Indicator.GetComponent<RectTransform>();
            var indicatorHalfWidth = (indicatorRect.rect.width * canvas.scaleFactor) / 2f;
            var indicatorHalfHeight = (indicatorRect.rect.height * canvas.scaleFactor) / 2f;
            if (deliveryScreenPos.x > 0f + indicatorHalfWidth && deliveryScreenPos.x < Screen.width - indicatorHalfWidth 
                && deliveryScreenPos.y > 0f + indicatorHalfHeight && deliveryScreenPos.y < Screen.height - indicatorHalfHeight)
            {
                fail.Indicator.transform.position = deliveryScreenPos;
                indicatorImage.enabled = false;
                continue;
            }

            if (!indicatorImage.enabled)
                indicatorImage.enabled = true;
            
            deliveryScreenPos.x = math.clamp(deliveryScreenPos.x, 0f + indicatorHalfWidth, Screen.width - indicatorHalfWidth);
            deliveryScreenPos.y = math.clamp(deliveryScreenPos.y, 0f + indicatorHalfHeight, Screen.height - indicatorHalfHeight);
            fail.Indicator.transform.position = deliveryScreenPos;
            var angle = Vector3.SignedAngle(Vector3.forward, deliveryDirection.normalized, Vector3.up);
            indicatorImage.rectTransform.rotation = Quaternion.Euler(new Vector3(0, 0, -angle));
        }
    }

    public void GainScore(int score)
    {
        Score += score;
        ScoreText.text = $"{Score}";
        CompletedDeliveries++;
        ScoreSound.Play();
    }
}

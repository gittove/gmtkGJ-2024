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
    private DrivingController Player;
    private DeliverySquare[] DeliverySquares;
    public Camera MainCamera;
    
    public TMP_Text ScoreText;
    public TMP_Text GameTimerText;
    public int GameTime = 5;
    private float gameTimer;

    public GameObject DeliveryScreenIndicator;
    private List<ScreenIndicator> DeliveryIndicators;

    void Start()
    {
        MainCamera = Camera.main;

        DeliveryIndicators = new List<ScreenIndicator>();
        DeliveryScreenIndicator.SetActive(false);
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
        //Image newImage = newObj.AddComponent<Image>();
        //newImage.material = DeliveryScreenIndicator.GetComponent<Image>().material;
        newObj.GetComponent<RectTransform>().SetParent(canvas.transform, false);
        //newObj.AddComponent<CanvasRenderer>();
        newObj.SetActive(true);
        var indicator = new ScreenIndicator();
        indicator.Indicator = newObj;
        indicator.Order = order;
        DeliveryIndicators.Add(indicator);
    }

    public void RemoveIndicator(Order order)
    {
        foreach (var indicator in DeliveryIndicators)
        {
            if (indicator.Order == order)
            {
                DeliveryIndicators.Remove(indicator);
                break;
            }
        }
    }
    
    void Update()
    {
        var screenRect = GetComponent<RectTransform>();
        DeliverySquares = FindObjectsByType<DeliverySquare>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        gameTimer -= Time.deltaTime;
        TimeSpan timeSpan = TimeSpan.FromSeconds(gameTimer);
        string timeText = string.Format("{0:D1}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
        GameTimerText.text = timeText;

        foreach (var delivery in DeliveryIndicators)
        {
            var deliveryPos = Vector3.zero;
            foreach (var square in DeliverySquares)
            {
                if (square.OrderID == delivery.Order.GetInstanceID())
                {
                    deliveryPos = square.transform.position;
                    break;
                }
            }
            var deliveryDirection = deliveryPos - Player.gameObject.transform.position;
            var depth = Vector3.Dot(deliveryPos - MainCamera.transform.position, MainCamera.transform.forward);
            var deliveryScreenPos = MainCamera.WorldToScreenPoint(deliveryPos);
            if (depth < 0)
            {
                deliveryScreenPos.x = screenRect.rect.width - deliveryScreenPos.x;
                deliveryScreenPos.y = screenRect.rect.height - deliveryScreenPos.y;
            }
            /*if (deliveryScreenPos.x > 0f && deliveryScreenPos.x < screenRect.rect.width && deliveryScreenPos.y > 0f &&
                deliveryScreenPos.y < screenRect.rect.height)
            {
                DeliveryScreenIndicator.rectTransform.position = deliveryScreenPos;
                continue;
            }*/
            var indicatorImage = delivery.Indicator.GetComponent<Image>();
            var imageRect = indicatorImage.rectTransform.rect;
            deliveryScreenPos.x = math.clamp(deliveryScreenPos.x, 0f + imageRect.width/2, screenRect.rect.width - imageRect.width/2);
            deliveryScreenPos.y = math.clamp(deliveryScreenPos.y, 0f + imageRect.height/2, screenRect.rect.height - imageRect.height/2);
            delivery.Indicator.transform.position = deliveryScreenPos;
            var angle = Vector3.SignedAngle(Vector3.forward, deliveryDirection.normalized, Vector3.up);
            delivery.Indicator.transform.rotation = Quaternion.Euler(new Vector3(0, 0, -angle));

            var indicatorTimer = delivery.Indicator.GetComponentInChildren<Slider>();
            indicatorTimer.value = delivery.Order._timer / 30f;
        }
    }
}

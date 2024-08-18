using System;
using TMPro;
using Unity.Mathematics;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    private DrivingController Player;
    private DeliverySquare[] Deliveries;
    public Camera MainCamera;
    
    public TMP_Text ScoreText;
    public TMP_Text GameTimerText;
    public int GameTime = 5;
    private float gameTimer;
    
    public Image DeliveryScreenIndicator;

    void Start()
    {
        var canvas = GetComponent<Canvas>();
        MainCamera = Camera.main;
        Player = FindFirstObjectByType<DrivingController>(FindObjectsInactive.Include);
        DeliveryScreenIndicator.enabled = false;
        ScoreText.text = "0";
        TimeSpan timeSpan = TimeSpan.FromMinutes(GameTime);
        string timeText = string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
        GameTimerText.text = timeText;
        gameTimer = (float)timeSpan.TotalSeconds;
    }

    void Update()
    {
        Deliveries = FindObjectsByType<DeliverySquare>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        gameTimer -= Time.deltaTime;
        TimeSpan timeSpan = TimeSpan.FromSeconds(gameTimer);
        string timeText = string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
        GameTimerText.text = timeText;

        foreach (var delivery in Deliveries)
        {
            DeliveryScreenIndicator.enabled = true;
            var deliveryScreenPos = MainCamera.WorldToScreenPoint(delivery.gameObject.transform.position);
            deliveryScreenPos.x = math.clamp(deliveryScreenPos.x, MainCamera.rect.xMin - DeliveryScreenIndicator.rectTransform.rect.width / 2, MainCamera.rect.xMax - DeliveryScreenIndicator.rectTransform.rect.width / 2);
            deliveryScreenPos.y = math.clamp(deliveryScreenPos.y, MainCamera.rect.yMin + DeliveryScreenIndicator.rectTransform.rect.height / 2, MainCamera.rect.yMax - DeliveryScreenIndicator.rectTransform.rect.height / 2);
            DeliveryScreenIndicator.rectTransform.position = deliveryScreenPos;
        }
    }
}

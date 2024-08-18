using System;
using TMPro;
using Unity.Mathematics;
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
        MainCamera = Camera.main;

        DeliveryScreenIndicator.enabled = false;
        Player = FindFirstObjectByType<DrivingController>(FindObjectsInactive.Include);
        DeliveryScreenIndicator.enabled = false;
        ScoreText.text = "0";
        TimeSpan timeSpan = TimeSpan.FromMinutes(GameTime);
        string timeText = string.Format("{0:0}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
        GameTimerText.text = timeText;
        gameTimer = (float)timeSpan.TotalSeconds;
    }

    void Update()
    {
        var screenRect = GetComponent<RectTransform>();
        
        Deliveries = FindObjectsByType<DeliverySquare>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        gameTimer -= Time.deltaTime;
        TimeSpan timeSpan = TimeSpan.FromSeconds(gameTimer);
        string timeText = string.Format("{0:D1}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
        GameTimerText.text = timeText;

        foreach (var delivery in Deliveries)
        {
            DeliveryScreenIndicator.enabled = true;
            var deliveryScreenPos = MainCamera.WorldToScreenPoint(delivery.gameObject.transform.position);
            /*if (deliveryScreenPos.x > 0f && deliveryScreenPos.x < screenRect.rect.width && deliveryScreenPos.y > 0f &&
                deliveryScreenPos.y < screenRect.rect.height)
            {
                DeliveryScreenIndicator.rectTransform.position = deliveryScreenPos;
                continue;
            }*/
            var deliveryDirection = delivery.gameObject.transform.position - Player.gameObject.transform.position;
            var angle = Vector3.SignedAngle(Vector3.forward, deliveryDirection.normalized, Vector3.up);
            deliveryScreenPos.x = math.clamp(deliveryScreenPos.x, 0f + DeliveryScreenIndicator.rectTransform.rect.width/2, screenRect.rect.width - DeliveryScreenIndicator.rectTransform.rect.width/2);
            deliveryScreenPos.y = math.clamp(deliveryScreenPos.y, 0f + DeliveryScreenIndicator.rectTransform.rect.height/2, screenRect.rect.height - DeliveryScreenIndicator.rectTransform.rect.height/2);
            DeliveryScreenIndicator.rectTransform.position = deliveryScreenPos;
            DeliveryScreenIndicator.rectTransform.rotation = Quaternion.Euler(new Vector3(0, 0, -angle));
        }
    }
}

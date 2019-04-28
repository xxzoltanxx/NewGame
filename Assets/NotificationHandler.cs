using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotificationHandler : MonoBehaviour
{
    public Sprite battleIcon;
    public float notificationSpace = 100.0f;
    public float startNotificationY = 100.0f;
    public GameObject notificationPrefab;
    public List<GameObject> activeNotifications = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddBattleWonNotification(Sprite graphic)
    {
        System.Func<bool> func = () =>
        {
            return true;
        };
        AddNotification(battleIcon, "Battle Won!", "Click for more details.", func, graphic);
    }
    public void AddBattleLostNotification(Sprite graphic)
    {
        System.Func<bool> func = () =>
        {
            return true;
        };
        AddNotification(battleIcon, "Battle Lost!", "Click for more details.", func, graphic);
    }
    public void AddNotification(Sprite notificationIcon, string notificationName, string notificationBody, System.Func<bool> func, Sprite notificationGraphic)
    {
        var notification = GameObject.Instantiate(notificationPrefab,transform);
        notification.transform.GetChild(0).gameObject.GetComponent<Notification>().InitNotification(notificationIcon, notificationName, notificationBody, notificationGraphic , func, this);
        Vector3 notificationPos = notification.GetComponent<RectTransform>().position;
        notificationPos.y = startNotificationY + notificationSpace * activeNotifications.Count;
        notification.GetComponent<RectTransform>().position = notificationPos;
        notification.transform.GetChild(0).gameObject.GetComponent<Notification>().index = activeNotifications.Count;
        activeNotifications.Add(notification);
    }

    //even though its not a stack
    public void PopFromStack(int index)
    {
        for (int i = index + 1; i < activeNotifications.Count; ++i)
        {
            activeNotifications[i].transform.GetChild(0).gameObject.GetComponent<Notification>().TriggerDown();
            --activeNotifications[i].transform.GetChild(0).gameObject.GetComponent<Notification>().index;
        }
        activeNotifications.RemoveAt(index);
    }
}
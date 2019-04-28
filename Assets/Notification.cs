using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Notification : MonoBehaviour
{
    Func<bool> editFunc = null;
    NotificationHandler handler = null;
    public int index = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void InitNotification(Sprite notificationIcon, string notificationName, string notificationBody, Sprite notificationGraphic, Func<bool> editFunc, NotificationHandler handler)
    {
        transform.GetChild(0).GetComponent<Image>().sprite = notificationIcon;
        transform.GetChild(1).GetComponent<Text>().text = notificationName;
        transform.GetChild(2).GetComponent<Text>().text = notificationBody;
        transform.GetChild(3).GetComponent<Image>().sprite = notificationGraphic;
        this.handler = handler;
        this.editFunc = editFunc;
    }

    public void TriggerDown()
    {
        GetComponent<Animator>().SetTrigger("slideDown");
    }

    public void ButtonUsed()
    {
        editFunc();
        handler.PopFromStack(index);
        Destroy(transform.parent.gameObject);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

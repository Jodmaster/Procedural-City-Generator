using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class stopCamOnDrag : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        EventTrigger dragTrigger = GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.callback.AddListener((data) => { OnDragDelegate((PointerEventData)data); });

    }

    private void OnDragDelegate(PointerEventData data) {
        Debug.Log("dragging"); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

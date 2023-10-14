using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class HoldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private float requiredHoldTime;
    [SerializeField] private Slider indicator;

    private float timer;
    private bool isPointerDown;
    public UnityEvent onLongClick;

    public void OnPointerDown(PointerEventData eventData)
    {
        isPointerDown = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Reset();
        isPointerDown = false;
    }

    private void Reset()
    {
        isPointerDown = false;
        timer = 0;
        indicator.value = 0;
    }

    void Update()
    {
        if (isPointerDown)
        {
            timer += Time.deltaTime;
            if(timer >= requiredHoldTime)
            {
                Reset();

                Debug.Log($"Hold Button: onLongClick unity event invoked");
                if(onLongClick != null)
                {
                    onLongClick.Invoke();
                }
            }   
            indicator.value = timer / requiredHoldTime;
        }
    }
}

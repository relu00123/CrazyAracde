using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_EventHandler : MonoBehaviour, IPointerClickHandler, IDragHandler
{
    public Action<PointerEventData> OnClickHandler = null;
    public Action<PointerEventData> OnDragHandler = null;
	public Action<PointerEventData> OnHoverHandler = null;
    public Action<PointerEventData> OnDoubleClickHandler = null;

    private float lastClickTime = 0f;
    private const float doubleClickTime = 0.3f; // 더블클릭으로 간주할 최대 시간 간격


	public void OnPointerClick(PointerEventData eventData)
	{
        // 현재 클릭 시간 기록
        float currentTime = Time.time;

        // 두 클릭 간의 시간 간격이 doubleClickTime 이내이면 더블 클릭으로 간주
        if (currentTime - lastClickTime  <= doubleClickTime)
        {
            if (OnDoubleClickHandler != null)
                OnDoubleClickHandler.Invoke(eventData);
        }

        else
        {
            if (OnClickHandler != null)
                OnClickHandler.Invoke(eventData);
        }

        // 마지막 클릭 시간을 현재 시간으로 갱신 
        lastClickTime = currentTime;
	}

	public void OnDrag(PointerEventData eventData)
    {
		if (OnDragHandler != null)
            OnDragHandler.Invoke(eventData);
	}

    public void OnHover(PointerEventData eventData)
    {
        if (OnHoverHandler != null)
            OnDragHandler.Invoke(eventData);
    }

  

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnPanelSubscription : MonoBehaviour, IPointerClickHandler
{
	public void OnPointerClick(PointerEventData eventData)
	{
		FindObjectOfType<IAPUserInterface>().OnPanelSubscriptionClicked();
	}
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class IAPUserInterface : MonoBehaviour
{
	[SerializeField] private CanvasGroup panelSubscription;
	[SerializeField] private CanvasGroup panelSubscriptionBad;
	[SerializeField] private CanvasGroup panelSubscriptionGood;
	[SerializeField] private CanvasGroup panelLoading;
	[SerializeField] private TMP_InputField inputPromoCode;

	private void Awake()
	{
		IAPManager.OnPurchase += IAPManager_OnPurchase;
	}

	private void OnDestroy()
	{
		IAPManager.OnPurchase -= IAPManager_OnPurchase;
	}

	private void IAPManager_OnPurchase(bool success)
	{
		if (success)
		{
			Utility.Instance.SetElementVisibility(panelSubscriptionGood, true);
			Utility.Instance.SetElementVisibility(panelSubscription, false);
		}
		else
		{
			Utility.Instance.SetElementVisibility(panelSubscriptionBad, true);
		}
		Utility.Instance.SetElementVisibility(panelLoading, false);
	}

	private void Start()
	{
		IAPManager.Instance.CheckSubscription(CheckSubscriptionCallback);
		Utility.Instance.SetElementVisibility(panelLoading, true);
	}

	private void CheckSubscriptionCallback(string response)
	{
		Utility.Instance.SetElementVisibility(panelLoading, false);
		if (response == "true" || !AppManager.Instance.IsNeedSubscription)
		{
			return;
		}
		Utility.Instance.SetElementVisibility(panelSubscription, true);
	}

	public void PurchaseSubscription()
	{
		Utility.Instance.SetElementVisibility(panelLoading, true);
		IAPManager.Instance.BuySubscription();
	}

	public void OnPanelSubscriptionClicked()
	{
		Utility.Instance.SetElementVisibility(panelSubscriptionGood, false);
		Utility.Instance.SetElementVisibility(panelSubscriptionBad, false);
	}

	public void OnButtonLogOut()
	{
		AppManager.Instance.LogOut();
	}

	public void SubmitPromoCode()
	{
		Utility.Instance.SetElementVisibility(panelLoading, true);

		//#SERVER
		//GET DATABASE USER CREATION DATE
		WWWForm form = new WWWForm();
		form.AddField("id_user", NetworkManager.IdUser);
		form.AddField("code", inputPromoCode.text.ToUpper());
		NetworkManager.SendDataServer(form, NetworkUrl.ServerUrlSelectPromoCode, SubmitPromoCodeCallback);
	}

	private void SubmitPromoCodeCallback(string response)
	{
		if (response == "ok")
		{
			IAPManager.Instance.UpdateSubcription(1);
			inputPromoCode.text = string.Empty;
		}
		else
		{
			Utility.Instance.SetElementVisibility(panelLoading, false);
			Utility.Instance.SetElementVisibility(panelSubscriptionBad, true);
		}
	}
}
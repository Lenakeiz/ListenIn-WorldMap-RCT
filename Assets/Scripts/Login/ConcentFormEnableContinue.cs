using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConcentFormEnableContinue : MonoBehaviour
{
	[SerializeField] private Toggle toggle1;
	[SerializeField] private Toggle toggle2;
	[SerializeField] private Toggle toggle3;
	[SerializeField] private GameObject continueButton;

	public void OnValueChange()
	{
		if(toggle1.isOn && toggle2.isOn && toggle3.isOn)
		{
			continueButton.SetActive(true);
		}
		else
		{
			continueButton.SetActive(false);
		}
	}
}

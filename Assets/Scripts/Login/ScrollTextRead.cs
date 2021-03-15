using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollTextRead : MonoBehaviour
{
	[SerializeField] private GameObject nextButton;
	private ScrollRect scrollText;

	private void Awake()
	{
		scrollText = GetComponent<ScrollRect>();
		nextButton.SetActive(false);
	}

	public void OnScrollTextValueChange(Vector2 vector)
	{
		if(vector.y <= 0.1f)
		{
			nextButton.SetActive(true);
		}
		else
		{
			nextButton.SetActive(false);
		}
	}
}

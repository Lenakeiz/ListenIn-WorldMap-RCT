using UnityEngine;
using System.Collections;

public class HitButton : MonoBehaviour {

	public static bool buttonPressed = false;

	public void Press(){
		buttonPressed = true;
	}

	public void Release(){
		buttonPressed = false;
	}
}

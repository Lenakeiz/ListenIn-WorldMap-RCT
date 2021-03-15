using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CheatLoader : MonoBehaviour {

	public GameObject CoinText;
	private int clickedCount = 0;

	void Start()
	{

	}

    /// <summary>
    /// AndreaLIRO: this lead to nowhere now. Check if can be deleted
    /// </summary>
	//public void ControlAccessToDebugMenu()
	//{
	//	clickedCount++;

	//	if(clickedCount > 7 && gameObject.GetComponent<ShowPanels>().inChallengeMenu)
	//	{
	//		//Show Debug Menu
	//		gameObject.GetComponent<ShowPanels>().ShowDebugMenu();
	//		GameObject.Find("Challenge(Clone)").GetComponentInChildren<GameControlScript>().SetLevelMenu(true);
	//		clickedCount = 0;
	//	}
	//}

    //No references anymore.
	public void Cheat()
	{
		StateChallenge.Instance.cheatActivated = true;
		GameObject.Find("Main Camera").GetComponent<SoundManager>().Stop(ChannelType.BackgroundNoise);
		GameController.Instance.ChangeState(GameController.States.StatePinball);
		StatePinball.Instance.InitLevelPinball(true);
	}

	public void AddCoinsToCurrentChallenge()
	{
		StateChallenge.Instance.AddCoin(10);
		ShowCurrentCoins();
	}

	public void ResetCounter()
	{
		clickedCount = 0;
	}

	public void ShowCurrentCoins()
	{
		CoinText.GetComponent<Text>().text = StateChallenge.Instance.GetCoinsEarned().ToString();
	}
}

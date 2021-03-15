using UnityEngine;
using System.Collections;

public class LevelTutorialManager : ILevel {

	public GameObject pointintHand;
	TransformLerper lerp = null;
	public CoinSpawnerB2_Final cannonFireScript;
	public CircleCollider2D fireTrigger;

	private enum TutorialStructure{IDLE, MOVE_TO_FIRE_BUTTON, WAIT_FOR_PRESSING}
	private TutorialStructure m_structure = TutorialStructure.IDLE;
	private TutorialStructure m_next_state = TutorialStructure.IDLE;

	private bool startTutorial = false;
	private bool restingCoroutine = false;

	public override void InitLevel()
	{
		base.InitLevel();

		pointintHand = gameObject.transform.Find("PointingHand").gameObject;
		if(pointintHand == null) Debug.LogError("Pointing Hand not found");

		lerp = pointintHand.GetComponent<TransformLerper>();
		lerp.ObjectToLerp = pointintHand;

		cannonFireScript = StatePinball.Instance.m_PinballMono.Cannon.GetComponent<CoinSpawnerB2_Final>();
		fireTrigger = StatePinball.Instance.m_PinballMono.FireButton.GetComponent<CircleCollider2D>();
		fireTrigger.enabled = false;
		startTutorial = true;
	}

	public override void EndLevel()
	{
		base.EndLevel();

		pointintHand.SetActive(false);
	}

	protected override void Start()
	{

	}

	private void MoveToPosition()
	{
		pointintHand.SetActive(true);
		lerp.ResetAll();

		Vector3 currDirection = (fireTrigger.transform.position - pointintHand.transform.position).normalized;

		lerp.AddPosition(pointintHand.transform.position);
		lerp.AddPosition(cannonFireScript.transform.position);
		lerp.AddPosition(fireTrigger.transform.position - currDirection * 2.0f);

		lerp.AddLookingTarget(fireTrigger.transform.position);

		lerp.singleStepDuration = 0.8f;

		lerp.StartAnimation();

	}

	IEnumerator StartRestingAnimation()
	{
		restingCoroutine = true;
		lerp.ResetAll();
				
		Vector3 currDirection = (fireTrigger.transform.position - pointintHand.transform.position).normalized;
		lerp.AddPosition(pointintHand.transform.position);
		lerp.AddPosition(pointintHand.transform.position + currDirection * 0.1f);
		lerp.AddPosition(pointintHand.transform.position);
		lerp.AddPosition(pointintHand.transform.position - currDirection * 0.1f);
		lerp.AddPosition(pointintHand.transform.position);
		lerp.AddPosition(pointintHand.transform.position + currDirection * 0.1f);
		lerp.AddPosition(pointintHand.transform.position);
		lerp.AddPosition(pointintHand.transform.position - currDirection * 0.1f);
		lerp.AddPosition(pointintHand.transform.position);
		lerp.AddPosition(pointintHand.transform.position + currDirection * 0.1f);
		lerp.AddPosition(pointintHand.transform.position);
		lerp.AddPosition(pointintHand.transform.position - currDirection * 0.1f);
		lerp.AddPosition(pointintHand.transform.position);
		lerp.AddPosition(pointintHand.transform.position + currDirection * 0.1f);
		lerp.AddPosition(pointintHand.transform.position);
		lerp.AddPosition(pointintHand.transform.position - currDirection * 0.1f);
		lerp.AddPosition(pointintHand.transform.position);
		lerp.singleStepDuration = 0.2f;
		lerp.slerpingIsActive = false;
		
		yield return new WaitForSeconds(1.5f);
		
		lerp.StartAnimation();
		restingCoroutine = false;
	}

	// Update is called once per frame
	public override void UpdateLevel() {

		switch (m_structure) {
		case TutorialStructure.IDLE:
			if(startTutorial)
			{
				if(Mathf.Approximately(cannonFireScript.gameObject.transform.position.x, 0.0f))
				{	
					MoveToPosition();
					m_structure =  TutorialStructure.MOVE_TO_FIRE_BUTTON;
					fireTrigger.enabled = true;
					cannonFireScript.stopDropper = true;
				}
			}
			break;
		case TutorialStructure.MOVE_TO_FIRE_BUTTON:
			if(lerp.animationEnded)
			{
				if(!restingCoroutine)
				{
					StartCoroutine(StartRestingAnimation());
				}
			}
			break;
		case TutorialStructure.WAIT_FOR_PRESSING:
			break;
		default:
			break;
		}			
	}
}

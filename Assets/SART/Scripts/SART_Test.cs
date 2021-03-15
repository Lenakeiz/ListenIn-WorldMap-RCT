using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using MadLevelManager;

public class SART_Test : MonoBehaviour {

	public GameObject door;
	public GameObject go;
	public GameObject noGo;
	public Text counter;
    public CSV_Maker csvMaker;


    public float trialWaitTime;
    public float doorCloseTime = 0.5f;
    private float remainingWaitTime;

	private BlockGenerator blockGenerator;

	private string blockType = "Test";
	private int currentBlock;
    private int previousBlock;
	private int currentTrial;
    private int previousTrial;

    private bool isButtonEnabled = true;
    private bool isButtonPressed = false;
    private int countHits = 0;

	//For writing CSV
	private float testStartTime;
	private float presentationTime;
    private float unityPresentationTime;
    private bool isGo;
	private int blockToWrite;
	private int trialNumberToWrite;
	private float firstHitTime;
	private float secondHitTime;
	private bool isBeingHit;

    private bool endOfSart = false;

	void Awake(){
		blockGenerator = GameObject.FindObjectOfType<BlockGenerator> ();
		csvMaker = GameObject.FindObjectOfType<CSV_Maker> ();

		previousBlock = currentBlock = 0;
		previousTrial = currentTrial = 0;
	}

	void Start(){

        remainingWaitTime = 0.0f;
        Reset ();
        StartCoroutine(StartGame());
	}

	void Update(){
        
	}

	IEnumerator StartGame(){ //Shows timer and then starts test
		for (int i = 3; i > 0; i--){
			counter.text = i.ToString ();
			yield return new WaitForSeconds (1);
		}

		counter.gameObject.SetActive (false);

        testStartTime = Time.time;

        StartCoroutine(TestLoop());
	}

    private IEnumerator TestLoop()
    {
        while (!endOfSart)
        {
            isGo = SelectTrial();

            if (isGo)
                ShowGo();
            else
                ShowNoGo();

            yield return new WaitForEndOfFrame();
            presentationTime = Time.time - testStartTime;
            unityPresentationTime = Time.time;

            firstHitTime = 0;
            secondHitTime = 0;
            isBeingHit = false;
            isButtonPressed = false;
            bool isDoorClosed = false;

            //Entire duration of the trial
            while ((trialWaitTime + doorCloseTime) - (Time.time - unityPresentationTime) > 0.0f)
            {
                if (isButtonPressed)
                {
                    isButtonPressed = false;
                    if (countHits < 2)
                    {
                        if (countHits == 0)
                        {
                            if (isGo)
                                isBeingHit = true;
                            firstHitTime = Time.time - testStartTime;
                        }
                        else if (countHits == 1)
                        {
                            secondHitTime = Time.time - testStartTime;
                        }
                        
                        countHits++;
                    }
                }

                if (!isDoorClosed && ((Time.time - unityPresentationTime) > trialWaitTime))
                {
                    isDoorClosed = true;
                    Reset();
                }
                
                yield return new WaitForEndOfFrame();

            }

            if (countHits == 0 && !isGo)
                isBeingHit = true;

            countHits = 0;

            //Andrea: +1 just for starting from 1 instead of 0
            csvMaker.Write(blockType, blockToWrite + 1, trialNumberToWrite + 1, isGo, presentationTime, firstHitTime, secondHitTime, isBeingHit);

            //Reset();
            yield return new WaitForEndOfFrame();
            isButtonPressed = false;
        }

        StartCoroutine(FinishSART());

    }

    IEnumerator FinishSART()
    {
        Reset();
        yield return new WaitForEndOfFrame();
        csvMaker.CreateCSVFile(csvMaker.path, csvMaker.dataCollected);
        yield return StartCoroutine(TherapyLIROManager.Instance.SaveSARTFinished());
        MadLevel.LoadLevelByName("MainHUB");
    }

	bool SelectTrial(){//Selects the next trial based on what was determiend by the blockgenerator.
		List<bool> block = blockGenerator.allBlocks [currentBlock];
		bool trial = block [currentTrial];

		blockToWrite = currentBlock;
		trialNumberToWrite = currentTrial;

        previousTrial = currentTrial;
		currentTrial++;

		if(currentTrial == block.Count && currentBlock == blockGenerator.allBlocks.Count - 1){
            endOfSart = true;
		}

		if(currentTrial == block.Count){
			currentTrial = 0;
            previousBlock = currentBlock;
			currentBlock++;
		}
	
		return trial;
			
	}

    private IEnumerator ButtonPressed()
    {
        isButtonEnabled = false;
        isButtonPressed = true;
        yield return new WaitForSeconds(0.25f);
        isButtonEnabled = true;
    }

    public void OnButtonClicked()
    {
        if (isButtonEnabled)
            StartCoroutine(ButtonPressed());
    }

	//These functions just choose which character to show or the door.
	void ShowGo(){
		door.SetActive (false);
		noGo.SetActive (false);
		go.SetActive (true);
	}

	void ShowNoGo(){
		door.SetActive (false);
		go.SetActive (false);
		noGo.SetActive (true);
	}

	void Reset(){
        isButtonPressed = false;
        go.SetActive (false);
		noGo.SetActive (false);
		door.SetActive (true);
	}
}

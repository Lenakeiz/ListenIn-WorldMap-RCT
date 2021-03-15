using UnityEngine;
using System.Collections;

using MadLevelManager;

public class MadPathChecker : MonoBehaviour {

    public int lastLevelUnlocked;
    private bool initialized = false;

    SpriteRenderer unlockedSpriteRender;

    void Awake()
    {

    }

    void InitializePath()
    {
        var currentLayout = MadLevelLayout.current;
        var configuration = MadLevel.activeConfiguration;
        var group = configuration.FindGroupById(currentLayout.configurationGroup);

        var lastUnlockedLevelName = MadLevel.FindLastUnlockedLevelName(group.name);
        int lastLevelUnlocked = MadLevel.GetOrdeal(lastUnlockedLevelName, MadLevel.Type.Level);
        
        lastLevelUnlocked = MadLevel.GetOrdeal(lastUnlockedLevelName, MadLevel.Type.Level);
        string[] levelNames = MadLevel.GetAllLevelNames(MadLevel.Type.Level);

        GameObject unlocked = gameObject.transform.Find("Unlocked").gameObject;
        if (unlocked == null)
        {
            Debug.LogError("Unlocked paths not found");
            return;
        }

        bool allBronzeUnlocked = true;

        unlockedSpriteRender = unlocked.transform.Find("PathArrow").gameObject.GetComponent<SpriteRenderer>();
        Sprite arrowSprite = null;

        if (levelNames.Length == levelNames.Length)
        {
            for (int i = 0; i < levelNames.Length; i++)
            {
                if (!MadLevelProfile.GetLevelBoolean(levelNames[i], "jigsaw_1"))
                {
                    allBronzeUnlocked = false;
                    break;
                }
            }

            if (allBronzeUnlocked)
            {
                arrowSprite = Resources.Load<Sprite>("Map/ArrowsMapTracks/PathArrow_20");
                unlockedSpriteRender.sprite = arrowSprite;
                return;
            }
        }

        if (lastLevelUnlocked > 1)
        {
            arrowSprite = Resources.Load<Sprite>(string.Format("Map/ArrowsMapTracks/PathArrow_{0}", lastLevelUnlocked - 1));
        }           

        unlockedSpriteRender.sprite = arrowSprite;

        GameObject blinkingPath = gameObject.transform.Find("Activated").gameObject;

        if (blinkingPath == null)
        {
            Debug.LogError("Unlocked paths not found");
            return;
        }

        GameObject nextPath = blinkingPath.transform.Find(string.Format("Path_{0}(Activated)", lastLevelUnlocked)).gameObject;

        if (nextPath == null)
        {
            Debug.LogError("Blinking path not found");
        }

        nextPath.SetActive(true);
    }

	void Start () {
        
    }
	
	void Update () {
	
	}

    void LateUpdate()
    {
        if (!initialized)
        {
            InitializePath();
            initialized = true;
        }
    }
}

/*
* Mad Level Manager by Mad Pixel Machine
* http://www.madpixelmachine.com
*/

using UnityEngine;
using MadLevelManager;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MadLevelYouAreHereScript : MonoBehaviour {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Fields
    // ===========================================================

    public Vector3 offset = new Vector3(0.37f, 0, 0);
    public float animationAmplitude = 0.02f;
    public float animationSpeed = 3f;
    
    private Transform lastUnlockedTransform;

    private bool initialized;

    // ===========================================================
    // Methods for/from SuperClass/Interfaces
    // ===========================================================

    // ===========================================================
    // Methods
    // ===========================================================

    void Update() {
        if (initialized) {
            float yChange = Mathf.Sin(Time.time * animationSpeed) * animationAmplitude;
            transform.position = lastUnlockedTransform.position + offset + new Vector3(0, yChange, 0);
        }
    }

    void LateUpdate() {
        if (!initialized) {
            Initialize();
        }
    }

    private void Initialize() {

        initialized = true;
        var currentLayout = MadLevelLayout.current;
        var configuration = MadLevel.activeConfiguration;
        var group = configuration.FindGroupById(currentLayout.configurationGroup);

        var lastUnlockedLevelName = MadLevel.FindLastUnlockedLevelName(group.name);
        int lastLevelUnlocked = MadLevel.GetOrdeal(lastUnlockedLevelName, MadLevel.Type.Level);

        List<string> levelNames = MadLevel.GetAllLevelNames(MadLevel.Type.Level).ToList();
        lastUnlockedLevelName = levelNames[lastLevelUnlocked - 1];
        
        if (lastLevelUnlocked == levelNames.Count && MadLevelProfile.GetLevelBoolean(lastUnlockedLevelName, "jigsaw_1"))
        {
            //All levels unlocked, let point to the level with less cup won
            int[] scorePoints = new int[levelNames.Count];

            for (int i = 0; i < levelNames.Count; i++)
            {
                int currScore = i;
                if (MadLevelProfile.GetLevelBoolean(levelNames[i], "jigsaw_1"))
                {
                    currScore += 10;
                }
                if (MadLevelProfile.GetLevelBoolean(levelNames[i], "jigsaw_2"))
                {
                    currScore += 100;
                }
                if (MadLevelProfile.GetLevelBoolean(levelNames[i], "jigsaw_3"))
                {
                    currScore += 1000;
                }
                scorePoints[i] = currScore;
            }

            int bestScore = 10000;
            int maximumTotalScore = 20000 + 2000 + 200 + ((19 * 20) / 2) - 1;
            int bestLevelIdx = 0;
            int currTotalScore = -1;
            //Calculate currScore
            for (int i = 0; i < scorePoints.Length; i++)
            {
                currTotalScore += scorePoints[i];
            }

            Debug.Log("Last PLayed level: " + MadLevel.lastPlayedLevelName);

            if (currTotalScore == maximumTotalScore)
            {
                //Choose a random level to start
                if (MadLevel.lastPlayedLevelName == "Null" || MadLevel.lastPlayedLevelName == "Setup Screen")
                {
                    bestLevelIdx = UnityEngine.Random.Range(0, levelNames.Count);
                }
                else
                {
                    bestLevelIdx = levelNames.FindIndex(x => x == MadLevel.lastPlayedLevelName);
                    bestLevelIdx++;
                    bestLevelIdx = bestLevelIdx % levelNames.Count;
                }
            }
            else
            {
                for (int i = 0; i < scorePoints.Length; i++)
                {
                    if (bestScore > scorePoints[i])
                    {
                        bestScore = scorePoints[i];
                        bestLevelIdx = i;
                    }
                }
            }            

            lastUnlockedLevelName = levelNames[bestLevelIdx];

        }
        

        var icon = currentLayout.GetIcon(lastUnlockedLevelName);
        lastUnlockedTransform = icon.transform;
        

    }

    // ===========================================================
    // Static Methods
    // ===========================================================

    // ===========================================================
    // Inner and Anonymous Classes
    // ===========================================================

}
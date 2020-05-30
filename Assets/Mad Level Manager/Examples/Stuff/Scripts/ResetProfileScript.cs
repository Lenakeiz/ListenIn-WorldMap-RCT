/*
* Mad Level Manager by Mad Pixel Machine
* http://www.madpixelmachine.com
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MadLevelManager;
using System.IO;

#if !UNITY_3_5
namespace MadLevelManager {
#endif

public class ResetProfileScript : MonoBehaviour {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Fields
    // ===========================================================
    
    public bool resetOnRKey;

    // ===========================================================
    // Methods for/from SuperClass/Interfaces
    // ===========================================================

    // ===========================================================
    // Methods
    // ===========================================================

    void Start() {
        var sprite = GetComponent<MadSprite>();
        if (sprite != null) {
            sprite.onMouseEnter += (s) => 
                sprite.AnimScaleTo(Vector3.one * 1.5f, 1, MadiTween.EaseType.easeOutElastic);
            sprite.onMouseExit += (s) => 
                sprite.AnimScaleTo(Vector3.one, 1, MadiTween.EaseType.easeOutElastic);
            sprite.onMouseDown += sprite.onTap = (s) => {
                MadLevelProfile.Reset();
                MadLevel.ReloadCurrent();
            };
        }
    }

    void Update()
    {
        if (resetOnRKey)
        {

            if (Input.GetKey(KeyCode.R))
            {
                MadLevelProfile.Reset();
                MadLevel.ReloadCurrent();
            }

            if (Input.GetKey(KeyCode.P))
            {
                string savethis =  MadLevelProfile.SaveProfileToString();
                File.WriteAllText(Application.persistentDataPath + "/MadLevelProfile.csv", savethis);
            }

            if (Input.GetKey(KeyCode.O))
            {
                string load = File.ReadAllText(Application.persistentDataPath + "/MadLevelProfile.csv");
                MadLevelProfile.LoadProfileFromString(load);
                MadLevel.ReloadCurrent();
             }

        }
    }

    // ===========================================================
    // Static Methods
    // ===========================================================

    // ===========================================================
    // Inner and Anonymous Classes
    // ===========================================================

}

#if !UNITY_3_5
} // namespace
#endif
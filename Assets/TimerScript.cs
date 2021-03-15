using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class TimerScript : MonoBehaviour {

    public Text _textTotal;
    public Text _textToday;
    
	// Use this for initialization
	void Start () {

        try
        {
            if (_textToday != null && _textToday != null)
            {
                string time = StateChallenge.Instance.GetTotalTherapyTime();
                _textTotal.text = time;
                time = StateChallenge.Instance.GetTodayTherapyTime();
                _textToday.text = time;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
        }
    }
}

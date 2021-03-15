using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DevelopmentScript : MonoBehaviour
{
    public Dropdown yearsDropdown;
    public Dropdown monthDropdown;

    public void OnValidate()
    {
        AddYears();
    }

    private void AddYears()
    {
        List<string> listOfYears = new List<string>();

        Debug.Log("cascasca");
        for (int i = 2020; i != 1910; i--)
        {
            listOfYears.Add(i.ToString());
        }

        yearsDropdown.ClearOptions();

        yearsDropdown.AddOptions(listOfYears);
    } 
}

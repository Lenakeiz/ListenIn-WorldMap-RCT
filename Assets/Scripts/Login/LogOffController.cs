using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LogOffController : MonoBehaviour
{
    public void LogOut()
    {
        PlayerPrefManager.LogOut();
    }
}

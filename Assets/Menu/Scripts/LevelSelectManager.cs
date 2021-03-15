using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelSelectManager : MonoBehaviour {

    [SerializeField] private GameObject OptionTint;
    [SerializeField] private GameObject PauseMenu;
    [SerializeField] private GameObject PauseButton;

    public void OpenPauseMenu()
    {
        Time.timeScale = 0.0f;
        UploadManager.Instance.ResetTimer(TimerType.Idle);
        UploadManager.Instance.SetIsMenu = true;
        OptionTint.GetComponent<CanvasGroup>().alpha = 1.0f;
        PauseMenu.GetComponent<CanvasGroup>().alpha = 1.0f;
        PauseButton.SetActive(false);
        OptionTint.SetActive(true);
        PauseMenu.SetActive(true);
    }

    public void HideMenu()
    {
        Time.timeScale = 1.0f;
        UploadManager.Instance.ResetTimer(TimerType.Idle);
        UploadManager.Instance.SetIsMenu = false;
        OptionTint.GetComponent<CanvasGroup>().alpha = 0.0f;
        PauseMenu.GetComponent<CanvasGroup>().alpha = 0.0f;
        PauseButton.SetActive(true);
        OptionTint.SetActive(false);
        PauseMenu.SetActive(false);
    }



}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AppManager : MonoBehaviour
{
    public static AppManager Instance;

    public CanvasGroup LoadingPanel;

    public const string UserIdKey = "user_id";

    public bool IsNeedSubscription;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    public void LogOut()
    {
        Utility.Instance.SetElementVisibility(LoadingPanel, true);
        RemoveUserTokenId();
        GoogleSignInManager.Instance.OnSignOut();
        SceneManager.LoadScene("GoogleSignIn");
    }

    public string GetUserTokenId()
    {
        if (PlayerPrefs.HasKey(UserIdKey))
        {
            return PlayerPrefs.GetString(UserIdKey);
        }
        else
        {
            return null;
        }
    }

    public void RemoveUserTokenId()
    {
        if (PlayerPrefs.HasKey(UserIdKey))
        {
            PlayerPrefs.DeleteKey(UserIdKey);
        }
    }

    public void SaveUserTokenId(string tokenId)
    {
        PlayerPrefs.SetString(UserIdKey, tokenId);
    }
}

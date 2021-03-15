using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google;
using MadLevelManager;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SignInManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup panelError;

    private void Start()
    {
#if UNITY_EDITOR
        CheckIdUser("test_user");
        return;
#endif

        GoogleSignInManager.OnUserSignedIn += GoogleSignInManager_OnUserSignedIn;

        if (AppManager.Instance.GetUserTokenId() != null)
        {
            GoogleSignInManager.Instance.OnSignInSilently();
        }
        else
        {
            Utility.Instance.SetElementVisibility(AppManager.Instance.LoadingPanel, false);
        }
    }

    private void OnDestroy()
    {
        GoogleSignInManager.OnUserSignedIn -= GoogleSignInManager_OnUserSignedIn;
    }

    private void GoogleSignInManager_OnUserSignedIn(bool success, string userId = null)
    {
        if (!success)
        {
            Utility.Instance.SetElementVisibility(AppManager.Instance.LoadingPanel, false);
            Utility.Instance.SetElementVisibility(panelError, true);
            return;
        }

        Utility.Instance.SetElementVisibility(AppManager.Instance.LoadingPanel, true);

        string currentUserId = AppManager.Instance.GetUserTokenId();

        if (currentUserId == null)
        {
            CheckIdUser(userId);
            return;
        }

        //IF EQUAL, PROCEED
        //THIS IS IN CASE GOOGLE FOR SOME REASON CHANGED THE TOKEN ID OF THE CURRENT LOGGED USER OR DIDN'T SIGNED OUT PROPERLY
        if (currentUserId == userId)
        {
            CheckIdUser(currentUserId);
        }
        else
        {
            //LOGGED IN BUT DIFFERENT TOKEN ID
            GoogleSignInManager.Instance.OnSignOut();
            AppManager.Instance.RemoveUserTokenId();
            Utility.Instance.SetElementVisibility(AppManager.Instance.LoadingPanel, false);
        }
    }

    public void OnLogInButtonPressed()
    {
        Utility.Instance.SetElementVisibility(AppManager.Instance.LoadingPanel, true);
        StartCoroutine(LogInButtonCoroutine());
    }

    private IEnumerator LogInButtonCoroutine()
    {
        yield return new WaitForSeconds(2);
        GoogleSignInManager.Instance.OnSignIn();
    }

    public void CloseErrorPanel()
    {
        Utility.Instance.SetElementVisibility(panelError, false);
    }

    private void CheckIdUser(string idUser)
    {
        NetworkManager.IdUser = idUser;
        WWWForm form = new WWWForm();
        form.AddField("id_user", idUser);
        NetworkManager.SendDataServer(form, NetworkUrl.UrlSqlLogin, CheckIdUserCallback);

        AppManager.Instance.SaveUserTokenId(idUser);
    }

    private void CheckIdUserCallback(string response)
    {
        string[] values = response.Split('+');
        if (values[0] == "new_user")
        {
            SceneManager.LoadScene("Registration");
        }
        else if (values[0] == "exist")
        {
            SceneManager.LoadScene("SetupScreen");
        }

        AppManager.Instance.IsNeedSubscription = values[1] == "1";
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using Google;
using UnityEngine;
using UnityEngine.UI;

public class GoogleSignInManager : MonoBehaviour
{
    public static GoogleSignInManager Instance;

    private string webClientId = "934722919527-dn1fk0oe3sdnr6uv2q7oh845orklbm6d.apps.googleusercontent.com";
    private GoogleSignInConfiguration configuration;

    public delegate void UserSignedIn(bool success, string tokenId = null);
    public static event UserSignedIn OnUserSignedIn;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        configuration = new GoogleSignInConfiguration
        {
            WebClientId = webClientId,
            RequestIdToken = true
        };
    }

    public void OnSignIn()
    {
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;
        Debug.Log("Calling SignIn");
        //AddStatusText("Calling SignIn");

        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(
          OnAuthenticationFinished);
    }

    public void OnSignInSilently()
    {
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;
        Debug.Log("Calling SignIn Silently");
        //AddStatusText("Calling SignIn Silently");

        GoogleSignIn.DefaultInstance.SignInSilently()
              .ContinueWith(OnAuthenticationFinished);
    }

    public void OnSignOut()
    {
        Debug.Log("Calling SignOut");
        //AddStatusText("Calling SignOut");
        GoogleSignIn.DefaultInstance.SignOut();
    }

    private void OnAuthenticationFinished(Task<GoogleSignInUser> task)
    {
        if (task.IsFaulted)
        {
            using (IEnumerator<System.Exception> enumerator =
                    task.Exception.InnerExceptions.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    GoogleSignIn.SignInException error =
                            (GoogleSignIn.SignInException)enumerator.Current;
                    //AddStatusText("Got Error: " + error.Status + " " + error.Message);
                    Debug.Log("Got Error: " + error.Status + " " + error.Message);
                    OnUserSignedIn.Invoke(false);
                }
                else
                {
                    //AddStatusText("Got Unexpected Exception?!?" + task.Exception);
                    Debug.Log("Got Unexpected Exception?!?" + task.Exception);
                    OnUserSignedIn.Invoke(false);
                }
            }
        }
        else if (task.IsCanceled)
        {
            //AddStatusText("Canceled");
            Debug.Log("Canceled");
            OnUserSignedIn.Invoke(false);
        }
        else
        {
            //AddStatusText("Welcome: " + task.Result.DisplayName + "!");
            //AddStatusText("ID: " + task.Result.UserId);
            Debug.Log("Welcome: " + task.Result.DisplayName + "!");
            OnUserSignedIn.Invoke(true, task.Result.UserId);
        }
    }

    public Text statusText;
    private List<string> messages = new List<string>();
    public void AddStatusText(string text)
    {
        if (messages.Count == 10)
        {
            messages.RemoveAt(0);
        }
        messages.Add(text);
        string txt = "";
        foreach (string s in messages)
        {
            txt += "\n" + s;
        }
        statusText.text = txt;
    }
}
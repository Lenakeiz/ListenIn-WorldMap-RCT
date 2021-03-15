using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class Connectivity : MonoBehaviour
{
    private bool isConnectivity;
    public bool IsConnectivity
    {
        get
        {
            return IsConnectivity;
        }

        set
        {
            if (isConnectivity == value)
            {
                return;
            }

            isConnectivity = value;

            string message = isConnectivity ? "<b><color=green>INTERNET ACCESS</color></b>" : "<b><color=red>NO INTERNET ACCESS</color></b>";
            Debug.Log(message);

            Time.timeScale = isConnectivity ? 1 : 0;
            SetElementVisibility(panelBlock, !isConnectivity);
        }
    }

    [SerializeField] private CanvasGroup panelBlock;
    [SerializeField] private string hostUrl;
    [SerializeField] private float checkEvery;
    private UnityWebRequest connectivityRequest;

    private int numberOfNoConnection;

    private void Awake()
    {
        if(FindObjectsOfType<Connectivity>().Length != 1)
        {
            DestroyImmediate(this);
            return;
        }

        DontDestroyOnLoad(gameObject);

        isConnectivity = true;
        IsConnectivity = true;

        StartCoroutine(CheckConnectivity());
    }

    private IEnumerator CheckConnectivity()
    {
        while(true)
        {
            yield return new WaitForSecondsRealtime(checkEvery + 0.2f);

            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                IsConnectivity = false;
            }
            else
            {
                connectivityRequest = new UnityWebRequest(hostUrl);
                connectivityRequest.timeout = (int)checkEvery;
                connectivityRequest.certificateHandler = new AcceptAllCertificatesSignedWithASpecificPublicKey();

                yield return connectivityRequest.SendWebRequest();

                if (connectivityRequest.isHttpError || connectivityRequest.isNetworkError)
                {
                    numberOfNoConnection++;
                    if(numberOfNoConnection >= 2)
                    {
                        IsConnectivity = false;
                    }
                }
                else
                {
                    numberOfNoConnection = 0;
                    IsConnectivity = true;
                }
            }            
        }
    }

    public void SetElementVisibility(CanvasGroup element, bool visibility)
    {
        element.blocksRaycasts = visibility;
        element.interactable = visibility;
        element.alpha = visibility ? 1 : 0;
    }
}
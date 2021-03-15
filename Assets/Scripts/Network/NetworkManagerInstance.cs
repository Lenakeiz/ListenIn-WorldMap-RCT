using UnityEngine;
using System.Collections;

public class NetworkManagerInstance : MonoBehaviour
{
    [SerializeField] private GameObject networkManagerInstance;

    private void Awake()
    {
        if(!FindObjectOfType<NetworkManager>())
        {
            Instantiate(networkManagerInstance);
        }
    }
}

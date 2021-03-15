using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

public class UploadAppendTest : MonoBehaviour {

    private List<string> texts = new List<string>();
	// Use this for initialization
	void Start () {
        for (int i = 0; i < 10; i++)
        {
            texts.Add("Il mattino ha l oro in bocca");
        }
	}
	
	// Update is called once per frame
	void Update () {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.LogError("Catching Some Error");
            GameObject go = null;
            string ciao = go.name;
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in texts)
            {
                sb.AppendLine(item);
            }

            byte[] dataAsBytes = Encoding.ASCII.GetBytes(sb.ToString());

            WWWForm form = new WWWForm();
            form.AddField("id_user", NetworkManager.IdUser);
            form.AddField("file_name", "Log");
            form.AddField("file_size", dataAsBytes.Length);
            form.AddField("folder_name", GlobalVars.LogFolderName);
            form.AddBinaryData("file_data", dataAsBytes, "Log");
            NetworkManager.SendDataServer(form, NetworkUrl.ServerUrlUploadLogFile);
        }
#endif
    }
}

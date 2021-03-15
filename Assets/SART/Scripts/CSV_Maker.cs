using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.Linq;

public class CSV_Maker : MonoBehaviour {

	[HideInInspector]
	public List<string[]> dataCollected = new List<string[]> ();
	[HideInInspector]
    public string path { get
        {
            string folder = GlobalVars.GetPathToLIROOutput(NetworkManager.IdUser);
            TherapyLadderStep currSARTinCycle = TherapyLIROManager.Instance.GetUserProfile.LIROStep;
            string filename = String.Format(GlobalVars.SARTStringFormat, currSARTinCycle.ToString(), TherapyLIROManager.Instance.GetCurrentTherapyCycle());
            return Path.Combine(folder, filename);
        }
    }
   
    [HideInInspector]
    public string filenameSart
    {
        get
        {
            TherapyLadderStep currSARTinCycle = TherapyLIROManager.Instance.GetUserProfile.LIROStep;
            return String.Format(GlobalVars.SARTStringFormat, currSARTinCycle.ToString(), TherapyLIROManager.Instance.GetCurrentTherapyCycle());
        }
    }
	private bool created = false;

	void Awake(){
	}

	void Start(){
		dataCollected = CreateHeader ();
	}

	public List<string[]> CreateHeader(){
		List<string[]> rowData = new List<string[]> ();
		string[] temp = new string[8];

		temp [0] = "BlockType";
		temp [1] = "BlockNumber";
		temp [2] = "TrialNumber";
		temp [3] = "isGo?";
		temp [4] = "PresentationTime";
		temp [5] = "ReactionTime";
		temp [6] = "ReactionTime2";
		temp [7] = "Hit";

		rowData.Add (temp);
		return rowData;
	}

	public void Write(string type, int block, int trial, bool isGo, float presentationTime, float reactionTime, float reactionTime_2, bool hit){
		string[] rowDataTemp = new string[8];

		rowDataTemp [0] = type;
		rowDataTemp [1] = block.ToString ();
		rowDataTemp [2] = trial.ToString();
		rowDataTemp [3] = isGo.ToString ();
		rowDataTemp [4] = presentationTime.ToString ();
		rowDataTemp [5] = reactionTime.ToString ();
		rowDataTemp [6] = reactionTime_2.ToString ();
		rowDataTemp [7] = hit.ToString ();

		dataCollected.Add (rowDataTemp);
	}

	public void CreateCSVFile(string directory, List<string[]> data){
        try
        {
            string[][] output = new string[data.Count][];

            for (int i = 0; i < output.Length; i++)
            {
                output[i] = data[i];
            }

            int lenght = output.GetLength(0);
            string delimeter = ",";

            StringBuilder sb = new StringBuilder();

            for (int index = 0; index < lenght; index++)
            {
                sb.AppendLine(string.Join(delimeter, output[index]));
            }

#if SAVE_LOCALLY
            //#ERASE
            using (StreamWriter outStream = System.IO.File.CreateText(directory))
            {
                outStream.WriteLine(sb);
                outStream.Close();
            }
            //#ERASE
#endif

            //SEND TO SERVER
            byte[] dataAsBytes = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
            WWWForm form = new WWWForm();
            form.AddField("id_user", NetworkManager.IdUser);
            form.AddField("file_name", filenameSart);
            form.AddField("file_size", dataAsBytes.Length);
            form.AddField("folder_name", GlobalVars.OutputFolderName);
            form.AddBinaryData("file_data", dataAsBytes, filenameSart);
            NetworkManager.SendDataServer(form, NetworkUrl.ServerUrlUploadFile);


        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
        }

	}


}

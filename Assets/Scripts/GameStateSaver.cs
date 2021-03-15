using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Text;
using MadLevelManager;

public class ChapterSaveState
{
    public string LevelName;
	public int LevelNumber;
	[XmlArrayAttribute("Pieces")]
	public float[] JigsawPeicesUnlocked = new float[12];
}

public class GameSaveState
{
	public List<ChapterSaveState> Chapters;

	public GameSaveState()
	{
		Chapters = new List<ChapterSaveState>();
	}
}

public class GameStateSaver : Singleton<GameStateSaver> {

	#region singleton
	//private static readonly GameStateSaver instance = new GameStateSaver();
	//public static GameStateSaver Instance
	//{
	//	get
	//	{
	//		return instance;
	//	}
	//}
	#endregion
    public void DownloadGameProgress()
    {
        WWWForm form = new WWWForm();
        form.AddField("id_user", NetworkManager.IdUser);
        form.AddField("file_name", FileName());
        form.AddField("folder_name", GlobalVars.GameProgressFolderName);
        NetworkManager.SendDataServer(form, NetworkUrl.ServerUrlGetFile, DownloadGameProgressCallback);
    }

    private void DownloadGameProgressCallback(string response)
    {
        if (string.IsNullOrEmpty(response)||String.Equals(response,"error"))
        {
            //CRITICAL ERROR
            Debug.LogError("<color=red>SERVER ERROR; could not retrieve jigsaw information from server. File missing or corrupted. Resetting automatically</color>");
            ///AndreaLIRO_TB: Resetting jigsaw states if the file is empty.
            ResetListenIn();
        }

        GlobalVars.GameProgressFile = response;
    }

    /// <summary>
    /// This function will download the world map progression from the server (separated from the jigsaw information
    /// </summary>
    public void DownloadWorldMapProgress()
    {
        WWWForm form = new WWWForm();
        form.AddField("id_user", NetworkManager.IdUser);
        form.AddField("file_name", WorldMapProgressFileName());
        form.AddField("folder_name", GlobalVars.GameProgressFolderName);
        NetworkManager.SendDataServer(form, NetworkUrl.ServerUrlGetFile, DownloadWorldMapProgressCallback);
    }

    private void DownloadWorldMapProgressCallback(string response)
    {
        if (string.IsNullOrEmpty(response) || String.Equals(response,"error"))
        {
            //CRITICAL ERROR
            Debug.LogError("<color=red>SERVER ERROR; could not retrieve jigsaw information from server. File missing or corrupted. Resetting automatically</color>");
            ResetWorldMapProgress();
        }
        else
        {
            //AndreaLiro_TB: prepared the MadLevel calls to load the profile from remote; 
            MadLevelProfile.LoadProfileFromString(response);
            GlobalVars.GameWorldMapProgressFile = response;
        }

    }

    public void LoadGameProgress()
    {
        if(string.IsNullOrEmpty(GlobalVars.GameProgressFile))
        {
            //Warning that this function should be called when having download the game progress from the server
            Debug.LogWarning("<color=yellow>SERVER Warning; jigsaw information from server not downloaded. Could be a reset.</color>");
            return;
        }

        GameSaveState gss;

        XmlSerializer serializer = new XmlSerializer(typeof(GameSaveState));
        StringReader stringReader = new StringReader(GlobalVars.GameProgressFile);
        gss = serializer.Deserialize(stringReader) as GameSaveState;

        for (int i = 0; i < gss.Chapters.Count; i++)
        {
            Debug.Log(
                "Level Name: " + gss.Chapters[i].LevelName + " " +
                    "LevelNumber: " + gss.Chapters[i].LevelNumber
                );

            Chapter currChapter;
            StateJigsawPuzzle.Instance.Chapters.TryGetValue(gss.Chapters[i].LevelName, out currChapter);

            if (currChapter != null)
            {
                for (int j = 0; j < gss.Chapters[i].JigsawPeicesUnlocked.Length; j++)
                {
                    currChapter.JigsawPeicesUnlocked[j] = gss.Chapters[i].JigsawPeicesUnlocked[j];
                }
            }
            else { Debug.LogError(String.Format("Cannot load game state as chapter with key {0} have not been found", gss.Chapters[i].LevelName)); }

        }
        Debug.Log("GameStateSaver: Load() jigsaw pieces game state");
    }

    /// <summary>
    /// This function save both the jigsaw and world map progress.
    /// </summary>
    public void SaveGame()
    {
        SaveGameProgress();
        SaveGameWorldProgress();
    }

    public void SaveGameProgress()
    {
        string gameProgress;
        //TODO now there is StateJigsawPuzzle
        XmlSerializer serializer = new XmlSerializer(typeof(GameSaveState));
        using (StringWriter stringWriter = new StringWriter())
        {
            GameSaveState gss = new GameSaveState();

            List<String> levelKeys = new List<string>(StateJigsawPuzzle.Instance.Chapters.Keys);

            for (int i = 0; i < levelKeys.Count; i++)
            {
                ChapterSaveState saveChapter = new ChapterSaveState();
                saveChapter.LevelName = levelKeys[i];

                Chapter currChapter;
                StateJigsawPuzzle.Instance.Chapters.TryGetValue(levelKeys[i], out currChapter);
                if (currChapter != null)
                {
                    saveChapter.LevelNumber = currChapter.LevelNumber;

                    for (int j = 0; j < saveChapter.JigsawPeicesUnlocked.Length; j++)
                    {
                        saveChapter.JigsawPeicesUnlocked[j] = currChapter.JigsawPeicesUnlocked[j];
                    }
                }
                else { Debug.LogError(String.Format("Cannot save game state as chapter with key {0} have not been found", levelKeys[i])); }

                gss.Chapters.Add(saveChapter);

            }
            serializer.Serialize(stringWriter, gss);
            gameProgress = stringWriter.ToString();
        }

        //SEND TO SERVER
        WWWForm form = new WWWForm();
        form.AddField("id_user", NetworkManager.IdUser);
        form.AddField("file_name", FileName());
        form.AddField("file_size", Encoding.ASCII.GetBytes(gameProgress).Length);
        form.AddField("folder_name", GlobalVars.GameProgressFolderName);
        form.AddBinaryData("file_data", Encoding.ASCII.GetBytes(gameProgress), FileName());
        NetworkManager.SendDataServer(form, NetworkUrl.ServerUrlUploadFile);
    }

    public void SaveGameWorldProgress()
    {
        string currProgress = MadLevelProfile.SaveProfileToString();

        //SEND TO SERVER
        WWWForm form = new WWWForm();
        form.AddField("id_user", NetworkManager.IdUser);
        form.AddField("file_name", WorldMapProgressFileName());
        form.AddField("file_size", Encoding.ASCII.GetBytes(currProgress).Length);
        form.AddField("folder_name", GlobalVars.GameProgressFolderName);
        form.AddBinaryData("file_data", Encoding.ASCII.GetBytes(currProgress), WorldMapProgressFileName());
        NetworkManager.SendDataServer(form, NetworkUrl.ServerUrlUploadFile);

        ///Saving the game progress for current section
        GlobalVars.GameWorldMapProgressFile = currProgress;
    }

 //   public void Load() //#ERASE
	//{
	//	XmlSerializer serializer = new XmlSerializer(typeof(GameSaveState));

	//	GameSaveState gss;

 //       Debug.Log("GameStateSaver: Load() current state saved filename:" + FilePath());

	//	if(File.Exists(FilePath()))
	//	{
 //           FileInfo gamestatesaverfileinfo = new FileInfo(FilePath());
 //           if (gamestatesaverfileinfo.Length == 0)
 //           {
 //               Debug.Log("GameStateSaver: Load() game state pieces CORRUPTED");
 //               ResetListenIn();
 //           }
 //           else
 //           {
 //               using (FileStream fs = new FileStream(FilePath(), FileMode.Open))
 //               {
 //                   gss = (GameSaveState)serializer.Deserialize(fs);

 //                   for (int i = 0; i < gss.Chapters.Count; i++)
 //                   {
 //                       Debug.Log(
 //                           "Level Name: " + gss.Chapters[i].LevelName + " " +
 //                             "LevelNumber: " + gss.Chapters[i].LevelNumber
 //                         );

 //                       Chapter currChapter;
 //                       StateJigsawPuzzle.Instance.Chapters.TryGetValue(gss.Chapters[i].LevelName, out currChapter);

 //                       if (currChapter != null)
 //                       {
 //                           for (int j = 0; j < gss.Chapters[i].JigsawPeicesUnlocked.Length; j++)
 //                           {
 //                               currChapter.JigsawPeicesUnlocked[j] = gss.Chapters[i].JigsawPeicesUnlocked[j];
 //                           }
 //                       }
 //                       else { Debug.LogError(String.Format("Cannot load game state as chapter with key {0} have not been found", gss.Chapters[i].LevelName)); }

 //                   }

 //                   fs.Close();
 //               }
 //               Debug.Log("GameStateSaver: Load() jigsaw pieces game state");
 //           }

	//	}
	//	else
	//	{
 //           ListenIn.Logger.Instance.Log("GameStateSaver: First initialization", ListenIn.LoggerMessageType.Info);
 //           ResetListenIn();
	//	}

	//}

	//public void Save() //#ERASE
 //   {
 //       //TODO now there is StateJigsawPuzzle
	//	XmlSerializer serializer = 	new XmlSerializer(typeof(GameSaveState));
	//	using(TextWriter writer = new StreamWriter(FilePath()))
	//	{			
	//		GameSaveState gss = new GameSaveState();

 //           List<String> levelKeys = new List<string>(StateJigsawPuzzle.Instance.Chapters.Keys);

 //           for (int i = 0; i < levelKeys.Count; i++)
 //           {
 //               ChapterSaveState saveChapter = new ChapterSaveState();
 //               saveChapter.LevelName = levelKeys[i];

 //               Chapter currChapter;
 //               StateJigsawPuzzle.Instance.Chapters.TryGetValue(levelKeys[i], out currChapter);
 //               if (currChapter != null)
 //               {
 //                   saveChapter.LevelNumber = currChapter.LevelNumber;

 //                   for (int j = 0; j < saveChapter.JigsawPeicesUnlocked.Length; j++)
 //                   {
 //                       saveChapter.JigsawPeicesUnlocked[j] = currChapter.JigsawPeicesUnlocked[j];
 //                   }
 //               }
 //               else { Debug.LogError(String.Format("Cannot save game state as chapter with key {0} have not been found", levelKeys[i])); }

 //               gss.Chapters.Add(saveChapter);

 //           }

	//		serializer.Serialize(writer, gss);
	//		writer.Close();

	//	}
 //       ListenIn.Logger.Instance.Log("GameStateSaver: save jigssaw pieces state locally. ", ListenIn.LoggerMessageType.Info);
 //       //Debug.Log("Save game state");
	//}

	public void ResetListenIn()
	{
        //Debug.Log("Initializing ListenIn");
        Debug.Log("GameStateSaver: resetting jigsaw pieces state");
        Debug.LogWarning("<color=yellow>LISTENIN Reset; Reset jigsaw state. Check if this wanted</color>");
        //Reset();
        ResetGameProgress();
        //Load();
    }

    public void ResetGameProgress()
    {
        Debug.Log("Listen In - resettning game progress");
        string gameProgress;
        //TODO now there is StateJigsawPuzzle
        XmlSerializer serializer = new XmlSerializer(typeof(GameSaveState));
        using (StringWriter stringWriter = new StringWriter())
        {
            GameSaveState gss = new GameSaveState();

            List<String> levelKeys = new List<string>(StateJigsawPuzzle.Instance.Chapters.Keys);

            for (int i = 0; i < levelKeys.Count; i++)
            {
                ChapterSaveState resetChapter = new ChapterSaveState();
                resetChapter.LevelName = levelKeys[i];

                Chapter currChapter;
                StateJigsawPuzzle.Instance.Chapters.TryGetValue(levelKeys[i], out currChapter);
                if (currChapter != null)
                {
                    resetChapter.LevelNumber = currChapter.LevelNumber;
                    for (int j = 0; j < resetChapter.JigsawPeicesUnlocked.Length; j++)
                    {
                        resetChapter.JigsawPeicesUnlocked[j] = 0.0f;
                    }
                    gss.Chapters.Add(resetChapter);
                }
                else
                {
                    Debug.LogError(String.Format("GameStateSaver: cannot reset as chapter with key {0} have not been found", levelKeys[i]));
                    //Debug.LogError(String.Format("Cannot reset as chapter with key {0} have not been found", levelKeys[i]));
                }
            }
            serializer.Serialize(stringWriter, gss);
            gameProgress = stringWriter.ToString();
        }

        //SEND TO SERVER
        WWWForm form = new WWWForm();
        form.AddField("id_user", NetworkManager.IdUser);
        form.AddField("file_name", FileName());
        form.AddField("file_size", Encoding.ASCII.GetBytes(gameProgress).Length);
        form.AddField("folder_name", GlobalVars.GameProgressFolderName);
        form.AddBinaryData("file_data", Encoding.ASCII.GetBytes(gameProgress), FileName());
        NetworkManager.SendDataServer(form, NetworkUrl.ServerUrlUploadFile);

        ///Saving the game progress for current section
        GlobalVars.GameProgressFile = gameProgress;

        LoadGameProgress();
    }

    /// <summary>
    /// Called to reset the MadLevelManager and send the initial configuration to the server
    /// </summary>
    public void ResetWorldMapProgress()
    {
        MadLevelProfile.Reset();
        SaveGameWorldProgress();
    }

	public void ResetLI()
    {
        GlobalVars.GameProgressFile = String.Empty;
        GlobalVars.GameWorldMapProgressFile = String.Empty;
        ResetGameProgress();
        ResetWorldMapProgress();
    }

    public string FileName()
    {
        return String.Format("user_{0}_SavedState.xml", NetworkManager.IdUser);
    }

    public string WorldMapProgressFileName()
    {
        return String.Format("user_{0}_WorldMap.xml", NetworkManager.IdUser);
    }

}

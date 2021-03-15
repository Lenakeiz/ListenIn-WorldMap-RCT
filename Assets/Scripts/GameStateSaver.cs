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

public class GameStateSaver : MonoBehaviour {

	#region singleton
	private static readonly GameStateSaver instance = new GameStateSaver();
	public static GameStateSaver Instance
	{
		get
		{
			return instance;
		}
	}
	#endregion
    
	public void Load()
	{
		XmlSerializer serializer = new XmlSerializer(typeof(GameSaveState));

		GameSaveState gss;

        Debug.Log("GameStateSaver: Load() current state saved filename:" + FilePath());

		if(File.Exists(FilePath()))
		{
            FileInfo gamestatesaverfileinfo = new FileInfo(FilePath());
            if (gamestatesaverfileinfo.Length == 0)
            {
                Debug.Log("GameStateSaver: Load() game state pieces CORRUPTED");
                ResetListenIn();
            }
            else
            {
                using (FileStream fs = new FileStream(FilePath(), FileMode.Open))
                {
                    gss = (GameSaveState)serializer.Deserialize(fs);

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

                    fs.Close();
                }
                Debug.Log("GameStateSaver: Load() jigsaw pieces game state");
            }

		}
		else
		{
            ListenIn.Logger.Instance.Log("GameStateSaver: First initialization", ListenIn.LoggerMessageType.Info);
            GameStateSaver.Instance.ResetListenIn();
		}

	}

	public void Save()
	{
        //TODO now there is StateJigsawPuzzle
		XmlSerializer serializer = 	new XmlSerializer(typeof(GameSaveState));
		using(TextWriter writer = new StreamWriter(GameStateSaver.FilePath()))
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

			serializer.Serialize(writer, gss);
			writer.Close();

		}
        ListenIn.Logger.Instance.Log("GameStateSaver: save jigssaw pieces state locally. ", ListenIn.LoggerMessageType.Info);
        //Debug.Log("Save game state");
	}

	public void ResetListenIn()
	{
        //Debug.Log("Initializing ListenIn");
        ListenIn.Logger.Instance.Log("GameStateSaver: resetting jigsaw pieces state", ListenIn.LoggerMessageType.Info);
        Reset();
		Load ();
	}

	public void Reset()
	{

        try
        {
            if (File.Exists(FilePath()))
            {
                File.Delete(FilePath());
            }

            //Unlocking partially tutorial level
            XmlSerializer serializer = new XmlSerializer(typeof(GameSaveState));
            using (TextWriter writer = new StreamWriter(GameStateSaver.FilePath()))
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
                    else {
                        ListenIn.Logger.Instance.Log(String.Format("GameStateSaver: cannot reset as chapter with key {0} have not been found", levelKeys[i]), ListenIn.LoggerMessageType.Error);
                        //Debug.LogError(String.Format("Cannot reset as chapter with key {0} have not been found", levelKeys[i]));
                    }
                }

                serializer.Serialize(writer, gss);
                writer.Close();

            }
        }
        catch (Exception ex)
        {
            ListenIn.Logger.Instance.Log(ex.Message, ListenIn.LoggerMessageType.Error);
        }
        
	}

	public static string FilePath()
	{
        return Application.persistentDataPath + "/user_" + DatabaseXML.Instance.PatientId.ToString() + "_SavedState.xml";
	}

}

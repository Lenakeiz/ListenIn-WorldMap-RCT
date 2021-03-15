using UnityEngine;
using System.IO;
using System;
using System.Collections;

public enum TimerType { Idle, WorldMap, Therapy, Pinball }

public static class GlobalVars {

    public static bool isProfileNewOrChanged = false;
    /// <summary>
    /// AndreaLIRO_TB: This should be changed if we realize that the networked profile is changed.
    /// </summary>
    public static bool isNetworkedProfileNewOrChanged = false;
    //Pedro: use this class for paths purposes. I ll change user id so to use a string
    public static int ChallengeLength = 20;
    public static int ActChallengeLength = 30;
    public static int TherapyFilesOffset = 100;
    public static string GetPath(string patientID)
    {
#if UNITY_STANDALONE_WIN
        string patientPath = Application.persistentDataPath + @"/ListenIn/LIRO";
        patientPath = Path.Combine(patientPath, patientID);
        return patientPath;
#elif UNITY_EDITOR
        string patientPath = Application.persistentDataPath + @"/ListenIn/LIRO";
        patientPath = Path.Combine(patientPath, patientID);
        return patientPath;
#elif UNITY_ANDROID
        string patientPath = Application.persistentDataPath + @"/ListenIn/LIRO";
        patientPath = Path.Combine(patientPath, patientID);
        return patientPath;
#else
return Application.dataPath;// +"/"+ fileName;
#endif
    }

    public static string LiroProfileTemplate = "user_{0}_profile_LIRO.json";
    public static string LiroCoreItems = @"Doc/2018-04-core-challenge-list";
    //Remember this are always referenced with 
    public static string LiroACT = @"Doc/ACT_LIRO/ACT_LIRO";
    public static string LiroACT_A = @"Doc/ACT_LIRO/ACT_A";
    public static string LiroACT_B = @"Doc/ACT_LIRO/ACT_B";
    public static string LiroGeneratedACT = @"ACT/GEN_ACT"; ////////////////OLD REMOVE
    public static string LiroACT_Basket = @"Doc/ACT_LIRO/ACT_BASKET";
    public static string LiroGeneratedACT_Basket = @"ACT/GEN_ACT_BASKET"; ////////////////OLD REMOVE

    public static string LiroGeneratedACTFileName = @"GEN_ACT";
    public static string LiroGeneratedACTBasketFileName = @"GEN_ACT_BASKET";

    public static string LiroGenActBasketFile = string.Empty; //GEN_ACT_BASKET
    public static string LiroGenActFile = string.Empty; //GEN_ACT
    public static string GameProgressFile = string.Empty; //GAMES PROGRESS
    public static string GameWorldMapProgressFile = string.Empty; //GAMES PROGRESS

    public static string GameProgressFolderName = "Game";
    public static string GetPathToLIROOutput(string patientID) ////////////////OLD REMOVE
    { 
        return Path.Combine(GetPath(patientID), @"Output");
    }
    public static string OutputFolderName = "Output";
    public static string LogFolderName = "Logs";

    public static string GetPathToLIROBaskets()
    {
        return @"DocLiro";
    }
    public static string GetPathToLIROUserProfile(string patientID) ////////////////OLD REMOVE
    {
        return GetPath(patientID);
    }
    public static string GetPathToLIROCurrentLadderSection(string patientID) ////////////////OLD REMOVE
    {
        return Path.Combine(GetPath(patientID), @"Section");
    }
    public static string SectionFolderName = "Section";
    public static string GetPathToLIROACTFolder(string patientID) ////////////////OLD REMOVE
    {
        return Path.Combine(GetPath(patientID), @"ACT");
    }
    public static string ActFolderName = "ACT";
    public static string GetPathToLIROACTGenerated(string patientID) ////////////////OLD REMOVE
    {
        return Path.Combine(GetPath(patientID), LiroGeneratedACT);
    }
    public static string GetPathToLIROBasketACTGenerated(string patientID) ////////////////OLD REMOVE
    {
        return Path.Combine(GetPath(patientID), LiroGeneratedACT_Basket);
    }

    //Naming conventions to save the files
    public static string ACTStringFormat = "{0}_Block_{1}_Cycle_{2}.csv"; // 0 Is the TherapyLIRO step: for each cycle you have two ACT 
    public static string SARTStringFormat = "{0}_Cycle_{1}.csv"; // 0 Is the TherapyLIRO step: for each cycle you have two SART


}

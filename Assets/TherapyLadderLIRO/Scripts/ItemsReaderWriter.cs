using System.Collections;
using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;


public class ACTItemReader : ICsvReader<ACTChallenge>
{
    private int currStep;
    int ICsvReader<ACTChallenge>.CurrentStep
    {
        get
        {
            return currStep;
        }
    }
    public IEnumerable<ACTChallenge> ParseCsv(string path, bool loadFromResources = false)
    {
        List<ACTChallenge> currList = new List<ACTChallenge>();

        //Read a single file and extract the list of challenges
        string currBlock = File.ReadAllText(path);

        string[] lines = currBlock.Split(new char[] { '\n' });

        foreach (string item in lines)
        {
            if (item != String.Empty)
            {
                ACTChallenge currChallenge = new ACTChallenge();
                string[] sections = item.Replace("\r", String.Empty).Trim().Split(new char[] { ',' });
                currChallenge.LexicalItem = sections[1];
                currChallenge.FileAudioID_F = sections[8];
                currChallenge.FileAudioID_M = sections[9];
                long id = long.MaxValue;
                //Recovering ChallengeID
                long.TryParse(sections[0], out id);
                currChallenge.ChallengeID = id;
                long.TryParse(sections[2], out id);
                currChallenge.CorrectImageID = id;
                currChallenge.Foils.Add(id);
                long.TryParse(sections[3], out id);
                currChallenge.Foils.Add(id);
                long.TryParse(sections[4], out id);
                currChallenge.Foils.Add(id);
                long.TryParse(sections[5], out id);
                currChallenge.Foils.Add(id);
                long.TryParse(sections[6], out id);
                currChallenge.Foils.Add(id);
                long.TryParse(sections[7], out id);
                currChallenge.Foils.Add(id);
                currList.Add(currChallenge);
            }
        }

        return currList;

    }

    public IEnumerable<ACTChallenge> ParseCsvFromContent(string content)
    {
        List<ACTChallenge> currList = new List<ACTChallenge>();

        string[] lines = content.Split(new char[] { '\n' });

        foreach (string item in lines)
        {
            if (item != String.Empty)
            {
                ACTChallenge currChallenge = new ACTChallenge();
                string[] sections = item.Replace("\r", String.Empty).Trim().Split(new char[] { ',' });
                currChallenge.LexicalItem = sections[1];
                currChallenge.FileAudioID_F = sections[8];
                currChallenge.FileAudioID_M = sections[9];
                long id = long.MaxValue;
                //Recovering ChallengeID
                long.TryParse(sections[0], out id);
                currChallenge.ChallengeID = id;
                long.TryParse(sections[2], out id);
                currChallenge.CorrectImageID = id;
                currChallenge.Foils.Add(id);
                long.TryParse(sections[3], out id);
                currChallenge.Foils.Add(id);
                long.TryParse(sections[4], out id);
                currChallenge.Foils.Add(id);
                long.TryParse(sections[5], out id);
                currChallenge.Foils.Add(id);
                long.TryParse(sections[6], out id);
                currChallenge.Foils.Add(id);
                long.TryParse(sections[7], out id);
                currChallenge.Foils.Add(id);
                currList.Add(currChallenge);
            }
        }

        return currList;

    }
}

public class ACTItemWriter : ICsvWriter<ChallengeResponseACT>
{
    public void WriteCsv(string path, string filename, IEnumerable<ChallengeResponseACT> listToWrite)
    {
        string fullPath = Path.Combine(path, filename);
        List <string> listString = new List<string>();

        foreach (var item in listToWrite)
        {
            listString.Add(String.Join(",", new string[] {

                  item.m_challengeID.ToString(), 
                  item.m_timeStamp.ToString("dd/MM/yyyy"),
                  item.m_timeStamp.ToString("HH:mm:ss"),
                  item.m_number.ToString(),
                  item.m_block.ToString(),
                  item.m_cycle.ToString(),
                  item.m_accuracy.ToString(),
                  item.m_reactionTime.ToString(),
                  item.m_repeat.ToString(),
                  item.m_pictureID.ToString()
    }));


        }

        try
        {
            File.WriteAllLines(fullPath, listString.ToArray());
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
            throw;
        }
    }
}

public class CoreItemReader : ICsvReader<Challenge>
{
    private int currStep;
    int ICsvReader<Challenge>.CurrentStep
    {
        get
        {
            return currStep;
        }
    }
    public IEnumerable<Challenge> ParseCsv(string path, bool loadFromResources = false)
    {
        List<Challenge> currList = new List<Challenge>();

        //Read a single file and extract the list of challenges
        string currBlock;
        if (!loadFromResources)
        {
            currBlock = File.ReadAllText(path);
        }
        else
        {
            TextAsset ta = Resources.Load<TextAsset>(path);
            currBlock = ta.text;
        }
       
        string currFileName = Path.GetFileName(path);

        //AndreaLIRO: don t put this here, have who call this when preparing the block, register the information 
        //Extracting number of file so to track the progress of the player
        //TherapyLadderStep currStep = TherapyLIROManager.Instance.GetCurrentLadderStep();
        //string currNumber = Regex.Replace(currFileName, String.Concat(currStep.ToString(), "_"), String.Empty);
        //int stepNumber;

        //if (int.TryParse(currNumber, out stepNumber))
        //{
        //    TherapyLIROManager.Instance.SectionCounter = stepNumber;
        //}

        string[] lines = currBlock.Split(new char[] { '\n' });

        foreach (string item in lines)
        {
            if (item != String.Empty)
            {

                Challenge currChallenge = new Challenge(PopulateChallenge(item));
                currList.Add(currChallenge);
            }
        }

        return currList;

    }

    public IEnumerable<Challenge> ParseCsvFromContent(string content)
    {
        List<Challenge> currList = new List<Challenge>();

        //AndreaLIRO: don t put this here, have who call this when preparing the block, register the information 
        //Extracting number of file so to track the progress of the player
        //TherapyLadderStep currStep = TherapyLIROManager.Instance.GetCurrentLadderStep();
        //string currNumber = Regex.Replace(currFileName, String.Concat(currStep.ToString(), "_"), String.Empty);
        //int stepNumber;

        //if (int.TryParse(currNumber, out stepNumber))
        //{
        //    TherapyLIROManager.Instance.SectionCounter = stepNumber;
        //}

        string[] lines = content.Split(new char[] { '\n' });

        foreach (string item in lines)
        {
            if (item != String.Empty)
            {
                Challenge currChallenge = new Challenge(PopulateChallenge(item));
                currList.Add(currChallenge);
            }
        }

        return currList;

    }

    private Challenge PopulateChallenge(string item)
    {
        Challenge currChallenge = new Challenge();
        string[] sections = item.Replace("\r", String.Empty).Trim().Split(new char[] { ',' });

        //ChallengeID
        long id = long.MaxValue;
        long.TryParse(sections[0], out id);
        currChallenge.ChallengeID = id;

        //Difficulty
        int difficulty = 0;
        int.TryParse(sections[1], out difficulty);
        currChallenge.Difficulty = difficulty;

        //Lexical Item
        currChallenge.LexicalItem = sections[2];

        int additionalPointer = 0;
        if (sections.Length > 15) 
        {

            //Presentation Number
            int presentationNumber = 0;
            int.TryParse(sections[3], out presentationNumber);
            currChallenge.PresentationNumber = presentationNumber;

            //Lexical Presentation Number
            int lexicalPresentationNumber = 0;
            int.TryParse(sections[4], out lexicalPresentationNumber);
            currChallenge.LexicalPresentationNumber = lexicalPresentationNumber;

            //Lexical Presentation Number
            int basketNumber = 0;
            int.TryParse(sections[5], out basketNumber);
            currChallenge.BasketNumber = basketNumber;

            //Addint the pointer for having the same parser depending if offseted information or not
            //Andrea_LIRO: may consider to do different functions
            additionalPointer = 3;
        }

        //AudioFiles
        int audionull = 0;
        for (int i = 0; i < 5; i++)
        {
            //AndreaLIRO: I try to parse it as number and if it fails I save it as 0
            if (!int.TryParse(sections[3 + additionalPointer + i], out audionull))
            {
                currChallenge.FileAudioIDs.Add(sections[3 + additionalPointer + i]);
            }
            else
            {
                currChallenge.FileAudioIDs.Add("0");
            }
        }

        //Images
        long idcorrectfoil = 0;
        long.TryParse(sections[8 + additionalPointer], out idcorrectfoil);
        currChallenge.CorrectImageID = idcorrectfoil;
        if (idcorrectfoil == 0)
        {
            Debug.LogError("CRITICAL: Unable to read target image number for challenge ID " + currChallenge.ChallengeID);
        }
        currChallenge.Foils.Add(idcorrectfoil);

        for (int i = 0; i < 5; i++)
        {
            long idfoil = 0;
            long.TryParse(sections[9 + additionalPointer + i], out idfoil);
            currChallenge.Foils.Add(idfoil);
        }
        return currChallenge;
    }

}

public class CoreItemWriter : ICsvWriter<ChallengeResponse>
{
    public void WriteCsv(string path, string filename, IEnumerable<ChallengeResponse> listToWrite)
    {
        string fullPath = Path.Combine(path, filename);
        List<string> listString = new List<string>();

        foreach (var item in listToWrite)
        {
            listString.Add(String.Join(",", new string[] {

                  item.m_challengeID.ToString(),
                  item.m_cycle.ToString(),
                  item.m_block.ToString(),

                  item.m_dateTimeStart.ToString("dd/MM/yyyy"),
                  item.m_dateTimeStart.ToString("HH:mm:ss"),
                  item.m_dateTimeEnd.ToString("dd/MM/yyyy"),
                  item.m_dateTimeEnd.ToString("HH:mm:ss"),
                  
                  item.m_accuracy.ToString(),
                  item.m_repeat.ToString(),

                  item.incorrectPicturesIDs[0],
                  item.incorrectPicturesIDs[1],
                  item.incorrectPicturesIDs[2],
                  item.incorrectPicturesIDs[3],
                  item.incorrectPicturesIDs[4]

    }));


        }

        try
        {
            File.WriteAllLines(fullPath, listString.ToArray());
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
            throw;
        }
    }
}

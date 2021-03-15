using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

//namespace Assets.Seperate_Core_Work.Scripts.TherapyItemRecSys
//{
class CRecommender
{
    CDataset m_dataset = new CDataset();
    List<CLexicalItem> m_lsLexicalItem = new List<CLexicalItem>();
    List<CChallengeItem> m_lsChallengeItem = new List<CChallengeItem>();
    List<CChallengeItemFeatures> m_lsChallengeItemFeatures = new List<CChallengeItemFeatures>();
    List<int> m_lsChallengeItemFeatures_Forced = new List<int>();
    List<int> m_lsChallengeItemFeatures_StarterPool = new List<int>();
    List<int> m_lsChallengeItemFeatures_MedianPool = new List<int>();
    List<CVector_ChallengeItemFeaturesNeighbours> m_lsVector_Neighbours = new List<CVector_ChallengeItemFeaturesNeighbours>();

    CUser m_user = new CUser();

    //List<int> m_lsCurrentBlock_ChallengeItemIdx = new List<int>();
    List<int> m_lsCurrentBlock_ChallengeItemFeaturesIdx = new List<int>();
    List<int> m_lsCurrentBlock_IsDiversity = new List<int>();
    List<CVector_ChallengeItemFeaturesNeighbours> m_lsCurrentBlock_VectorNeighbours = new List<CVector_ChallengeItemFeaturesNeighbours>();
    List<CRecCandidate> m_lsCurrentBlock_RecCandidate = new List<CRecCandidate>();
    List<CRecCandidate> m_lsCurrentBlock_RecCandidate_Word = new List<CRecCandidate>();
    List<CRecCandidate> m_lsCurrentBlock_RecCandidate_ESentence = new List<CRecCandidate>();
    List<CRecCandidate> m_lsCurrentBlock_RecCandidate_HSentence = new List<CRecCandidate>();

    List<int> m_lsCurrentBlock_ChallengeItemIdx_Diversity = new List<int>();
    int m_intCurrentBlock_DiversityNum = 0;

    double m_dCurrentBlock_UserAbility = 0;
    //string m_strCurrentBlock_StartTime = "";
    DateTime m_dtCurrentBlock_StartTime = new DateTime();

    string m_strAppPath = "";

    //----------------------------------------------------------------------------------------------------
    // CRecommender
    //----------------------------------------------------------------------------------------------------
    public CRecommender()
    {
        /*
        m_dataset.loadDataset();
        m_lsLexicalItem = m_dataset.getLexicalItemList();
        m_lsChallengeItem = m_dataset.getChallengeItemList();
        m_lsChallengeItem_NeighbourList = m_dataset.getChallengeItem_NeighbourList();
        */
    }

    //----------------------------------------------------------------------------------------------------
    // init - load dataset & user profiles
    //----------------------------------------------------------------------------------------------------
    public void init(string strAppPath, string strUserId, int intDatasetId)
    {
        m_strAppPath = strAppPath;

        // if this is a new user, use the assigned dataset id, otherwise use user's dataset
        //int intUserDatasetId = m_user.getUserDatasetId(strAppPath, strUserId, intDatasetId);

        // load dataset
        m_dataset.loadDataset(intDatasetId);
        m_lsLexicalItem = m_dataset.getLexicalItemList();
        m_lsChallengeItem = m_dataset.getChallengeItemList();
        m_lsChallengeItemFeatures = m_dataset.getChallengeItemFeaturesList();
        m_lsVector_Neighbours = m_dataset.getVectorNeighboursList();
        m_lsChallengeItemFeatures_Forced = m_dataset.getChallengeItemFeaturesForcedList();
        m_lsChallengeItemFeatures_StarterPool = m_dataset.getChallengeItemFeaturesStarterPoolList();
        m_lsChallengeItemFeatures_MedianPool = m_dataset.getChallengeItemFeaturesMedianPoolList();

        // load user
        m_user.loadProfile(strAppPath, strUserId, intDatasetId, m_dataset.getCorpusComplexityStdDeviation(), m_dataset.getCifComplexity_DistinctList(), m_lsLexicalItem, m_lsChallengeItem, m_lsChallengeItemFeatures);

    }

    //----------------------------------------------------------------------------------------------------
    // LoadUserProfile - when switching users, dataset remains same, just need to reload user profiles
    //----------------------------------------------------------------------------------------------------
    /*public void loadUserProfile(string strAppPath, string strUserId)
    {
        m_user.loadProfile(strAppPath, strUserId, m_lsLexicalItem, m_lsChallengeItem, m_lsChallengeItemFeatures);
    }*/

    //----------------------------------------------------------------------------------------------------
    // init
    //----------------------------------------------------------------------------------------------------
    /*public void resetUserProfile()
    {
        //m_strAppPath = strAppPath;
        //m_dataset.loadDataset();

        m_user.resetUserProfile();

        //m_lsLexicalItem = m_dataset.getLexicalItemList();
        //m_lsChallengeItem = m_dataset.getChallengeItemList();
        //m_lsChallengeItem_NeighbourList = m_dataset.getChallengeItem_NeighbourList();
    }*/

    //----------------------------------------------------------------------------------------------------
    // getChallengeItemList
    //----------------------------------------------------------------------------------------------------
    public List<CChallengeItem> getChallengeItemList()
    {
        return m_lsChallengeItem;
    }

    //----------------------------------------------------------------------------------------------------
    // getChallengeItemFeaturesList
    //----------------------------------------------------------------------------------------------------
    public List<CChallengeItemFeatures> getChallengeItemFeaturesList()
    {
        return m_lsChallengeItemFeatures;
    }

    //----------------------------------------------------------------------------------------------------
    // getCurrentBlock_ChallengeItemIdxList
    //----------------------------------------------------------------------------------------------------
    /*public List<int> getCurrentBlock_ChallengeItemIdxList()
    {
        return m_lsCurrentBlock_ChallengeItemIdx;
    }*/

    //----------------------------------------------------------------------------------------------------
    // getCurrentBlock_ChallengeItemFeaturesIdxList
    //----------------------------------------------------------------------------------------------------
    public List<int> getCurrentBlock_ChallengeItemFeaturesIdxList()
    {
        return m_lsCurrentBlock_ChallengeItemFeaturesIdx;
    }

    //----------------------------------------------------------------------------------------------------
    // getCurrentBlock_ChallengeItemIdxDiversityList
    //----------------------------------------------------------------------------------------------------
    public List<int> getCurrentBlock_ChallengeItemIdxDiversityList()
    {
        return m_lsCurrentBlock_ChallengeItemIdx_Diversity;
    }

    //----------------------------------------------------------------------------------------------------
    // getCurrentBlock_RecCandidateList
    //----------------------------------------------------------------------------------------------------
    public List<CRecCandidate> getCurrentBlock_RecCandidateList()
    {
        return m_lsCurrentBlock_RecCandidate;
    }

    //----------------------------------------------------------------------------------------------------
    // getCurrentBlock_RecCandidateList_Word
    //----------------------------------------------------------------------------------------------------
    public List<CRecCandidate> getCurrentBlock_RecCandidateList_Word()
    {
        return m_lsCurrentBlock_RecCandidate_Word;
    }

    //----------------------------------------------------------------------------------------------------
    // getCurrentBlock_RecCandidateList_ESentence
    //----------------------------------------------------------------------------------------------------
    public List<CRecCandidate> getCurrentBlock_RecCandidateList_ESentence()
    {
        return m_lsCurrentBlock_RecCandidate_ESentence;
    }

    //----------------------------------------------------------------------------------------------------
    // getCurrentBlock_RecCandidateList_HSentence
    //----------------------------------------------------------------------------------------------------
    public List<CRecCandidate> getCurrentBlock_RecCandidateList_HSentence()
    {
        return m_lsCurrentBlock_RecCandidate_HSentence;
    }

    //----------------------------------------------------------------------------------------------------
    // getCurNoiseLevel
    //----------------------------------------------------------------------------------------------------
    public int getCurNoiseLevel()
    {
        return m_user.getCurNoiseLevel();
    }

    //----------------------------------------------------------------------------------------------------
    // getBlockType
    //----------------------------------------------------------------------------------------------------
    public int getCurBlockType()
    {
        return m_user.getBlockType();
    }

    //----------------------------------------------------------------------------------------------------
    // getCurBlockUserAbility
    //----------------------------------------------------------------------------------------------------
    public double getCurBlockUserAbility()
    {
        return m_dCurrentBlock_UserAbility;
    }

    //----------------------------------------------------------------------------------------------------
    // getFirstBlock
    //----------------------------------------------------------------------------------------------------
    public List<int> getFirstBlock()
    {
        m_lsCurrentBlock_ChallengeItemFeaturesIdx.Clear();
        m_lsCurrentBlock_IsDiversity.Clear();

        // randomise the list
        List<int> lsTemp = new List<int>();
        for (int i = 0; i < m_lsChallengeItemFeatures_StarterPool.Count; i++)
            lsTemp.Add(m_lsChallengeItemFeatures_StarterPool[i]);

        System.Random rnd1 = new System.Random();
        for (int i = 0; i < lsTemp.Count; i++)
        {
            int temp = lsTemp[i];
            int intRandomIdx = rnd1.Next(i, lsTemp.Count);
            lsTemp[i] = lsTemp[intRandomIdx];
            lsTemp[intRandomIdx] = temp;
        }
        for (int i = 0; i < lsTemp.Count; i++)
        {
            bool bExist = false;
            int intCandidateChallengeItemIdx = m_lsChallengeItemFeatures[lsTemp[i]].m_intChallengeItemIdx;
            for (int j = 0; j < m_lsCurrentBlock_ChallengeItemFeaturesIdx.Count; j++)
            {
                int intChallengeItemIdx = m_lsChallengeItemFeatures[m_lsCurrentBlock_ChallengeItemFeaturesIdx[j]].m_intChallengeItemIdx;
                if (intCandidateChallengeItemIdx == intChallengeItemIdx)
                {
                    bExist = true;
                    break;
                }
            }
            if (!bExist)
            {
                m_lsCurrentBlock_ChallengeItemFeaturesIdx.Add(lsTemp[i]);
                m_lsCurrentBlock_IsDiversity.Add(0);
            }
            if (m_lsCurrentBlock_ChallengeItemFeaturesIdx.Count >= CConstants.g_intItemNumPerBlock)
                break;
        }

        /*for (int i = 0; i < m_lsChallengeItemFeatures.Count; i++)
        {
            if ((m_lsChallengeItemFeatures[i].m_dComplexity_Frequency == 0.1) &&
                 (m_lsChallengeItemFeatures[i].m_dComplexity_Concreteness == 0.1) &&
                 (m_lsChallengeItemFeatures[i].m_dComplexity_DistractorNum == 0.1) &&
                 (m_lsChallengeItemFeatures[i].m_dComplexity_LinguisticCategory == 0.1)  )
            {
                m_lsCurrentBlock_ChallengeItemFeaturesIdx.Add(i);
                m_lsCurrentBlock_IsDiversity.Add(0);
            }

            if (m_lsCurrentBlock_ChallengeItemFeaturesIdx.Count >= CConstants.g_intItemNumPerBlock)
                break;
        }*/

        //m_strCurrentBlock_StartTime = System.DateTime.Now.ToString();
        m_dtCurrentBlock_StartTime = System.DateTime.UtcNow;

        return m_lsCurrentBlock_ChallengeItemFeaturesIdx;
    }

    //----------------------------------------------------------------------------------------------------
    // getMedianBlock
    //----------------------------------------------------------------------------------------------------
    public List<int> getMedianBlock()
    {
        //Console.WriteLine("Meidan block");

        m_lsCurrentBlock_ChallengeItemFeaturesIdx.Clear();
        m_lsCurrentBlock_IsDiversity.Clear();

        // randomise the list
        List<int> lsTemp = new List<int>();
        for (int i = 0; i < m_lsChallengeItemFeatures_MedianPool.Count; i++)
            lsTemp.Add(m_lsChallengeItemFeatures_MedianPool[i]);

        System.Random rnd1 = new System.Random();
        for (int i = 0; i < lsTemp.Count; i++)
        {
            int temp = lsTemp[i];
            int intRandomIdx = rnd1.Next(i, lsTemp.Count);
            lsTemp[i] = lsTemp[intRandomIdx];
            lsTemp[intRandomIdx] = temp;
        }
        for (int i = 0; i < lsTemp.Count; i++)
        {
            bool bExist = false;
            int intCandidateChallengeItemIdx = m_lsChallengeItemFeatures[lsTemp[i]].m_intChallengeItemIdx;
            for (int j = 0; j < m_lsCurrentBlock_ChallengeItemFeaturesIdx.Count; j++)
            {
                int intChallengeItemIdx = m_lsChallengeItemFeatures[m_lsCurrentBlock_ChallengeItemFeaturesIdx[j]].m_intChallengeItemIdx;
                if (intCandidateChallengeItemIdx == intChallengeItemIdx)
                {
                    bExist = true;
                    break;
                }
            }
            if (!bExist)
            {
                m_lsCurrentBlock_ChallengeItemFeaturesIdx.Add(lsTemp[i]);
                m_lsCurrentBlock_IsDiversity.Add(0);
            }
            if (m_lsCurrentBlock_ChallengeItemFeaturesIdx.Count >= CConstants.g_intItemNumPerBlock)
                break;
        }

        //m_strCurrentBlock_StartTime = System.DateTime.Now.ToString();
        m_dtCurrentBlock_StartTime = System.DateTime.UtcNow;

        return m_lsCurrentBlock_ChallengeItemFeaturesIdx;
    }

    //----------------------------------------------------------------------------------------------------
    // getForcedBlock
    //----------------------------------------------------------------------------------------------------
    public List<int> getForcedBlock()
    {
        //Console.WriteLine("*** forced block ");

        // calculate the last three block average number of distractors
        double dTotal = 0;
        int intAverageDistractor = 2;
        List<CUser_TherapyBlock> lsTherapyBlock = m_user.getTherapyBlockList();
        if (lsTherapyBlock.Count >= 3)
        {
            for (int i = lsTherapyBlock.Count - 1; i >= lsTherapyBlock.Count - 3; i--)
                dTotal += lsTherapyBlock[i].m_dMean_DistractorNum;
            intAverageDistractor = (int)(dTotal / 3);
            if (intAverageDistractor < 2)
                intAverageDistractor = 2;
        }

        m_lsCurrentBlock_RecCandidate.Clear();
        for (int i = 0; i < m_lsChallengeItemFeatures_Forced.Count; i++)
        {
            CChallengeItemFeatures features = m_lsChallengeItemFeatures[m_lsChallengeItemFeatures_Forced[i]];
            if (convertDistractorNum(features.m_dComplexity_DistractorNum) <= intAverageDistractor)
            {
                int intChallengeItemIdx = m_lsChallengeItemFeatures[m_lsChallengeItemFeatures_Forced[i]].m_intChallengeItemIdx;
                int intLexicalItemIdx = m_lsChallengeItem[intChallengeItemIdx].m_intLexicalItemIdx;

                CRecCandidate recCandidate = new CRecCandidate();
                recCandidate.m_intChallengeItemFeaturesIdx = m_lsChallengeItemFeatures_Forced[i];
                recCandidate.m_dExposureWeight = Math.Round((((double)CConstants.g_intItemMaxExposureCtr - m_user.getLexicalItemExposureCtr(intLexicalItemIdx)) / (double)CConstants.g_intItemMaxExposureCtr), 4);
                recCandidate.m_dRecommendationStrength = features.m_dComplexity_DistractorNum;
                m_lsCurrentBlock_RecCandidate.Add(recCandidate);
            }
        }

        // cluster candidates into word and sentence groups
        m_lsCurrentBlock_RecCandidate_Word.Clear();
        m_lsCurrentBlock_RecCandidate_ESentence.Clear();
        m_lsCurrentBlock_RecCandidate_HSentence.Clear();
        for (int i = 0; i < m_lsCurrentBlock_RecCandidate.Count; i++)
        {
            int intFeaturesIdx = m_lsCurrentBlock_RecCandidate[i].m_intChallengeItemFeaturesIdx;
            if (m_lsChallengeItemFeatures[intFeaturesIdx].m_intLinguisticType == (int)CConstants.g_LinguisticType.HardSentence)
            {
                if (m_lsCurrentBlock_RecCandidate[i].m_dRecommendationStrength > 0)
                    m_lsCurrentBlock_RecCandidate_HSentence.Add(m_lsCurrentBlock_RecCandidate[i]);
            }
            else if (m_lsChallengeItemFeatures[intFeaturesIdx].m_intLinguisticType == (int)CConstants.g_LinguisticType.EasySentence)
            {
                if (m_lsCurrentBlock_RecCandidate[i].m_dRecommendationStrength > 0)
                    m_lsCurrentBlock_RecCandidate_ESentence.Add(m_lsCurrentBlock_RecCandidate[i]);
            }
            else
            {
                if (m_lsCurrentBlock_RecCandidate[i].m_dRecommendationStrength > 0)
                    m_lsCurrentBlock_RecCandidate_Word.Add(m_lsCurrentBlock_RecCandidate[i]);
            }
        }
        Console.WriteLine("word num = " + m_lsCurrentBlock_RecCandidate_Word.Count + " E sentence num = " + m_lsCurrentBlock_RecCandidate_ESentence.Count + " H sentence num = " + m_lsCurrentBlock_RecCandidate_HSentence.Count);
        m_lsCurrentBlock_RecCandidate_Word = m_lsCurrentBlock_RecCandidate_Word.OrderByDescending(o => o.m_dExposureWeight).ThenByDescending(o => o.m_dRecommendationStrength).ToList();
        m_lsCurrentBlock_RecCandidate_ESentence = m_lsCurrentBlock_RecCandidate_ESentence.OrderByDescending(o => o.m_dExposureWeight).ThenByDescending(o => o.m_dRecommendationStrength).ToList();
        m_lsCurrentBlock_RecCandidate_HSentence = m_lsCurrentBlock_RecCandidate_HSentence.OrderByDescending(o => o.m_dExposureWeight).ThenByDescending(o => o.m_dRecommendationStrength).ToList();

        // decide word or sentence
        bool bEnoughWord = false;
        bool bEnoughESentence = false;
        bool bEnoughHSentence = false;
        List<CRecCandidate> lsRecCandidateFinal = new List<CRecCandidate>();

        lsRecCandidateFinal.Clear();
        bEnoughWord = checkHasEnoughCandidates(m_lsCurrentBlock_RecCandidate_Word, 0, false);
        bEnoughESentence = checkHasEnoughCandidates(m_lsCurrentBlock_RecCandidate_ESentence, 0, false);
        bEnoughHSentence = checkHasEnoughCandidates(m_lsCurrentBlock_RecCandidate_HSentence, 0, false);

        if (bEnoughWord && bEnoughESentence && bEnoughHSentence)
        {
            // pick the one with the highest exposureWeight
            List<double> lsExposureWeight = new List<double>();
            lsExposureWeight.Add(m_lsCurrentBlock_RecCandidate_Word[0].m_dExposureWeight);
            lsExposureWeight.Add(m_lsCurrentBlock_RecCandidate_ESentence[0].m_dExposureWeight);
            lsExposureWeight.Add(m_lsCurrentBlock_RecCandidate_HSentence[0].m_dExposureWeight);
            double dHighest = lsExposureWeight[0];
            int intIdx = 0;
            for (int j = 1; j < lsExposureWeight.Count; j++)
            {
                if (lsExposureWeight[j] > dHighest)
                {
                    dHighest = lsExposureWeight[j];
                    intIdx = j;
                }
            }
            if (intIdx == 0)
                lsRecCandidateFinal = m_lsCurrentBlock_RecCandidate_Word;
            else if (intIdx == 1)
                lsRecCandidateFinal = m_lsCurrentBlock_RecCandidate_ESentence;
            else
                lsRecCandidateFinal = m_lsCurrentBlock_RecCandidate_HSentence;
        }
        else if (bEnoughWord && bEnoughESentence)
        {
            if (m_lsCurrentBlock_RecCandidate_Word[0].m_dExposureWeight > m_lsCurrentBlock_RecCandidate_ESentence[0].m_dExposureWeight)
                lsRecCandidateFinal = m_lsCurrentBlock_RecCandidate_Word;
            else
                lsRecCandidateFinal = m_lsCurrentBlock_RecCandidate_ESentence;
        }
        else if (bEnoughWord && bEnoughHSentence)
        {
            if (m_lsCurrentBlock_RecCandidate_Word[0].m_dExposureWeight > m_lsCurrentBlock_RecCandidate_HSentence[0].m_dExposureWeight)
                lsRecCandidateFinal = m_lsCurrentBlock_RecCandidate_Word;
            else
                lsRecCandidateFinal = m_lsCurrentBlock_RecCandidate_HSentence;
        }
        else if (bEnoughESentence && bEnoughHSentence)
        {
            if (m_lsCurrentBlock_RecCandidate_ESentence[0].m_dExposureWeight > m_lsCurrentBlock_RecCandidate_HSentence[0].m_dExposureWeight)
                lsRecCandidateFinal = m_lsCurrentBlock_RecCandidate_ESentence;
            else
                lsRecCandidateFinal = m_lsCurrentBlock_RecCandidate_HSentence;
        }
        else if (bEnoughHSentence)
            lsRecCandidateFinal = m_lsCurrentBlock_RecCandidate_HSentence;
        else if (bEnoughESentence)
            lsRecCandidateFinal = m_lsCurrentBlock_RecCandidate_ESentence;
        else if (bEnoughWord)
            lsRecCandidateFinal = m_lsCurrentBlock_RecCandidate_Word;

        //Console.WriteLine("final candidate num = " + lsRecCandidateFinal.Count + " bEnoughWord = " + bEnoughWord + " bEnoughESentence = " + bEnoughESentence + " bEnoughHSentence = " + bEnoughHSentence);

        // pick the top-N
        m_lsCurrentBlock_ChallengeItemFeaturesIdx.Clear();
        m_lsCurrentBlock_IsDiversity.Clear();
        for (int i = 0; i < lsRecCandidateFinal.Count; i++)
        {
            bool bExist = false;
            int intCandidateChallengeItemIdx = m_lsChallengeItemFeatures[lsRecCandidateFinal[i].m_intChallengeItemFeaturesIdx].m_intChallengeItemIdx;
            for (int j = 0; j < m_lsCurrentBlock_ChallengeItemFeaturesIdx.Count; j++)
            {
                int intChallengeItemIdx = m_lsChallengeItemFeatures[m_lsCurrentBlock_ChallengeItemFeaturesIdx[j]].m_intChallengeItemIdx;
                if (intCandidateChallengeItemIdx == intChallengeItemIdx)
                {
                    bExist = true;
                    break;
                }
            }
            if (!bExist)
            {
                m_lsCurrentBlock_ChallengeItemFeaturesIdx.Add(lsRecCandidateFinal[i].m_intChallengeItemFeaturesIdx);
                m_lsCurrentBlock_IsDiversity.Add(0);  // no diversity for forced block
            }
            if (m_lsCurrentBlock_ChallengeItemFeaturesIdx.Count >= CConstants.g_intItemNumPerBlock)
                break;
        }

        //m_strCurrentBlock_StartTime = System.DateTime.Now.ToString();
        m_dtCurrentBlock_StartTime = System.DateTime.Now;

        return m_lsCurrentBlock_ChallengeItemFeaturesIdx;
    }

    //----------------------------------------------------------------------------------------------------
    // getNextBlock
    //----------------------------------------------------------------------------------------------------
    public List<int> getNextBlock()
    {       
        m_intCurrentBlock_DiversityNum = 0;
        //m_strCurrentBlock_StartTime = System.DateTime.Now.ToString();
        m_dtCurrentBlock_StartTime = System.DateTime.Now;

        if (m_user.getTherapyBlockList().Count == 0)
        {
            if (m_user.getRoundIdx() > 0)
                return getMedianBlock();
            else
                return getFirstBlock();
        }

        m_user.resetBlock();

        int intCurBlockType = m_user.getCurBlockType();
        if (intCurBlockType > 0)
            return getForcedBlock();

        int intCurBlockNoiseLevel = m_user.getCurBlockNoiseLevel();

        // get user's last therapy block        
        List<CUser_TherapyBlock> lsTherapyBlock = m_user.getTherapyBlockList();
        Debug.Log(String.Format("CRecommender: getNextBlock() getNextBlock - lsTherapyBlock = {0}",lsTherapyBlock.Count));
        CUser_TherapyBlock lastTherapyBlock = lsTherapyBlock.Last();
        for (int i = lsTherapyBlock.Count - 1; i >= 0; i--)
        {
            if ( (lsTherapyBlock[i].m_intRoundIdx == m_user.getRoundIdx()) && (lsTherapyBlock[i].m_intNoiseLevel == 0) && (lsTherapyBlock[i].m_intBlockType == 0))
            {
                // if cur block is to be a noise block then its complexity should be the same as the previous block 
                // and thus the calcualtion should be based on the non-noise block bef the previous block
                if (intCurBlockNoiseLevel > 0)
                {
                    for (int j = i - 1; j >= 0; j--)
                    {
                        if ((lsTherapyBlock[j].m_intRoundIdx == m_user.getRoundIdx()) && (lsTherapyBlock[j].m_intNoiseLevel == 0) && (lsTherapyBlock[j].m_intBlockType == 0))
                        {
                            lastTherapyBlock = lsTherapyBlock[j];
                            break;
                        }
                    }
                }
                else
                    lastTherapyBlock = lsTherapyBlock[i];
                break;
            }
        }

        // select neighbours from the last 5 blocks        
        m_lsCurrentBlock_VectorNeighbours.Clear();
        int intBlockCtr = 0;
        int intNoiseBlockCtr = 0;
        int intForcedBlockCtr = 0;
        //double dTotalUserAbility = 0;
        while ((intBlockCtr < CConstants.g_intBlockNumPerWindow) && ((intBlockCtr + intNoiseBlockCtr + intForcedBlockCtr) < lsTherapyBlock.Count))
        {
            CUser_TherapyBlock curTherapyBlock = lsTherapyBlock[lsTherapyBlock.Count - intBlockCtr - intNoiseBlockCtr - intForcedBlockCtr - 1];
            if (curTherapyBlock.m_intNoiseLevel > 0) // exclude noise blocks
                intNoiseBlockCtr++;
            else if (curTherapyBlock.m_intBlockType > 0) // exclude forced blocks
                intForcedBlockCtr++;
            else
            {
                //dTotalUserAbility += curTherapyBlock.m_dUserAbility;

                for (int i = 0; i < curTherapyBlock.m_lsChallengeItemFeaturesIdx.Count; i++)
                {
                    int intChallengeItemFeaturesIdx = curTherapyBlock.m_lsChallengeItemFeaturesIdx[i];
                    //Console.WriteLine("\n" + printFeatures(intChallengeItemFeaturesIdx));

                    CVector_ChallengeItemFeaturesNeighbours vector_Neighbours = new CVector_ChallengeItemFeaturesNeighbours();

                    // find similar items for only items that the patient has responded correctly 
                    if (curTherapyBlock.m_lsResponseAccuracy[i] == 1) // commented out coz if patient does not have any correct responses, no neighbours will be found
                    {
                        for (int j = 0; j < m_lsVector_Neighbours[intChallengeItemFeaturesIdx].m_lsChallengeItemFeatures_Neighbour.Count; j++)
                        {
                            CChallengeItemFeatures_Neighbour neighbour = m_lsVector_Neighbours[intChallengeItemFeaturesIdx].m_lsChallengeItemFeatures_Neighbour[j];

                            // exclude neighbour that has already been presented in the last block
                            int intIdx = lastTherapyBlock.m_lsChallengeItemFeaturesIdx.FindIndex(a => a == neighbour.m_intChallengeItemFeaturesIdx);

                            // exclude neighbour whose lexical item has already been exposed more than 100 times
                            int intChallengeItemIdx = m_lsChallengeItemFeatures[neighbour.m_intChallengeItemFeaturesIdx].m_intChallengeItemIdx;
                            int intLexicalItemIdx = m_lsChallengeItem[intChallengeItemIdx].m_intLexicalItemIdx;
                            int intExposure = m_user.getLexicalItemExposureCtr(intLexicalItemIdx);

                            // if this item has incorrect response, then include ony the neighbour with the same or less complexity
                            bool bAdd = true;
                            if (curTherapyBlock.m_lsResponseAccuracy[i] == 0)
                            {
                                double dItemComplexity = curTherapyBlock.m_lsChallengeItemFeatures_Complexity[i];
                                double dNeighbourComplexity = calculateCurComplexity(neighbour.m_intChallengeItemFeaturesIdx);
                                if (dNeighbourComplexity > dItemComplexity)
                                    bAdd = false;
                            }

                            if ((intIdx <= -1) && (bAdd) && (intExposure < CConstants.g_intItemMaxExposureCtr))
                            {
                                vector_Neighbours.m_lsChallengeItemFeatures_Neighbour.Add(neighbour);
                                //Console.Write(printFeatures(neighbour.m_intChallengeItemFeaturesIdx) + "  ");                               
                            }
                        }
                    }
                    m_lsCurrentBlock_VectorNeighbours.Add(vector_Neighbours);
                }
                intBlockCtr++;
            }
        }
        
        // compute recommendation strength of each neighbour - convert neighbour matrix into nX1 vector
        m_lsCurrentBlock_RecCandidate.Clear();
        for (int i = 0; i < m_lsCurrentBlock_VectorNeighbours.Count; i++)
        {
            // it is alwasy 1 coz all neighbours are from items with response accuracy 1
            double dCurResponseAccuracy = 1; // lastTherapyBlock.m_lsResponseAccuracy[i];                                   

            for (int j = 0; j < m_lsCurrentBlock_VectorNeighbours[i].m_lsChallengeItemFeatures_Neighbour.Count; j++)
            {
                CChallengeItemFeatures_Neighbour neighbour = m_lsCurrentBlock_VectorNeighbours[i].m_lsChallengeItemFeatures_Neighbour[j];
                int intIdx = m_lsCurrentBlock_RecCandidate.FindIndex(a => a.m_intChallengeItemFeaturesIdx == neighbour.m_intChallengeItemFeaturesIdx);
                if (intIdx > -1)
                {
                    m_lsCurrentBlock_RecCandidate[intIdx].m_intNeighbourForCtr++;
                    //m_lsCurrentBlock_RecCandidate[intIdx].m_intPresentedCtr = m_user.getChallengeItemPresentedCtr(m_lsCurrentBlock_RecCandidate[intIdx].m_intChallengeItemIdx);
                    m_lsCurrentBlock_RecCandidate[intIdx].m_dSimilarityStrength += Math.Round((neighbour.m_dSimilarity * dCurResponseAccuracy), 4);
                }
                else
                {
                    CRecCandidate recCandidate = new CRecCandidate();
                    recCandidate.m_intChallengeItemFeaturesIdx = neighbour.m_intChallengeItemFeaturesIdx;
                    recCandidate.m_intNeighbourForCtr++;
                    //recCandidate.m_intPresentedCtr = m_user.getChallengeItemPresentedCtr(neighbour.m_intChallengeItemIdx);
                    recCandidate.m_dSimilarityStrength += Math.Round((neighbour.m_dSimilarity * dCurResponseAccuracy), 4);
                    m_lsCurrentBlock_RecCandidate.Add(recCandidate);
                }
            }
        }

        // normalise Similarity strength, compute recommendation strength = simliarityStrength + exposureWeight + neighbourWeight
        int intTotalItemsPerWindow = CConstants.g_intItemNumPerBlock * CConstants.g_intBlockNumPerWindow;
        for (int i = 0; i < m_lsCurrentBlock_RecCandidate.Count; i++)
        {
            if (m_lsCurrentBlock_RecCandidate[i].m_intNeighbourForCtr > 0)
                m_lsCurrentBlock_RecCandidate[i].m_dSimilarityStrength = Math.Round(m_lsCurrentBlock_RecCandidate[i].m_dSimilarityStrength / m_lsCurrentBlock_RecCandidate[i].m_intNeighbourForCtr, 4);

            // calculate neighbour weight
            m_lsCurrentBlock_RecCandidate[i].m_dNeighbourWeight = Math.Round((1 - ((double)intTotalItemsPerWindow - m_lsCurrentBlock_RecCandidate[i].m_intNeighbourForCtr) / (double)intTotalItemsPerWindow), 4);

            // calculate exposure weight
            int intChallengeItemIdx = m_lsChallengeItemFeatures[m_lsCurrentBlock_RecCandidate[i].m_intChallengeItemFeaturesIdx].m_intChallengeItemIdx;
            int intLexicalItemIdx = m_lsChallengeItem[intChallengeItemIdx].m_intLexicalItemIdx;
            m_lsCurrentBlock_RecCandidate[i].m_dExposureWeight = Math.Round((((double)CConstants.g_intItemMaxExposureCtr - m_user.getLexicalItemExposureCtr(intLexicalItemIdx)) / (double)CConstants.g_intItemMaxExposureCtr), 4);

            if (m_lsCurrentBlock_RecCandidate[i].m_dExposureWeight < 0)  // discard items which has been exposed > 100 times
                m_lsCurrentBlock_RecCandidate[i].m_dRecommendationStrength = 0;
            else
                m_lsCurrentBlock_RecCandidate[i].m_dRecommendationStrength = m_lsCurrentBlock_RecCandidate[i].m_dSimilarityStrength +
                                                                            m_lsCurrentBlock_RecCandidate[i].m_dNeighbourWeight +
                                                                            m_lsCurrentBlock_RecCandidate[i].m_dExposureWeight;
        }
        //Console.WriteLine("num neighbour = " + m_lsCurrentBlock_RecCandidate.Count);
        Debug.Log(String.Format("CRecommender: getNextBlock() num neighbour = {0}", m_lsCurrentBlock_RecCandidate.Count));

        // zero out candidate which has complexity higher than user's ability
        double dLastUserAbility = lastTherapyBlock.m_dUserAbility_Accumulated;        
        m_dCurrentBlock_UserAbility = dLastUserAbility; // dAverageUserAbility; // Math.Round(dAverageUserAbility + (m_dataset.getCorpusComplexityStdDeviation() / 2), 4); 
        //Console.WriteLine("current user ability = " + m_dCurrentBlock_UserAbility);

        bool bEnoughWord = false;
        bool bEnoughESentence = false;
        bool bEnoughHSentence = false;
        List<CRecCandidate> lsRecCandidateFinal = new List<CRecCandidate>();
        double dPruningFactor = 0;
        // loop till find enough words or sentences
        while (!bEnoughWord && !bEnoughESentence && !bEnoughHSentence && (dPruningFactor <= 2))
        {
            dPruningFactor += 1;  //dPruningFactor += 0.3;
            //Console.WriteLine("dPruningFactor = " + dPruningFactor);
            if (dPruningFactor > 2) break;

            int intCtr = 0;
            List<CRecCandidate> lsCandidateTemp = new List<CRecCandidate>();
            for (int i = 0; i < m_lsCurrentBlock_RecCandidate.Count; i++)
            {
                CRecCandidate candidate = new CRecCandidate(m_lsCurrentBlock_RecCandidate[i]);
                lsCandidateTemp.Add(candidate);
                if (m_lsCurrentBlock_RecCandidate[i].m_dRecommendationStrength > 0)
                    intCtr++;
            }
            //Console.WriteLine("candidate ctr = " + m_lsCurrentBlock_RecCandidate.Count + " intCtr = " + intCtr);

            int intLowerPrune = 0;
            int intUpperPrune = 0;
            for (int i = 0; i < lsCandidateTemp.Count; i++)
            {
                int intFeatureIdx = lsCandidateTemp[i].m_intChallengeItemFeaturesIdx;
                lsCandidateTemp[i].m_dCurComplexity = calculateCurComplexity(intFeatureIdx);

                List<double> lsValue1 = new List<double>();
                List<double> lsValue2 = new List<double>();
                lsValue1.Add(dLastUserAbility);
                lsValue2.Add(lsCandidateTemp[i].m_dCurComplexity);
                lsCandidateTemp[i].m_dComplexityWeight = Math.Round(CEuclideanDistance.EuclideanSimilarity(lsValue1, lsValue2), 4);

                lsCandidateTemp[i].m_dRecommendationStrength = lsCandidateTemp[i].m_dSimilarityStrength +
                                                                 lsCandidateTemp[i].m_dNeighbourWeight +
                                                                 lsCandidateTemp[i].m_dExposureWeight + lsCandidateTemp[i].m_dComplexityWeight;

                //Console.WriteLine("candidate complexity = " + lsCandidateTemp[i].m_dCurComplexity);

                // 2016-07-23 - lower prune
                if (lsCandidateTemp[i].m_dCurComplexity < lastTherapyBlock.m_dNextBlock_DiversityThresholdLower)
                {
                    lsCandidateTemp[i].m_dRecommendationStrength = 0;
                    intLowerPrune++;
                    //Console.WriteLine("lower prune - candidate complexity = " + lsCandidateTemp[i].m_dCurComplexity +
                    //                " dLastUserAbility = " + dLastUserAbility + " DiversityStepLower = " + lastTherapyBlock.m_dNextBlock_DiversityStepLower +
                    //                " lower threshold = " + (dLastUserAbility - (lastTherapyBlock.m_dNextBlock_DiversityStepLower * dPruningFactor)) );
                }
                // upper prune
                if (lsCandidateTemp[i].m_dCurComplexity > lastTherapyBlock.m_dNextBlock_DiversityThresholdUpper)
                {
                    lsCandidateTemp[i].m_dRecommendationStrength = 0;
                    intUpperPrune++;
                    //Console.WriteLine("candidate complexity = " + lsCandidateTemp[i].m_dCurComplexity +
                    //                    " upper threshold = " + (dLastUserAbility + dFactor));
                }
            }
            //Console.WriteLine("intLowerPrune = " + intLowerPrune + " intUpperPrune = " + intUpperPrune);
            Debug.Log(String.Format("CRecommender: getNextBlock() intLowerPrune = {0} intUpperPrune = {1} ", intLowerPrune, intUpperPrune));

            // cluster candidates into word and sentence groups
            m_lsCurrentBlock_RecCandidate_Word.Clear();
            m_lsCurrentBlock_RecCandidate_ESentence.Clear();
            m_lsCurrentBlock_RecCandidate_HSentence.Clear();
            for (int i = 0; i < lsCandidateTemp.Count; i++)
            {
                int intFeaturesIdx = lsCandidateTemp[i].m_intChallengeItemFeaturesIdx;
                if (m_lsChallengeItemFeatures[intFeaturesIdx].m_intLinguisticType == (int)CConstants.g_LinguisticType.HardSentence)
                {
                    if (lsCandidateTemp[i].m_dRecommendationStrength > 0)
                        m_lsCurrentBlock_RecCandidate_HSentence.Add(lsCandidateTemp[i]);
                }
                else if (m_lsChallengeItemFeatures[intFeaturesIdx].m_intLinguisticType == (int)CConstants.g_LinguisticType.EasySentence)
                {
                    if (lsCandidateTemp[i].m_dRecommendationStrength > 0)
                        m_lsCurrentBlock_RecCandidate_ESentence.Add(lsCandidateTemp[i]);
                }
                else
                {
                    if (lsCandidateTemp[i].m_dRecommendationStrength > 0)
                        m_lsCurrentBlock_RecCandidate_Word.Add(lsCandidateTemp[i]);
                }
            }
            //Console.WriteLine("word num = " + m_lsCurrentBlock_RecCandidate_Word.Count + " E sentence num = " + m_lsCurrentBlock_RecCandidate_ESentence.Count + " H sentence num = " + m_lsCurrentBlock_RecCandidate_HSentence.Count);
            m_lsCurrentBlock_RecCandidate_Word = m_lsCurrentBlock_RecCandidate_Word.OrderByDescending(o => o.m_dCurComplexity).ThenByDescending(o => o.m_dRecommendationStrength).ToList();
            m_lsCurrentBlock_RecCandidate_ESentence = m_lsCurrentBlock_RecCandidate_ESentence.OrderByDescending(o => o.m_dCurComplexity).ThenByDescending(o => o.m_dRecommendationStrength).ToList();
            m_lsCurrentBlock_RecCandidate_HSentence = m_lsCurrentBlock_RecCandidate_HSentence.OrderByDescending(o => o.m_dCurComplexity).ThenByDescending(o => o.m_dRecommendationStrength).ToList();

            Debug.Log(String.Format("CRecommender: getNextBlock() word num = {0} E sentence num = {1} H sentence num = {2}", m_lsCurrentBlock_RecCandidate_Word.Count, m_lsCurrentBlock_RecCandidate_ESentence.Count, m_lsCurrentBlock_RecCandidate_HSentence.Count));
            
            // decide word or sentence
            lsRecCandidateFinal.Clear();
            bEnoughWord = checkHasEnoughCandidates(m_lsCurrentBlock_RecCandidate_Word, dLastUserAbility, true);
            bEnoughESentence = checkHasEnoughCandidates(m_lsCurrentBlock_RecCandidate_ESentence, dLastUserAbility, true);
            bEnoughHSentence = checkHasEnoughCandidates(m_lsCurrentBlock_RecCandidate_HSentence, dLastUserAbility, true);

            // train the patient with the starter pool till he can manage 0.1322
            if ((!bEnoughWord) && (!bEnoughESentence) && (!bEnoughHSentence) && (dLastUserAbility <= 0.1322))
            {
                //Console.WriteLine("Keep training with starter pool");
                return getFirstBlock();
            }

            //if ( (m_lsCurrentBlock_RecCandidate_Word.Count > CConstants.g_intItemNumPerBlock) && (m_lsCurrentBlock_RecCandidate_Sentence.Count > CConstants.g_intItemNumPerBlock) )
            if (bEnoughWord && bEnoughESentence && bEnoughHSentence)
            {
                // if last therapy block = word, this therapy block = sentence, otherwise = word
                if (lastTherapyBlock.m_intLinguisticType >= (int)CConstants.g_LinguisticType.HardSentence) // last block = hard
                    lsRecCandidateFinal = m_lsCurrentBlock_RecCandidate_Word.OrderByDescending(o => o.m_dCurComplexity).ThenByDescending(o => o.m_dRecommendationStrength).ToList();
                else if (lastTherapyBlock.m_intLinguisticType >= (int)CConstants.g_LinguisticType.EasySentence) // last block = easy
                    lsRecCandidateFinal = m_lsCurrentBlock_RecCandidate_HSentence.OrderByDescending(o => o.m_dCurComplexity).ThenByDescending(o => o.m_dRecommendationStrength).ToList();
                else   // last block = word
                    lsRecCandidateFinal = m_lsCurrentBlock_RecCandidate_ESentence.OrderByDescending(o => o.m_dCurComplexity).ThenByDescending(o => o.m_dRecommendationStrength).ToList();
            }
            else if (bEnoughWord && bEnoughESentence)
            {
                if ((lastTherapyBlock.m_intLinguisticType >= 2) || (lastTherapyBlock.m_intLinguisticType >= 1)) // last block = easyS / hardS 
                    lsRecCandidateFinal = m_lsCurrentBlock_RecCandidate_Word.OrderByDescending(o => o.m_dCurComplexity).ThenByDescending(o => o.m_dRecommendationStrength).ToList();
                else
                    lsRecCandidateFinal = m_lsCurrentBlock_RecCandidate_ESentence.OrderByDescending(o => o.m_dCurComplexity).ThenByDescending(o => o.m_dRecommendationStrength).ToList();
            }
            else if (bEnoughWord && bEnoughHSentence)
            {
                if ((lastTherapyBlock.m_intLinguisticType >= 2) || (lastTherapyBlock.m_intLinguisticType >= 1)) // last block = easyS / hardS 
                    lsRecCandidateFinal = m_lsCurrentBlock_RecCandidate_Word.OrderByDescending(o => o.m_dCurComplexity).ThenByDescending(o => o.m_dRecommendationStrength).ToList();
                else
                    lsRecCandidateFinal = m_lsCurrentBlock_RecCandidate_HSentence.OrderByDescending(o => o.m_dCurComplexity).ThenByDescending(o => o.m_dRecommendationStrength).ToList();
            }
            else if (bEnoughESentence && bEnoughHSentence)
            {
                if (lastTherapyBlock.m_intLinguisticType >= 2)  // last block = hardS 
                    lsRecCandidateFinal = m_lsCurrentBlock_RecCandidate_ESentence.OrderByDescending(o => o.m_dCurComplexity).ThenByDescending(o => o.m_dRecommendationStrength).ToList();
                else if (lastTherapyBlock.m_intLinguisticType >= 1)  // last block = easyS 
                    lsRecCandidateFinal = m_lsCurrentBlock_RecCandidate_HSentence.OrderByDescending(o => o.m_dCurComplexity).ThenByDescending(o => o.m_dRecommendationStrength).ToList();
                else  // last block = word
                    lsRecCandidateFinal = m_lsCurrentBlock_RecCandidate_ESentence.OrderByDescending(o => o.m_dCurComplexity).ThenByDescending(o => o.m_dRecommendationStrength).ToList();
            }
            else if (bEnoughHSentence) // (m_lsCurrentBlock_RecCandidate_Sentence.Count > CConstants.g_intItemNumPerBlock)
                lsRecCandidateFinal = m_lsCurrentBlock_RecCandidate_HSentence.OrderByDescending(o => o.m_dCurComplexity).ThenByDescending(o => o.m_dRecommendationStrength).ToList();
            else if (bEnoughESentence)
                lsRecCandidateFinal = m_lsCurrentBlock_RecCandidate_ESentence.OrderByDescending(o => o.m_dCurComplexity).ThenByDescending(o => o.m_dRecommendationStrength).ToList();
            else if (bEnoughWord) // (m_lsCurrentBlock_RecCandidate_Word.Count > CConstants.g_intItemNumPerBlock)
                lsRecCandidateFinal = m_lsCurrentBlock_RecCandidate_Word.OrderByDescending(o => o.m_dCurComplexity).ThenByDescending(o => o.m_dRecommendationStrength).ToList();

            //Console.WriteLine("final candidate num = " + lsRecCandidateFinal.Count + " bEnoughWord = " + bEnoughWord + " bEnoughESentence = " + bEnoughESentence + " bEnoughHSentence = " + bEnoughHSentence);
            Debug.Log(String.Format("CRecommender: getNextBlock() final candidate num = {0} bEnoughWord = {1} bEnoughESentence = {2} bEnoughHSentence = {3}", lsRecCandidateFinal.Count, bEnoughWord, bEnoughESentence, bEnoughHSentence));
            
        } // end while loop

        // select top-N
        m_lsCurrentBlock_ChallengeItemFeaturesIdx.Clear();
        m_lsCurrentBlock_IsDiversity.Clear();
        int intDifficultNum = CConstants.g_intDifficultItemNumPerBlock;  // (int)(CConstants.g_intItemNumPerBlock * 0.2);
                                                                         //lsRecCandidateFinal = lsRecCandidateFinal.OrderByDescending(o => o.m_dCurComplexity).ThenByDescending(o => o.m_dRecommendationStrength).ToList();
        lsRecCandidateFinal = lsRecCandidateFinal.OrderByDescending(o => o.m_dRecommendationStrength).ToList();
        for (int i = 0; i < lsRecCandidateFinal.Count; i++)
        {
            if (lsRecCandidateFinal[i].m_dCurComplexity > dLastUserAbility)
            {
                //Console.WriteLine("difficult item = " + lsRecCandidateFinal[i].m_dCurComplexity);
                bool bExist = false;
                int intCandidateChallengeItemIdx = m_lsChallengeItemFeatures[lsRecCandidateFinal[i].m_intChallengeItemFeaturesIdx].m_intChallengeItemIdx;
                // exclude items with same challenge item
                for (int j = 0; j < m_lsCurrentBlock_ChallengeItemFeaturesIdx.Count; j++)
                {
                    int intChallengeItemIdx = m_lsChallengeItemFeatures[m_lsCurrentBlock_ChallengeItemFeaturesIdx[j]].m_intChallengeItemIdx;
                    if (intCandidateChallengeItemIdx == intChallengeItemIdx)
                    {
                        bExist = true;
                        break;
                    }
                }
                if ((lsRecCandidateFinal[i].m_dRecommendationStrength > 0) && (!bExist))
                {
                    m_lsCurrentBlock_ChallengeItemFeaturesIdx.Add(lsRecCandidateFinal[i].m_intChallengeItemFeaturesIdx);
                    m_lsCurrentBlock_IsDiversity.Add(1);
                }
            }

            if (m_lsCurrentBlock_ChallengeItemFeaturesIdx.Count >= intDifficultNum)
                break;
        }
        //lsRecCandidateFinal = lsRecCandidateFinal.OrderByDescending(o => o.m_dRecommendationStrength).ToList();
        for (int i = 0; i < lsRecCandidateFinal.Count; i++)
        {
            //Console.WriteLine("easy item = " + lsRecCandidateFinal[i].m_dCurComplexity);
            if (lsRecCandidateFinal[i].m_dCurComplexity <= dLastUserAbility)
            {
                bool bExist = false;
                int intCandidateChallengeItemIdx = m_lsChallengeItemFeatures[lsRecCandidateFinal[i].m_intChallengeItemFeaturesIdx].m_intChallengeItemIdx;
                // exclude items with same challenge item
                for (int j = 0; j < m_lsCurrentBlock_ChallengeItemFeaturesIdx.Count; j++)
                {
                    int intChallengeItemIdx = m_lsChallengeItemFeatures[m_lsCurrentBlock_ChallengeItemFeaturesIdx[j]].m_intChallengeItemIdx;
                    if (intCandidateChallengeItemIdx == intChallengeItemIdx)
                    {
                        bExist = true;
                        break;
                    }
                }
                if ((lsRecCandidateFinal[i].m_dRecommendationStrength > 0) && (!bExist))
                {
                    m_lsCurrentBlock_ChallengeItemFeaturesIdx.Add(lsRecCandidateFinal[i].m_intChallengeItemFeaturesIdx);
                    m_lsCurrentBlock_IsDiversity.Add(0);
                }
            }
            if (m_lsCurrentBlock_ChallengeItemFeaturesIdx.Count >= CConstants.g_intItemNumPerBlock)
                break;
        }

        // if end of corpus, set next round starting from the median
        if (m_lsCurrentBlock_ChallengeItemFeaturesIdx.Count < CConstants.g_intItemNumPerBlock)
        {
            m_user.setNewRound();
            return getMedianBlock();
        }

        //m_strCurrentBlock_StartTime = System.DateTime.Now.ToString();
        m_dtCurrentBlock_StartTime = System.DateTime.Now;

        // randomise list
        System.Random rnd1 = new System.Random();
        for (int i = 0; i < m_lsCurrentBlock_ChallengeItemFeaturesIdx.Count; i++)
        {
            int temp = m_lsCurrentBlock_ChallengeItemFeaturesIdx[i];
            int intRandomIdx = rnd1.Next(i, m_lsCurrentBlock_ChallengeItemFeaturesIdx.Count);
            m_lsCurrentBlock_ChallengeItemFeaturesIdx[i] = m_lsCurrentBlock_ChallengeItemFeaturesIdx[intRandomIdx];
            m_lsCurrentBlock_ChallengeItemFeaturesIdx[intRandomIdx] = temp;

            int temp2 = m_lsCurrentBlock_IsDiversity[i];
            m_lsCurrentBlock_IsDiversity[i] = m_lsCurrentBlock_IsDiversity[intRandomIdx];
            m_lsCurrentBlock_IsDiversity[intRandomIdx] = temp2;
        }

        return m_lsCurrentBlock_ChallengeItemFeaturesIdx;
    }

    //----------------------------------------------------------------------------------------------------
    // calculateCurComplexity
    //----------------------------------------------------------------------------------------------------
    private double calculateCurComplexity(int intFeaturesIdx)
    {
        double dCurComplexity = 0;

        dCurComplexity = Math.Round(m_lsChallengeItemFeatures[intFeaturesIdx].m_dComplexity_Overall, 4); // Math.Round(dItemCurComplexity * dChallengeItemCurComplexity * dLexicalItemCurComplexity, 4);

        return dCurComplexity;
    }

    //----------------------------------------------------------------------------------------------------
    // checkHasEnoughCandidates
    //----------------------------------------------------------------------------------------------------
    private bool checkHasEnoughCandidates(List<CRecCandidate> lsCandidate, double dLastUserAbility, bool bCheckEasyDifficult)
    {
        List<int> lsChallengeItemIdx = new List<int>();
        List<CRecCandidate> lsCandidateFiltered = new List<CRecCandidate>();

        List<CRecCandidate> lsRecCandidate = lsCandidate.OrderBy(o => o.m_dCurComplexity).ToList();

        // filter items with same challenge item
        for (int i = 0; i < lsRecCandidate.Count; i++)
        {
            int intChallengeItemIdx = m_lsChallengeItemFeatures[lsRecCandidate[i].m_intChallengeItemFeaturesIdx].m_intChallengeItemIdx;
            int intIdx = lsChallengeItemIdx.FindIndex(a => a == intChallengeItemIdx);
            if (intIdx <= -1)
            {
                lsChallengeItemIdx.Add(intChallengeItemIdx);
                lsCandidateFiltered.Add(lsRecCandidate[i]);

                /*int intFeatureIdx = lsCandidateFiltered[lsCandidateFiltered.Count-1].m_intChallengeItemFeaturesIdx;
                double dCurComplexity = calculateCurComplexity(intFeatureIdx);
                if (dCurComplexity <= 0.1)
                    Console.WriteLine("dCurComplexity = " + dCurComplexity + " add intChallengeItemIdx = " + intChallengeItemIdx);*/
            }
            /*else
            {
                int intFeatureIdx = lsCandidateFiltered[lsCandidateFiltered.Count - 1].m_intChallengeItemFeaturesIdx;
                double dCurComplexity = calculateCurComplexity(intFeatureIdx);
                if (dCurComplexity <= 0.1)
                    Console.WriteLine("dCurComplexity = " + dCurComplexity + " exist intChallengeItemIdx = " + intChallengeItemIdx + " lsChallengeItemIdx[intIdx] = " + lsChallengeItemIdx[intIdx]);
            }*/
        }

        bool bEnough = false;
        int intStartIdx = -1;
        for (int i = 0; i < lsCandidateFiltered.Count; i++)
        {
            int intFeatureIdx = lsCandidateFiltered[i].m_intChallengeItemFeaturesIdx;
            double dCurComplexity = calculateCurComplexity(intFeatureIdx);
            if (dCurComplexity > dLastUserAbility)
            {
                intStartIdx = i;
                break;
            }
        }
        int intEasyNum = CConstants.g_intEasyItemNumPerBlock; // (int)(CConstants.g_intItemNumPerBlock * 0.7);
        int intDifficultNum = CConstants.g_intDifficultItemNumPerBlock;  // (int)(CConstants.g_intItemNumPerBlock * 0.3);      
        bool bEnoughEasyNum = false;
        bool bEnoughDifficultNum = false;

        Console.WriteLine("checkHasEnoughCandidates - intEasyNum = " + intEasyNum + " intDifficultNum = " + intDifficultNum + " intStartIdx = " + intStartIdx + " candidateNum = " + lsCandidateFiltered.Count);

        if (bCheckEasyDifficult)
        {
            // check only whether have easy items
            int intCtr = 0;
            for (int i = 0; i < lsCandidateFiltered.Count; i++)
            {
                int intFeatureIdx = lsCandidateFiltered[i].m_intChallengeItemFeaturesIdx;
                double dCurComplexity = calculateCurComplexity(intFeatureIdx);
                if (dCurComplexity <= dLastUserAbility)
                    intCtr++;
                if (intCtr > (intEasyNum + intDifficultNum)) break;
            }
            if ((intCtr > (intEasyNum + intDifficultNum)) && (lsCandidateFiltered.Count >= CConstants.g_intItemNumPerBlock))
                bEnoughEasyNum = true;
            /*if ((intStartIdx > intEasyNum) && (lsCandidateFiltered.Count >= CConstants.g_intItemNumPerBlock))
                bEnoughEasyNum = true;
            if (((lsCandidateFiltered.Count - intStartIdx) >= intDifficultNum) && (lsCandidateFiltered.Count >= CConstants.g_intItemNumPerBlock))
                bEnoughDifficultNum = true;*/
        }
        else
        {
            if ((intStartIdx > intEasyNum) || (lsCandidateFiltered.Count >= CConstants.g_intItemNumPerBlock))
                bEnoughEasyNum = true;
            if (((lsCandidateFiltered.Count - intStartIdx) >= intDifficultNum) || (lsCandidateFiltered.Count >= CConstants.g_intItemNumPerBlock))
                bEnoughDifficultNum = true;
        }

        /*if (intStartIdx > intDifficultNum)
            bEnoughDifficultNum = true;
        if ((lsCandidateFiltered.Count - intStartIdx) >= (intEasyNum) )
            bEnoughEasyNum = true;*/

        if (/*bEnoughDifficultNum &&*/ bEnoughEasyNum) bEnough = true;
        return bEnough;
    }

    //----------------------------------------------------------------------------------------------------
    // calculateFeatureSimilarity
    //----------------------------------------------------------------------------------------------------
    /*public double calculateFeatureSimilarity(CChallengeItemFeatures features1, CChallengeItemFeatures features2)
    {
        double dSimilarity = 0;

        List<double> lsFeatures1 = new List<double>();
        List<double> lsFeatures2 = new List<double>();
        lsFeatures1.Add(features1.m_dComplexity_Frequency);
        lsFeatures1.Add(features1.m_dComplexity_Concreteness);
        lsFeatures1.Add(features1.m_dComplexity_DistractorNum);
        lsFeatures1.Add(features1.m_dComplexity_LinguisticCategory);
        //lsFeatures1.Add(m_lsChallengeItemFeatures[i].m_dComplexity_NoiseLevel);

        lsFeatures2.Add(features2.m_dComplexity_Frequency);
        lsFeatures2.Add(features2.m_dComplexity_Concreteness);
        lsFeatures2.Add(features2.m_dComplexity_DistractorNum);
        lsFeatures2.Add(features2.m_dComplexity_LinguisticCategory);
        //lsFeatures2.Add(m_lsChallengeItemFeatures[j].m_dComplexity_NoiseLevel);

        dSimilarity = Math.Round(CEuclideanDistance.EuclideanSimilarity(lsFeatures1, lsFeatures2), 4);

        return dSimilarity;
    }*/

    //----------------------------------------------------------------------------------------------------
    // printFeatures
    //----------------------------------------------------------------------------------------------------
    public string printFeatures(int intChallengeItemFeaturesIdx)
    {
        string str = "";
        int intChallengeItemIdx = m_lsChallengeItemFeatures[intChallengeItemFeaturesIdx].m_intChallengeItemIdx;
        int intLexicalItemIdx = m_lsChallengeItem[intChallengeItemIdx].m_intLexicalItemIdx;

        double dComplexityDistractorNum = m_lsChallengeItemFeatures[intChallengeItemFeaturesIdx].m_dComplexity_DistractorNum;
        int intNum = 0;
        if (dComplexityDistractorNum == 0.1)
            intNum = 2;
        else if (dComplexityDistractorNum == 0.4)
            intNum = 3;
        else if (dComplexityDistractorNum == 0.7)
            intNum = 4;
        else if (dComplexityDistractorNum == 1)
            intNum = 5;

        str = m_lsChallengeItemFeatures[intChallengeItemFeaturesIdx].m_intFrequency + "-" + m_lsChallengeItemFeatures[intChallengeItemFeaturesIdx].m_intConcreteness +
            "-" + intNum;

        return str;
    }

    //----------------------------------------------------------------------------------------------------
    // updateUserHistory & save to xml files
    //----------------------------------------------------------------------------------------------------
    public void updateUserHistory(List<int> lsResponse, List<float> lsResponseRtSec, double dTherapyBlockIdleTimeSec, double dTotalTherapyTimeMin)
    {
        if (m_lsCurrentBlock_ChallengeItemFeaturesIdx.Count == 0)
            return;

        //double dLinguisticCategory = m_lsChallengeItemFeatures[m_lsCurrentBlock_ChallengeItemFeaturesIdx[0]].m_dComplexity_LinguisticCategory;
        //double dNoiseLevel = m_lsChallengeItemFeatures[m_lsCurrentBlock_ChallengeItemFeaturesIdx[0]].m_dComplexity_NoiseLevel;

        // calculate mean & std deviation for frequency, concreteness, distractorNum
        List<int> lsLexicalItemIdx = new List<int>();
        List<double> lsFrequency = new List<double>();
        List<double> lsConcreteness = new List<double>();
        List<double> lsDistractorNum = new List<double>();
        for (int i = 0; i < m_lsCurrentBlock_ChallengeItemFeaturesIdx.Count; i++)
        {
            int intChallengeItemFeaturesIdx = m_lsCurrentBlock_ChallengeItemFeaturesIdx[i];
            int intChallengeItemIdx = m_lsChallengeItemFeatures[intChallengeItemFeaturesIdx].m_intChallengeItemIdx;
            lsLexicalItemIdx.Add(m_lsChallengeItem[intChallengeItemIdx].m_intLexicalItemIdx);

            lsFrequency.Add((double)m_lsChallengeItemFeatures[intChallengeItemFeaturesIdx].m_intFrequency);
            lsConcreteness.Add((double)m_lsChallengeItemFeatures[intChallengeItemFeaturesIdx].m_intConcreteness);
            lsDistractorNum.Add((double)m_lsChallengeItemFeatures[intChallengeItemFeaturesIdx].m_intDistractorNum);

            /*lsFrequency.Add((double)m_lsChallengeItemFeatures[intChallengeItemFeaturesIdx].m_dComplexity_Frequency);
            lsConcreteness.Add((double)m_lsChallengeItemFeatures[intChallengeItemFeaturesIdx].m_dComplexity_Concreteness);
            lsDistractorNum.Add((double)m_lsChallengeItemFeatures[intChallengeItemFeaturesIdx].m_dComplexity_DistractorNum);*/
        }
        double dStdDeviation_Frequency = Math.Round(calculateStdDeviation(lsFrequency), 4);
        double dStdDeviation_Concreteness = Math.Round(calculateStdDeviation(lsConcreteness), 4);
        double dStdDeviation_DistractorNum = Math.Round(calculateStdDeviation(lsDistractorNum), 4);

        //Updating history
        m_user.updateHistory(m_dtCurrentBlock_StartTime, lsLexicalItemIdx, m_lsCurrentBlock_ChallengeItemFeaturesIdx, m_lsCurrentBlock_IsDiversity, lsResponse, lsResponseRtSec, m_intCurrentBlock_DiversityNum,
                                lsFrequency.Average(), lsConcreteness.Average(), lsDistractorNum.Average(),
                                dStdDeviation_Frequency, dStdDeviation_Concreteness, dStdDeviation_DistractorNum, dTherapyBlockIdleTimeSec, dTotalTherapyTimeMin);

    }

    //----------------------------------------------------------------------------------------------------
    // calculateStdDeviation
    //----------------------------------------------------------------------------------------------------
    private double calculateStdDeviation(IEnumerable<double> values)
    {
        double ret = 0;
        if (values.Count() > 0)
        {
            //Compute the Average      
            double avg = values.Average();
            //Perform the Sum of (value-avg)_2_2      
            double sum = values.Sum(d => Math.Pow(d - avg, 2));
            //Put it all together      
            ret = Math.Sqrt((sum) / (values.Count()));  // Math.Sqrt((sum) / (values.Count() - 1));
        }
        return ret;
    }

    //----------------------------------------------------------------------------------------------------
    // getChallengeItemPresentedCtr
    //----------------------------------------------------------------------------------------------------
    public int getChallengeItemFeaturesPresentedCtr(int intChallengeItemFeaturesIdx)
    {
        return m_user.getChallengeItemFeaturesPresentedCtr(intChallengeItemFeaturesIdx);
    }

    //----------------------------------------------------------------------------------------------------
    // getLastTherapyBlock
    //----------------------------------------------------------------------------------------------------
    public CUser_TherapyBlock getLastTherapyBlock()
    {
        return m_user.getLastTherapyBlock();
    }

    //----------------------------------------------------------------------------------------------------
    // getLastTherapyBlockIdx
    //----------------------------------------------------------------------------------------------------
    public int getLastTherapyBlockIdx()
    {
        return m_user.getLastTherapyBlockIdx();
    }

    //----------------------------------------------------------------------------------------------------
    // convertDistractorNum
    //----------------------------------------------------------------------------------------------------
    private int convertDistractorNum(double dComplexityDistractorNum)
    {
        int intNum = 0;
        if (dComplexityDistractorNum == 0.1)
            intNum = 2;
        else if (dComplexityDistractorNum == 0.4)
            intNum = 3;
        else if (dComplexityDistractorNum == 0.7)
            intNum = 4;
        else if (dComplexityDistractorNum == 1)
            intNum = 5;
        return intNum;
    }

    //----------------------------------------------------------------------------------------------------
    // getTotalTherapyTime
    //----------------------------------------------------------------------------------------------------
    public double getTotalTherapyTimeMin()
    {
        return m_user.getTotalTherapyTimeMin();
    }

    //----------------------------------------------------------------------------------------------------
    // getTodayTherapyTime
    //----------------------------------------------------------------------------------------------------
    public double getTodayTherapyTimeMin()
    {
        return m_user.getTodayTherapyTimeMin();
    }

}
//}

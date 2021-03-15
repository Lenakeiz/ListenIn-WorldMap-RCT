//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;

//using System.Xml;
//using System.Xml.Linq;

//public class XmlSplitterTest : MonoBehaviour {

    
//    public void TestSplitting()
//    {
//        //Yean: can you insert data in the databasexml as well please? (Andrea)
//        for (int i = 0; i < DatabaseXML.MaximumQueriesPerFile; i++)
//        {
//            Dictionary<string, string> dailyTherapy = new Dictionary<string, string>();

//            int pid = DatabaseXML.Instance.PatientId;

//            dailyTherapy.Add("patient", pid.ToString());
//            dailyTherapy.Add("level_start", "level");
//            dailyTherapy.Add("date", "aaaaaaaa");

//            //DatabaseXML.Instance.WriteDatabaseXML(dailyTherapy, DatabaseXML.Instance.therapy_session_update);
//        }
//        //simulate_populateDatabaseXML();

//        // simulated user profile & therapy data
//        //simulate_saveUserProfileToXml();
//        //simulate_saveTherapyBlocksToCsv();
//        //simulate_saveChallengeItemFeaturesHistoryToXml();

//    }

//    /*
//    private void simulate_populateDatabaseXML()
//    */

//    //----------------------------------------------------------------------------------------------------
//    // simulate_saveUserProfileToXml
//    //----------------------------------------------------------------------------------------------------
//    /*
//    public void simulate_saveUserProfileToXml()
//    {
//        string strAppPath = Application.persistentDataPath;
//        string strUserId = DatabaseXML.Instance.PatientId.ToString();

//        // save lsTrial to xml 
//        XmlDocument doc = new XmlDocument();
//        doc.LoadXml("<?xml version='1.0' encoding='utf-8'?>" +
//            "<root>" +
//            "</root>");

//        string strXmlFile = System.IO.Path.Combine(strAppPath, "user_" + strUserId + "_profile.xml");

//        string strXmlFileNew = strXmlFile + ".new";
//        string strXmlFileOld = strXmlFile + ".old";

//        XmlElement xmlChild = doc.CreateElement("userid");
//        xmlChild.InnerText = strUserId;
//        doc.DocumentElement.AppendChild(xmlChild);

//        xmlChild = doc.CreateElement("datasetId");
//        xmlChild.InnerText = "0";  // m_intDatasetId.ToString();
//        doc.DocumentElement.AppendChild(xmlChild);

//        xmlChild = doc.CreateElement("roundIdx");
//        xmlChild.InnerText = "0";  //m_intRoundIdx.ToString();
//        doc.DocumentElement.AppendChild(xmlChild);

//        xmlChild = doc.CreateElement("totalTherapyTimeMin");
//        xmlChild.InnerText = "5000"; // m_dTotalTherapyTimeMin.ToString();
//        doc.DocumentElement.AppendChild(xmlChild);

//        xmlChild = doc.CreateElement("curNoiseLevel");
//        xmlChild.InnerText = "1"; // m_intCurNoiseLevel.ToString();
//        doc.DocumentElement.AppendChild(xmlChild);

//        xmlChild = doc.CreateElement("noiseHistory");
//        for (var i = 0; i < 50 ; i++) //i < m_lsNoiseLevelHistory.Count
//        {
//            XmlElement xmlChild2 = doc.CreateElement("level");
//            xmlChild2.InnerText = "2"; // m_lsNoiseLevelHistory[i].ToString();
//            xmlChild.AppendChild(xmlChild2);
//        }
//        doc.DocumentElement.AppendChild(xmlChild);

//        xmlChild = doc.CreateElement("noiseBlocks");
//        for (var i = 0; i < 50 ; i++) // i < m_lsNoiseBlockIdx.Count
//        {
//            XmlElement xmlChild2 = doc.CreateElement("idx");
//            xmlChild2.InnerText = "101"; // m_lsNoiseBlockIdx[i].ToString();
//            xmlChild.AppendChild(xmlChild2);
//        }
//        doc.DocumentElement.AppendChild(xmlChild);

//        // save forcedBlockHistory
//        xmlChild = doc.CreateElement("forcedBlocks");
//        for (var i = 0; i < 12; i++) // i < m_lsForcedBlockHistory_Weekly.Count
//        {
//            XmlElement xmlChild2 = doc.CreateElement("weekly");

//            // add weekly date
//            XmlElement xmlChild3 = doc.CreateElement("date");
//            xmlChild3.InnerText = "2016-11-03 12:50:22"; // m_lsForcedBlockHistory_Weekly[i].m_strDate;
//            xmlChild2.AppendChild(xmlChild3);

//            // add weekly's daily 
//            for (var j = 0; j < 4 ; j++) //i < m_lsForcedBlockHistory_Weekly[i].m_lsForcedBlockHistory_Daily.Count
//            {
//                xmlChild3 = doc.CreateElement("daily");
//                // daily date
//                XmlElement xmlChild4 = doc.CreateElement("date");
//                xmlChild4.InnerText = "2016-11-03 12:50:22"; //m_lsForcedBlockHistory_Weekly[i].m_lsForcedBlockHistory_Daily[j].m_strDate;
//                xmlChild3.AppendChild(xmlChild4);
//                // daily block idx list
//                for (var k = 0; k < 4 ; k++) //k < m_lsForcedBlockHistory_Weekly[i].m_lsForcedBlockHistory_Daily[j].m_lsBlockIdx.Count
//                {
//                    xmlChild4 = doc.CreateElement("idx");
//                    xmlChild4.InnerText = "16"; //m_lsForcedBlockHistory_Weekly[i].m_lsForcedBlockHistory_Daily[j].m_lsBlockIdx[k].ToString();
//                    xmlChild3.AppendChild(xmlChild4);
//                }
//                xmlChild2.AppendChild(xmlChild3);
//            }

//            xmlChild.AppendChild(xmlChild2);
//        }
//        doc.DocumentElement.AppendChild(xmlChild);
                        
//        {
//            // Write to file.txt.new
//            // Move file.txt to file.txt.old
//            // Move file.txt.new to file.txt
//            // Delete file.txt.old
//            //doc.PreserveWhitespace = true;

//            doc.Save(strXmlFileNew);
//            if (System.IO.File.Exists(strXmlFile))
//                System.IO.File.Move(strXmlFile, strXmlFileOld);
//            System.IO.File.Move(strXmlFileNew, strXmlFile);

//            string strXmlFile_ = System.IO.Path.Combine(strAppPath, "user_" + strUserId + "_profile_.xml");
//            System.IO.File.Copy(strXmlFile, strXmlFile_, true);

//            // backup
//            //string strDate = System.DateTime.Now.ToString("yyyy-MM-dd");
//            //string xml_backup = strAppPath + @"/ListenIn/Therapy/" + "user_" + strUserId + "_profile-" + strDate + ".xml";
//            if (System.IO.File.Exists(strXmlFileOld))
//            {
//                //System.IO.File.Copy(strXmlFileOld, xml_backup, true);
//                System.IO.File.Delete(strXmlFileOld);
//            }
//        }        
//    }
//    */

//    //----------------------------------------------------------------------------------------------------
//    // simulate_saveTherapyBlocksToCsv
//    //----------------------------------------------------------------------------------------------------
//    public void simulate_saveTherapyBlocksToCsv()
//    {
//        string strAppPath = Application.persistentDataPath;
//        string strUserId = DatabaseXML.Instance.PatientId.ToString();

//        string strCsvFile = System.IO.Path.Combine(strAppPath, "user_" + strUserId + "_therapyblocks.csv");

//        string strCsvFileNew = strCsvFile + ".new";
//        string strCsvFileOld = strCsvFile + ".old";

//        using (System.IO.StreamWriter sw = new System.IO.StreamWriter(strCsvFileNew))
//        {
//            for (int i = 0; i < 5000 /*m_lsTherapyBlock.Count*/; i++)
//            {
//                string strRow = "";
//                strRow = strRow + i + ",";

//                strRow = strRow + "2016,12,02,11,12,33" + ",";
//                //strRow = strRow + m_lsTherapyBlock[i].m_dtStartTime.Year + "," + m_lsTherapyBlock[i].m_dtStartTime.Month.ToString("00") + "," +
//                //            m_lsTherapyBlock[i].m_dtStartTime.Day.ToString("00") + "," + m_lsTherapyBlock[i].m_dtStartTime.Hour.ToString("00") + "," +
//                //            m_lsTherapyBlock[i].m_dtStartTime.Minute.ToString("00") + "," + m_lsTherapyBlock[i].m_dtStartTime.Second.ToString("00") + ",";

//                strRow = strRow + "2016,12,02,11,12,33" + ",";
//                //strRow = strRow + m_lsTherapyBlock[i].m_dtEndTime.Year + "," + m_lsTherapyBlock[i].m_dtEndTime.Month.ToString("00") + "," +
//                //            m_lsTherapyBlock[i].m_dtEndTime.Day.ToString("00") + "," + m_lsTherapyBlock[i].m_dtEndTime.Hour.ToString("00") + "," +
//                //            m_lsTherapyBlock[i].m_dtEndTime.Minute.ToString("00") + "," + m_lsTherapyBlock[i].m_dtEndTime.Second.ToString("00") + ",";
                
//                strRow = strRow /*+ m_lsTherapyBlock[i].m_intRoundIdx.ToString()*/ + "0,";
//                strRow = strRow /*+ m_lsTherapyBlock[i].m_intLinguisticType.ToString()*/ + "1,";
//                strRow = strRow /*+ m_lsTherapyBlock[i].m_intNoiseLevel.ToString()*/ + "1,";
//                strRow = strRow /*+ m_lsTherapyBlock[i].m_intBlockType.ToString()*/ + "0,";

//                for (var j = 0; j < 15 /*m_lsTherapyBlock[i].m_lsChallengeItemFeaturesIdx.Count*/; j++)
//                {
//                    strRow = strRow /*+ m_lsTherapyBlock[i].m_lsChallengeItemFeaturesIdx[j].ToString()*/ + "12391,";
//                    strRow = strRow /*+ m_lsTherapyBlock[i].m_lsIsDiversity[j].ToString()*/ + "0,";
//                    strRow = strRow /*+ m_lsTherapyBlock[i].m_lsResponseAccuracy[j].ToString()*/ + "1,";
//                    strRow = strRow /*+ m_lsTherapyBlock[i].m_lsResponseRtSec[j].ToString()*/ + "0.2224,";
//                    strRow = strRow /*+ m_lsTherapyBlock[i].m_lsChallengeItemFeatures_Complexity[j].ToString()*/ + "0.5339,";
//                }

//                strRow = strRow /*+ m_lsTherapyBlock[i].m_dAccuracyRate.ToString()*/ + "0.7333,";
//                strRow = strRow /*+ m_lsTherapyBlock[i].m_dBlockComplexity.ToString()*/ + "0.5093,";
//                strRow = strRow /*+ m_lsTherapyBlock[i].m_dUserAbility_Accumulated.ToString()*/ + "0.5268,";
//                strRow = strRow /*+ m_lsTherapyBlock[i].m_dUserAbility_ThisBlockAvg.ToString()*/ + "0.5106,";
//                strRow = strRow /*+ m_lsTherapyBlock[i].m_dTrainingStep.ToString()*/ + "0.002,";
//                strRow = strRow /*+ m_lsTherapyBlock[i].m_dNextBlock_DiversityThresholdUpper.ToString()*/ + "0.5393,";
//                strRow = strRow /*+ m_lsTherapyBlock[i].m_dNextBlock_DiversityThresholdLower.ToString()*/ + "0.5072,";
//                strRow = strRow /*+ m_lsTherapyBlock[i].m_dMean_Frequency.ToString()*/ + "7.2971,";
//                strRow = strRow /*+ m_lsTherapyBlock[i].m_dStdDeviation_Frequency.ToString()*/ + "0.1137,";
//                strRow = strRow /*+ m_lsTherapyBlock[i].m_dMean_Concreteness.ToString()*/ + "7,";
//                strRow = strRow /*+ m_lsTherapyBlock[i].m_dStdDeviation_Concreteness.ToString()*/ + "0.2035,";
//                strRow = strRow /*+ m_lsTherapyBlock[i].m_dMean_DistractorNum.ToString()*/ + "2,";
//                strRow = strRow /*+ m_lsTherapyBlock[i].m_dStdDeviation_DistractorNum.ToString()*/ + "0.147,";

//                // write to file
//                sw.WriteLine(strRow);
//            }
//        }
        
//        {
//            // Write to file.txt.new
//            // Move file.txt to file.txt.old
//            // Move file.txt.new to file.txt
//            // Delete file.txt.old            

//            if (System.IO.File.Exists(strCsvFile))
//                System.IO.File.Move(strCsvFile, strCsvFileOld);
//            System.IO.File.Move(strCsvFileNew, strCsvFile);

//            string strCsvFile_ = System.IO.Path.Combine(strAppPath, "user_" + strUserId + "_therapyblocks_.csv");
//            System.IO.File.Copy(strCsvFile, strCsvFile_, true);

//            // backup
//            //string strDate = System.DateTime.Now.ToString("yyyy-MM-dd");
//            //string xml_backup = m_strAppPath + @"/ListenIn/Therapy/" + "user_" + m_strUserId + "_therapyblocks-" + strDate + ".csv";
//            if (System.IO.File.Exists(strCsvFileOld))
//            {
//                //System.IO.File.Copy(strCsvFileOld, xml_backup, true);
//                System.IO.File.Delete(strCsvFileOld);
//            }
//        }
        
//    }

//    //----------------------------------------------------------------------------------------------------
//    // simulate_saveChallengeItemFeaturesHistoryToXml
//    //----------------------------------------------------------------------------------------------------
//    public void simulate_saveChallengeItemFeaturesHistoryToXml()
//    {
//        string strAppPath = Application.persistentDataPath;
//        string strUserId = DatabaseXML.Instance.PatientId.ToString();

//        // save lsTrial to xml 
//        XmlDocument doc = new XmlDocument();
//        doc.LoadXml("<?xml version='1.0' encoding='utf-8'?>" +
//            "<root>" +
//            "</root>");

//        // Save the document to a file. White space is preserved (no white space).
//        // Write to file.txt.new
//        // Move file.txt to file.txt.old
//        // Move file.txt.new to file.txt
//        // Delete file.txt.old
//        //string strXmlFile = m_strAppPath + "user_" + m_strUserId + "_challengeitemfeatures_history.xml";
//        string strXmlFile = System.IO.Path.Combine(strAppPath, "user_" + strUserId + "_challengeitemfeatures_history.xml");

//        string strXmlFileNew = strXmlFile + ".new";
//        string strXmlFileOld = strXmlFile + ".old";

//        /*
//        <item idx="0">
//            <challengeItemIdx>0</challengeItemIdx>			  			  	
//            <presentHistory>
//                <history sessionIdx="0" blockIdx="0" acc="0" /history>
//                <history sessionIdx="0" blockIdx="0" acc="0" /history>
//                <history sessionIdx="0" blockIdx="0" acc="0" /history>
//            </presentHistory>          
//        </item> 
//        */

//        for (int i = 0; i < 6000 /*m_lsChallengeItemFeatures_History.Count*/; i++)
//        {
//            XmlElement xmlNode = doc.CreateElement("item");
//            XmlAttribute attr = doc.CreateAttribute("idx");
//            attr.Value = i.ToString();
//            xmlNode.SetAttributeNode(attr);

//            XmlElement xmlChild2 = doc.CreateElement("cifIdx");
//            xmlChild2.InnerText = "7768"; // m_lsChallengeItemFeatures_History[i].m_intChallengeItemFeaturesIdx.ToString();
//            xmlNode.AppendChild(xmlChild2);

//            // add history
//            xmlChild2 = doc.CreateElement("hist");
//            for (var j = 0; j < 5 /*m_lsChallengeItemFeatures_History[i].m_lsPresentHistory.Count*/; j++)
//            {
//                XmlElement xmlChild3 = doc.CreateElement("h");
//                attr = doc.CreateAttribute("rIdx");
//                attr.Value = "0"; // m_lsChallengeItemFeatures_History[i].m_lsPresentHistory[j].m_intRoundIdx.ToString();
//                xmlChild3.SetAttributeNode(attr);

//                attr = doc.CreateAttribute("sIdx");
//                attr.Value = "0"; // m_lsChallengeItemFeatures_History[i].m_lsPresentHistory[j].m_intSessionIdx.ToString();
//                xmlChild3.SetAttributeNode(attr);

//                attr = doc.CreateAttribute("bIdx");
//                attr.Value = "1466"; // m_lsChallengeItemFeatures_History[i].m_lsPresentHistory[j].m_intBlockIdx.ToString();
//                xmlChild3.SetAttributeNode(attr);

//                attr = doc.CreateAttribute("acc");
//                attr.Value = "1"; // m_lsChallengeItemFeatures_History[i].m_lsPresentHistory[j].m_intAccuracy.ToString();
//                xmlChild3.SetAttributeNode(attr);

//                xmlChild2.AppendChild(xmlChild3);
//            }
//            xmlNode.AppendChild(xmlChild2);

//            doc.DocumentElement.AppendChild(xmlNode);
//        }
                
//        {
//            // Write to file.txt.new
//            // Move file.txt to file.txt.old
//            // Move file.txt.new to file.txt
//            // Delete file.txt.old
//            //doc.PreserveWhitespace = true;

//            doc.Save(strXmlFileNew);
//            if (System.IO.File.Exists(strXmlFile))
//                System.IO.File.Move(strXmlFile, strXmlFileOld);
//            System.IO.File.Move(strXmlFileNew, strXmlFile);

//            string strXmlFile_ = System.IO.Path.Combine(strAppPath, "user_" + strUserId + "_challengeitemfeatures_history_.xml");
//            System.IO.File.Copy(strXmlFile, strXmlFile_, true);

//            // backup
//            //string strDate = System.DateTime.Now.ToString("yyyy-MM-dd");
//            //string xml_backup = m_strAppPath + @"/ListenIn/Therapy/" + "user_" + m_strUserId + "_challengeitemfeatures_history-" + strDate + ".xml";
//            if (System.IO.File.Exists(strXmlFileOld))
//            {
//                //System.IO.File.Copy(strXmlFileOld, xml_backup, true);
//                System.IO.File.Delete(strXmlFileOld);
//            }
//        }
        
//    }
//	//----------------------------------------------------------------------------------------------------
//    // SaveTrials_Csv
//    //----------------------------------------------------------------------------------------------------
//    private void SaveTrials_Csv()
//    {
//        string strCsvFile = System.IO.Path.Combine(Application.persistentDataPath, "user_" + DatabaseXML.Instance.PatientId.ToString() + "_therapyblocks_all.csv");

//        /*string strCsvFileNew = strCsvFile + ".new";
//        string strCsvFileOld = strCsvFile + ".old";

//        if (System.IO.File.Exists(strCsvFile))
//            System.IO.File.Copy(strCsvFile, strCsvFileOld, true);*/

//        using (System.IO.StreamWriter sw = new System.IO.StreamWriter(strCsvFile))
//        {
//            for (int k = 0; k < 2000; k++)
//            {
//                string strRow = "";

//                int intBlockIdx = k;
//                strRow = strRow + intBlockIdx + ",";

//                for (int i = 0; i < 15 /*m_lsTrial.Count*/; i++)
//                {
//                    // trial idx
//                    strRow = strRow + i + ",";
//                    strRow = strRow /*+ m_lsTrial[i].m_intChallengeItemIdx.ToString()*/ + "1123,";
//                    strRow = strRow /*+ m_lsTrial[i].m_intChallengeItemFeaturesIdx.ToString()*/ + "1123,";

//                    for (var j = 0; j < 6 /*m_lsTrial[i].m_lsStimulus.Count*/; j++)
//                    {
//                        strRow = strRow + j + ",";
//                        strRow = strRow /*+ m_lsTrial[i].m_lsStimulus[j].intOriginalIdx.ToString()*/ + "0,";
//                        strRow = strRow /*+ m_lsTrial[i].m_lsStimulus[j].m_strType*/ + "target,";
//                        strRow = strRow /*+ m_lsTrial[i].m_lsStimulus[j].m_strPType*/ + "P2,";
//                    }
//                }

//                for (int i = 0; i < 6 /*m_lsResponse.Count*/; i++)
//                {
//                    strRow = strRow + i + ",";
//                    strRow = strRow /*+ m_lsResponse[i].m_intScore.ToString()*/ + "1,";
//                    strRow = strRow /*+ m_lsResponse[i].m_fRTSec.ToString()*/ + "0.6667,";
//                    strRow = strRow /*+ m_lsResponse[i].m_intReplayBtnCtr.ToString()*/ + "0,";

//                    for (var j = 0; j < 6 /*m_lsResponse[i].m_lsSelectedStimulusIdx.Count*/; j++)
//                    {
//                        strRow = strRow /*+ m_lsResponse[i].m_lsSelectedStimulusIdx[j].ToString()*/ + "0,";
//                    }
//                }

//                // write to file
//                sw.WriteLine(strRow);
//            }
//        }

//        /*try
//        {
//            // Write to file.txt.new
//            // Move file.txt to file.txt.old
//            // Move file.txt.new to file.txt
//            // Delete file.txt.old
//            //doc.PreserveWhitespace = true;

//            //if (System.IO.File.Exists(strCsvFile))
//            //    System.IO.File.Move(strCsvFile, strCsvFileOld);
//            //System.IO.File.Move(strCsvFileNew, strCsvFile);

//            string strCsvFile_ = System.IO.Path.Combine(Application.persistentDataPath, "user_" + DatabaseXML.Instance.PatientId.ToString() + "_therapyblocks_all_.csv");
//            System.IO.File.Copy(strCsvFile, strCsvFile_, true);

//            // backup
//            string strDate = System.DateTime.Now.ToString("yyyy-MM-dd");
//            string csv_backup = Application.persistentDataPath + @"/ListenIn/Therapy/" + "user_" + DatabaseXML.Instance.PatientId.ToString() + "_therapyblocks_all-" + strDate + ".csv";
//            if (System.IO.File.Exists(strCsvFileOld))
//            {
//                System.IO.File.Copy(strCsvFileOld, csv_backup, true);
//                System.IO.File.Delete(strCsvFileOld);
//            }
//        }
//        catch (System.Xml.XmlException ex)
//        {
//            //ListenIn.Logger.Instance.Log("CUserTherapy-SaveTrials-" + ex.Message, ListenIn.LoggerMessageType.Info);
//            Debug.Log("CUserTherapy-SaveTrials_Csv-" + ex.Message);
//        }
//        catch (Exception e)
//        {
//            //Console.WriteLine("The process failed: {0}", e.ToString());
//            //ListenIn.Logger.Instance.Log("CUserTherapy-SaveTrials-" + e.ToString(), ListenIn.LoggerMessageType.Info);
//            Debug.Log("CUserTherapy-SaveTrials_Csv-" + e.ToString());
//        }*/
//    }


//}

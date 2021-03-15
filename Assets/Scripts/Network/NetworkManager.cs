using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;
using System.IO;
using System.Linq;
using UnityEngine.Networking;
using System.Security.Cryptography.X509Certificates;
using MadLevelManager;

public class Tuple<T1, T2>
{
    public T1 First { get; private set; }
    public T2 Second { get; private set; }
    internal Tuple(T1 first, T2 second)
    {
        First = first;
        Second = second;
    }
}

public static class Tuple
{
    public static Tuple<T1, T2> New<T1, T2>(T1 first, T2 second)
    {
        var tuple = new Tuple<T1, T2>(first, second);
        return tuple;
    }
}

public class NetworkManager : MonoBehaviour
{
    public static string IdUser;

    #region CSV
    public static char LineSeparatorCSV = '\n';
    private static char fieldSeparatorCSV = ',';
    public static char FileNameSeparator = '/';
    public static string FilePathCSV;
    [SerializeField] private bool isLocal = false;
    #endregion

    #region FILE NAME
    public enum FileType { InsertLevelData, InsertSequenceData };
    public List<FileTypeHolder> ListOfFileNames;
    public static List<FileTypeHolder> ListOfFileNamesStatic;

    [System.Serializable]
    public struct FileTypeHolder
    {
        public FileType FileType;
        public string FileName;
    }

    public static string GetFileName(FileType fileType)
    {
        return ListOfFileNamesStatic.Where(s => s.FileType == fileType).SingleOrDefault().FileName;
    }
    #endregion

    #region DATABASE OBJECTS
    public struct DataObject
    {
        public WWWForm DataForm;
        public string ServerURL;
        public Action<string> callBackMethod;

        public DataObject(WWWForm dataForm, string serverURL, Action<string> callbackMethod)
        {
            DataForm = dataForm;
            ServerURL = serverURL;
            callBackMethod = callbackMethod;
        }
    }

    public struct CSVDataObject
    {
        public string Content;
        public string Header;
        public string FileName;

        public CSVDataObject(string CSVContent, string CSVHeader, string FileName)
        {
            this.Content = CSVContent;
            this.Header = CSVHeader;
            this.FileName = FileName;
        }

        public CSVDataObject(string CSVContent, string FileName)
        {
            this.Content = CSVContent;
            this.Header = null;
            this.FileName = FileName;
        }
    }

    private static List<Tuple<DataObject, CSVDataObject>> DataToSend = new List<Tuple<DataObject, CSVDataObject>>();
    #endregion

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        FilePathCSV = string.Concat(Application.persistentDataPath, FileNameSeparator, "files");
        ListOfFileNamesStatic = ListOfFileNames;
    }

    private void Start()
    {
        StartCoroutine(SendDataToServer());
    }

    IEnumerator SendDataToServer()
    {
        Debug.Log("<color=green><b>SendDataToServer initialized</b></color>");

        for (; ; )
        {
            if (DataToSend.Count != 0)
                yield return new WaitForSeconds(0.1f);
            else
                yield return new WaitForSeconds(1);

            if (DataToSend.Count != 0) //IF THERE IS DATA TO SEND
            {
                Debug.Log(DataToSend[0].First.ServerURL);
                using (UnityWebRequest www = UnityWebRequest.Post(DataToSend[0].First.ServerURL, DataToSend[0].First.DataForm))
                {
                    www.chunkedTransfer = true;
                    www.certificateHandler = new AcceptAllCertificatesSignedWithASpecificPublicKey();

                    yield return www.SendWebRequest();

                    if (www.isNetworkError || www.isHttpError)
                    {
                        Debug.LogError("<color=red>SERVER ERROR</color> Couldn't post the event SendDataServer(): " + www.error);
                    }
                    else if (www.downloadHandler.text == "checksum_error")
                    {
                        Debug.LogError("<color=red>SERVER ERROR</color> checksum failed, trying again... SendDataServer()");
                    }
                    else
                    {
                        Debug.Log("<color=green>SUCCESS: </color>SendDataServer()");
                        if (DataToSend[0].First.callBackMethod != null)
                        {
                            Debug.Log("DataToSend[0].callBackMethod: " + DataToSend[0].First.callBackMethod.Method.Name);
                            DataToSend[0].First.callBackMethod(www.downloadHandler.text);
                        }

                        DataToSend.RemoveAt(0);
                        Debug.Log("QUERY SUCCESSFUL AND REMOVED FROM QUEUE");
                    }
                }
            }
        }
    }

    public static void SendDataServer(WWWForm form, string serverURL, Action<string> callBackMethod = null)
    {
        //add form to the 'queue'
        DataObject dataObject = new DataObject(form, serverURL, callBackMethod);
        CSVDataObject csvDataObject = new CSVDataObject();
        DataToSend.Add(new Tuple<DataObject, CSVDataObject>(dataObject, csvDataObject));
    }

    public static void SendDataServer(WWWForm form, string serverURL, string contentCrc, Action<string> callBackMethod = null)
    {
        byte[] bytes = new byte[contentCrc.Length * sizeof(char)];
        System.Buffer.BlockCopy(contentCrc.ToCharArray(), 0, bytes, 0, bytes.Length);
        uint checksum = crc32(contentCrc);

        form.AddField("checksum", checksum.ToString());

        //add form to the 'queue'
        DataObject dataObject = new DataObject(form, serverURL, callBackMethod);
        CSVDataObject csvDataObject = new CSVDataObject();
        DataToSend.Add(new Tuple<DataObject, CSVDataObject>(dataObject, csvDataObject));
    }

    public static void SendDataServer(WWWForm form, string serverURL, string CSVContent, string CSVHeader, string CSVFileName, Action<string> callBackMethod = null)
    {
        byte[] bytes = new byte[CSVContent.Length * sizeof(char)];
        System.Buffer.BlockCopy(CSVContent.ToCharArray(), 0, bytes, 0, bytes.Length);
        uint checksum = crc32(CSVContent);

        form.AddField("checksum", checksum.ToString());

        //add form to the 'queue'
        DataObject dataObject = new DataObject(form, serverURL, callBackMethod);
        CSVDataObject csvDataObject = new CSVDataObject(CSVContent, CSVHeader, CSVFileName);
        DataToSend.Add(new Tuple<DataObject, CSVDataObject>(dataObject, csvDataObject));
    }

    public static void SendDataServer(WWWForm form, string serverURL, string CSVContent, string CSVFileName, Action<string> callBackMethod = null)
    {
        byte[] bytes = new byte[CSVContent.Length * sizeof(char)];
        System.Buffer.BlockCopy(CSVContent.ToCharArray(), 0, bytes, 0, bytes.Length);
        uint checksum = crc32(CSVContent);

        form.AddField("checksum", checksum.ToString());

        //add form to the 'queue'
        DataObject dataObject = new DataObject(form, serverURL, callBackMethod);
        CSVDataObject csvDataObject = new CSVDataObject(CSVContent, CSVFileName);
        DataToSend.Add(new Tuple<DataObject, CSVDataObject>(dataObject, csvDataObject));
    }

    public void AppendDataCSV(CSVDataObject csvDataObject)
    {
        string fileLocation = string.Concat(FilePathCSV, FileNameSeparator, 1, FileNameSeparator, csvDataObject.FileName);
        Debug.Log(fileLocation);
        if (File.Exists(fileLocation))
        {
            using (StreamWriter therapyFile = File.AppendText(fileLocation))
            {
                therapyFile.Write(csvDataObject.Content);
                therapyFile.Close();
            }
        }
        else
        {
            //IF NOT HERE CREATE
            string pathLocation = string.Concat(FilePathCSV, FileNameSeparator, 1);
            Directory.CreateDirectory(pathLocation);
            using (StreamWriter TherapyFile = File.CreateText(fileLocation))
            {
                TherapyFile.Write(string.Concat(csvDataObject.Header, LineSeparatorCSV, csvDataObject.Content));
                TherapyFile.Close();
            }
        }
    }

    public static void AppendServerDataCSV(string content)
    {
        string[] contentArray = content.Split('#');
        string fileLocation = string.Concat(FilePathCSV, FileNameSeparator, 1, FileNameSeparator, contentArray[0]);
        Debug.Log("<b>APPENDING LOCAL DATA TO:</b> " + contentArray[0]);

        if (File.Exists(fileLocation))
        {
            using (StreamWriter TherapyFile = File.AppendText(fileLocation))
            {
                TherapyFile.Write(contentArray[1]);
                TherapyFile.Close();
            }
        }
        else
        {
            //IF NOT HERE CREATE
            using (StreamWriter TherapyFile = File.CreateText(fileLocation))
            {
                TherapyFile.Write(contentArray[1]);
                TherapyFile.Close();
            }
        }
    }

    public static string GenerateFormContent(params object[] dataContent)
    {
        string content = "";

        foreach (object data in dataContent)
        {
            content = string.Concat(content, data.ToString(), fieldSeparatorCSV);
        }

        content = string.Concat(content, LineSeparatorCSV);
        Debug.Log(content);
        return content;
    }

    #region CRC
    public static uint crc32(string input)
    {
        var table = new uint[]{
            0x00000000, 0x77073096, 0xEE0E612C, 0x990951BA, 0x076DC419, 0x706AF48F,
                0xE963A535, 0x9E6495A3, 0x0EDB8832, 0x79DCB8A4, 0xE0D5E91E, 0x97D2D988,
                0x09B64C2B, 0x7EB17CBD, 0xE7B82D07, 0x90BF1D91, 0x1DB71064, 0x6AB020F2,
                0xF3B97148, 0x84BE41DE, 0x1ADAD47D, 0x6DDDE4EB, 0xF4D4B551, 0x83D385C7,
                0x136C9856, 0x646BA8C0, 0xFD62F97A, 0x8A65C9EC, 0x14015C4F, 0x63066CD9,
                0xFA0F3D63, 0x8D080DF5, 0x3B6E20C8, 0x4C69105E, 0xD56041E4, 0xA2677172,
                0x3C03E4D1, 0x4B04D447, 0xD20D85FD, 0xA50AB56B, 0x35B5A8FA, 0x42B2986C,
                0xDBBBC9D6, 0xACBCF940, 0x32D86CE3, 0x45DF5C75, 0xDCD60DCF, 0xABD13D59,
                0x26D930AC, 0x51DE003A, 0xC8D75180, 0xBFD06116, 0x21B4F4B5, 0x56B3C423,
                0xCFBA9599, 0xB8BDA50F, 0x2802B89E, 0x5F058808, 0xC60CD9B2, 0xB10BE924,
                0x2F6F7C87, 0x58684C11, 0xC1611DAB, 0xB6662D3D, 0x76DC4190, 0x01DB7106,
                0x98D220BC, 0xEFD5102A, 0x71B18589, 0x06B6B51F, 0x9FBFE4A5, 0xE8B8D433,
                0x7807C9A2, 0x0F00F934, 0x9609A88E, 0xE10E9818, 0x7F6A0DBB, 0x086D3D2D,
                0x91646C97, 0xE6635C01, 0x6B6B51F4, 0x1C6C6162, 0x856530D8, 0xF262004E,
                0x6C0695ED, 0x1B01A57B, 0x8208F4C1, 0xF50FC457, 0x65B0D9C6, 0x12B7E950,
                0x8BBEB8EA, 0xFCB9887C, 0x62DD1DDF, 0x15DA2D49, 0x8CD37CF3, 0xFBD44C65,
                0x4DB26158, 0x3AB551CE, 0xA3BC0074, 0xD4BB30E2, 0x4ADFA541, 0x3DD895D7,
                0xA4D1C46D, 0xD3D6F4FB, 0x4369E96A, 0x346ED9FC, 0xAD678846, 0xDA60B8D0,
                0x44042D73, 0x33031DE5, 0xAA0A4C5F, 0xDD0D7CC9, 0x5005713C, 0x270241AA,
                0xBE0B1010, 0xC90C2086, 0x5768B525, 0x206F85B3, 0xB966D409, 0xCE61E49F,
                0x5EDEF90E, 0x29D9C998, 0xB0D09822, 0xC7D7A8B4, 0x59B33D17, 0x2EB40D81,
                0xB7BD5C3B, 0xC0BA6CAD, 0xEDB88320, 0x9ABFB3B6, 0x03B6E20C, 0x74B1D29A,
                0xEAD54739, 0x9DD277AF, 0x04DB2615, 0x73DC1683, 0xE3630B12, 0x94643B84,
                0x0D6D6A3E, 0x7A6A5AA8, 0xE40ECF0B, 0x9309FF9D, 0x0A00AE27, 0x7D079EB1,
                0xF00F9344, 0x8708A3D2, 0x1E01F268, 0x6906C2FE, 0xF762575D, 0x806567CB,
                0x196C3671, 0x6E6B06E7, 0xFED41B76, 0x89D32BE0, 0x10DA7A5A, 0x67DD4ACC,
                0xF9B9DF6F, 0x8EBEEFF9, 0x17B7BE43, 0x60B08ED5, 0xD6D6A3E8, 0xA1D1937E,
                0x38D8C2C4, 0x4FDFF252, 0xD1BB67F1, 0xA6BC5767, 0x3FB506DD, 0x48B2364B,
                0xD80D2BDA, 0xAF0A1B4C, 0x36034AF6, 0x41047A60, 0xDF60EFC3, 0xA867DF55,
                0x316E8EEF, 0x4669BE79, 0xCB61B38C, 0xBC66831A, 0x256FD2A0, 0x5268E236,
                0xCC0C7795, 0xBB0B4703, 0x220216B9, 0x5505262F, 0xC5BA3BBE, 0xB2BD0B28,
                0x2BB45A92, 0x5CB36A04, 0xC2D7FFA7, 0xB5D0CF31, 0x2CD99E8B, 0x5BDEAE1D,
                0x9B64C2B0, 0xEC63F226, 0x756AA39C, 0x026D930A, 0x9C0906A9, 0xEB0E363F,
                0x72076785, 0x05005713, 0x95BF4A82, 0xE2B87A14, 0x7BB12BAE, 0x0CB61B38,
                0x92D28E9B, 0xE5D5BE0D, 0x7CDCEFB7, 0x0BDBDF21, 0x86D3D2D4, 0xF1D4E242,
                0x68DDB3F8, 0x1FDA836E, 0x81BE16CD, 0xF6B9265B, 0x6FB077E1, 0x18B74777,
                0x88085AE6, 0xFF0F6A70, 0x66063BCA, 0x11010B5C, 0x8F659EFF, 0xF862AE69,
                0x616BFFD3, 0x166CCF45, 0xA00AE278, 0xD70DD2EE, 0x4E048354, 0x3903B3C2,
                0xA7672661, 0xD06016F7, 0x4969474D, 0x3E6E77DB, 0xAED16A4A, 0xD9D65ADC,
                0x40DF0B66, 0x37D83BF0, 0xA9BCAE53, 0xDEBB9EC5, 0x47B2CF7F, 0x30B5FFE9,
                0xBDBDF21C, 0xCABAC28A, 0x53B39330, 0x24B4A3A6, 0xBAD03605, 0xCDD70693,
                0x54DE5729, 0x23D967BF, 0xB3667A2E, 0xC4614AB8, 0x5D681B02, 0x2A6F2B94,
                0xB40BBE37, 0xC30C8EA1, 0x5A05DF1B, 0x2D02EF8D
        };

        unchecked
        {
            uint crc = (uint)(((uint)0) ^ (-1));
            var len = input.Length;
            for (var i = 0; i < len; i++)
            {
                crc = (crc >> 8) ^ table[
                    (crc ^ (byte)input[i]) & 0xFF
            ];
            }
            crc = (uint)(crc ^ (-1));

            if (crc < 0)
            {
                crc += (uint)4294967296;
            }

            return crc;
        }
    }
    #endregion
}

public class AcceptAllCertificatesSignedWithASpecificPublicKey : CertificateHandler
{
    // Encoded RSAPublicKey
    private static string PUB_KEY = "3082010A0282010100CAD7471DC02FD00C6578CCA7F9C94B951CE0DFD5B294248F61051E92B0E50D23E92B45BE603895E046C8178F87347F202578B229924FA5E38BF88CC0AE4FB913C87AC329A66A9E5C3E61FFF90176071698F36A5EC9F9A9BA892DC5323748189100062D7B7004071CC1E24A70C5201B73B64E8C8C7FFE01749439302D1F17303FDC2AC0F647E36684A5E4345D57E0CC8762E8B843FAD4D08CBE7D4108B08D23E2D784C749AEAD1E68E47B4726AD8B00E30D1893B78CE36221A9F04BD02A5CE5804A555124CB8671FBC80DA15854F9D8B96234E69E3A5C2CF943B6F6D610E0BBC78FD8F4A915FDF1EDA74389F085C41946DC4A3D11F24BD1BAC16055D4249B68810203010001";

    protected override bool ValidateCertificate(byte[] certificateData)
    {
        X509Certificate2 certificate = new X509Certificate2(certificateData);
        string pk = certificate.GetPublicKeyString();
        if (pk.Equals(PUB_KEY))
        {
            return true;
        }
        return false;
    }
}
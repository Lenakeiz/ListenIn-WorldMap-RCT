using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using MadLevelManager;
using System.Text;
using TMPro;

public enum QuestionType { InputText, Slider};

public class QuestionStructure
{
    public QuestionType qt;
    public string text;
    public Text uiText;
}
public class QuestionaireManager : MonoBehaviour
{

    [SerializeField]
    public GameObject[] questionnaireStructure;

    [SerializeField]
    [Tooltip("Assigned statically")]
    private static int INDEXMIDQUESTIONNAIRE = 5;
    private static int INDEXENDQUESTIONNAIRE = 9;

    [SerializeField]
    private GameObject ButtonsGO;

    [SerializeField]
    private GameObject InputTextGO;

    [SerializeField]
    private TMP_InputField inputField;

    [SerializeField]
    private TextMeshProUGUI inputText;

    [SerializeField]
    private GameObject SliderGO;

    [SerializeField]
    private GameObject sliderObject;

    [SerializeField]
    private Button buttonNext;

    [SerializeField]
    private Button infoButton;

    [SerializeField]
    private GameObject[] faceList;

    private int currQuestionaireCount = 0;
    private int currQuestionairePart = 0;

    private bool endSaving = false;
    private bool isSkipped = false;

    private List<string> responses = new List<string>();
    private string formatResponse = "q{0}:{1}";

    //Saving private response from texts
    private string currResponse = "";
    //Saving private slider input
    private int currSliderValue;
       
    void Start()
    {

        currQuestionairePart = TherapyLIROManager.Instance.GetUserProfile.m_userProfile.m_QuestionaireUserProfile.questionnairStage;
        if (currQuestionairePart == 1)
            currQuestionaireCount = INDEXMIDQUESTIONNAIRE + 1;
        else
            currQuestionaireCount = 0;

        ActivateQuestion(currQuestionaireCount);

    }

    private void ActivateQuestion(int questionNumber)
    {
        foreach (var item in questionnaireStructure)
        {
            item.SetActive(false);
        }

        SliderGO.SetActive(false);
        InputTextGO.SetActive(false);

        buttonNext.interactable = false;
        infoButton.interactable = false;

        RectTransform currTransform = sliderObject.GetComponent<RectTransform>();
        Vector3 position = currTransform.localPosition;
        position.x = 0;
        sliderObject.transform.localPosition = position;

        GameObject currQuestion = questionnaireStructure[currQuestionaireCount];
        if (currQuestion.tag == "QSlider")
        {
            SliderGO.SetActive(true);
            buttonNext.interactable = false;
            infoButton.interactable = true;
        }
        else if (currQuestion.tag == "QInput")
        {
            InputTextGO.SetActive(true);
            buttonNext.interactable = true;
            infoButton.interactable = true;
            inputField.text = string.Empty;
        }
        else
        {
            buttonNext.interactable = true;
            infoButton.interactable = true;
        }

        currQuestion.SetActive(true);

        if (questionNumber == INDEXENDQUESTIONNAIRE)
        {
            StartCoroutine(Save());
            buttonNext.interactable = false;
            TherapyLIROManager.Instance.StartCoroutine(TherapyLIROManager.Instance.SaveSecondHalfQuestionaire(true));

        }
        else if (questionNumber == INDEXMIDQUESTIONNAIRE)
        {
            StartCoroutine(Save());
            TherapyLIROManager.Instance.StartCoroutine(TherapyLIROManager.Instance.SaveHalfQuestionnaire(true));
            currQuestionairePart++;
        }

    }

    private IEnumerator Save()
    {
        SaveQuestionnaire();
        yield return null;
    }

    private void SaveQuestionnaire()
    {
        string directory = GlobalVars.GetPathToLIROOutput(NetworkManager.IdUser);
        string filename = string.Format("Questionaire_Part_{0}_Cycle_{1}.txt", (currQuestionairePart + 1).ToString(), TherapyLIROManager.Instance.GetCurrentTherapyCycle());
        string fullPath = Path.Combine(directory, filename);

        if (responses.Count != 0)
        {
            StringBuilder sb = new StringBuilder();
            using (StreamWriter sw = System.IO.File.CreateText(fullPath))
            {
                foreach (string line in responses)
                {
                    sw.WriteLine(line);
                    sb.AppendLine(line);
                }
                sw.Close();
            }

            //SEND TO SERVER
            WWWForm form = new WWWForm();
            form.AddField("id_user", NetworkManager.IdUser);
            form.AddField("file_name", filename);
            form.AddField("file_size", Encoding.ASCII.GetBytes(sb.ToString()).Length);
            form.AddField("folder_name", GlobalVars.OutputFolderName);
            form.AddBinaryData("file_data", Encoding.ASCII.GetBytes(sb.ToString()), filename);
            NetworkManager.SendDataServer(form, NetworkUrl.ServerUrlUploadFile);

            responses.Clear();
        }


    }

    #region Button Events
    public void SetCurrentFace(int faceIdx)
    {
        buttonNext.interactable = true;

        currSliderValue = faceIdx + 1;
        GameObject currFace = faceList[faceIdx];

        RectTransform currTransform = sliderObject.GetComponent<RectTransform>();
        Vector3 position = currTransform.localPosition;
        position.x = currFace.GetComponent<RectTransform>().localPosition.x;
        sliderObject.transform.localPosition = position;

    }

    public void SaveAnswerAndContinue()
    {
        buttonNext.interactable = false;
        infoButton.interactable = false;

        GameObject currQuestion = questionnaireStructure[currQuestionaireCount];
        if (currQuestion.tag == "QSlider")
        {
            string counter = currQuestionaireCount > INDEXMIDQUESTIONNAIRE ? string.Concat((responses.Count + 1).ToString(), "_carer") : (responses.Count + 1).ToString();
            responses.Add(string.Format(formatResponse, counter, currSliderValue.ToString()));
        }
        else if (currQuestion.tag == "QInput")
        {
            currResponse = inputField.text;
            string counter = currQuestionaireCount > INDEXMIDQUESTIONNAIRE ? string.Concat((responses.Count+1).ToString(),"_carer") : (responses.Count+1).ToString();
            responses.Add(string.Format(formatResponse, counter, currResponse));
        }

        currQuestionaireCount++;

        ActivateQuestion(currQuestionaireCount);

    }

    public void HalfQuestionnaireReturn()
    {
        buttonNext.interactable = false;
        onEndQuestionaireClicked();
    }

    public void SkipToEndOfTheQuestionnaire()
    {
        buttonNext.interactable = false;
        currQuestionaireCount = INDEXENDQUESTIONNAIRE;
        isSkipped = true;
        ActivateQuestion(currQuestionaireCount);
    }

    public void onEndQuestionaireClicked()
    {
        MadLevel.LoadLevelByName("MainHUB");
    }

    #endregion

}

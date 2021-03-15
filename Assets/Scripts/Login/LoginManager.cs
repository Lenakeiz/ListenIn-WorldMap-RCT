using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using MadLevelManager;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System;
using UnityEngine.Networking;

public class LoginManager : MonoBehaviour
{
    public void Register()
    {
        Utility.Instance.SetElementVisibility(AppManager.Instance.LoadingPanel, true);

        WWWForm form = new WWWForm();
        form.AddField("id_user", NetworkManager.IdUser);
        form.AddField("genre", RegistrationController.Instance.RegistrationGenre);
        form.AddField("cause", RegistrationController.Instance.RegistrationCause);
        form.AddField("date_onset", !RegistrationController.Instance.RegistrationHasConcent ? string.Empty : RegistrationController.Instance.RegistrationUnknownDateOfStroke ? "none" : RegistrationController.Instance.MonthOfOnset.options[RegistrationController.Instance.MonthOfOnset.value].text + "/" + RegistrationController.Instance.YearOfOnset.options[RegistrationController.Instance.YearOfOnset.value].text);
        form.AddField("date_birth", !RegistrationController.Instance.RegistrationHasConcent ? string.Empty : RegistrationController.Instance.MonthOfBirth.options[RegistrationController.Instance.MonthOfBirth.value].text + "/" + RegistrationController.Instance.YearOfBirth.options[RegistrationController.Instance.YearOfBirth.value].text);
        form.AddField("concent", RegistrationController.Instance.RegistrationHasConcent.ToString());
        form.AddField("can_contact", RegistrationController.Instance.RegistrationCanContact.ToString());

        NetworkManager.SendDataServer(form, NetworkUrl.UrlSqlRegister, RegisterCallback);
    }

    public void RegisterCallback(string response)
    {
        if (response == "bien")
        {
            MadLevel.LoadLevelByName("Setup Screen");
        }
        else
        {
            Utility.Instance.SetElementVisibility(AppManager.Instance.LoadingPanel, false);
        }
    }
}

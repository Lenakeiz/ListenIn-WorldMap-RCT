using UnityEngine;
using System.Collections;
using MadLevelManager;

public class PlayerPrefManager : MonoBehaviour
{
    public delegate void OnLogOut();
    public static event OnLogOut OnLogOutEvent;

    private const string idUserPlayerPref = "IdUser";
    private const string emailPlayerPref = "Email";

    public static bool IsLogged()
    {
        return PlayerPrefs.HasKey(idUserPlayerPref);
    }

    public static string GetIdUser()
    {
        return PlayerPrefs.GetString(idUserPlayerPref);
    }

    public static string GetEmail()
    {
        return PlayerPrefs.GetString(emailPlayerPref);
    }

    public static void SetPlayerPrefData(string email, string idUser)
    {
        PlayerPrefs.SetString(idUserPlayerPref, idUser);
        PlayerPrefs.SetString(emailPlayerPref, email);
        PlayerPrefs.Save();
    }

    public static void LogOut()
    {
        PlayerPrefs.SetString(idUserPlayerPref, string.Empty);
        PlayerPrefs.SetString(emailPlayerPref, string.Empty);
        PlayerPrefs.Save();
         
        if(OnLogOutEvent != null)
        {
            OnLogOutEvent();
        }

        MadLevel.LoadLevelByName("Login");
    }

    public static string GetHiddenEmail()
    {
        string email = GetEmail();
        char[] firstPart = email.Split('@')[0].ToCharArray();
        string secondPart = email.Split('@')[1];
        char[] emailPart = secondPart.Split('.')[0].ToCharArray();

        return string.Concat(HideString(firstPart), "@", HideString(emailPart), ".", secondPart.Split('.')[1]);
    }

    private static string HideString(char[] word)
    {
        if (word.Length > 5)
        {
            for (int i = 3; i < word.Length; i++)
            {
                word[i] = '*';
            }
        }
        else
        {
            for (int i = 2; i < word.Length; i++)
            {
                word[i] = '*';
            }
        }
        return new string(word);
    }
}

using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class LoginButtonTransitionGenre : LoginButtonTransition
{
    [SerializeField] private string genre;

    public override void OnPointerClick(PointerEventData eventData)
    {
        //RegistrationController.Instance.CurrentRegistrationStep = changeTo;
        RegistrationController.Instance.RegistrationGenre = genre;
    }
}

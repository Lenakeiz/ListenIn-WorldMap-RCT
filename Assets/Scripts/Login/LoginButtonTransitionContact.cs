using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class LoginButtonTransitionContact : LoginButtonTransition
{
    [SerializeField] private bool contact;

    public override void OnPointerClick(PointerEventData eventData)
    {
        RegistrationController.Instance.CurrentRegistrationStep = changeTo;
        RegistrationController.Instance.RegistrationCanContact = contact;
    }
}
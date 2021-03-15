using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class LoginButtonTransitionTime : LoginButtonTransition
{
    [SerializeField] private bool uknownStroke = false;

    public override void OnPointerClick(PointerEventData eventData)
    {
        RegistrationController.Instance.CurrentRegistrationStep = changeTo;
        RegistrationController.Instance.RegistrationUnknownDateOfStroke = uknownStroke;
    }
}

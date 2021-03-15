using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class LoginButtonTransitionConcent : LoginButtonTransition
{
    [SerializeField] private bool concent;

    public override void OnPointerClick(PointerEventData eventData)
    {
        RegistrationController.Instance.CurrentRegistrationStep = changeTo;
        RegistrationController.Instance.RegistrationHasConcent = concent;
    }
}
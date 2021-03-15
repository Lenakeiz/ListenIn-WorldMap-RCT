using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class LoginButtonTransitionCause : LoginButtonTransition
{
    [SerializeField] private string cause;

    public override void OnPointerClick(PointerEventData eventData)
    {
        //RegistrationController.Instance.CurrentRegistrationStep = changeTo;
        RegistrationController.Instance.RegistrationCause = cause;
    }
}

using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class LoginButtonTransition : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] protected RegistrationStep changeTo;

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        RegistrationController.Instance.CurrentRegistrationStep = changeTo;
    }
}

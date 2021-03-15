using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;



[RequireComponent(typeof(TMP_Text))]
public class LinkOpener : MonoBehaviour, IPointerClickHandler
{
    [Header("Assigned in inspector")]
    public CreditsController cc;
    public void OnPointerClick(PointerEventData eventData)
    {
        cc.StopScrolling(false);
        TMP_Text pTextMeshPro = GetComponent<TMP_Text>();
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(pTextMeshPro, eventData.position, Camera.main);  // If you are not in a Canvas using Screen Overlay, put your camera instead of null
        if (linkIndex != -1)
        { // was a link clicked?
            TMP_LinkInfo linkInfo = pTextMeshPro.textInfo.linkInfo[linkIndex];
            Application.OpenURL(linkInfo.GetLinkID());
        }
    }

}

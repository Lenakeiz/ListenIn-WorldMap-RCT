using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnMouseDownEvent : MonoBehaviour
{
    public UnityEvent OnMouseDownUnityEvent;
    public Sprite[] buttons;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnMouseDown()
    {
        if(buttons.Length > 1)
            spriteRenderer.sprite = buttons[1];
        OnMouseDownUnityEvent.Invoke();

    }

    void OnMouseUp()
    {
        if (buttons.Length > 1)
            spriteRenderer.sprite = buttons[0];
    }
}

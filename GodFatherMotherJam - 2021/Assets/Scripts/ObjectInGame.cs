using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectInGame : MonoBehaviour
{
    public int id; //pass to private
    private SpriteRenderer sr;

    public delegate void OnClickEvent();
    public OnClickEvent OnClick;

    public void SetupObject(Sprite tempSprite, int tempId)
    {
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = tempSprite;
        id = tempId;
        ActivateObject();
    }

    private void ActivateObject()
    {
        gameObject.SetActive(true);
    }

    private void OnMouseDown()
    {
        if (!GameManager.Instance.GetIsGamePlaying() || GameManager.Instance.GetDisableObjectInteraction()) return;

        CheckObject();
    }

    private void CheckObject()
    {
        GameManager.Instance.CheckObject(id);
        OnClick();
    }
}

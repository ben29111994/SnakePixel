using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnlockIngredient : MonoBehaviour
{
    public Image iconImg;
    public UI currentUI;

    public void OnAwake(Sprite _icon,UI _currentUI)
    {
        currentUI = _currentUI;
        iconImg.sprite = _icon;
        gameObject.SetActive(true);
    }

    public void Onclick_OK()
    {
        currentUI.MoveToTargetPage();
        gameObject.SetActive(false);
    }
}

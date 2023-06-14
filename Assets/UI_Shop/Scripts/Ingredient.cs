using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ingredient : MonoBehaviour
{
    public int ID;
    public bool isBuy;

    [Header("References")]
    public UI currentUI;
    public Button btnSelect;
    public GameObject lockObject;
    public GameObject unlockObject;
    public GameObject selectObject;
    public Image[] iconImg;
    public Animator outlineAnimator;

    public void Init(int _id,Sprite _icon,UI _currentUI)
    {
        currentUI = _currentUI;
        ID = _id;

        for(int i = 0; i < iconImg.Length; i++)
        {
            iconImg[i].sprite = _icon;
        }

        btnSelect.onClick.AddListener(() => OnClick_Select(ID));
        
        if(lockObject.transform.childCount >= 2)
        {
            Text levelText = lockObject.transform.GetChild(1).GetComponent<Text>();
            levelText.text = "LEVEL\n" + (_id * 5);
        }
    }

    public void UpdateIngredient()
    {
        int _number = PlayerPrefs.GetInt(currentUI.nameItem + ID);

        if(_number == 0)
        {
            isBuy = false;
            lockObject.SetActive(true);
            unlockObject.SetActive(false);
        }
        else
        {
            if(currentUI.justBuy)
            {
                isBuy = true;
                lockObject.SetActive(true);
                unlockObject.SetActive(false);
            }
            else
            {
                isBuy = true;
                lockObject.SetActive(false);
                unlockObject.SetActive(true);
            }
        }
    }

    public void ShowAnimationOutline(float time)
    {
        StartCoroutine(C_ShowAnimationOutline(time));
    }

    private IEnumerator C_ShowAnimationOutline(float time)
    {
        yield return new WaitForSeconds(time);

        outlineAnimator.SetTrigger("Active");
        lockObject.SetActive(false);
        unlockObject.SetActive(true);

        currentUI.justBuy = false;
    }


    public void OnClick_Select(int number)
    {
        currentUI.SelectSkinPlayer(number);
    }

    public void UpdateSelect(int number)
    {
        if (number != ID)
        {
            if (isBuy)
            {
                unlockObject.SetActive(true);
            }
            else
            {
                lockObject.SetActive(true);
            }

            selectObject.SetActive(false);
            return;
        }

        lockObject.SetActive(false);
        unlockObject.SetActive(false);
        selectObject.SetActive(true);
        currentUI.CurrentSkinPlayer = number;
    }
}

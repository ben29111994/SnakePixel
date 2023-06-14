using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DanielLochner.Assets.SimpleScrollSnap;

public class UI : MonoBehaviour
{
    public string nameItem;

    public List<Ingredient> listItem = new List<Ingredient>();

    public int LastIngredient
    {
        get
        {
            return PlayerPrefs.GetInt("last_" + nameItem);
        }
        set
        {
            PlayerPrefs.SetInt("last_" + nameItem, value);
        }
    }

    [Header("Input Data")]
    public List<Sprite> listData = new List<Sprite>();


    [Header("References")]
    public SimpleScrollSnap simpleScrollSnap_Ingredient;
    public UnlockIngredient unlockIngredientScript;
    public Animator btnBuyCoinAnimator;
    public bool justBuy;

    public GameObject ingredientTab;
    public Ingredient ingredientPrefab;
    public GameObject pageIngredientPrefab;
    public Transform pageIngredientParent;

    public GameObject dotPrefab;
    public Transform paginationIngredientParent;

    public int CurrentSkinPlayer
    {
        get
        {
            return PlayerPrefs.GetInt("Current_" + nameItem);
        }
        set
        {
            PlayerPrefs.SetInt("Current_" + nameItem, value);
        }
    }

    public void SetIngredient(int number)
    {
        PlayerPrefs.SetInt(nameItem + number,1);
    }

    public int GetIngredientNumber(int number)
    {
        return PlayerPrefs.GetInt(nameItem + number);
    }

    private void Awake()
    {
        if(PlayerPrefs.GetInt("FirstInit_" + nameItem) == 0)
        {
            PlayerPrefs.GetInt("FirstInit_" + nameItem, 1);
            LastIngredient = -1;
        }

        GenerateIngredientFromData();
        UnlockIngredientBase();
        UpdateShop();
    }

    public void OnStart()
    {
        SelectSkinPlayer(CurrentSkinPlayer);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            PlayerPrefs.DeleteAll();
            Debug.Log("PlayerPrefs.DeleteAll();");
        }
    }

    private void GenerateIngredientFromData()
    {
        GameObject page = null;

        int indexPage = 0;
        int maxPage = (int)(listData.Count / 6);

        for(int i = 0; i < listData.Count; i++)
        {
            if(i % 6 == 0)
            {
                indexPage++;
                page = Instantiate(pageIngredientPrefab, pageIngredientParent);
                Instantiate(dotPrefab, paginationIngredientParent);
            }

            Ingredient _ingredient = Instantiate(ingredientPrefab, page.transform.GetChild(0));
            int ID = i;
            Sprite icon = listData[i];
            string name = listData[i].name;
            bool isChest = (indexPage < maxPage - 1) ? false : true;
            _ingredient.Init(ID, icon,this);
            listItem.Add(_ingredient);
        }
    }

    private void UnlockIngredientBase()
    {
        //if (nameItem == "skill")
        //{
        //    for (int i = 0; i < 6; i++)
        //    {
        //        SetIngredient(i);
        //    }
        //    return;
        //}

        for (int i = 0; i < 1; i++)
        {
            SetIngredient(i);
        }
    }

    public void UpdateShop()
    {
        int count = listItem.Count;

        for(int i = 0; i < count; i++)
        {
            listItem[i].UpdateIngredient();
        }
    }

    public void OnClick_UnlockCoin()
    {
        btnBuyCoinAnimator.SetTrigger("Bubble");

        // check xem da unlock het chua , neu unlock het roi return
        if (CanUnlock() == false) return;

        // kiem tra coin , neu ko du coin return
        int price = 300;
        int myCoin = DataManager.Instance.Coin;

        if (myCoin < price) return;

        Debug.Log("unlock random ingredient _ COIN");
        // myCoin -= price;

        DataManager.Instance.Coin -= price;
        UnlockIngredient();
    }

    private bool CanUnlock()
    {
        int n = 0;

        for (int i = 0; i < listData.Count; i++)
        {
            if (GetIngredientNumber(i) == 1)
            {
                n++;
            }
        }

        if (n == listData.Count)
        {
            Debug.Log("ban da unlock het ingredient");
            return false;
        }

        return true;
    }

    private void UnlockIngredient()
    {
        justBuy = true;

        // random value
        int randomValue = 0;
        int numberLoop = 0;

        for (int i = 0; i < 1; i++)
        {
            randomValue = Random.Range(0, listData.Count);

            if (GetIngredientNumber(randomValue) == 1)
            {
                randomValue = Random.Range(0, listData.Count);
                i--;

                Debug.Log(numberLoop);
                if (numberLoop >= 200) return;
            }
        }

        Debug.Log("ingredient  " + randomValue + "  UNLOCKED");
        
        LastIngredient = randomValue;

        // unlock
        SetIngredient(randomValue);
        listItem[randomValue].UpdateIngredient();

        // chay animation
        unlockIngredientScript.OnAwake(listItem[randomValue].iconImg[0].sprite,this);

        // set value when onlick OK will be call move to target page
        currentPage = simpleScrollSnap_Ingredient.TargetPanel;
        targetPage = (int)(randomValue / 6);
        step = Mathf.Abs(currentPage - targetPage);
        isNext = (targetPage - currentPage > 0) ? true : false;

        CurrentSkinPlayer = randomValue;
        SelectSkinPlayer(CurrentSkinPlayer);
    }

    private int currentPage;
    private int targetPage;
    private int step;
    private bool isNext;

    public void MoveToTargetPage()
    {
        StartCoroutine(C_MovePageToTarget(step, isNext));
    }

    private IEnumerator C_MovePageToTarget(int step,bool isNext)
    {
        if (step == 0)
        {
            listItem[LastIngredient].ShowAnimationOutline(0.1f);
            yield break;
        }

        if (isNext)
        {
            for (int i = 0; i < step; i++)
            {
                simpleScrollSnap_Ingredient.GoToNextPanel();
                yield return new WaitForSeconds(0.1f);
            }
        }
        else
        {
            for (int i = 0; i < step; i++)
            {
                simpleScrollSnap_Ingredient.GoToPreviousPanel();
                yield return new WaitForSeconds(0.1f);
            }
        }

        listItem[LastIngredient].ShowAnimationOutline(0.2f);
    }

    public void SelectSkinPlayer(int number)
    {
        for (int i = 0; i < listItem.Count; i++)
        {
            listItem[i].UpdateSelect(number);
        }

        if (nameItem == "player")
        {
            Debug.Log("set skin " + number);
            //GameManager.Instance.player.SetSkin(number);
        }
    }
}

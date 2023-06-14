using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    [Header("References")]
    public GameObject MainMenuUI;
    public GameObject InGameUI;
    public GameObject CompleteUI;
    public GameObject FailUI;
    public GameObject ShopUI;

    public void Show_MainMenuUI()
    {
        MainMenuUI.SetActive(true);
        InGameUI.SetActive(false);
        CompleteUI.SetActive(false);
        FailUI.SetActive(false);
    }

    public void Show_InGameUI()
    {
        MainMenuUI.SetActive(false);
        InGameUI.SetActive(true);
        CompleteUI.SetActive(false);
        FailUI.SetActive(false);
    }

    public void Show_CompleteUI()
    {
        MainMenuUI.SetActive(false);
        CompleteUI.SetActive(true);
        FailUI.SetActive(false);
    }

    public void Show_FailUI()
    {
        MainMenuUI.SetActive(false);
        InGameUI.SetActive(false);
        CompleteUI.SetActive(false);
        FailUI.SetActive(true);
    }

    public void OnClick_LoadScene()
    {
        SceneManager.LoadScene(0);
    }

    public void OnClick_LoadScenePlayGame()
    {
        OnClick_LoadScene();
        Show_InGameUI();
    }

    public void OnClick_Next()
    {
        GameManager.Instance.LevelUp();
        OnClick_LoadScene();
    }

    public void OnClick_Previous()
    {
        GameManager.Instance.LevelDown();
        OnClick_LoadScene();
    }

    public void OnClick_OpenShop()
    {
        GameManager.Instance.isShopping = true;
        ShopUI.SetActive(true);
    }

    public void OnClick_CloseShop()
    {
        GameManager.Instance.isShopping = false;
        ShopUI.SetActive(false);
    }
}
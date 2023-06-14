using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public int LevelFixed
    {
        get
        {
            return PlayerPrefs.GetInt("LevelFixed");
        }
        set
        {
            PlayerPrefs.SetInt("LevelFixed", value);
        }
    }

    public int Level
    {
        get
        {
            return PlayerPrefs.GetInt("Level");
        }
        set
        {
            PlayerPrefs.SetInt("Level", value);
        }
    }

    public int Task
    {
        get
        {
            return PlayerPrefs.GetInt("Task");
        }
        set
        {
            PlayerPrefs.SetInt("Task", value);
        }
    }

    [Header("Coin")]
    public Text coinText_1;
    public Text coinText_2;

    public int Coin
    {
        get
        {
            return PlayerPrefs.GetInt("Coin");
        }
        set
        {
            PlayerPrefs.SetInt("Coin", value);
            coinText_1.text = coinText_2.text = value + "";
        }
    }

    private void Start()
    {
        Coin += 0;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            PlayerPrefs.DeleteAll();
            UIManager.Instance.OnClick_LoadScene();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            Coin += 9999;
        }
    }
}

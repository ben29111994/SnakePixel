using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.NiceVibrations;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Status Game")]
    public bool isStartGame;
    public bool isComplete;
    public bool isVibration;
    public bool isVibration2;
    public bool isShopping;
    public bool isShakeCamera;
    public int numberCount;
    private float timeDelayNumber;

    [Header("Level Manager")]
    public int taskGame;
    public int levelGame;
    public int levelFixed;
    public int levelGame2;
    public int levelPicture;

    [Header("Public References")]
    public GameObject mapObject;
    public Renderer ground;
    public UI UIShop;
    public Player player;
    public GenerateMap generateMap;
    public Transform offsetCamera;
    public List<Shape> listShape = new List<Shape>();

    [Header("UI References")]
    public GameObject[] taskArray;
    public Text currentLevelText;
    public Text nextLevelText;

    public Material[] m_ground;


    private void Awake()
    {
        Application.targetFrameRate = 60;
        MMVibrationManager.iOSInitializeHaptics();
        Instance = this;
    }
    
    private void Start()
    {
        levelGame2 = PlayerPrefs.GetInt("levelGame2");
        levelGame = DataManager.Instance.Level;
        levelFixed = DataManager.Instance.LevelFixed;
        taskGame = DataManager.Instance.Task;

        currentLevelText.text = (levelGame + 1).ToString();
        nextLevelText.text = (levelGame + 2).ToString();
        taskArray[taskGame].SetActive(true);

        int n = PlayerPrefs.GetInt("ground");
        ground.material = m_ground[n];
        n++;
        if (n >= 3) n = 0;
        PlayerPrefs.SetInt("ground", n);


        GenerateMap();
        UIShop.OnStart();
        StartCoroutine(C_StartEvent());
    }

    private void Update()
    {
        NumberUpdate();
    }

    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.U)) Complete();
    }

    private void GenerateMap()
    {
        int lvl = levelGame2;

        string name = "Map " + lvl;
        int _n = 0;
        int _d = lvl;

        GameObject _m = Resources.Load("Map/" + name) as GameObject;
        if(_m == null)
        {

            while (true)
            {
                GameObject g = Resources.Load("Map/Map " + _n) as GameObject;
                if (g == null) break;
                _n++;

                if(_n == 1000)
                {
                    Debug.LogError("LOAD MAP ERROR");
                    break;
                }
            }

            _d = Random.Range(0, _n);
            name = "Map " + _d;
            _m = Resources.Load("Map/" + name) as GameObject;
        }

        levelPicture = _d;
        generateMap.GeneratePicture(levelPicture);

        mapObject = Instantiate(_m);
        mapObject.transform.position = generateMap.pictureObject.transform.position;

 

    }

    private IEnumerator C_StartEvent()
    {
        yield return new WaitForSeconds(1.0f);
    }

    private IEnumerator C_EndEvent()
    {
        yield return new WaitForSeconds(1.0f);
    }

    public void StartGame()
    {
        if (isStartGame) return;

        isStartGame = true;
        UIManager.Instance.Show_InGameUI();
    }

    public void Swipe(Vector3 direction)
    {
        if (isShopping) return;
        StartGame();
        player.MoveControl(direction);
    }

    public void UpdateShape(Shape _shape)
    {
        listShape.Remove(_shape);
        if(listShape.Count == 0)
        {
            Complete();
        }
    }

    public void LevelDown()
    {
        if(levelGame == 0 && taskGame == 0)
        {
            return;
        }

        if (levelGame2 > 0) levelGame2--;
        PlayerPrefs.SetInt("levelGame2", levelGame2);

        taskGame--;
        levelFixed--;
        if (levelFixed <= 0) levelFixed = 0;
        DataManager.Instance.LevelFixed = levelFixed;

        if (taskGame < 0)
        {
            taskGame = 2;
            levelGame--;
            DataManager.Instance.Level = levelGame;
        }
        DataManager.Instance.Task = taskGame;
    }

    public void LevelUp()
    {
        levelGame2++;
        PlayerPrefs.SetInt("levelGame2", levelGame2);

        taskGame++;
        levelFixed++;
        if (levelFixed >= generateMap.mapTexture2D.Length)
        {
            levelFixed = Random.Range(0, generateMap.mapTexture2D.Length);
        }
        DataManager.Instance.LevelFixed = levelFixed;

        if (taskGame >= 3)
        {
            taskGame = 0;
            levelGame++;
            DataManager.Instance.Level = levelGame;
        }
        DataManager.Instance.Task = taskGame;
        StartCoroutine(C_EndEvent());
    }

    public void Complete()
    {
        if (isComplete) return;

        isComplete = true;
        StartCoroutine(C_Complete());
    }

    private IEnumerator C_Complete()
    {
        LevelUp();
        DataManager.Instance.Coin += Random.Range(20, 40);
        player.isDone = true;

        ActivePicture();
        yield return new WaitForSeconds(4.0f);
        UIManager.Instance.Show_CompleteUI();
    }

    public void Fail()
    {
        if (isComplete) return;

        isComplete = true;
        StartCoroutine(C_Fail());
    }

    private IEnumerator C_Fail()
    {
        yield return new WaitForSeconds(0.6f);
        UIManager.Instance.Show_FailUI();
    }

    public void SetCamera(Vector3 _position)
    {
        offsetCamera.transform.position = _position;
    }

    public void NumberUpdate()
    {
        if(timeDelayNumber > 0.0f)
        {
            timeDelayNumber -= Time.deltaTime;
            return;
        }

        if(numberCount > 0)
        {
            timeDelayNumber = 0.02f;
            numberCount--;
            NumberEffect();
        }
    }

    public void NumberEffect()
    {
        if (player.snakeManager.balls.Count == 0) return;
        GameObject _rt = PoolManager.Instance.GetObject(PoolManager.NameObject.number);
        Vector3 pos = _rt.transform.position;
        pos.y += Random.Range(-2.0f, 2.0f);
        pos.x += Random.Range(-3.0f, 3.0f);
        _rt.transform.position =  pos;
        StartCoroutine(C_ActiveObject(_rt.gameObject, 1.0f));
    }

    public void NumberTruEffect()
    {
        if (player.snakeManager.balls.Count == 0) return;
        GameObject _rt = PoolManager.Instance.GetObject(PoolManager.NameObject.numberTru);
        Vector3 pos = _rt.transform.position;
        pos.y += Random.Range(-2.0f, 2.0f);
        pos.x += Random.Range(-3.0f, 3.0f);
        _rt.transform.position = pos;
        StartCoroutine(C_ActiveObject(_rt.gameObject, 1.0f));
    }

    public void CubeEffect(Vector3 _pos,Vector3 _scale,Material _m)
    {
        GameObject _cubeEffect = PoolManager.Instance.GetObject(PoolManager.NameObject.cubeEffect);
        _cubeEffect.transform.position = _pos;
        _cubeEffect.transform.localScale = _scale;

        ParticleSystemRenderer _par = _cubeEffect.GetComponent<ParticleSystemRenderer>();
        _par.material = _m;

        StartCoroutine(C_ActiveObject(_cubeEffect, 2.0f));
    }

    public void PlayerExplosionEffect(Vector3 _pos, Material _m)
    {
        GameObject _cubeEffect = PoolManager.Instance.GetObject(PoolManager.NameObject.playerExplosion);
        _cubeEffect.transform.position = _pos;

        ParticleSystemRenderer _par = _cubeEffect.GetComponent<ParticleSystemRenderer>();
        _par.material = _m;

        StartCoroutine(C_ActiveObject(_cubeEffect, 2.0f));
    }

    private IEnumerator C_ActiveObject(GameObject _obj,float _time)
    {
        _obj.SetActive(true);
        yield return new WaitForSeconds(_time);
        _obj.SetActive(false);
    }

    public void Vibration()
    {
        if (isVibration) return;

        StartCoroutine(C_Vibration());
    }

    private IEnumerator C_Vibration()
    {
        MMVibrationManager.Haptic(HapticTypes.MediumImpact);
        isVibration = true;
        yield return new WaitForSeconds(0.2f);
        isVibration = false;
    }

    public void Vibration2()
    {
        if (isVibration2) return;

        StartCoroutine(C_Vibration2());
    }

    private IEnumerator C_Vibration2()
    {
        MMVibrationManager.Haptic(HapticTypes.LightImpact);
        isVibration2 = true;
        yield return new WaitForSeconds(0.2f);
        isVibration2 = false;
    }

    public void VibrationWave()
    {
        StartCoroutine(C_VibrationWave());
    }

    private IEnumerator C_VibrationWave()
    {
        for(int i = 0;i< 8; i++)
        {
            MMVibrationManager.Haptic(HapticTypes.HeavyImpact);
            yield return new WaitForSeconds(0.2f);

        }
    }
    [NaughtyAttributes.Button]
    public void ShakeCamera()
    {
        if (isShakeCamera) return;

        StartCoroutine(C_ShakeCamera());
    }

    private IEnumerator C_ShakeCamera()
    {
        //offsetCamera.DOShakePosition(.6f, .30f, 30);
        isShakeCamera = true;
        yield return new WaitForSeconds(0.5f);
        isShakeCamera = false;
    }

    public void ActivePicture()
    {
        StartCoroutine(C_ActivePicture());
    }

    private IEnumerator C_ActivePicture()
    {
        Vector3 pos = mapObject.transform.position;
        pos.y = -10.0f;
        //mapObject.transform.DOMove(pos,1.0f).SetEase(Ease.InOutSine);
        SimpleTween.Instance.Tween(mapObject.transform, mapObject.transform.position, pos, 1);
        yield return new WaitForSeconds(0.2f);
        generateMap.pictureObject.SetActive(true);
        Vector3 target = generateMap.pictureObject.transform.position;
        target.y = -12.0f;
        generateMap.pictureObject.transform.position = target;
        target.y = 0.0f;
        //generateMap.pictureObject.transform.DOMove(target, 1.2f).SetEase(Ease.InOutSine);
        SimpleTween.Instance.Tween(generateMap.pictureObject.transform, generateMap.pictureObject.transform.position, target, 1.2f);
        yield return new WaitForSeconds(1.2f);
        generateMap.WaveAnimationTrigger();
    }
}
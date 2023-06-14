using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class GenerateMap : MonoBehaviour
{
    [Header("Status")]
    public int maxCubeIndex;
    public int currentCubeIndex;
    public string nameTexture;

    [Header("References")]
    public int currentWidthHeight;
    public Texture2D[] mapTexture2D;
    public Texture2D[] pictureTexture2D;
    private const float Width = 24.0f;

    [Header("Materials")]
    public Material m_nen;
    public Material m_tuong;
    public Material m_cong;
    public Material m_tru;
    public Material m_player;
    public Material m_truHit;
    public Material m_congHit;
    public Material m_cube;

    [Header("Picture")]
    public GameObject pictureObject;
    public Cube cubePrefab;

    public Transform waveTrigger;
    public Transform wavePoint_a;
    public Transform wavePoint_b;


    [Header("Map")]
    public GameObject mapObject;
    public GameObject nenParent;
    public GameObject tuongParent;
    public GameObject congParent;
    public GameObject truParent;
    public GameObject playerParent;
    public Nen nenPrefab;
    public Tuong tuongPrefab;
    public Cong congPrefab;
    public Tru truPrefab;
    public Player playerPrefab;

    public Color[] Color_Alpha; // nen
    public Color[] Color_Black; // tuong
    public Color[] Color_White; // chan
    public Color[] Color_Red;    // tru
    public Color[] Color_Blue;  // cong
    public Color[] Color_Yellow; // snake
    public Color Color_Test;

    [Header("Input")]
    public int levelInput;

    public void GeneratePicture(int n)
    {
        Texture2D _texture2d = pictureTexture2D[n];
        // congatulations width height
        int _w = _texture2d.width;
        int _h = _texture2d.height;
        float _r = (Width - 4.0f) / (float)_w;
        currentWidthHeight = (int)((_w + _h) / 2.0f);
        Vector3 picturePosition = Vector3.zero;
        picturePosition.x = ((float)(_w - 1) * _r) / 2.0f;
        picturePosition.z = ((float)(_h - 1) * _r) / 2.0f;
        pictureObject.transform.position = picturePosition;


        for (int h = 0; h < _h; h++)
        {
            for (int w = 0; w < _w; w++)
            {
                Color _color = _texture2d.GetPixel(w, h);
                Vector3 _position = new Vector3(w, 0.0f, h);

                if(_color.a != 0)
                {
                    Cube _cube = Instantiate(cubePrefab, pictureObject.transform);
                    _cube.Init(_position, _r, _color);
                }

            }
        }

        // set camera
        Vector3 _pivotPosition = Vector3.zero;
        _pivotPosition.x = ((float)(_w - 1) * _r) / 2.0f;
        _pivotPosition.z = ((float)(_h - 1) * _r) / 2.0f;
        float y = ((float)_h / (float)_w) * 25.0f - 25.0f;
        _pivotPosition.y = y > 0 ? y : 0.0f;
        GetComponent<GameManager>().SetCamera(_pivotPosition);

        // set wave
        Vector3 point_b = Vector3.zero;
        point_b.x = (float)(_w - 1) * _r;
        point_b.z = (float)(_h - 1) * _r;
        wavePoint_b.position = point_b;

        Vector3 point_a = Vector3.zero + -point_b * 0.1f;
        wavePoint_a.position = point_a;

        waveTrigger.position = wavePoint_a.position;
        waveTrigger.rotation = Quaternion.LookRotation(point_b - point_a);
    }

    [NaughtyAttributes.Button]
    public void WaveAnimationTrigger()
    {
        StartCoroutine(C_WaveAnimationTrigger());
    }

    private IEnumerator C_WaveAnimationTrigger()
    {
        for (int i = 0; i < 3; i++)
        {
            waveTrigger.position = wavePoint_a.position;
            waveTrigger.DOMove(wavePoint_b.position, 1.4f).SetEase(Ease.Linear);
            GameManager.Instance.VibrationWave();
            yield return new WaitForSeconds(1.9f);
        }
    }

#if UNITY_EDITOR


    [NaughtyAttributes.Button]
    public void GenerateMapFromTexture()
    {
        // init
        mapObject = new GameObject();
        mapObject.name = "Map " + levelInput;
        mapObject.AddComponent<Map>();

        nenParent = new GameObject();
        nenParent.name = "Nen";
        nenParent.transform.SetParent(mapObject.transform);

        tuongParent = new GameObject();
        tuongParent.name = "Tuong";
        tuongParent.transform.SetParent(mapObject.transform);

        congParent = new GameObject();
        congParent.name = "Cong";
        congParent.transform.SetParent(mapObject.transform);

        truParent = new GameObject();
        truParent.name = "Tru";
        truParent.transform.SetParent(mapObject.transform);

        playerParent = new GameObject();
        playerParent.name = "Player";
        playerParent.transform.SetParent(mapObject.transform);


        Texture2D _texture2d = mapTexture2D[levelInput];

        // congatulations width height
        int _w = _texture2d.width;
        int _h = _texture2d.height;
        float _r = Width / (float)_w;
        currentWidthHeight = (int)((_w + _h) / 2.0f);

        Vector3 mapPosition = Vector3.zero;
        mapPosition.x = ((float)(_w -1) * _r) / 2.0f;
        mapPosition.z = ((float)(_h -1) * _r) / 2.0f;
        mapObject.transform.position = mapPosition;

        for (int h = 0; h < _h; h++)
        {
            for (int w = 0; w < _w; w++)
            {
                Color _color = _texture2d.GetPixel(w, h);
                Vector3 _position = new Vector3(w, 0.0f, h);
                ColorHandle(_color, _position, _r);
            }

        }


    }

    private void ColorHandle(Color32 _color,Vector3 _position,float _ratio)
    {

        //   Debug.Log(_color);
        if (_color.a == 0.0f)
        {
            GenerateNen(_position, _ratio);
            Debug.Log("Color Alpha");
            return;
        }

        for (int i = 0; i < Color_Black.Length; i++)
        {
            if (IsSameColor(_color, Color_Black[i]))
            {
                GenerateTuong(_position, _ratio,true);
                Debug.Log("Color Black");
                return;
            }
        }

        for (int i = 0; i < Color_White.Length; i++)
        {
            if (IsSameColor(_color, Color_White[i]))
            {
                GenerateNen(_position, _ratio);
                GenerateTuong(_position, _ratio,false);
                Debug.Log("Color White");
                return;
            }
        }

        for (int i = 0; i < Color_Red.Length; i++)
        {
            if (IsSameColor(_color, Color_Red[i]))
            {
                GenerateNen(_position, _ratio);
                GenerateTru(_position, _ratio);
                Debug.Log("Color Red");
                return;
            }
        }

        for (int i = 0; i < Color_Blue.Length; i++)
        {
            if (IsSameColor(_color, Color_Blue[i]))
            {
                GenerateNen(_position, _ratio);
                GenerateCong(_position, _ratio);
                Debug.Log("Color Blue");
                return;
            }
        }

        for (int i = 0; i < Color_Yellow.Length; i++)
        {
            if (IsSameColor(_color, Color_Yellow[i]))
            {
                GenerateNen(_position, _ratio);
                GeneratePlayer(_position, _ratio);
                Debug.Log("Color Yellow");
                return;
            }
        }

        Debug.Log(_color + " Color Error");
    }

    private bool IsSameColor(Color32 a,Color32 b)
    {
        bool _result = false;
        bool _r = (a.r == b.r) ? true : false;
        bool _g = (a.g == b.g) ? true : false;
        bool _b = (a.b == b.b) ? true : false;
        bool _a = (a.a == b.a) ? true : false;
        _result = (_r && _g && _b && _a) ? true : false;
        return _result;
    }

    private void GenerateNen(Vector3 _position,float _ratio)
    {
        Nen _s = PrefabUtility.InstantiatePrefab(nenPrefab, nenParent.transform) as Nen;
        _s.SetTransform(_position, _ratio);
        _s.SetMaterial(m_nen);
    }

    private void GenerateTuong(Vector3 _position, float _ratio,bool _isHide)
    {
        Tuong _s = PrefabUtility.InstantiatePrefab(tuongPrefab, tuongParent.transform) as Tuong;
        _s.SetTransform(_position, _ratio);
        _s.SetMaterial(m_tuong);
        _s.HideRenderer(_isHide);
    }

    private void GenerateCong(Vector3 _position, float _ratio)
    {
        Cong _s = PrefabUtility.InstantiatePrefab(congPrefab, congParent.transform) as Cong;
        _s.SetTransform(_position, _ratio);
        _s.SetMaterial(m_cong);
    }

    private void GenerateTru(Vector3 _position, float _ratio)
    {
        Tru _s = PrefabUtility.InstantiatePrefab(truPrefab, truParent.transform) as Tru;
        _s.SetTransform(_position, _ratio);
        _s.SetMaterial(m_tru);
    }

    private void GeneratePlayer(Vector3 _position, float _ratio)
    {
        Player _s = PrefabUtility.InstantiatePrefab(playerPrefab, playerParent.transform) as Player;
        _s.SetTransform(_position, _ratio);
    }

#endif
}
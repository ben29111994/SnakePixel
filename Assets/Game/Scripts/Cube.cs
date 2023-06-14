using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
    public int ID;

    public AnimationCurve ac;
    public AnimationCurve ac2;

    public Color mainColor;
    public Color whiteColor;
    public Color hiddenColor;

    public Transform model;
    public Renderer rend;
    private MaterialPropertyBlock mpb;

    public void Init(Vector3 _position, float _ratio, Color _mainColor)
    {
        ID = GetInstanceID();
        Vector3 posFixed = _position * _ratio;
        transform.localScale = Vector3.one * _ratio;
        transform.position = posFixed;

        rend.material = GameManager.Instance.generateMap.m_cube;
        mpb = new MaterialPropertyBlock();
        mainColor = _mainColor;
        rend.GetPropertyBlock(mpb);
        mpb.SetColor("_Color", hiddenColor);
        rend.SetPropertyBlock(mpb);
        gameObject.SetActive(true);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("WaveTrigger"))
        {
            WaveCompleteAnimation();
        }
    }

    public void WaveCompleteAnimation()
    {
        StartCoroutine(C_WaveCompleteAnimation());
    }

    private IEnumerator C_WaveCompleteAnimation()
    {
        float t = 0.0f;
        Vector3 startPosition = model.transform.localPosition;

        Color startColor = mpb.GetColor("_Color");

        while (t < 1.0f)
        {
            t += Time.deltaTime * 2.0f;

            Vector3 pos = model.transform.localPosition;
            float y = (ac.Evaluate(t) - 1.0f) * (4.0f * Ratio());
            pos.y = y;
            model.transform.localPosition = pos;

            rend.GetPropertyBlock(mpb);
            Color _color = Color.Lerp(startColor, mainColor, t);
            mpb.SetColor("_Color", _color);
            rend.SetPropertyBlock(mpb);

            yield return null;
        }

        t = 0.0f;

        yield return new WaitForSeconds(0.1f);

        while (t < 1.0f)
        {
            t += Time.deltaTime * 1.6f;

            Vector3 pos = model.transform.localPosition;
            float y = (ac2.Evaluate(t) - 1.0f) * (3.0f * Ratio());
            pos.y = y;
            model.transform.localPosition = pos;
            yield return null;
        }
    }

    private float Ratio()
    {
        float _r = (float)GameManager.Instance.generateMap.currentWidthHeight / 60.0f;
        return _r;
    }
}

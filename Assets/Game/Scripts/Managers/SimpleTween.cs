using System.Collections;
using UnityEngine;

public class SimpleTween : MonoBehaviour
{
    public static SimpleTween Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    public void Tween(Transform trans, Vector3 start, Vector3 end, float dur)
    {
        StartCoroutine(EasePosition(trans, start, end, dur));
    }

    public IEnumerator EasePosition(Transform trans, Vector3 start, Vector3 end, float dur)
    {
        float t = 0f;
        while (t < dur)
        {
            trans.position = Vector3.LerpUnclamped(start, end, dur);
            yield return null;
            t += Time.deltaTime;
        }
        trans.position = end;
    }
}

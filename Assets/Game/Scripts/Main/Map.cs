using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public int totalNumber;

    [Range(0.0f,1.0f)]
    public float percentTru;

    [NaughtyAttributes.Button]
    public void AutoSeNumber()
    {
        int _nTru = (int)((float)totalNumber * percentTru);

        List<Cong> listCong = new List<Cong>();
        for(int i = 0; i < transform.GetChild(2).childCount; i++)
        {
            listCong.Add(transform.GetChild(2).GetChild(i).GetComponent<Cong>());
        }

        List<Tru> listTru = new List<Tru>();
        for (int i = 0; i < transform.GetChild(3).childCount; i++)
        {
            listTru.Add(transform.GetChild(3).GetChild(i).GetComponent<Tru>());
        }

        int totalNumber2 = 0;
        for(int i = 0; i < listCong.Count; i++)
        {
            int targetNumber = (int)((totalNumber - totalNumber2) * 0.3f);
            if (targetNumber < 7) targetNumber = 7;
            int _numberRandom = Random.Range(3, targetNumber);

            if(i == listCong.Count - 1)
            {
                _numberRandom = totalNumber - totalNumber2;
            }
            else
            {
                totalNumber2 += _numberRandom;
            }

            listCong[i].UpdateNumber(_numberRandom);
        }

        totalNumber2 = 0;
        for (int i = 0; i < listTru.Count; i++)
        {
            int targetNumber = (int)((_nTru - totalNumber2) * 0.4f);
            if (targetNumber < 7) targetNumber = 7;
            int _numberRandom = Random.Range(3, targetNumber);

            if (i == listTru.Count - 1)
            {
                _numberRandom = _nTru - totalNumber2;
            }
            else
            {
                totalNumber2 += _numberRandom;
            }


            listTru[i].UpdateNumber(_numberRandom);
        }
    }
}

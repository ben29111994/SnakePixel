using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class Tru : Shape
{
    public int number;
    public Text numberText;

    private void Start()
    {
        number = Mathf.Abs(number);
        GameManager.Instance.listShape.Add(this);
        UpdateNumber(number);
    }

    public void UpdateNumber(int _n)
    {
        number = _n;
        numberText.text = "-" + number.ToString();
    }

    public void DecreaseNumber()
    {
        if (number > 0)
        {
            number--;
            numberText.text = "-" + number.ToString();

            if (number == 0)
            {
                GameManager.Instance.CubeEffect(transform.position, Vector3.one, GameManager.Instance.generateMap.m_tru);
                GameManager.Instance.UpdateShape(this);
            }
        }
    }
}

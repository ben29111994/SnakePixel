using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseView : MonoBehaviour
{
    public bool isEnable;

    public bool isHold;
    public GameObject mouseObject;
    public LayerMask layerMouseView;

    private void Start()
    {
        mouseObject.SetActive(isEnable);
    }

    private void Update()
    {
        if (isEnable == false) return;

        if (Input.GetMouseButtonDown(0))
        {
            mouseObject.SetActive(true);
            isHold = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            mouseObject.SetActive(false);
            isHold = false;
        }

        if (isHold)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray,out hit, Mathf.Infinity, layerMouseView))
            {
                if(hit.collider != null)
                {
                    Vector3 mpos = hit.point;
                    mouseObject.transform.position = mpos;
                }
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shape : MonoBehaviour
{
    public TypeShape typeShape;
    public Renderer _renderer;
    public Animator anim;

    public enum TypeShape
    {
        nen,
        tuong,
        cong,
        tru
    }

    public void SetMaterial(Material _m)
    {
        _renderer.material = _m;   
    }

    public void HideRenderer(bool _isHide)
    {
        _renderer.enabled = !_isHide;
    }

    public void SetTransform(Vector3 _position, float _ratioScale)
    {
        transform.position = _position * _ratioScale;
        transform.localScale = Vector3.one * _ratioScale;

        if (typeShape == TypeShape.nen)
        {
            Vector3 p = transform.position;
            p.y -= transform.localScale.y;
            transform.position = p;
        }
    }

    public void Anim_Hit()
    {
        anim.SetTrigger("Hit");
    }

    public void BreakShape()
    {
        gameObject.SetActive(false);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body : MonoBehaviour
{
    public Renderer renderer;
    public Collider collider;
    public LayerMask layer;
    public Body c;

    private void OnEnable()
    {
        renderer.material = GameManager.Instance.generateMap.m_player;
        collider.enabled = true;
    }

    private void Update()
    {
        if (GameManager.Instance.player.snakeManager.balls.Count == 0) return;
        Transform a = gameObject.transform;
        Transform b = GameManager.Instance.player.snakeManager.balls[0];


        if (a == b)
        {
            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hit;
            float distance = GameManager.Instance.player.transform.localScale.x * 0.5f;
            if(Physics.Raycast(ray,out hit, distance, layer))
            {
                if (hit.collider.CompareTag("Body"))
                {
                    Debug.Log("HIT BODY");
                    c = hit.collider.GetComponent<Body>();
                    Body _body = hit.collider.GetComponent<Body>();
                    GameManager.Instance.player.snakeManager.RemoveTail(_body);
                }
            }
        }
    }

    public void DeadTail()
    {
        StartCoroutine(C_DeadTail());
    }

    public void DisableCollider()
    {
        collider.enabled = false;
    }

    private IEnumerator C_DeadTail()
    {
        renderer.material = GameManager.Instance.generateMap.m_tru;
        yield return new WaitForSeconds(0.06f);
        renderer.material = GameManager.Instance.generateMap.m_truHit;
        yield return new WaitForSeconds(0.06f);
        renderer.material = GameManager.Instance.generateMap.m_tru;
        yield return new WaitForSeconds(0.06f);
        renderer.material = GameManager.Instance.generateMap.m_truHit;
        yield return new WaitForSeconds(0.06f);
        renderer.material = GameManager.Instance.generateMap.m_tru;
        yield return new WaitForSeconds(0.06f);
        renderer.material = GameManager.Instance.generateMap.m_truHit;
        yield return new WaitForSeconds(0.06f);
        //   GameManager.Instance.PlayerExplosionEffect(transform.position, GameManager.Instance.generateMap.m_tru);
        gameObject.SetActive(false);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Body"))
        {
         

       
        }
    }
}

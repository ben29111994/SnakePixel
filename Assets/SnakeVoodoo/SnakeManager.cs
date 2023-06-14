using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeManager : MonoBehaviour
{
    public Player player;

    [Header("Snake Manager")]
    public int startCount;
    public float distance;
    public Transform prefab;
    public Transform parent;
    private Vector3 lastPosition;
    public List<Transform> balls = new List<Transform>();
    private List<Vector3> points = new List<Vector3>();
    private List<Transform> ballsInPool = new List<Transform>();

    [Header("Head Manager")]
    public GameObject target;
    private Vector3 velocity = Vector3.zero;
    private Vector3 a, b, c;

    private void Awake()
    {
        distance = transform.localScale.x;

        for(int i = 0; i <startCount; i++)
        {
            Transform _transform = Instantiate(prefab, parent).transform;

            balls.Add(_transform);
            ballsInPool.Add(_transform);
            balls[i].transform.position = target.transform.position;
            points.Add(balls[i].position);
        }

        lastPosition = balls[0].position;
    }

    public void FixedUpdateStep()
    {
        HeadControl();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            AddSnake();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            RemoveSnake_Head();
        }

        if (GameManager.Instance.player.isDone) return;
        SnakeMove();
    }

    public void AddSnake()
    {
        Transform _transform = BallObject();
        _transform.position = balls[balls.Count -1].transform.position;
        balls.Add(_transform);
        ballsInPool.Add(_transform);
        points.Add(_transform.position);
    }

    public Transform BallObject()
    {
        Transform a = null;

        for(int i = 0; i < ballsInPool.Count; i++)
        {
            if(ballsInPool[i].gameObject.activeInHierarchy == false)
            {
                a = ballsInPool[i];
                a.gameObject.SetActive(true);
                break;
            }
        }

        if(a == null)
        {
            a = Instantiate(prefab, parent).transform;
        }

        return a;
    }

    public void RemoveSnake_Head()
    {
        Vector3 zero = Vector3.zero;

        //remove the main ball
        if (this.balls.Count > 0)
        {
      //      GameManager.Instance.PlayerExplosionEffect(balls[0].transform.position, GameManager.Instance.generateMap.m_player);
            balls[0].gameObject.SetActive(false);
            zero = balls[0].position;
            this.balls.RemoveAt(0);
        }

        //gameover
        if (this.balls.Count <= 0)
        {
            // game over
        }
        else // set next ball as main ball
        {
            Vector3 vector2 = balls[0].position;
            Vector3 vector4 = zero - vector2;
            Vector3 normalized = vector4.normalized;
            int count = 0;
            for (int i = 0; i < this.points.Count; i++)
            {
                if (Vector3.Dot(vector2 - this.points[i], normalized) >= 0f)
                {
                    break;
                }
                count = i;
            }

            points.RemoveRange(0, count);
            SnakeMove();
        }
    }

    public void RemoveTail(Body _body)
    {
        List<Body> listBody = new List<Body>();

        while (balls.Count > 0)
        {
            int a = balls[balls.Count - 1].transform.GetInstanceID();
            int b = _body.transform.GetInstanceID();

            if (a == b)
            {
                //    GameManager.Instance.PlayerExplosionEffect(balls[balls.Count -1].transform.position, GameManager.Instance.generateMap.m_player);

                listBody.Add(balls[balls.Count - 1].gameObject.GetComponent<Body>());
                //   balls[balls.Count - 1].gameObject.SetActive(false);
                balls.RemoveAt(balls.Count - 1);
                player.RemoveTail();
                break;
            }
            else
            {
                //    GameManager.Instance.PlayerExplosionEffect(balls[balls.Count - 1].transform.position, GameManager.Instance.generateMap.m_player);

                listBody.Add(balls[balls.Count - 1].gameObject.GetComponent<Body>());
                //     balls[balls.Count - 1].gameObject.SetActive(false);
                balls.RemoveAt(balls.Count - 1);
                player.RemoveTail();
            }

        }

        for(int i = 0; i < listBody.Count; i++)
        {
            listBody[i].DisableCollider();
        }

        StartCoroutine(C_EffectRemeTail(listBody));
    }

    private IEnumerator C_EffectRemeTail(List<Body> _listBody)
    {
        for(int i =  _listBody.Count -1;i >= 0; i--)
        {
            _listBody[i].DeadTail();
            yield return new WaitForSeconds(0.04f);
        }
    }

    // move of snake hit blocks
    private void SnakeMove()
    {
        if (balls.Count == 0) return;

        if (lastPosition != balls[0].position)
        {
            lastPosition = balls[0].position;
            points.Insert(0, lastPosition);

            int num = 1;
            float spacing = distance;
            int index = 0;
            while ((index < (this.points.Count - 1)) && (num < this.balls.Count))
            {
                Vector3 vector2 = this.points[index];
                Vector3 vector3 = this.points[index + 1];
                Vector3 vector4 = vector3 - vector2;
                float magnitude = vector4.magnitude;
                if (magnitude > 0f)
                {
                    Vector3 vector6 = vector3 - vector2;
                    Vector3 normalized = vector6.normalized;
                    Vector3 vector7 = vector2;
                    //update other ball's position and keep the distance between two balls
                    while ((spacing <= magnitude) && (num < this.balls.Count))
                    {
                        vector7 += (Vector3)(normalized * spacing);
                        magnitude -= spacing;
                        this.balls[num].transform.position = vector7;
                        num++;
                        spacing = distance;
                    }
                    spacing -= magnitude;
                }
                index++;
            }
            Vector3 vector8 = this.points[this.points.Count - 1];
            for (int i = num; i < this.balls.Count; i++)
            {
                this.balls[num].transform.position = vector8;
            }
            index++;
            //remove the  unnecessary point
            if (index < this.points.Count)
            {
                this.points.RemoveRange(index, this.points.Count - index);
            }
        }
    }

    private void HeadControl()
    {
        if (balls.Count == 0) return;
        //  balls[0].transform.position = Vector3.SmoothDamp(balls[0].transform.position, target.transform.position, ref velocity, 0.14f);
        balls[0].transform.position = Vector3.MoveTowards(balls[0].transform.position, target.transform.position, Time.deltaTime * 20.0f);
        Vector3 dir = target.transform.position - balls[0].transform.position;
        dir.y = 0.0f;
        if (dir != Vector3.zero) balls[0].transform.rotation = Quaternion.LookRotation(dir);

        for(int i = 1;i< balls.Count; i++)
        {
            Vector3 d = balls[i - 1].transform.position - balls[i].transform.position;
            d.y = 0.0f;
            if(d != Vector3.zero) balls[i].transform.rotation = Quaternion.LookRotation(d);
        }

        //   balls[0].transform.position = Vector3.Lerp(balls[0].transform.position, target.transform.position, 5.0f * Time.deltaTime);
    }
}

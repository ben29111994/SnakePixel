using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [Header("Input")]
    public float moveSpeed;

    [Header("Status")]
    public StatusPlayer statusPlayer;
    public bool isDone;

    public int bodyCurrentNumber;
    public int bodyHideNumber;
    public int bodySumNumber;
    public int BodySumNumber
    {
        get
        {
            return bodySumNumber;
        }
        set
        {
            bodySumNumber = value;
            bodyNumberText.text = bodySumNumber.ToString();
        }
    }

    [Header("UI")]
    public GameObject canvasUI;
    public Text bodyNumberText;

    [Header("References")]
    public SnakeManager snakeManager;
    public Transform target;
    public Rigidbody rigidTarget;

    private Cong currentCong;
    private Tru currentTru;
    

    [Header("LayerMask")]
    public LayerMask layerMove;


    private Vector3 lastDirection;

    public enum StatusPlayer
    {
        None,
        Move,
        Cong,
        Tru
    }

    private void Awake()
    {
        GameManager.Instance.player = this;
        bodyCurrentNumber = 1;
        bodyHideNumber = 0;
        BodySumNumber = bodyCurrentNumber + bodyHideNumber;
        posA = posB = target.transform.position;
    }

    private void FixedUpdate()
    {
        if(isDone == false) snakeManager.FixedUpdateStep();

        if(snakeManager.balls.Count > 0)
        {
            if(statusPlayer == StatusPlayer.Tru)
            {
                canvasUI.transform.position = Vector3.Lerp(canvasUI.transform.position, target.transform.position, Time.deltaTime * 40.0f);

            }
            else
            {
                canvasUI.transform.position = Vector3.Lerp(canvasUI.transform.position, snakeManager.balls[0].transform.position, Time.deltaTime * 40.0f);
            }
        }
        else
        {
            canvasUI.SetActive(false);
        }
    }

    private void Update()
    {
        if (isDone) return;

        UpdateAutoGenerateBody();
    }

    private void LateUpdate()
    {
    }

    private Vector3 posA;
    private Vector3 posB;
    private Vector3 posC;

    private void UpdateAutoGenerateBody()
    {
        if (bodyHideNumber <= 0) return;
        if (snakeManager.balls.Count == 0) return;

        posB = snakeManager.balls[0].transform.position;
        posC = posB - posA;
        if(posC.magnitude > transform.localScale.x)
        {
            posA = posB;
            AddBody();
        }
    }

    public void SetTransform(Vector3 _position, float _ratioScale)
    {
        transform.position = _position * _ratioScale;
        transform.localScale = Vector3.one * _ratioScale;
    }

    public void MoveControl(Vector3 _direction)
    {
     //   Debug.Log(_direction + " swipe");
        Move(_direction);
    }

    public void SetSkin(int _number)
    {
        Debug.Log("current skin is : " + _number);
    }

    public void AddBody()
    {
        if (isDone) return;
        if (bodyHideNumber == 0) return;
        bodyHideNumber--;
        bodyCurrentNumber++;
        BodySumNumber = bodyCurrentNumber + bodyHideNumber;
        snakeManager.AddSnake();
    }

    public void RemoveBody()
    {
        if (isDone) return;
        
        if(bodyCurrentNumber > 0)
        {
            AddBody();

            snakeManager.RemoveSnake_Head();
            bodyCurrentNumber--;
            BodySumNumber = bodyCurrentNumber + bodyHideNumber;
            GameManager.Instance.Vibration();

            if (bodyCurrentNumber == 0)
            {
                isDone = true;
                StopAllCoroutines();
                GameManager.Instance.Fail();
                GameManager.Instance.ShakeCamera();
                Debug.Log("___FAIL___");
            }
        }
    }

    public void RemoveTail()
    {
        bodyCurrentNumber--;
        BodySumNumber = bodyCurrentNumber + bodyHideNumber;
    }

    private void Move(Vector3 _direction)
    {
        if (isDone) return;

        if(snakeManager.balls.Count > 1)
        {
            if (_direction == -target.forward) return;
        }

        if (statusPlayer == StatusPlayer.Move) return;
        if (C2_Move != null) StopCoroutine(C2_Move);
        C2_Move = C_Move(_direction);
        StartCoroutine(C2_Move);
    }


    private IEnumerator C2_Move;
    private IEnumerator C_Move(Vector3 _direction)
    {
        Ray ray2 = new Ray(target.position, _direction);
        RaycastHit hit2;
        if (Physics.Raycast(ray2, out hit2, 100.0f, layerMove))
        {
            if (hit2.collider.gameObject.CompareTag("Tuong"))
            {
                float ds = Vector3.Distance(target.position, hit2.collider.gameObject.transform.position);
                if (ds <= transform.localScale.x * (1.02f))
                {
                    yield break;
                    
                }
            }
        }

            statusPlayer = StatusPlayer.Move;
        lastDirection = _direction;

        currentCong = null;
        currentTru = null;

        // rotate head to direction
        target.transform.rotation = Quaternion.LookRotation(_direction);

        Ray ray = new Ray(target.position, target.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100.0f, layerMove))
        {
            // wait balls[0] (head) go to target then target will move
            float _d1 = Vector3.Distance(snakeManager.balls[0].transform.position, target.transform.position);
            while(_d1 > 0.02f)
            {
                _d1 = Vector3.Distance(snakeManager.balls[0].transform.position, target.transform.position);
                yield return null;
            }


            // target move
            target.transform.position = hit.collider.gameObject.transform.position;
            target.transform.position -= target.transform.forward * transform.localScale.x;

            if (hit.collider.CompareTag("Tuong"))
            {
                posA = posB = target.transform.position;

            }
            else if (hit.collider.CompareTag("Cong"))
            {
                currentCong = hit.collider.gameObject.GetComponent<Cong>();             
            }
            else if (hit.collider.CompareTag("Tru"))
            {
                currentTru = hit.collider.gameObject.GetComponent<Tru>();
                posA = posB = target.transform.position;

            }
        }

        float _d = Vector3.Distance(snakeManager.balls[0].transform.position, target.transform.position);
        while(_d > 0.2f)
        {
            _d = Vector3.Distance(snakeManager.balls[0].transform.position, target.transform.position);
            yield return null;
        }


        if(currentCong != null)
        {
            statusPlayer = StatusPlayer.Cong;

            while (currentCong.number > 0)
            {
                currentCong.Anim_Hit();
                currentCong.DecreaseNumber();
                bodyHideNumber++;
                BodySumNumber = bodyCurrentNumber + bodyHideNumber;
                GameManager.Instance.Vibration();
                GameManager.Instance.NumberEffect();

                if (currentCong.number > 0)
                {
                    yield return new WaitForSeconds(0.06f);
                }
                else
                {
                    currentCong.BreakShape();
                    statusPlayer = StatusPlayer.None;
                    Move(lastDirection);
                    yield break;
                }
            }
        }
        else if(currentTru != null)
        {
            statusPlayer = StatusPlayer.Tru;

            while (currentTru.number > 0)
            {
                currentTru.Anim_Hit();
                currentTru.DecreaseNumber();
                RemoveBody();
                GameManager.Instance.Vibration();
                GameManager.Instance.NumberTruEffect();

                if (currentTru.number > 0)
                {
                    if (isDone) yield break;
                    _d = Vector3.Distance(snakeManager.balls[0].transform.position, target.transform.position);
                    while (_d > 0.1f)
                    {
                        _d = Vector3.Distance(snakeManager.balls[0].transform.position, target.transform.position);
                        yield return null;
                    }
                }
                else
                {
                    currentTru.BreakShape();
                    statusPlayer = StatusPlayer.None;
                    Move(lastDirection);
                    yield break;
                }
            }
        }

        statusPlayer = StatusPlayer.None;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Tuong"))
        {
            Debug.Log("Col-Tuong");
        }
    }
}
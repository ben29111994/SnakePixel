using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeControl : MonoBehaviour
{
    public static SwipeControl Instance;

    private void Awake()
    {
        Instance = (Instance == null) ? this : Instance;
    }

    private float magnitudeSwipe = 15.0f;
    private bool isDelaySwipe;
    private bool isSwipe;
    private Vector2 currentTouchPosition;
    private Vector2 lastTouchPosition;
    private Vector2 deltaTouchPosition;
    private Player player;
    private Vector3 lastDirection;

    private void Start()
    {
        player = GetComponent<Player>();
    }

    private void Update()
    {
        UpdateSwipe();
    }

    private void UpdateSwipe()
    {
        bool touchBegan = false;
        bool touchMoved = false;
        bool touchEnded = false;

   

#if UNITY_EDITOR
        touchBegan = Input.GetMouseButtonDown(0);
        touchMoved = Input.GetMouseButton(0);
        touchEnded = Input.GetMouseButtonUp(0);
#elif UNITY_IOS
        if(Input.touchCount > 0)
        {
            touchBegan = Input.touches[0].phase == TouchPhase.Began;
            touchMoved = Input.touches[0].phase == TouchPhase.Moved;
            touchEnded = Input.touches[0].phase == TouchPhase.Ended;
        }
#endif

        if (touchBegan)
        {
            currentTouchPosition = lastTouchPosition = Input.mousePosition;
        }
        else if (touchMoved)
        {
            currentTouchPosition = Input.mousePosition;
            deltaTouchPosition = currentTouchPosition - lastTouchPosition;
            lastTouchPosition = currentTouchPosition;

            if (deltaTouchPosition.magnitude >= magnitudeSwipe)
            {
                isSwipe = true;

                float x = deltaTouchPosition.x;
                float y = deltaTouchPosition.y;

                if (Mathf.Abs(x) > Mathf.Abs(y))
                {
                    if (x < 0)
                    {
                        SwipeLeft();
                    }
                    else
                    {
                        SwipeRight();
                    }
                }
                else
                {
                    if (y < 0)
                    {
                        SwipeDown();
                    }
                    else
                    {
                        SwipeUp();
                    }
                }
            }
        }
        else if (touchEnded)
        {
            currentTouchPosition = lastTouchPosition = deltaTouchPosition = Vector2.zero;
            isSwipe = false;
        }

        if (isSwipe)
        {
            if (isDelaySwipe) return;
            StartCoroutine(C_DelaySwipe(0.1f));
            GameManager.Instance.Swipe(lastDirection);
        }
    }

    private void SwipeLeft()
    {
        if (isDelaySwipe) return;
        StartCoroutine(C_DelaySwipe(0.2f));
        GameManager.Instance.Swipe(Vector3.left);
        lastDirection = Vector3.left;
        // Debug.Log("swipe left");
    }

    private void SwipeRight()
    {
        if (isDelaySwipe) return;
        StartCoroutine(C_DelaySwipe(0.2f));
        GameManager.Instance.Swipe(Vector3.right);
        lastDirection = Vector3.right;
        //    Debug.Log("swipe right");
    }

    private void SwipeUp()
    {
        if (isDelaySwipe) return;
        StartCoroutine(C_DelaySwipe(0.2f));
        GameManager.Instance.Swipe(Vector3.forward);
        lastDirection = Vector3.forward;
        //  Debug.Log("swipe up");
    }

    private void SwipeDown()
    {
        if (isDelaySwipe) return;
        StartCoroutine(C_DelaySwipe(0.2f));
        GameManager.Instance.Swipe(Vector3.back);
        lastDirection = Vector3.back;
        // Debug.Log("swipe down");
    }

    private IEnumerator C_DelaySwipe(float _time)
    {
        isDelaySwipe = true;
        yield return new WaitForSeconds(_time);
        isDelaySwipe = false;
    }
}
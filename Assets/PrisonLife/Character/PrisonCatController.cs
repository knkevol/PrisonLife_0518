using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrisonCatController : MonoBehaviour
{
    // Start is called before the first frame update
    CharacterController cc;
    Animator anim;
    public float speed = 20f;

    float gravity = -20f;
    float yVelocity = 1f;
    public Vector3 childScale = new Vector3(7f, 7f, 7f);

    // ─── 터치 관련 변수 추가 ───────────────────────
    private Vector2 touchStartPos;
    private Vector2 moveDirection;
    private bool isTouching = false;
    public float deadZone = 15f; // 이 픽셀 이상 드래그해야 이동
    // ────────────────────────────────────────────────
    void Start()
    {
        cc = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();
        
    }

    // Update is called once per frame
    void Update()
    {
        HandleTouchInput();
        Vector3 move = new Vector3(moveDirection.x, 0, moveDirection.y);

        if (cc.isGrounded)
            yVelocity = -1f;
        else
            yVelocity += gravity * Time.deltaTime;

        move.y = yVelocity;
        cc.Move(move * speed * Time.deltaTime);

        Vector3 flatMove = new Vector3(moveDirection.x, 0, moveDirection.y);
        if (flatMove.magnitude > 0.1f)
        {
            anim.SetBool("isRunning", true);
            transform.rotation = Quaternion.LookRotation(flatMove);
        }
        else
        {
            anim.SetBool("isRunning", false);
        }
    }

    void HandleTouchInput()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            touchStartPos = Input.mousePosition;
            isTouching = true;
        }
        else if (Input.GetMouseButton(0)) // 꾹 누른 상태
        {
            Vector2 delta = (Vector2)Input.mousePosition - touchStartPos;
            if (delta.magnitude > deadZone)
                moveDirection = delta.normalized;
            else
                moveDirection = Vector2.zero;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isTouching = false;
            moveDirection = Vector2.zero;
        }

// 모바일 → 실제 터치
#else
            if (Input.touchCount == 0)
            {
                isTouching = false;
                moveDirection = Vector2.zero;
                return;
            }

            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    touchStartPos = touch.position;
                    isTouching = true;
                    break;

                case TouchPhase.Moved:
                case TouchPhase.Stationary:
                    Vector2 delta = touch.position - touchStartPos;
                    if (delta.magnitude > deadZone)
                        moveDirection = delta.normalized;
                    else
                        moveDirection = Vector2.zero;
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    isTouching = false;
                    moveDirection = Vector2.zero;
                    break;
            }
#endif
    }

    private void LateUpdate()
    {
        if (transform.childCount > 0)
        {
            transform.GetChild(0).localScale = childScale;
        }
    }
}

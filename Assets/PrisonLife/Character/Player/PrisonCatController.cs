using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrisonCatController : MonoBehaviour
{
    // Start is called before the first frame update
    CharacterController cc;
    Animator anim;

    [Header("Movement Settings")]
    public float speed = 20f;
    float gravity = -20f;
    float yVelocity = -1f;
    public Vector3 childScale = new Vector3(7f, 7f, 7f);

    [Header("Joystick Input")]
    public DynamicJoystick joystick;

    // ─── 터치 관련 변수 추가 ───────────────────────
    private Vector2 touchStartPos;
    private Vector2 moveDirection;
    private bool isTouching = false;
    public float deadZone = 10f; // 이 픽셀 이상 드래그해야 이동
    // ────────────────────────────────────────────────
    
    void Start()
    {
        cc = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = joystick.Horizontal;
        float vertical = joystick.Vertical;
        Vector3 move = new Vector3(horizontal, 0f, vertical);

        if (move.sqrMagnitude > 0.01f)
        {
            anim.SetBool("isRunning", true);

            Quaternion lookRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
        }
        else
        {
            anim.SetBool("isRunning", false);
            move = Vector3.zero;
        }

        if (cc.isGrounded)
        {
            yVelocity = -1f;
        }
        else
        {
            yVelocity += gravity * Time.deltaTime;
        }
        move.y = yVelocity;

        cc.Move(move * speed * Time.deltaTime);
    }

    private void LateUpdate()
    {
        if (transform.childCount > 0)
        {
            transform.GetChild(0).localScale = childScale;
        }
    }
}

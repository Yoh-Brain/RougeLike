using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    private int facingDirection = 1;
    private Rigidbody2D rb;
    public Animator animator;

    public int FacingDirection => facingDirection;
    public Inventory bag = new Inventory();

    // 【新增】是否允许翻转（攻击时会被 PlayerAttack 设为 false）
    public bool canFlip = true;

    // 【新增】攻击时减半的倍率
    [Range(0f, 2f)]
    public float speedMultiplier = 1f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        rb.velocity = new Vector2(horizontal, vertical) * speed * speedMultiplier;

        // 尝试翻转，如果正在攻击 (canFlip == false)，则会被 Flip() 内部拦截
        if ((facingDirection == 1 && horizontal < 0) ||
            (facingDirection == -1 && horizontal > 0))
        {
            Flip();
        }

        animator.SetFloat("horizontal", Mathf.Abs(horizontal));
        animator.SetFloat("vertical", Mathf.Abs(vertical));
    }

    public void Flip()
    {
        // 【新增】如果开关是关着的，直接返回，禁止翻转
        if (!canFlip) return;

        facingDirection = -facingDirection;
        transform.localScale = new Vector3(facingDirection, 1, 1);
    }
}
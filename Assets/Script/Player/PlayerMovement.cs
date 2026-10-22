using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;   // 移动速度，可在 Inspector 调整
    private int facingDirection = 1;                 // 1 朝右，-1 朝左
    private Rigidbody2D rb;
    public Animator animator;

    public int FacingDirection => facingDirection;
    public Inventory bag = new Inventory();

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();//rb相当于是获得刚体的遥控器
        animator = GetComponent<Animator>();
       
    }

    private void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        
        rb.velocity = new Vector2(horizontal, vertical) * speed;//velocity是速度矢量，有方向，有速度

        if ((facingDirection == 1 && horizontal < 0) ||
            (facingDirection == -1 && horizontal > 0))
        {
            Flip();
        }


        //给 Animator 动画状态机传参数，取绝对值
        animator.SetFloat("horizontal", Mathf.Abs(horizontal));
        animator.SetFloat("vertical", Mathf.Abs(vertical));
    }

     void Flip()
    {
        facingDirection = -facingDirection;
        transform.localScale = new Vector3(facingDirection, 1, 1);
    }
}


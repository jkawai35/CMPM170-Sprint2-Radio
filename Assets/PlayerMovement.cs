using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody2D rb;
    private Animator animator;
    bool isMoving;
        [SerializeField] float speed;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }


    // Update is called once per frame
    void Update()
    {
        if (!isMoving)
        {
            float x = Input.GetAxisRaw("Horizontal");
            float y = Input.GetAxisRaw("Vertical");

            Vector2 movement = new Vector2(x,y);
            movement.Normalize();

            rb.velocity = new Vector2(movement.x * speed, movement.y * speed);

            Vector2 moveDirection = rb.velocity;
            if (moveDirection != Vector2.zero) {
                animator.SetFloat("moveX", x);
                animator.SetFloat("moveY", y);
                isMoving = true;

            }

            animator.SetBool("isMoving", isMoving);
        }
        isMoving = false;
    }
}

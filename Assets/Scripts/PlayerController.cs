using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] private Transform lightTransform; // Reference to the light object's Transform

    Rigidbody2D rb;
    Animator animator;
    bool isMoving;

    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    
    void Update()
    {
        Radio.Instance.currentState = Input.GetKey(KeyCode.Space) ? Radio.RadioState.isCharging : Radio.RadioState.isOn;
        
        switch(Radio.Instance.currentState){
            case Radio.RadioState.isCharging:
            break;
            case Radio.RadioState.isOn:
                PlayerMovement();
            break;
            case Radio.RadioState.isOff:
                PlayerMovement();
            break;
        }
    }

    void PlayerMovement(){
        if (!isMoving)
        {
            float x = Input.GetAxisRaw("Horizontal");
            float y = Input.GetAxisRaw("Vertical");

            Vector2 movement = new Vector2(x,y);
            movement.Normalize();

            rb.velocity = new Vector2(movement.x * speed, movement.y * speed);

            Vector2 moveDirection = rb.velocity;
            if (moveDirection != Vector2.zero) {
                float angle = Mathf.Atan2(moveDirection.x, -moveDirection.y) * Mathf.Rad2Deg;
                lightTransform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                animator.SetFloat("moveX", x);
                animator.SetFloat("moveY", y);
                isMoving = true;

            }

            animator.SetBool("isMoving", isMoving);
        }
        isMoving = false;
    }
}

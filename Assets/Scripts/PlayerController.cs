using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float speed;
    Rigidbody2D rb;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    
    void Update()
    {
        PlayerMovement();
    }

    void PlayerMovement(){
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        Vector2 movement = new Vector2(x,y);
        movement.Normalize();

        rb.velocity = new Vector2(movement.x * speed, movement.y * speed);
        
        Vector2 moveDirection = rb.velocity;
        if (moveDirection != Vector2.zero) {
        	float angle = Mathf.Atan2(moveDirection.x, -moveDirection.y) * Mathf.Rad2Deg;
        	transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }
}

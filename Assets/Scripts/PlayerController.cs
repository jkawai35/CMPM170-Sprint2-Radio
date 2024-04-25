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
    }
}

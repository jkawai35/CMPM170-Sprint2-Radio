using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MonsterController : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] GameObject player;
    Rigidbody2D rb;

    bool los;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    Vector2 pathfindingInstruction;

    void Update()
    {
        Vector2 playerLoc = new Vector2(player.transform.position.x,player.transform.position.y);
        Vector2 monsterLoc = new Vector2(transform.position.x,transform.position.y);
        RaycastHit2D[] ray = Physics2D.RaycastAll(monsterLoc,playerLoc-monsterLoc,50f);
        los = false;
        if(ray.Length>1){
            for (int i = 1; i < ray.Length; i++){
                RaycastHit2D hit = ray[i];
                if(hit.collider.CompareTag("Wall")){
                    break;
                }
                if(hit.collider.CompareTag("Player")){
                    los = true;
                    break;
                }
            }
        }
        
        if(los){
            Debug.DrawRay(monsterLoc,playerLoc-monsterLoc,Color.green);
            MonsterMovement(playerLoc-monsterLoc);
        }
        else{
            Vector2 dest = GridGenerator.Instance.MonsterInstructions();
            MonsterMovement(dest-monsterLoc);
        }
    }

    void MonsterMovement(Vector2 movement){
        Vector2 normal = movement.normalized;
        rb.velocity = new Vector2(normal.x * speed, normal.y * speed);
    }
}

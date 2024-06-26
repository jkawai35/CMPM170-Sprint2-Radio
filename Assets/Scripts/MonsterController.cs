using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;

public class MonsterController : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] GameObject player;
    [SerializeField] Volume postProc;    
    Rigidbody2D rb;
    AudioSource audioSource;
    bool los;
    [SerializeField] GameObject sprite;

    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
    }

    Vector2 pathfindingInstruction;

    void Update()
    {
        sprite.transform.rotation = Quaternion.Euler(Vector3.zero);
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
        Vector2 dest = GridGenerator.Instance.MonsterInstructions();
        float monsterDist = Mathf.Clamp01((8-(float)GridGenerator.Instance.monsterPathLength)/8);
        postProc.weight = monsterDist;
        audioSource.volume = monsterDist;
        Radio.Instance.interference = monsterDist;
        if(los){
            float perc = Mathf.Clamp01((25-Vector2.Distance(playerLoc,monsterLoc))/25);
            Color debugRayColor = Color.Lerp(Color.green,Color.red,perc);
            Debug.DrawRay(monsterLoc,playerLoc-monsterLoc,debugRayColor);
            MonsterMovement(playerLoc-monsterLoc,2);
        }
        else{
            Debug.DrawRay(monsterLoc,playerLoc-monsterLoc,Color.green);
            MonsterMovement(dest-monsterLoc,1);
        }
    }

    void MonsterMovement(Vector2 movement, float x){
        Vector2 normal = movement.normalized;
        rb.velocity = new Vector2(normal.x * speed, normal.y * speed *x);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] float enemyHorizontalSpeed = 1f;
    Rigidbody2D rbEnemy;
    BoxCollider2D enemyBoxCollider;
    
    void Awake()
    {
        enemyBoxCollider = GetComponent<BoxCollider2D>();
        rbEnemy = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        rbEnemy.velocity = new Vector2(enemyHorizontalSpeed, 0f);        
    }

    void OnTriggerExit2D(Collider2D other) 
    {
        if (other.tag != "Platform") { return; }
        enemyHorizontalSpeed = -enemyHorizontalSpeed;
        FlipEnemySprite();
    }

    void FlipEnemySprite()
    {
        transform.localScale = new Vector2(-(Mathf.Sign(rbEnemy.velocity.x)), 1f);
    }
}

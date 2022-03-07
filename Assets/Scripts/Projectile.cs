using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float projectileVelocity = 20f;
    Rigidbody2D rbProjectile;
    PlayerMovement player;
    float ProjectileVectorX;

    void Start()
    {
        player = FindObjectOfType<PlayerMovement>();
        rbProjectile = GetComponent<Rigidbody2D>(); 

        ProjectileVectorX = player.transform.localScale.x * projectileVelocity;  
    }

    void Update()
    {
        rbProjectile.velocity = new Vector2(ProjectileVectorX, 0f);
    }

    void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.tag == "Enemy")
        {
            Destroy(other.gameObject);
        }

        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D other) 
    {
        Destroy(gameObject);
    }
}

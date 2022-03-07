using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float projectileVelocity = 10f;
    [SerializeField] float projectileDropOff = -0.1f;
    Rigidbody2D rbProjectile;
    PlayerMovement player;
    float xSpeed;

    void Start()
    {
        player = FindObjectOfType<PlayerMovement>();
        rbProjectile = GetComponent<Rigidbody2D>(); 

        xSpeed = player.transform.localScale.x * projectileVelocity;  
        Debug.Log(xSpeed); 
    }

    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    [SerializeField] AudioClip CoinPickupSFX;
    [SerializeField] float CoinPickupVolume = 0.3f;
    [SerializeField] int points = 100;

    bool wasCollected = false;

    void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.tag == "Player" && !wasCollected)
        {
            wasCollected = true;
            FindObjectOfType<GameSession>().ProcessPointsCount(points);
            AudioSource.PlayClipAtPoint(CoinPickupSFX, Camera.main.transform.position, CoinPickupVolume);
            gameObject.SetActive(false); // failsafe for bug double pick up
            Destroy(gameObject);
        }   
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartBehaviour : MonoBehaviour
{
    // Method to handle collision with other objects
    void OnTriggerEnter(Collider other)
    {
        // Check if the heart has collided with the player
        if (other.gameObject.name == "FirstPersonWalker")
        {
            // Get the PlayerFunctions script from the player
            PlayerFunctions playerFunctions = other.transform.parent.GetComponent<PlayerFunctions>();

            // Restore the player's health
            playerFunctions.playerHealth = 5;

            // Update the health bar
            playerFunctions.UpdateHealthBar();

            // Destroy the heart
            Destroy(gameObject);
        }
    }
}

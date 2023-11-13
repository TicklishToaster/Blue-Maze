using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    
    // Store the hand object
    private GameObject hand;

    // Store the distance at which the enemy will attack the player
    public float triggerDistance;

    // Boolean to determine if the enemy should attack the player
    private bool attackPlayer = false;    

    // Store the player object
    private GameObject player;

    // Store the player's model or collider object
    private Transform playerModel;    

    // Store the weapon prefab
    public GameObject weaponPrefab;

    // Store the attack speed (lower values = faster attacks)
    public float attackSpeed;

    // Store the time since the last attack
    private float attackTimer = 0f;

    // Store the enemy's body object
    private Transform enemyBody;

    // Start is called before the first frame update
    void Start()
    {
        // Find the player GameObject by name or tag
        player = GameObject.Find("Player");

        // Find the player's model object
        playerModel = player.transform.Find("FirstPersonWalker/Model");

        // Find the enemy's body object
        enemyBody = transform.Find("Body");

        // Find the hand object
        hand = transform.Find("Hand").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the player is within range
        CheckPlayerInRange();

        // If the enemy should attack the player
        if (attackPlayer)
        {
            // Turn towards the player
            TurnTowardsPlayer();

            // Throw the weapon
            ThrowWeapon();         
        }
    }

    // Check if the player is within range
    void CheckPlayerInRange()
    {
        // Get the distance to the player
        float distanceToPlayer = Vector3.Distance(enemyBody.position, playerModel.position);

        // If the player is within the trigger distance
        if (distanceToPlayer <= triggerDistance)
        {
            // Check line of sight
            RaycastHit hit;
            Vector3 directionToPlayer = (playerModel.position - enemyBody.position).normalized;

            if (Physics.Raycast(enemyBody.position, directionToPlayer, out hit))
            {
                // If the first object the raycast hits is the player, the enemy has line of sight to the player
                if (hit.transform == playerModel.parent && !attackPlayer)
                {
                    attackPlayer = true;
                    return;
                }
            }
        }

        // If the player is outside the trigger distance or the enemy doesn't have line of sight to the player
        if (attackPlayer)
        {
            RaycastHit hit;
            Vector3 directionToPlayer = (playerModel.position - enemyBody.position).normalized;
            if (Physics.Raycast(enemyBody.position, directionToPlayer, out hit))
            {
                if (distanceToPlayer > triggerDistance || hit.transform != playerModel.parent)
                {
                    // Set attackPlayer to false
                    attackPlayer = false;

                    // Reset the attack timer
                    attackTimer = 0f;
                }
            }
        }
    }

    // Turn towards the player
    void TurnTowardsPlayer()
    {
        // Get the direction to the player
        Vector3 directionToPlayer = (playerModel.position - enemyBody.position);
        directionToPlayer.y = 0; // Ignore the Y component
        directionToPlayer = directionToPlayer.normalized;

        // Calculate the target rotation
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);

        // Smoothly rotate towards the player
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime);
    }

    // Throw the stored weapon
    void ThrowWeapon()
    {
        // Only proceed if attackPlayer is true and the enemy is facing the player
        if (attackPlayer && Vector3.Dot(transform.forward, (playerModel.position - transform.position).normalized) > 0.9f)
        {
            // Increase the attack timer
            attackTimer += Time.deltaTime;

            // If the attack timer is complete
            if (attackTimer >= attackSpeed)
            {
                // Reset the attack timer
                attackTimer = 0f;

                // Create a new weapon at the right hand's position with the same rotation as the enemy
                GameObject newWeapon = Instantiate(weaponPrefab, hand.transform.position, transform.rotation);

                // Change the tag of the new weapon to "Enemy"
                newWeapon.tag = "Enemy";

                // Get the Rigidbody component of the new weapon
                Rigidbody rb = newWeapon.GetComponent<Rigidbody>();

                // Get the ProjectileBehaviour script of the new weapon
                ProjectileBehaviour projectileBehaviour = newWeapon.GetComponent<ProjectileBehaviour>();

                // Set the itemThrown flag to true
                projectileBehaviour.itemThrown = true;

                // Set the isEnemy flag to true
                projectileBehaviour.isEnemy = true;

                // Set the target position to the player's position
                projectileBehaviour.targetPosition = playerModel.position;

                // Set the throw direction to the direction of the target position
                projectileBehaviour.throwDirection = (projectileBehaviour.targetPosition - newWeapon.transform.position).normalized;

                // Calculate the direction to the target coordinate
                Vector3 direction = (projectileBehaviour.targetPosition - newWeapon.transform.position).normalized;

                // Apply a force to the new weapon in the calculated direction
                rb.AddForce(direction * projectileBehaviour.throwForce);
            }
        }
    }
}

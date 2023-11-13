using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{
    // Reference to the main camera
    private Camera mainCamera;

    // Store the Rigidbody component
    private Rigidbody rb;

    // Flag to check if the projectile has been thrown
    public bool itemThrown = false; 

    // Store this object's throw force
    public float throwForce;

    // Store the target position
    public Vector3 targetPosition;

    // Store the direction the projectile is thrown
    public Vector3 throwDirection;

    // Flag to check if the projectile is thrown by an enemy
    public bool isEnemy = false;

    // Store the item type
    public string itemType;

    // Start is called before the first frame update
    void Start()
    {
        // Get the Rigidbody component
        rb = GetComponent<Rigidbody>();

        if (itemThrown)
        {
            StartCoroutine(CheckImpactPosition());
        }     
    }

    // Update is called once per frame
    void Update()
    {
        if (itemThrown && rb.velocity != Vector3.zero)
        {
            // Set the rotation of the projectile to face the direction it's moving
            transform.rotation = Quaternion.LookRotation(rb.velocity);

            // Start the CheckImpactPosition coroutine
            StartCoroutine(CheckImpactPosition());
        }
    }
    
    // This method is called when the projectile collider first touches another collider
    void OnTriggerEnter(Collider other)
    {
        // Check if the projectile has been thrown and if it's colliding with a wall
        if (itemThrown && other.gameObject.tag == "Wall")
        {
            // Freeze the projectile position and rotation in all three axes
            rb.constraints = RigidbodyConstraints.FreezeAll;

            // Calculate the new target position
            Vector3 newTargetPosition = targetPosition - transform.forward * (transform.localScale.z / 8);

            // Teleport the projectile to the new target position
            transform.position = newTargetPosition;

            // Set the itemThrown flag to false
            itemThrown = false;
        }

        // Check if the projectile has not been thrown by an enemy and if it's colliding with an enemy
        if (itemThrown && !isEnemy && other.gameObject.tag == "Enemy")
        {
            if (itemType == "Javelin")
            {
                // Redirect the projectile downward
                rb.velocity = Vector3.down * rb.velocity.magnitude;

                // Make the projectile face downward
                transform.rotation = Quaternion.LookRotation(Vector3.down);

                // Shoot a raycast downward
                RaycastHit hit;
                if (Physics.Raycast(transform.position, Vector3.down, out hit))
                {
                    // Set targetPosition to the hit position
                    targetPosition = hit.point;
                    transform.position = targetPosition;
                }
            }

            // Destroy the enemy object
            Destroy(other.gameObject);

            if (itemType == "Dagger")
            {
                Destroy(gameObject);
            }            
        }


        // Check if the projectile has been thrown by an enemy and if it's not colliding with an enemy
        if (isEnemy && other.gameObject.tag != "Enemy")
        {
            if (other.gameObject.name == "FirstPersonWalker")
            {
                // Get the PlayerFunctions script attached to the parent of the player
                PlayerFunctions playerFunctions = other.gameObject.GetComponentInParent<PlayerFunctions>();

                // Call the TakeDamage function
                playerFunctions.TakeDamage();
            }
            Destroy(gameObject);
        }        
    }

    // This coroutine checks the impact position every quarter second
    IEnumerator CheckImpactPosition()
    {
        // Only run the coroutine once
        if (!itemThrown)
        {
            yield break;
        }

        // Check if the projectile is currently colliding with a wall
        bool isColliding = Physics.CheckBox(transform.position, transform.localScale / 2, transform.rotation);

        RaycastHit hit;
        if (!isColliding && Physics.Raycast(transform.position, throwDirection, out hit))
        {
            if (hit.point != targetPosition)
            {
                targetPosition = hit.point;
            }
        }

        yield return new WaitForSeconds(0.25f);
    }
}
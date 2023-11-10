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

    // This coroutine checks the impact position every quarter second
    IEnumerator CheckImpactPosition()
    {
        // Only run the coroutine once
        if (!itemThrown)
        {
            yield break;
        }

        RaycastHit hit;
        if (Physics.Raycast(transform.position, throwDirection, out hit))
        {
            if (hit.point != targetPosition)
            {
                targetPosition = hit.point;
            }
        }

        yield return new WaitForSeconds(0.25f);
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
    }
}
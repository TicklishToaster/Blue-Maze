using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DaggerBehaviour : MonoBehaviour
{
    // Store the Rigidbody component
    private Rigidbody rb;

    // Flag to check if the dagger has been thrown
    public bool itemThrown = false;

    // Store the target coordinate
    public Vector3 targetCoordinate;

    // Start is called before the first frame update
    void Start()
    {
        // Get the Rigidbody component
        rb = GetComponent<Rigidbody>();
    }

    // This method is called when the dagger collider first touches another collider
    void OnTriggerEnter(Collider other)
    {
        // Check if the dagger has been thrown and if it's colliding with a wall
        if (itemThrown && other.gameObject.tag == "Wall")
        {
            // Freeze the dagger position and rotation in all three axes
            rb.constraints = RigidbodyConstraints.FreezeAll;

            // Calculate the new target coordinate
            Vector3 newTargetCoordinate = targetCoordinate - transform.forward * (transform.localScale.z / 8);

            // Teleport the dagger to the new target coordinate
            transform.position = newTargetCoordinate;
        }
    }
}

// public class DaggerBehaviour : MonoBehaviour
// {
//     // Store the Rigidbody component
//     private Rigidbody rb;

//     // Flag to check if the dagger has been thrown
//     public bool itemThrown = false;

//     // Start is called before the first frame update
//     void Start()
//     {
//         // Get the Rigidbody component
//         rb = GetComponent<Rigidbody>();
//     }

//     // This method stops the Dagger's movement and makes it kinematic
//     public void StopDagger()
//     {
//         // rb.velocity = Vector3.zero;
//         // rb.angularVelocity = Vector3.zero;
//         // rb.isKinematic = true;

//         // Freeze the Dagger's position and rotation in all three axes
//         rb.constraints = RigidbodyConstraints.FreezeAll;        
//     }
// }

// public class DaggerBehaviour : MonoBehaviour
// {
//     // Store the Rigidbody component
//     private Rigidbody rb;

//     // Store a reference to the child object at the tip of the dagger
//     public Transform daggerTip;    

//     // Flag to check if the dagger has been thrown
//     public bool itemThrown = false;

//     // Start is called before the first frame update
//     void Start()
//     {
//         // Get the Rigidbody component
//         rb = GetComponent<Rigidbody>();
//     }

//     // Update is called once per frame
//     void Update()
//     {

//     }

//     // This method is called when the Dagger's collider first touches another collider
//     void OnTriggerEnter(Collider other)
//     {
//         // Check if the dagger has been thrown and if it's colliding with a wall
//         if (itemThrown && other.gameObject.tag == "Wall")
//         {
//             // Stop the Dagger's movement and make it kinematic
//             rb.velocity = Vector3.zero;
//             rb.angularVelocity = Vector3.zero;
//             rb.isKinematic = true;

//             // Start the PullBack coroutine
//             StartCoroutine(PullBack());
//         }
//     }

//     // This coroutine continuously pulls the dagger back until the daggerTip's collider is no longer colliding with the wall
//     IEnumerator PullBack()
//     {
//         // Get the BoxCollider component of the daggerTip
//         BoxCollider bc = daggerTip.GetComponent<BoxCollider>();
//         Debug.Log(bc);

//         // While the daggerTip's collider is colliding with the wall
//         while (bc.bounds.Intersects(GameObject.FindGameObjectWithTag("Wall").GetComponent<Collider>().bounds))
//         {
//             Debug.Log("Pulling back");

//             // Move the dagger back along the direction it came from by a small amount
//             transform.position -= transform.forward * 0.01f;

//             // Wait for the next frame
//             yield return null;
//         }
//     } 
// }


// public class DaggerBehaviour : MonoBehaviour
// {
//     // Store the tip of the dagger
//     public GameObject DaggerTip;

//     // Store the Rigidbody component
//     private Rigidbody rb;    

//     // Start is called before the first frame update
//     void Start()
//     {
//         // Find the child GameObject named "Tip Collider"
//         DaggerTip = transform.Find("Tip").gameObject;

//         // Get the Rigidbody component
//         rb = GetComponent<Rigidbody>();
//     }

//     // Update is called once per frame
//     void Update()
//     {

//     }

//     // This method is called when the DaggerTip's collider first touches a wall
//     public void OnDaggerTipCollision(Vector3 newPosition, Quaternion newRotation)
//     {
//         // Stop the Dagger's movement and make it kinematic
//         rb.velocity = Vector3.zero;
//         rb.angularVelocity = Vector3.zero;
//         rb.isKinematic = true;

//         // Apply the new position and rotation to the dagger
//         transform.position = newPosition;
//         transform.rotation = newRotation;        
//     } 
// }
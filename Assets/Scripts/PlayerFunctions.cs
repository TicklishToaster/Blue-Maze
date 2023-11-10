using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerFunctions : MonoBehaviour
{
    // Camera reference
    private Camera mainCamera;

    // Arm References
    public GameObject leftArm;
    public GameObject rightArm;

    // Hand references
    private Transform leftHand;
    private Transform rightHand;    

    // Item References
    public GameObject daggerPrefab;
    public GameObject javelinPrefab;

    // Dictionary to store collectedItems for each arm/hand
    public Dictionary<GameObject, GameObject> collectedItems = new Dictionary<GameObject, GameObject>();

    // The minimum time delay before an item can be thrown after it's grabbed
    public float throwDelay = 0.2f;

    // The time when the item was grabbed
    private float grabTime;

    // Track if the player is throwing an object
    private bool isThrowingLeft = false;
    private bool isThrowingRight = false;

    // LineRenderers for aim lines
    private LineRenderer leftAimLine;
    private LineRenderer rightAimLine;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        leftHand = leftArm.transform.Find("Hand");
        rightHand = rightArm.transform.Find("Hand");

        // // Initialize LineRenderers
        // leftAimLine = leftHand.gameObject.AddComponent<LineRenderer>();
        // rightAimLine = rightHand.gameObject.AddComponent<LineRenderer>();
        // InitializeAimLine(leftAimLine);
        // InitializeAimLine(rightAimLine);

        // // Initialize LineRenderers
        // leftAimLine = leftHand.gameObject.AddComponent<LineRenderer>();
        // rightAimLine = rightHand.gameObject.AddComponent<LineRenderer>();
        // HandleAimLine(leftAimLine, Vector3.zero, Vector3.zero, false);
        // HandleAimLine(rightAimLine, Vector3.zero, Vector3.zero, false);

        // test

        // Initialize LineRenderers
        leftAimLine = leftHand.gameObject.AddComponent<LineRenderer>();
        rightAimLine = rightHand.gameObject.AddComponent<LineRenderer>();
        SetupAimLine(leftAimLine);
        SetupAimLine(rightAimLine);        
    }

    // Update is called once per frame
    void Update()
    {
        // If the left mouse button is pressed down, set isThrowingLeft to true
        if (Input.GetMouseButtonDown(0))
        {
            isThrowingLeft = true;
        }
        // If the left mouse button is released and isThrowingLeft is true, handle input for the left arm
        else if (Input.GetMouseButtonUp(0) && isThrowingLeft)
        {
            HandleInput(leftArm, leftHand);
            isThrowingLeft = false;
        }

        // If the right mouse button is pressed down, set isThrowingRight to true
        if (Input.GetMouseButtonDown(1))
        {
            isThrowingRight = true;
        }
        // If the right mouse button is released and isThrowingRight is true, handle input for the right arm
        else if (Input.GetMouseButtonUp(1) && isThrowingRight)
        {
            HandleInput(rightArm, rightHand);
            isThrowingRight = false;
        }

        // // Declare raycast hit
        // RaycastHit hit;

        // // Draw aim lines
        // if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit))
        // {
        //     if (isThrowingLeft && collectedItems.ContainsKey(leftArm))
        //     {
        //         DrawAimLine(leftAimLine, leftHand.position, hit.point);
        //     }
        //     else
        //     {
        //         HideAimLine(leftAimLine);
        //     }

        //     if (isThrowingRight && collectedItems.ContainsKey(rightArm))
        //     {
        //         DrawAimLine(rightAimLine, rightHand.position, hit.point);
        //     }
        //     else
        //     {
        //         HideAimLine(rightAimLine);
        //     }
        // }
        // else
        // {
        //     HideAimLine(leftAimLine);
        //     HideAimLine(rightAimLine);
        // }

        // Declare raycast hit
        RaycastHit hit;

        // Draw aim lines
        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit))
        {
            HandleAimLine(leftAimLine, leftHand.position, hit.point, isThrowingLeft && collectedItems.ContainsKey(leftArm));
            HandleAimLine(rightAimLine, rightHand.position, hit.point, isThrowingRight && collectedItems.ContainsKey(rightArm));
        }
        else
        {
            HandleAimLine(leftAimLine, Vector3.zero, Vector3.zero, false);
            HandleAimLine(rightAimLine, Vector3.zero, Vector3.zero, false);
        }
    }   

    // Handle input for a specific arm
    void HandleInput(GameObject arm, Transform hand)
    {
        // If the arm has an item and enough time has passed since it was grabbed, throw the item
        if (collectedItems.ContainsKey(arm) && Time.time - grabTime > throwDelay)
        {
            ThrowObject(arm);
        }
        // Otherwise, try to grab an item
        else
        {
            GrabObject(arm, hand);
            // Record the time when the item was grabbed
            grabTime = Time.time;
        }
    }

    // Attempt to grab a collectable object
    void GrabObject(GameObject arm, Transform hand)
    {
        // Declare raycast hit
        RaycastHit hit;

        // Shoot a raycast from the camera
        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit))
        {
            // If the hit object has the "Collectable" tag
            if (hit.collider.gameObject.CompareTag("Collectable"))
            {
                // Store a reference to the hit object
                GameObject hitObject = hit.collider.gameObject;

                // Set the hit object as the prefab to instantiate
                GameObject prefabToInstantiate = hitObject;

                // Destroy the hit object
                Destroy(hitObject);

                // Create a new position offset
                Vector3 positionOffset = new Vector3(0.0f, 0.0f, 0.0f);

                // If the hit object name contains "Dagger", adjust the rotation and offset and set the instantiate prefab to daggerPrefab
                if (hitObject.name.Contains("Dagger"))
                {
                    positionOffset = new Vector3(0.0f, 0.0f, 0.1f);
                    prefabToInstantiate = daggerPrefab;
                }

                // If the hit object name contains "Javelin", adjust the rotation and offset and set the instantiate prefab to javelinPrefab
                if (hitObject.name.Contains("Javelin"))
                {
                    positionOffset = new Vector3(0.0f, 0.0f, -0.3f);
                    prefabToInstantiate = javelinPrefab;
                }

                // Instantiate the new object
                InstantiateObject(arm, prefabToInstantiate, positionOffset, hand);
            }
        }
    } 

    // Instantiate a new object at the hand's position with a specific offset
    void InstantiateObject(GameObject arm, GameObject prefab, Vector3 positionOffset, Transform hand)
    {
        // Calculate the offset in world space
        Vector3 worldSpaceOffset = hand.TransformDirection(positionOffset);
        // Instantiate the new object at the hand's position with the offset
        GameObject newObject = Instantiate(prefab, hand.position + worldSpaceOffset, hand.rotation);

        // Adjust the object's rotation
        newObject.transform.rotation *= Quaternion.Euler(00, 00, 90);
        // Set the object's parent to the hand
        newObject.transform.parent = hand;
        // Add the object to the collected items
        collectedItems[arm] = newObject;
    }

    // Checks if a raycast from the camera is hitting a collectable item (used in CrosshairBehaviour)
    public bool IsCameraRaycastHittingCollectable()
    {
        // Declare raycast hit
        RaycastHit hit;
        return Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit) && hit.collider.gameObject.CompareTag("Collectable");
    }

    // Throw the object currently held by a specific arm
    void ThrowObject(GameObject arm)
    {
        // Get the object currently held by the arm
        GameObject heldObject = collectedItems[arm];

        // Check if the held object is a dagger or a javelin
        bool isDagger = heldObject.name.Contains("Dagger");
        bool isJavelin = heldObject.name.Contains("Javelin");

        // Remove the object from the collected items
        collectedItems.Remove(arm);

        // Delete the currently held object
        Destroy(heldObject);

        // Create a new object at the arm's position with the same rotation as the camera
        GameObject newObject = Instantiate(isDagger ? daggerPrefab : javelinPrefab, arm.transform.position, Camera.main.transform.rotation);

        // Get the Rigidbody component of the new object
        Rigidbody rb = newObject.GetComponent<Rigidbody>();

        // Get the ProjectileBehaviour script of the new object
        ProjectileBehaviour projectileBehaviour = newObject.GetComponent<ProjectileBehaviour>();

        // Set the itemThrown flag to true
        projectileBehaviour.itemThrown = true;

        // Shoot a raycast where the camera is aiming and store the hit position in the ProjectileBehaviour script
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit))
        {
            projectileBehaviour.targetPosition = hit.point;
        }
        else
        {
            // Set a default target position at a certain distance along the camera's forward vector
            projectileBehaviour.targetPosition = Camera.main.transform.position + Camera.main.transform.forward * 1000f;
        }

        projectileBehaviour.throwDirection = (projectileBehaviour.targetPosition - newObject.transform.position).normalized;

        // Calculate the direction to the target coordinate
        Vector3 direction = (projectileBehaviour.targetPosition - newObject.transform.position).normalized;

        // Apply a force to the new object in the calculated direction
        rb.AddForce(direction * projectileBehaviour.throwForce);
    }    

    // void InitializeAimLine(LineRenderer aimLine)
    // {
    //     // Use the built-in Sprites-Default material
    //     aimLine.material = new Material(Shader.Find("Sprites/Default"));

    //     // Set color with alpha
    //     aimLine.startColor = new Color(1f, 1f, 1f, 0.25f);
    //     aimLine.endColor = new Color(1f, 1f, 1f, 0.25f);

    //     aimLine.startWidth = 0.02f;
    //     aimLine.endWidth = 0.02f;
    //     aimLine.positionCount = 2;
    // }

    // void DrawAimLine(LineRenderer aimLine, Vector3 startPoint, Vector3 endPoint)
    // {
    //     aimLine.SetPosition(0, startPoint);
    //     aimLine.SetPosition(1, endPoint);
    //     aimLine.enabled = true;
    // }

    // void HideAimLine(LineRenderer aimLine)
    // {
    //     aimLine.enabled = false;
    // }

    void SetupAimLine(LineRenderer aimLine)
    {
        // Use the built-in Sprites-Default material
        aimLine.material = new Material(Shader.Find("Sprites/Default"));

        // Set color with alpha
        aimLine.startColor = new Color(1f, 1f, 1f, 0.25f);
        aimLine.endColor = new Color(1f, 1f, 1f, 0.25f);

        aimLine.startWidth = 0.02f;
        aimLine.endWidth = 0.02f;
        aimLine.positionCount = 2;
    }    

    void HandleAimLine(LineRenderer aimLine, Vector3 startPoint, Vector3 endPoint, bool isVisible)
    {
        if (isVisible)
        {
            aimLine.SetPosition(0, startPoint);
            aimLine.SetPosition(1, endPoint);
            aimLine.enabled = true;
        }
        else
        {
            aimLine.enabled = false;
        }
    }    
}

    // // Throw the object currently held by a specific arm
    // void ThrowObject(GameObject arm)
    // {
    //     // Get the object currently held by the arm
    //     GameObject heldObject = collectedItems[arm];

    //     // Check if the held object is a dagger or a javelin
    //     bool isDagger = heldObject.name.Contains("Dagger");
    //     bool isJavelin = heldObject.name.Contains("Javelin");

    //     // Remove the object from the collected items
    //     collectedItems.Remove(arm);

    //     // Delete the currently held object
    //     Destroy(heldObject);

    //     // Create a new object at the arm's position with the same rotation as the camera
    //     GameObject newObject = Instantiate(isDagger ? daggerPrefab : javelinPrefab, arm.transform.position, Camera.main.transform.rotation);

    //     // Get the Rigidbody component of the new object
    //     Rigidbody rb = newObject.GetComponent<Rigidbody>();

    //     // Get the ProjectileBehaviour script of the new object
    //     ProjectileBehaviour projectileBehaviour = newObject.GetComponent<ProjectileBehaviour>();

    //     // Set the itemThrown flag to true
    //     projectileBehaviour.itemThrown = true;

    //     // Shoot a raycast where the camera is aiming and store the hit position in the ProjectileBehaviour script
    //     RaycastHit hit;
    //     if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit))
    //     {
    //         projectileBehaviour.targetPosition = hit.point;
    //         projectileBehaviour.throwDirection = (hit.point - newObject.transform.position).normalized;
    //     }        

    //     // Calculate the direction to the target coordinate
    //     Vector3 direction = (projectileBehaviour.targetPosition - newObject.transform.position).normalized;

    //     // Apply a force to the new object in the calculated direction
    //     rb.AddForce(direction * projectileBehaviour.throwForce);
    // } 

// public class PlayerFunctions : MonoBehaviour
// {
//     // Camera reference
//     private Camera mainCamera;

//     // Arm References
//     public GameObject leftArm;
//     public GameObject rightArm;

//     // Hand references
//     private Transform leftHand;
//     private Transform rightHand;    

//     // Item References
//     public GameObject daggerPrefab;
//     public GameObject javelinPrefab;

//     // Dictionary to store collectedItems for each arm/hand
//     public Dictionary<GameObject, GameObject> collectedItems = new Dictionary<GameObject, GameObject>();

//     // The minimum time delay before an item can be thrown after it's grabbed
//     public float throwDelay = 0.2f;

//     // The time when the item was grabbed
//     private float grabTime;

//     // Track if the player is throwing an object
//     private bool isThrowingLeft = false;
//     private bool isThrowingRight = false;    

//     // Start is called before the first frame update
//     void Start()
//     {
//         mainCamera = Camera.main;
//         leftHand = leftArm.transform.Find("Hand");
//         rightHand = rightArm.transform.Find("Hand");
//     }

//     // Update is called once per frame
//     void Update()
//     {
//         // If the left mouse button is pressed down, set isThrowingLeft to true
//         if (Input.GetMouseButtonDown(0))
//         {
//             isThrowingLeft = true;
//         }
//         // If the left mouse button is released and isThrowingLeft is true, handle input for the left arm
//         else if (Input.GetMouseButtonUp(0) && isThrowingLeft)
//         {
//             HandleInput(leftArm, leftHand);
//             isThrowingLeft = false;
//         }

//         // If the right mouse button is pressed down, set isThrowingRight to true
//         if (Input.GetMouseButtonDown(1))
//         {
//             isThrowingRight = true;
//         }
//         // If the right mouse button is released and isThrowingRight is true, handle input for the right arm
//         else if (Input.GetMouseButtonUp(1) && isThrowingRight)
//         {
//             HandleInput(rightArm, rightHand);
//             isThrowingRight = false;
//         }        
//     }   

//     // Handle input for a specific arm
//     void HandleInput(GameObject arm, Transform hand)
//     {
//         // If the arm has an item and enough time has passed since it was grabbed, throw the item
//         if (collectedItems.ContainsKey(arm) && Time.time - grabTime > throwDelay)
//         {
//             ThrowObject(arm);
//         }
//         // Otherwise, try to grab an item
//         else
//         {
//             GrabObject(arm, hand);
//             // Record the time when the item was grabbed
//             grabTime = Time.time;
//         }
//     }

//     // Attempt to grab a collectable object
//     void GrabObject(GameObject arm, Transform hand)
//     {
//         RaycastHit hit;

//         // Shoot a raycast from the camera
//         if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit))
//         {
//             // If the hit object has the "Collectable" tag
//             if (hit.collider.gameObject.CompareTag("Collectable"))
//             {
//                 // Store a reference to the hit object
//                 GameObject hitObject = hit.collider.gameObject;

//                 // Set the hit object as the prefab to instantiate
//                 GameObject prefabToInstantiate = hitObject;

//                 // Destroy the hit object
//                 Destroy(hitObject);

//                 // Create a new position offset
//                 Vector3 positionOffset = new Vector3(0.0f, 0.0f, 0.0f);

//                 // If the hit object name contains "Dagger", adjust the rotation and offset and set the instantiate prefab to daggerPrefab
//                 if (hitObject.name.Contains("Dagger"))
//                 {
//                     positionOffset = new Vector3(0.0f, 0.0f, 0.1f);
//                     prefabToInstantiate = daggerPrefab;
//                 }

//                 // If the hit object name contains "Javelin", adjust the rotation and offset and set the instantiate prefab to javelinPrefab
//                 if (hitObject.name.Contains("Javelin"))
//                 {
//                     positionOffset = new Vector3(0.0f, 0.0f, -0.3f);
//                     prefabToInstantiate = javelinPrefab;
//                 }

//                 // Instantiate the new object
//                 InstantiateObject(arm, prefabToInstantiate, positionOffset, hand);
//             }
//         }
//     } 

//     // Instantiate a new object at the hand's position with a specific offset
//     void InstantiateObject(GameObject arm, GameObject prefab, Vector3 positionOffset, Transform hand)
//     {
//         // Calculate the offset in world space
//         Vector3 worldSpaceOffset = hand.TransformDirection(positionOffset);
//         // Instantiate the new object at the hand's position with the offset
//         GameObject newObject = Instantiate(prefab, hand.position + worldSpaceOffset, hand.rotation);

//         // Adjust the object's rotation
//         newObject.transform.rotation *= Quaternion.Euler(00, 00, 90);
//         // Set the object's parent to the hand
//         newObject.transform.parent = hand;
//         // Add the object to the collected items
//         collectedItems[arm] = newObject;
//     }

//     // Checks if a raycast from the camera is hitting a collectable item (used in CrosshairBehaviour)
//     public bool IsCameraRaycastHittingCollectable()
//     {
//         RaycastHit hit;
//         return Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit) && hit.collider.gameObject.CompareTag("Collectable");
//     }

//     // Throw the object currently held by a specific arm
//     void ThrowObject(GameObject arm)
//     {
//         // Get the object currently held by the arm
//         GameObject heldObject = collectedItems[arm];

//         // Check if the held object is a dagger or a javelin
//         bool isDagger = heldObject.name.Contains("Dagger");
//         bool isJavelin = heldObject.name.Contains("Javelin");

//         // Remove the object from the collected items
//         collectedItems.Remove(arm);

//         // Delete the currently held object
//         Destroy(heldObject);

//         // Create a new object at the arm's position with the same rotation as the camera
//         GameObject newObject = Instantiate(isDagger ? daggerPrefab : javelinPrefab, arm.transform.position, Camera.main.transform.rotation);

//         // Get the Rigidbody component of the new object
//         Rigidbody rb = newObject.GetComponent<Rigidbody>();

//         // Get the ProjectileBehaviour script of the new object
//         ProjectileBehaviour projectileBehaviour = newObject.GetComponent<ProjectileBehaviour>();

//         // Set the itemThrown flag to true
//         projectileBehaviour.itemThrown = true;

//         // Shoot a raycast where the camera is aiming and store the hit position in the ProjectileBehaviour script
//         RaycastHit hit;
//         if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit))
//         {
//             projectileBehaviour.targetPosition = hit.point;
//             projectileBehaviour.throwDirection = (hit.point - newObject.transform.position).normalized;
//         }        

//         // Calculate the direction to the target coordinate
//         Vector3 direction = (projectileBehaviour.targetPosition - newObject.transform.position).normalized;

//         // Apply a force to the new object in the calculated direction
//         rb.AddForce(direction * projectileBehaviour.throwForce);
//     }    
// }
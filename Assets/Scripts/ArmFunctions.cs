using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmFunctions : MonoBehaviour
{
    // References
    public PlayerFunctions playerFunctions;
    public Camera mainCamera;
    public Transform hand;
    public Transform pivot;
    private LineRenderer aimLine;

    // GameObjects and related properties
    public GameObject heldItem;
    private string lastHeldItem;
    private bool hasJustGrabbed;

    // Arm state
    private bool isCharging, isThrowing, isReturning;
    private bool isChargingFinished = false;
    public bool isLeftArm;

    // Dagger properties
    private Quaternion chargeRotation;
    private Quaternion throwRotation;
    private float daggerChargeSpeed = 60f;
    private float daggerThrowSpeed  = 90f * 8;
    private float daggerReturnSpeed = 45f;

    // Javelin properties
    private Vector3 javelinChargePosition   = new Vector3(0.0f, 0.0f, -0.2f);
    private Vector3 javelinThrowPosition    = new Vector3(0.0f, 0.0f, +0.3f);
    private Vector3 javelinReturnPosition;
    private float javelinChargeSpeed    = 0.25f;
    private float javelinThrowSpeed     = 8.00f;
    private float javelinReturnSpeed    = 0.50f;

    // Aim line properties
    private Vector3 aimLineStart;
    private Vector3 aimLineEnd;

    // Start is called before the first frame update
    void Start()
    {
        // If isLeftArm is true
        if (isLeftArm)
        {
            // Set rotations for the left dagger arm when charging and throwing
            chargeRotation  = Quaternion.Euler(0, 45, 0);
            throwRotation   = Quaternion.Euler(0, -25, 0);

            // Set position for the left javelin arm when returning (default position)
            javelinReturnPosition = new Vector3(-0.4f, -0.25f, +0.175f);
        }
        // If isLeftArm is false
        else
        {
            // Set rotations for the right dagger arm when charging and throwing
            chargeRotation  = Quaternion.Euler(0, -45, 0);
            throwRotation   = Quaternion.Euler(0, +25, 0);

            // Set position for the right javelin arm when returning (default position)
            javelinReturnPosition = new Vector3(+0.4f, -0.25f, +0.175f);            
        }        

        // Create a new LineRenderer component for the aim line
        aimLine = hand.gameObject.AddComponent<LineRenderer>();
        SetupAimLine(aimLine);        
    } 

    void Update()
    {
        // Check if the arm is not holding an item
        if (heldItem == null)
        {
            // If the mouse button is pressed, try to grab an object
            if ((isLeftArm ? Input.GetMouseButtonDown(0) : Input.GetMouseButtonDown(1)))
            {
                GrabObject();
            }         
        }
        else if (!hasJustGrabbed)
        {
            // If the mouse button is held down and the arm is not returning or throwing, start charging
            if (isLeftArm ? Input.GetMouseButton(0) : Input.GetMouseButton(1))
            {
                if (!isThrowing && !isReturning && !isChargingFinished) 
                {
                    isCharging = true;
                }
            }

            // If the mouse button is released and the arm was charging/has finished charging, throw the object
            if ((isLeftArm ? Input.GetMouseButtonUp(0) : Input.GetMouseButtonUp(1)) && isCharging && isChargingFinished)
            {
                isCharging = false;
                isChargingFinished = false;
                isThrowing = true;
                ThrowObject();
            }

            // If the arm is throwing and has reached the throw rotation, start returning
            if (isThrowing && Quaternion.Equals(pivot.rotation, throwRotation))
            {
                isThrowing = false;
                isReturning = true;
            }
        }  

        // Call the HandleArmAnimation function to handle the rotation of the arm
        HandleArmAnimation(Input.GetMouseButton(isLeftArm ? 0 : 1), Input.GetMouseButtonUp(isLeftArm ? 0 : 1));

        // If the arm is charging and the object wasn't just grabbed, show the aim line
        if (isCharging && !hasJustGrabbed)
        {
            // Update the start position of the aim line
            aimLineStart = hand.position;

            // Shoot a raycast where the camera is aiming and store the hit position in aimLineEnd
            RaycastHit hit;
            if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit))
            {
                aimLineEnd = hit.point;
            }
            else
            {
                // Set a default aimLineEnd at a certain distance along the camera's forward vector
                aimLineEnd = mainCamera.transform.position + mainCamera.transform.forward * 100f;
            }

            // Call HandleAimLine to update the aim line's position and make it visible
            HandleAimLine(aimLineStart, aimLineEnd, true);
        }
        else
        {
            // If the arm is not charging or the object was just grabbed, hide the aim line
            HandleAimLine(Vector3.zero, Vector3.zero, false);
        }
    }

    // Pick up a "Collectable" object if the arm is not holding anything.
    public void GrabObject()
    {
        // Only proceed if the arm is not holding anything and isThrowing and isReturning is false
        if (heldItem == null && !isThrowing && !isReturning)
        {
            // Declare raycast hit
            RaycastHit hit;

            // Shoot a raycast from the camera
            if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit))
            {
                // If the hit object has the "Collectable" tag and is within 3 units of the player
                if (hit.collider.gameObject.CompareTag("Collectable") && Vector3.Distance(mainCamera.transform.position, hit.transform.position) <= 3f)
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
                        prefabToInstantiate = playerFunctions.daggerPrefab;
                    }                

                    // If the hit object name contains "Javelin", adjust the rotation and offset and set the instantiate prefab to javelinPrefab
                    if (hitObject.name.Contains("Javelin"))
                    {
                        positionOffset = new Vector3(0.0f, 0.0f, -0.3f);
                        prefabToInstantiate = playerFunctions.javelinPrefab;
                    }

                    // Instantiate the new object
                    CreateObject(prefabToInstantiate, positionOffset);

                    // Set hasJustGrabbed to true after an object is successfully grabbed
                    hasJustGrabbed = true;
                    StartCoroutine(ResetHasJustGrabbed());                    
                }
            }
        }
    }

    // Coroutine to reset hasJustGrabbed after a delay
    IEnumerator ResetHasJustGrabbed()
    {
        yield return new WaitForSeconds(0.2f);
        hasJustGrabbed = false;
    }    

    // Instantiate a new object at the hand's position
    public void CreateObject(GameObject prefab, Vector3 positionOffset)
    {
        // Calculate the offset in world space
        Vector3 worldSpaceOffset = hand.TransformDirection(positionOffset);
        // Instantiate the new object at the hand's position with the offset
        GameObject newObject = Instantiate(prefab, hand.position + worldSpaceOffset, hand.rotation);

        // Adjust the object's rotation
        newObject.transform.rotation *= Quaternion.Euler(00, 00, 00);
        // Set the object's parent to the hand
        newObject.transform.parent = hand;

        // Assign the new object to heldItem
        heldItem = newObject;

        // Store the type of the held item
        lastHeldItem = newObject.name.Contains("Dagger") ? "Dagger" : "Javelin";
    }

    // Throw the object currently held by the arm
    public void ThrowObject()
    {
        // Only proceed if the arm is holding an object
        if (heldItem != null)
        {
            // Check if the held object is a dagger or a javelin
            bool isDagger = lastHeldItem == "Dagger";
            bool isJavelin = lastHeldItem == "Javelin";

            // Destroy the held object
            Destroy(heldItem);

            // Remove the reference to the held item
            heldItem = null;

            // Create a new object at the arm's position with the same rotation as the camera
            GameObject newObject = Instantiate(isDagger ? playerFunctions.daggerPrefab : playerFunctions.javelinPrefab, transform.position, Quaternion.identity);

            // Get the Rigidbody component of the new object
            Rigidbody rb = newObject.GetComponent<Rigidbody>();

            // Get the ProjectileBehaviour script of the new object
            ProjectileBehaviour projectileBehaviour = newObject.GetComponent<ProjectileBehaviour>();

            // Set the itemThrown flag to true
            projectileBehaviour.itemThrown = true;

            // Set the itemType to "Dagger" or "Javelin"
            projectileBehaviour.itemType = isDagger ? "Dagger" : "Javelin";

            // Shoot a raycast where the camera is aiming and store the hit position in the ProjectileBehaviour script
            RaycastHit hit;
            if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit))
            {
                projectileBehaviour.targetPosition = hit.point;
            }
            else
            {
                // Set a default target position at a certain distance along the camera's forward vector
                projectileBehaviour.targetPosition = mainCamera.transform.position + mainCamera.transform.forward * 100f;
            }

            // Set the throw direction to the direction of the target position
            projectileBehaviour.throwDirection = (projectileBehaviour.targetPosition - newObject.transform.position).normalized;

            // Calculate the direction to the target coordinate
            Vector3 direction = (projectileBehaviour.targetPosition - newObject.transform.position).normalized;

            // Apply a force to the new object in the calculated direction
            rb.AddForce(direction * projectileBehaviour.throwForce);
        
            // Set isThrowing to true
            isThrowing = true;
        }
    }

    // Handle the animation of the arm
    public void HandleArmAnimation(bool mouseButton, bool mouseButtonUp)
    {
        // If the mouse button is pressed and the arm is charging
        if (mouseButton && isCharging)
        {
            if (lastHeldItem == "Dagger")
            {
                // Dagger charge animation
                pivot.localRotation = Quaternion.RotateTowards(pivot.localRotation, chargeRotation, daggerChargeSpeed * Time.deltaTime);

                // If the pivot has reached the charge rotation
                if (Quaternion.Equals(pivot.localRotation, chargeRotation))
                {
                    isChargingFinished = true;
                }   
            }
            else if (lastHeldItem == "Javelin")
            {             
                // Javelin charge animation
                pivot.localPosition = Vector3.MoveTowards(pivot.localPosition, javelinReturnPosition + javelinChargePosition, javelinChargeSpeed * Time.deltaTime);

                // If the pivot has reached the charge position
                if (pivot.localPosition == javelinReturnPosition + javelinChargePosition)
                {
                    isChargingFinished = true;
                }
            }

            // Set isReturning to false incase the arm was returning
            if (isReturning)
            {
                isReturning = false;
            }
        }

        // If the mouse button is released and the arm is charging but hasn't finished charging, start returning
        if (mouseButtonUp && isCharging && !isChargingFinished && !isThrowing)
        {
            isReturning = true;
            isCharging = false;
        }

        // If the mouse button is released and the arm is charging and has finished charging, stop charging
        if (mouseButtonUp && isCharging && isChargingFinished)
        {
            isCharging = false;
        }        

        // If the arm is throwing
        if (isThrowing)
        {
            if (lastHeldItem == "Dagger")
            {
                // Dagger throw animation
                pivot.localRotation = Quaternion.RotateTowards(pivot.localRotation, throwRotation, daggerThrowSpeed * Time.deltaTime);

                // If the pivot has reached the throw rotation, stop throwing and start returning
                if (Quaternion.Equals(pivot.localRotation, throwRotation))
                {
                    isThrowing = false;
                    isReturning = true;
                }                
            }
            else if (lastHeldItem == "Javelin")
            {
                // Javelin throw animation
                pivot.localPosition = Vector3.MoveTowards(pivot.localPosition, javelinReturnPosition + javelinThrowPosition, javelinThrowSpeed * Time.deltaTime);  

                // If the pivot has reached the javelinThrowPosition position, stop throwing and start returning
                if (pivot.localPosition == javelinReturnPosition + javelinThrowPosition)
                {
                    isThrowing = false;
                    isReturning = true;
                }
            }
        }

        // If the arm is returning
        if (isReturning)
        {
            if (lastHeldItem == "Dagger")
            {
                // Dagger return animation
                pivot.localRotation = Quaternion.RotateTowards(pivot.localRotation, Quaternion.identity, daggerReturnSpeed * Time.deltaTime);
                // I love the amount of time I spent on this one line. 
                // Quaternions are so much fun. So painfully fun.

                // If the pivot has reached the return rotation, stop returning
                if (Quaternion.Equals(pivot.localRotation, Quaternion.identity))
                {
                    isReturning = false;
                }
            }
            else if (lastHeldItem == "Javelin")
            {
                // Javelin return animation
                pivot.localPosition = Vector3.MoveTowards(pivot.localPosition, javelinReturnPosition, javelinReturnSpeed * Time.deltaTime);

                // If the pivot has reached the return position, stop returning
                if (pivot.localPosition == javelinReturnPosition)
                {
                    isReturning = false;
                }                
            }
        }
    }

    // This function sets up the properties for the aim line
    void SetupAimLine(LineRenderer aimLine)
    {
        // Use the built-in Sprites-Default material (otherwise it just renders a opaque purple line)
        aimLine.material = new Material(Shader.Find("Sprites/Default"));

        // Set the colour of the aim line with alpha
        aimLine.startColor = new Color(1f, 1f, 1f, 0.25f);
        aimLine.endColor = new Color(1f, 1f, 1f, 0.25f);

        // Set the width of the aim line and the number of positions
        aimLine.startWidth = 0.02f;
        aimLine.endWidth = 0.02f;
        aimLine.positionCount = 2;
    }

    // This function handles the position and visibility of the aim line
    void HandleAimLine(Vector3 startPosition, Vector3 endPosition, bool isVisible)
    {
        // Set the positions of the aim line
        aimLine.SetPosition(0, startPosition);
        aimLine.SetPosition(1, endPosition);

        // If the aim line should be visible, enable the LineRenderer
        if (isVisible)
        {
            aimLine.enabled = true;
        }
        // Else if the aim line should not be visible, disable the LineRenderer
        else
        {
            aimLine.enabled = false;
        }
    }   
}

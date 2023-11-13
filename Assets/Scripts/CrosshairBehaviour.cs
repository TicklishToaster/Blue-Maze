using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrosshairBehaviour : MonoBehaviour
{
    public Camera mainCamera; 
    public PlayerFunctions PlayerObject; 
    public Sprite[] sprites; 
    private Image image; 
    private float spriteChangeTime; 
    private int index; 
    private int direction = 1;

    // Start is called before the first frame update
    void Start()
    {
        // Get the Image component from the child object
        image = GetComponentInChildren<Image>();
        spriteChangeTime = Time.time;
        index = 0;
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the raycast from the camera hits a collectable item
        if (CanGetCollectable())
        {
            direction = 1;
        }
        else if (image.sprite != sprites[0])
        {
            direction = -1;
        }

        // If it's time to update the sprite
        if (Time.time - spriteChangeTime >= 0.05f)
        {
            spriteChangeTime = Time.time;
            index += direction;
            index = Mathf.Clamp(index, 0, sprites.Length - 1);
            image.sprite = sprites[index];
        }
    }

    // Checks if a raycast from the camera is hitting a collectable item within range
    bool CanGetCollectable()
    {
        RaycastHit hit;
        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit) && hit.collider.gameObject.CompareTag("Collectable"))
        {
            // Check if the object is within 3 units of the camera
            if (Vector3.Distance(mainCamera.transform.position, hit.transform.position) <= 3f)
            {
                return true;
            }
        }
        return false;
    } 
}
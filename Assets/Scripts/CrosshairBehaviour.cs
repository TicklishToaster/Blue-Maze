using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrosshairBehaviour : MonoBehaviour
{
    public PlayerFunctions PlayerObject; // Reference to the PlayerFunctions script
    public Sprite[] sprites; // Array of sprites for the spritesheet
    private Image image; // Reference to the Image component
    private float spriteChangeTime; // The time when the sprite last changed
    private int index; // The current index of the sprite
    private int direction = 1; // The direction of the sprite animation (1 for forward, -1 for reverse)

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
        if (PlayerObject.IsCameraRaycastHittingCollectable())
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
}
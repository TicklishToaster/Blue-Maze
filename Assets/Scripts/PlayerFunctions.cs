using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerFunctions : MonoBehaviour
{
    [Header("References")]
    public Camera mainCamera;
    public GameObject leftArm, rightArm;
    public GameObject daggerPrefab, javelinPrefab;
    public Image healthBar;
    public Sprite[] healthBarSprites;
    public int playerHealth = 5;
    private bool gameEnd = false;

    // Start is called before the first frame update
    void Start()
    {
        // Enable the camera's depth texture
        mainCamera.depthTextureMode = DepthTextureMode.Depth;

        // Set the health bar to full health
        healthBar.sprite = healthBarSprites[playerHealth];
    }

    // Update is called once per frame
    void Update()
    {
        // If "H" key is released
        if (Input.GetKeyUp(KeyCode.H))
        {
            // Increase health by 1
            playerHealth = Mathf.Min(playerHealth + 1, 5);

            // Update the health bar
            UpdateHealthBar();
        }

        // If the player's health has reached 0
        if (playerHealth == 0 && !gameEnd)
        {
            // Start the RestartGame coroutine
            StartCoroutine(RestartGame());

            // Set gameEnd to true so this check doesn't repeat
            gameEnd = true;

            Debug.Log("TEST");
        }
    }

    // Method to update the health bar UI
    public void UpdateHealthBar()
    {
        healthBar.sprite = healthBarSprites[playerHealth];
    }

    // Take damage
    public void TakeDamage()
    {
        // Decrease health by 1
        playerHealth = Mathf.Max(playerHealth - 1, 0);

        // Update the health bar
        UpdateHealthBar();
    }

    // Coroutine to restart the game
    IEnumerator RestartGame()
    {
        // Wait for 2 seconds
        yield return new WaitForSeconds(2);

        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
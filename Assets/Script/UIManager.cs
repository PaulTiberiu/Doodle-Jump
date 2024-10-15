using UnityEngine;
using UnityEngine.SceneManagement; // For scene management
using System.Collections;

/// <summary>
/// The UIManager class is responsible for managing the user interface elements in the game,
/// specifically the end page panel that appears when the game ends. It handles the sliding
/// animation of the end page panel, pausing the game when the panel is fully visible, and
/// provides methods to navigate back to the main menu or restart the game.
/// </summary>
public class UIManager : MonoBehaviour
{
    // UI Elements
    public GameObject endPagePanel; // Reference to the End Page Panel

    // Slide parameters
    public float slideSpeed = 1000f; // Speed at which the panel slides up
    private RectTransform endPageRect; // RectTransform of the end page panel
    private bool isSliding = false; // To check if sliding is active

    void Start()
    {
        // Get the RectTransform component of the EndPage Panel
        endPageRect = endPagePanel.GetComponent<RectTransform>();

        // Ensure the panel is positioned below the screen and adjust with the screen size
        endPageRect.anchoredPosition = new Vector2(0, -endPageRect.rect.height);
    }

    public void TriggerEndPage()
    {
        endPagePanel.SetActive(true); // Ensure the panel is active
        // Trigger sliding when needed (e.g., doodle dies)
        if (!isSliding)
        {
            isSliding = true;
            StartCoroutine(SlideEndPageUp());
        }
    }

    IEnumerator SlideEndPageUp()
    {
        // Slide the panel upwards until it is fully visible
        while (endPageRect.anchoredPosition.y < 0)
        {
            Vector2 incrementedV = new Vector2(0, slideSpeed * Time.deltaTime);
            endPageRect.anchoredPosition += incrementedV;
            yield return null;
        }
        
        // Once the end page reaches the top, pause the game
        endPageRect.anchoredPosition = Vector2.zero; // Ensure it's exactly at the top
        Time.timeScale = 0; // Optional: Pause the game
    }

    // Method to go back to the main menu
    public void GoToMainMenu()
    {
        Time.timeScale = 1; // Resume time before loading the new scene
        SceneManager.LoadScene("MainMenu"); // Load the main menu scene
    }

    // Method to restart the current game
    public void ReplayGame()
    {
        Time.timeScale = 1; // Resume time before reloading the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reload the current scene
    }
}
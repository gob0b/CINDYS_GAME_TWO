using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Add this to use Button

public class ButtonPressGame : MonoBehaviour
{
    public int requiredPresses = 20; // Number of times the player must press the button
    public float timeLimit = 10f; // Time limit to complete the presses
    public string successScene = "SuccessScene"; // Scene to go to if successful
    public string failScene = "FailScene"; // Scene to go to if failed
    public Button pressButton; // Reference to the button that the player will click

    private int currentPresses = 0; // Counter for player button presses
    private float timer = 0f; // Timer to track the elapsed time
    private bool gameStarted = false; // Flag to check if the game has started

    // Start is called before the first frame update
    void Start()
    {
        // Ensure the game starts immediately when the scene loads
        StartGame();

        // Ensure the button click increments the press count
        if (pressButton != null)
        {
            pressButton.onClick.AddListener(OnButtonPress); // Subscribe to the button's OnClick event
        }
    }

    // Method to start the game (reset variables)
    void StartGame()
    {
        currentPresses = 0; // Reset presses at the start of the game
        timer = 0f; // Reset timer at the start of the game
        gameStarted = true; // Flag the game as started
    }

    // Called when the button is clicked
    void OnButtonPress()
    {
        currentPresses++; // Increment the press count when the button is clicked

        // Check if the player has completed the required presses immediately
        if (currentPresses >= requiredPresses)
        {
            EndGame(true); // Player succeeded, go to success scene immediately
        }
    }

    void Update()
    {
        // Only proceed with game logic if the game has started
        if (!gameStarted) return;

        // Increase timer based on elapsed time
        timer += Time.deltaTime;

        // Check if the time limit is exceeded (only if player hasn't succeeded yet)
        if (timer >= timeLimit && currentPresses < requiredPresses)
        {
            EndGame(false); // Time's up, fail the game
        }
    }

    // Method to handle the end of the game (success or failure)
    void EndGame(bool success)
    {
        if (success)
        {
            Debug.Log("You succeeded!");
            SceneManager.LoadScene(successScene); // Load the success scene
        }
        else // If failed, load the fail scene
        {
            Debug.Log("You failed!");
            SceneManager.LoadScene(failScene); // Load the fail scene
        }

        // Disable further interaction after the game ends
        gameStarted = false; // Stop the game logic from running
    }
}





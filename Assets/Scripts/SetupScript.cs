using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SetupScript : MonoBehaviour
{
    public InputField playerCountInput; // Input for number of players
    public InputField roundCountInput;  // Input for number of rounds
    public Button startGameButton;      // Reference to Start Game button
    public Text warningText;            // Text for displaying warnings (optional)

    private void Update()
    {
        ValidateInputs();
    }

    private void ValidateInputs() // finds the input so he can find the players needed to play the game
    {
        bool isValid = int.TryParse(playerCountInput.text, out int players) && players > 1 &&
                       int.TryParse(roundCountInput.text, out int rounds) && rounds > 0;

        startGameButton.interactable = isValid;

        if (warningText != null)
        {
            warningText.text = isValid ? "" : "Enter valid inputs (Players > 1, Rounds > 0)"; // scearch if you set the right amount of players and rounds other gives this error
        }
    }

    public void StartGame()
    {
        // Check if GameManager is properly initialized
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager instance is not initialized! Please check the GameManager setup.");
            return;
        }

        // Parse input values
        int players = int.Parse(playerCountInput.text);
        int rounds = int.Parse(roundCountInput.text);

        // Initialize the game with player count and round count
        GameManager.Instance.InitializeGame(players, rounds);

        // Load the game scene
        SceneManager.LoadScene("GameScene");
    }
}